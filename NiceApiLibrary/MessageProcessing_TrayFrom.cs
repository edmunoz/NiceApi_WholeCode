using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public delegate void d_OnScreenShot(string b64Data, NiceSystemInfo subSystem);

    /// <summary>
    /// Processes the response from the TrayApp
    /// </summary>
    public class MessageProcessing_TrayFrom
    {
        private d_OnScreenShot onScreenshot;
        IMyLog trayLog;
        LogForEmailSend log4Email;
        NiceSystemInfo niceSystem;

        public MessageProcessing_TrayFrom(NiceSystemInfo niceSystem, d_OnScreenShot onScreenshot, IMyLog trayLog, LogForEmailSend log4Email)
        {
            this.onScreenshot = onScreenshot;
            this.trayLog = trayLog;
            this.log4Email = log4Email;
            this.niceSystem = niceSystem;
        }

        public void Process_TrayFrom(Data_Net_Tray2ASP fromTray, bool sendEmails)
        {
            // Process each file from the incoming object
            foreach (ASPTrayBase o1 in fromTray.ObjectList)
            {
                try
                {
                    ASPTrayBase.eASPtrayType e1 = o1.GetEnumType();
                    switch (e1)
                    {
                        case ASPTrayBase.eASPtrayType.NormalMessageResult:
                            processNormalMessageResult((Data_Net__01NormalMessageResult)o1);
                            break;

                        case ASPTrayBase.eASPtrayType.ScreenShotResult:
                            processScreenShotResult((Data_Net__03ScreenshotResult)o1);
                            break;

                        case ASPTrayBase.eASPtrayType.CheckTelNumbersResult:
                            processCheckTelNumbersResult((Data_Net__05CheckTelNumbersResult)o1, sendEmails);
                            break;

                        default:
                            throw new IOException("Unknown file type in TrayApp response");
                    }
                }
                catch (SystemException se)
                {
                    trayLog.Error("Incoming file processing: Exception: " + se.Message + " " + se.ToString());
                }
            } // end of foreach
        }

        private void processNormalMessageResult(Data_Net__01NormalMessageResult _01)
        {
            if (_01.Success)
            {
                // Normal Message, success
                // 1) read in and delete msg file
                Data_Net__00NormalMessage msg00 = DSSwitch.msgFile00().ReadOne(niceSystem, _01.FileName, Data_Net__00NormalMessage.eLocation.Queued, trayLog);
                DSSwitch.msgFile00().Delete(niceSystem, _01.FileName, Data_Net__00NormalMessage.eLocation.Queued, trayLog);

                // 2) update counters in user file (if needed)
                DSSwitch.appUser().Update_General(
                    msg00.UserId,
                    OnProcessedHandler.OnProcessed,
                    new OnProcessedHandler(msg00.Msg.Length, log4Email, !msg00.NoCounterUpdate),
                    OnProcessedHandler.PostProcess,
                    trayLog);

                // 3) write msg file to processed folder
                DSSwitch.msgFile00().Store(niceSystem, (Data_Net__00NormalMessage)msg00, Data_Net__00NormalMessage.eLocation.Processed, trayLog);
            }
            else
            {
                // Normal Message, failed
                // 1) read in and delete msg file
                Data_Net__00NormalMessage msg00 = DSSwitch.msgFile00().ReadOne(niceSystem, _01.FileName, Data_Net__00NormalMessage.eLocation.Queued, trayLog);
                DSSwitch.msgFile00().Delete(niceSystem, _01.FileName, Data_Net__00NormalMessage.eLocation.Queued, trayLog);

                // 2) Increas failed counter and see whether we dispose of it
                msg00.FailedCounter++;
                if ((msg00.DisposeAfterNFailed != -1) && (msg00.FailedCounter >= msg00.DisposeAfterNFailed))
                {
                    // take it off the running loop
                    // 2) update counters in user file (if needed)
                    DSSwitch.appUser().Update_General(
                        msg00.UserId,
                        OnProcessedHandler.OnProcessed,
                        new OnProcessedHandler(msg00.Msg.Length, log4Email, !msg00.NoCounterUpdate),
                        OnProcessedHandler.PostProcess,
                        trayLog);

                    // 3) write msg file to disposed folder
                    DSSwitch.msgFile00().Store(niceSystem, msg00, Data_Net__00NormalMessage.eLocation.Disposed, trayLog);
                }
                else if (msg00.AliveSince().TotalHours > 12)
                {
                    // take it off the running loop after 12 hours of failure
                    // 2) update counters in user file (if needed)
                    DSSwitch.appUser().Update_General(
                        msg00.UserId,
                        OnProcessedHandler.OnProcessed,
                        new OnProcessedHandler(msg00.Msg.Length, log4Email, !msg00.NoCounterUpdate),
                        OnProcessedHandler.PostProcess,
                        trayLog);

                    // 3) write msg file to disposed folder
                    DSSwitch.msgFile00().Store(niceSystem, msg00, Data_Net__00NormalMessage.eLocation.Disposed, trayLog);

                    // 4) Inform the administrator about it
                    StringBuilder sbBody = new StringBuilder();
                    sbBody.AppendLine("Message removed after 12 hours of failure");
                    foreach (var s in msg00.ToFullString())
                    {
                        sbBody.AppendLine(s);
                    }
                    sbBody.AppendLine("You might consider blocking this user.");
                    EMail.SendGeneralEmail(null, true, "12 hours of failure", sbBody.ToString(), log4Email);
                }
                else
                {
                    // keep in running
                    DSSwitch.msgFile00().Store(niceSystem, msg00, Data_Net__00NormalMessage.eLocation.Queued, trayLog);
                }
            }
        }

        private void processCheckTelNumbersResult(Data_Net__05CheckTelNumbersResult _05, bool sendEmails)
        {
            // 1) delete the triggering file
            ASPTrayBase.MsgFileParts info = ASPTrayBase.s_MsgFile_GetPartsFromMessageFile(_05.RequestFileName);//mg check this
            Data_Net__04CheckTelNumbers msg04 = DSSwitch.msgFile04().ReadOne(niceSystem, _05.RequestFileName, trayLog);
            DSSwitch.msgFile04().Delete(niceSystem, _05.RequestFileName, trayLog);

            string niceEmail = null;
            bool sendWelcomeMessages = false;
            string sendWelcomeMessages_GUID = null;
            string sendWelcomeMessages_TelList = null;

            if ((_05.TelListOk == null) && (_05.TelListDoRetry == null) && (_05.TelListNotWorking == null))
            {
                // this is from directTel, so the first checked is not ok
                MobileNoHandler hOk = new MobileNoHandler(msg04.TelListChecked);
                MobileNoHandler hToDo = new MobileNoHandler(msg04.TelList);
                hOk.AddIfNew(hToDo.RemoveAndReturnFirst());

                _05.TelListOk = hOk.getVal;
                _05.TelListDoRetry = hToDo.getVal;
                _05.TelListNotWorking = "";
            }

            // 2) do retry if not all worked and non failed
            if ((_05.TelListDoRetry.Length > 0) && (_05.TelListNotWorking.Length == 0))
            {
                // early retry
                Data_Net__04CheckTelNumbers _04 = new Data_Net__04CheckTelNumbers(info.Email, DateTime.UtcNow.Ticks, _05.TelListDoRetry, _05.TelListOk);
                DSSwitch.msgFile04().Store(niceSystem, _04, trayLog);
            }
            else
            {
                // open the userfile and merge
                MobileNoHandler doAgain = new MobileNoHandler(_05.TelListDoRetry);
                DSSwitch.appUser().Update_General(info.Email, delegate(Data_AppUserFile user, Object args)
                {
                    //  action
                    niceEmail = user.Email;
                    foreach (string telGood in (new MobileNoHandler(_05.TelListOk)).MobileNumberArray)
                    {
                        user.MobileNumbers_AllConfirmed__.AddIfNew(telGood);
                        user.MobileNumbers_AllUnConfirmed__.Remove(telGood);
                    }
                    foreach (string telNotWorking in (new MobileNoHandler(_05.TelListNotWorking)).MobileNumberArray)
                    {
                        user.AddCommentLine(telNotWorking + " not working", true);
                        user.MobileNumbers_AllUnConfirmed__.Remove(telNotWorking);
                        user.MobileNumbers_AllConfirmed__.Remove(telNotWorking);
                    }
                    foreach (string telAgain in user.MobileNumbers_AllUnConfirmed__.MobileNumberArray)
                    {
                        doAgain.AddIfNew(telAgain);
                    }

                    if (doAgain.MobileNumbersCount == 0)
                    {
                        // nothing elso to check, so make a desision
                        if (user.AccountStatus == Data_AppUserFile.eUserStatus.verified_checkingTelNumbers)
                        {
                            if (user.MobileNumbers_AllConfirmed__.MobileNumbersCount > 0)
                            {
                                // we have good numbers, so upgrade
                                user.AccountStatus = Data_AppUserFile.eUserStatus.free_account;
                                if (sendEmails) EMail.SendJustActivated(user, log4Email);
                                sendWelcomeMessages = true;
                                sendWelcomeMessages_GUID = user.ApiGuId;
                                sendWelcomeMessages_TelList = user.MobileNumbers_AllConfirmed__.getVal;
                            }
                            else
                            {
                                // we have no valid numbers, so block 
                                user.AccountStatus = Data_AppUserFile.eUserStatus.blocked;
                                if (sendEmails) EMail.SendWrongTelRegistered(user, log4Email);
                            }
                        }
                        else if (user.AccountStatus == Data_AppUserFile.eUserStatus.free_account)
                        {
                            // this could be a free account number adding
                            if (sendEmails) EMail.SendJustActivated(user, log4Email);
                            sendWelcomeMessages = true;
                            sendWelcomeMessages_GUID = user.ApiGuId;
                            sendWelcomeMessages_TelList = user.MobileNumbers_AllConfirmed__.getVal;
                        }
                    }
                }, null, delegate(Object args)
                {
                    // post user file processing
                    if (doAgain.MobileNumbersCount != 0)
                    {
                        // still unprocessed numbers
                        Data_Net__04CheckTelNumbers _04 = new Data_Net__04CheckTelNumbers(info.Email, DateTime.UtcNow.Ticks, doAgain.getVal, "");
                        DSSwitch.msgFile04().Store(niceSystem, _04, trayLog);
                    }
                    if (sendWelcomeMessages)
                    {
                        MessageProcessing_API api = new MessageProcessing_API(sendWelcomeMessages_GUID);
                        string additionalInfo;
                        api.SendWhatsApp(
                            niceSystem,
                            sendWelcomeMessages_TelList,
                            "Welcome to NiceApi.net\r\n",
                            true,
                            trayLog,
                            out additionalInfo);
                    }
                }, trayLog);
            }
        }

        private void processScreenShotResult(Data_Net__03ScreenshotResult _03)
        {
            trayLog.Debug("ScreenshotResult " + _03.GetFileName());
            // 1) Delete the request file
            DSSwitch.msgFile02().Delete(niceSystem, _03.FileName, trayLog);
            // 2) Update the loopback file
            this.onScreenshot(_03.B64ScreenshotData, niceSystem);
        }
    }
}
