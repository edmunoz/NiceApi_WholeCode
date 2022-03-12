using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    /// <summary>
    /// API to create the requests (to create the files that will be sent to the trayApp)
    /// </summary>
    public class MessageProcessing_API
    {
        private string Email;
        private string APIId;
        private DateTime timeNow;

        enum eTelNum_Instruction
        {
            Add,
            Remove,
            ShowConfirmed,
            ShowUnconfirmed,
            Show
        }
        public MessageProcessing_API(string APIId)
        {
            Init(APIId, false);
        }

        public MessageProcessing_API(string data, bool dataIsEmail)
        {
            Init(data, dataIsEmail);
        }

        private void Init(string data, bool dataIsEmail)
        {
            if (dataIsEmail)
            {
                this.Email = data;
                this.APIId = null;
            }
            else
            {
                this.Email = Data_AppUserFile.API_IdToEmail(data);
                this.APIId = data;
            }
        }

        public string SendWhatsApp(NiceSystemInfo niceSystem, string XAPIMobile, string Message, IMyLog log)
        {
            string fileName;
            return SendWhatsApp(niceSystem, XAPIMobile, Message, false, log, out fileName);
        }
        public string SendWhatsApp(NiceSystemInfo niceSystem, string XAPIMobile, string Message, bool isWelcomeMessage, IMyLog log, out string fileName)
        {
            fileName = "";
            string[] requTelList = null;
            string cleanMessage = null;
            bool sendFooter = true;
            bool deleteOnFailed = false;
            try
            {
                timeNow = DateTime.UtcNow;
                // 1 extract (check) input data
#region 1 extract (check) input data
                if (string.IsNullOrEmpty(APIId))
                {
                    throw new ArgumentException("X-APIId missing");
                }

                if (string.IsNullOrEmpty(XAPIMobile))
                {
                    throw new ArgumentException("X-APIMobile missing");
                }

                requTelList = XAPIMobile.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < requTelList.Length; i++)
                {
                    requTelList[i] = "+" + requTelList[i];
                }

                cleanMessage = Message.Replace(":SE", ":  SE").Replace(": SE", ":  SE");
                if (string.IsNullOrEmpty(cleanMessage))
                {
                    throw new ArgumentException("No POST data");
                }
#endregion

                // 2 load user file and check guid
#region 2 load user file and check guid
                string email = Data_AppUserFile.API_IdToEmail(APIId);
                if (email == null)
                {
                    throw new ArgumentException("X-APIId unknown");
                }
                DSSwitch.appUser().Update_General(email, delegate(Data_AppUserFile user, Object args) 
                {
                    if (user == null)
                    {
                        throw new ArgumentException("X-APIId unknown");
                    }
                    if (!user.IsAccountActive(cleanMessage))
                    {
                        throw new ArgumentException("Account not active (7). " + user.AccountStatusExplained());
                    }
                    if (user.ApiGuId != APIId)
                    {
                        throw new ArgumentException("X-APIId unknown");
                    }
#endregion

                    // 3 action
                    user.CommitOrThrow_Send(requTelList, isWelcomeMessage, cleanMessage.Length, ref user.AccountStatus, out sendFooter);
                    deleteOnFailed = user.DeleteOnFailed;
                }, null, null, log);

                // 5) createMessageFile
                bool noCounterUpdate = false;
                if (Message.StartsWith("__NoSend"))
                {
                    noCounterUpdate = true;
                }
                else if (Message == "Welcome\r\n")
                {
                    noCounterUpdate = true;
                }
                else if (Message == "Welcome to NiceApi.net\r\n")
                {
                    noCounterUpdate = true;
                }

                int telNoId = 0;
                foreach (string tel1 in requTelList)
                {
                    fileName += 
                    createMessageFile(
                        niceSystem,
                        telNoId,
                        requTelList[telNoId],
                        cleanMessage,
                        email,
                        timeNow,
                        log, 
                        noCounterUpdate,
                        sendFooter,
                        deleteOnFailed ?
                        1 : -1) + ", ";
                    // move on
                    telNoId++;
                }

                if (telNoId == 1)
                {
                    return "queued";
                }
                else
                {
                    return "queued x" + telNoId.ToString();
                }
            }
            catch (ArgumentException ae)
            {
                return ae.Message;
            }
        }

        private string createMessageFile(NiceSystemInfo niceSystem, int telId, string telNumber, string cleanMessage, string email, DateTime utcNow, IMyLog log, bool noCounterUpdate, bool sendFooter, Int32 MaxFailBeforeDispose)
        {
            Data_Net__00NormalMessage msg = new Data_Net__00NormalMessage(
                Data_AppUserFile.EmailSaveChars(email),
                "zapi_" + telNumber,
                utcNow.Ticks + (Int64)telId,
                sendFooter ?
                    cleanMessage + "\r\nSent via NiceApi.net\r\n" :
                    cleanMessage + "\r\n",
                    0,
                    MaxFailBeforeDispose,
                    noCounterUpdate);
            DSSwitch.msgFile00().Store(niceSystem, msg, Data_Net__00NormalMessage.eLocation.Queued, log);
            return msg.GetFileName();
        }

        public string Process_TelNumAPI(NiceSystemInfo niceSystem, string XAPIInstruction, string Message, IMyLog log)
        {
            StringBuilder sbRet = new StringBuilder();
            try
            {
                timeNow = DateTime.UtcNow;
                // 1 extract (check) input data
#region 1 extract input data
                if (string.IsNullOrEmpty(APIId))
                {
                    throw new ArgumentException("X-APIId missing");
                }
                eTelNum_Instruction telNum_Instruction = castInstractionOrThrow(XAPIInstruction);
                MobileNoHandler telNum_Handler = new MobileNoHandler(Message.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", ""));
#endregion

                // 2 load user file and check guid
#region 2 load user file and check guid
                if (Email == null)
                {
                    throw new ArgumentException("X-APIId unknown");
                }
                MobileNoHandler telAddToCheckingFile = new MobileNoHandler("");
                DSSwitch.appUser().Update_General(Email, delegate(Data_AppUserFile user, Object args)
                {
                    if (user == null)
                    {
                        throw new ArgumentException("X-APIId unknown");
                    }
                    if (user.ApiGuId != APIId)
                    {
                        throw new ArgumentException("X-APIId unknown");
                    }
                    if (!user.IsAccountActive(""))
                    {
                        throw new ArgumentException("Account not active (8). " + user.AccountStatusExplained());
                    }
                    if (!user.AddNumber_AllowedWithAPI)
                    {
                        throw new ArgumentException("Not allowed for this account");
                    }
#endregion

                    // 3 action format retString according to telNum_Instruction
                    #region 3 action format retString according to telNum_Instruction
                    switch (telNum_Instruction)
                    {
                        case eTelNum_Instruction.ShowConfirmed:
                            sbRet.Append(user.MobileNumbers_AllConfirmed__.getVal);
                            break;

                        case eTelNum_Instruction.ShowUnconfirmed:
                            sbRet.Append(user.MobileNumbers_AllUnConfirmed__.getVal);
                            break;

                        case eTelNum_Instruction.Show:
                            sbRet.AppendLine("Plan: " + user.AccountStatus.ToString());
                            sbRet.AppendLine(String.Format("Counter: {0} / {1}", user.MobileNumbers_AllConfirmed__.MobileNumbersCount, user.MobileNumbers_AllUnConfirmed__.MobileNumbersCount));
                            sbRet.AppendLine("Confirmed: " + user.MobileNumbers_AllConfirmed__.getVal);
                            sbRet.AppendLine("Unconfirmed: " + user.MobileNumbers_AllUnConfirmed__.getVal);
                            break;

                        case eTelNum_Instruction.Add:
                            {
                                MobileNoHandler XTelList = new MobileNoHandler(Message, true);

                                foreach (string tel1 in XTelList.MobileNumberArray)
                                {
                                    if (user.MobileNumbers_AllConfirmed__.Contains(tel1))
                                    {
                                        sbRet.AppendLine(tel1 + " is already confirmed.");
                                    }
                                    else if (user.MobileNumbers_AllUnConfirmed__.Contains(tel1))
                                    {
                                        sbRet.AppendLine(tel1 + " is already on the list.");
                                    }
                                    else if (!user.GetCheckerBase(true).FundManagement_CommitAddOneNumber(tel1))
                                    {
                                        sbRet.AppendLine("Not enough funds to add " + tel1 + ".");
                                    }
                                    else
                                    {
                                        sbRet.AppendLine(tel1 + " added.");
                                        user.MobileNumbers_AllUnConfirmed__.Add(tel1);
                                        telAddToCheckingFile.Add(tel1);
                                    }
                                }
                            }
                            break;

                        case eTelNum_Instruction.Remove:
                            throw new ArgumentException("Remove not allowed for this account");
                            //break;

                        default:
                            throw new ArgumentException("Internel Error");
                    }
                    #endregion
                }, null, delegate(Object args) 
                {
                    if (telAddToCheckingFile.MobileNumbersCount > 0)
                    {
                        Data_Net__04CheckTelNumbers _04 = new Data_Net__04CheckTelNumbers(Email, timeNow.Ticks, telAddToCheckingFile.getVal, "");
                        DSSwitch.msgFile04().Store(niceSystem, _04, log);
                    }
                }, log);
            }
            catch (ArgumentException ae)
            {
                sbRet = new StringBuilder();
                sbRet.Append(ae.Message);
            }
            catch (Exception)
            {
                sbRet = new StringBuilder();
                sbRet.Append("ERROR");
            }
            return sbRet.ToString(); ;
        }

        public string Process_MGUseAddTelToFreeAccounts(NiceSystemInfo niceSystem, MobileNoHandler XTelList, IMyLog log)
        {
            return Process_MGUseAddTelToFreeAccounts(niceSystem, XTelList, 5, log);
        }
        public string Process_MGUseAddTelToFreeAccounts(NiceSystemInfo niceSystem, MobileNoHandler XTelList, int maxNumbers, IMyLog log)
        {
            StringBuilder sbInfo = new StringBuilder();
            MobileNoHandler telForCheckFile = new MobileNoHandler("");
            try
            {
                timeNow = DateTime.UtcNow;
                // 1 extract (check) input data
                #region 1 extract (check) input data
                if (string.IsNullOrEmpty(Email))
                {
                    throw new ArgumentException("X-APIId missing");
                }
                #endregion

                // 2 load user file and check guid
                #region 2 load user file and check guid
                DSSwitch.appUser().Update_General(Email, delegate(Data_AppUserFile user, Object args)
                {
                    if (user == null)
                    {
                        throw new ArgumentException("X-APIId unknown");
                    }
                #endregion

                    // 3 action, add up to 5 numbers and produce telCheck file
                    #region 3 action, add up to 5 numbers and produce telCheck file
                    if (user.AccountStatus == Data_AppUserFile.eUserStatus.blocked)
                    {
                        // unblock, as this is an instraction from the admin
                        user.AccountStatus = Data_AppUserFile.eUserStatus.free_account;
                    }

                    if (user.AccountStatus != Data_AppUserFile.eUserStatus.free_account)
                    {
                        throw new ArgumentException("This applies only to free_account. Not to " + user.AccountStatus.ToString());
                    }
                    if ((
                        user.MobileNumbers_AllConfirmed__.MobileNumbersCount +
                        user.MobileNumbers_AllUnConfirmed__.MobileNumbersCount +
                        XTelList.MobileNumbersCount) > maxNumbers)
                    {
                        throw new ArgumentException("Only up to 5 numbers allowed");
                    }
                    foreach (string tel1 in XTelList.MobileNumberArray)
                    {
                        user.MobileNumbers_AllUnConfirmed__.Add(tel1);
                        telForCheckFile.Add(tel1);
                        sbInfo.AppendFormat("Added {0}\r\n", tel1);
                    }
                    sbInfo.AppendFormat("Now {0} # registered\r\n", (user.MobileNumbers_AllConfirmed__.MobileNumbersCount + user.MobileNumbers_AllUnConfirmed__.MobileNumbersCount));
                    #endregion
                }, null, delegate(Object args) 
                // 4 post user file handling, create telCheck file
                {
                    Data_Net__04CheckTelNumbers _04 = new Data_Net__04CheckTelNumbers(Email, timeNow.Ticks, telForCheckFile.getVal, "");
                    DSSwitch.msgFile04().Store(niceSystem, _04, log);
                    sbInfo.AppendFormat("File {0} created\r\n", _04.GetFileName());
                }, log);
            }
            catch (ArgumentException ae)
            {
                sbInfo.Append(ae.Message);
            }
            catch (Exception)
            {
                sbInfo.Append("ERROR");
            }
            return sbInfo.ToString();
        }

        public string Process_CreateCheckTelFileForTestAccount(NiceSystemInfo niceSystem, MobileNoHandler XTelList, IMyLog log)
        {
            APIId = null;
            StringBuilder sbInfo = new StringBuilder();
            MobileNoHandler telForCheckFile = XTelList;
            try
            {
                timeNow = DateTime.UtcNow;

                string email = "edmunozg@gmail.com"; 
                Data_Net__04CheckTelNumbers _04 = new Data_Net__04CheckTelNumbers(email, timeNow.Ticks, telForCheckFile.getVal, "");
                DSSwitch.msgFile04().Store(niceSystem, _04, log);
                sbInfo.AppendFormat("File {0} created\r\n", _04.GetFileName());
            }
            catch (ArgumentException ae)
            {
                sbInfo.Append(ae.Message);
            }
            catch (Exception)
            {
                sbInfo.Append("ERROR");
            }
            return sbInfo.ToString();
        }

        public string Process_Registration_JustVerified(NiceSystemInfo niceSystem, out bool ok, bool sendAdminNotification, bool sendAdminNotificationToWhatsapp, IMyLog log, LogForEmailSend log4Email)
        {
            StringBuilder sbInfo = new StringBuilder();
            string telToCheck = null;
            ok = false;
            string friendlyEmail = null;
            bool okIntern = false;
            try
            {
                timeNow = DateTime.UtcNow;
                // 1 extract (check) input data
                if (string.IsNullOrEmpty(APIId))
                {
                    throw new ArgumentException("X-APIId missing");
                }
                
                // 2 load user file and check guid
                string email = Data_AppUserFile.API_IdToEmail(APIId);
                if (email == null)
                {
                    throw new ArgumentException("X-APIId unknown");
                }
                DSSwitch.appUser().Update_General(email, delegate(Data_AppUserFile user, Object args)
                {
                    if (user == null)
                    {
                        throw new ArgumentException("X-APIId unknown");
                    }
                    if (user.ApiGuId != APIId)
                    {
                        throw new ArgumentException("X-APIId unknown");
                    }
                    if (user.AccountStatus != Data_AppUserFile.eUserStatus.email_sent_for_verification)
                    {
                        throw new ArgumentException("AccountStatus is wrong");
                    }

                    // 3 action, produce telCheck file and update AccountStatus
                    user.AccountStatus = Data_AppUserFile.eUserStatus.verified_checkingTelNumbers;
                    telToCheck = user.MobileNumbers_AllConfirmed__.getVal + user.MobileNumbers_AllUnConfirmed__.getVal;
                    friendlyEmail = user.Email;
                    okIntern = true;
                }, null, delegate(Object args)
                {
                    // post process
                    if (log != null)
                    {
                        log.Info(friendlyEmail + " just verified his email");
                    }
                    if (sendAdminNotification)
                    {
                        EMail.SendAdminNotification(friendlyEmail + " just verified his email", log4Email);
                    }
                    if (sendAdminNotificationToWhatsapp)
                    {
                        NotificationInfo notificationInfo = DSSwitch.full().GetNotificationInfo();
                        NiceSystemInfo notifyToUse = DSSwitch.full().GetSystems(false).FirstOrDefault(s => s.Name == notificationInfo.Name);
                        if (notifyToUse != null)
                        {
                            Data_Net__00NormalMessage msg = new Data_Net__00NormalMessage(
                                Data_AppUserFile.API_IdToEmail(notifyToUse.APIId),
                                "zapi_" + notificationInfo.RxTel,
                                DateTime.UtcNow.Ticks,
                                friendlyEmail + " just verified his email",
                                0, 10, true);
                            DSSwitch.msgFile00().Store(notifyToUse, msg, Data_Net__00NormalMessage.eLocation.Queued, log);
                        }
                    }
                    Data_Net__04CheckTelNumbers _04 = new Data_Net__04CheckTelNumbers(friendlyEmail, timeNow.Ticks, telToCheck, "");
                    DSSwitch.msgFile04().Store(niceSystem, _04, log);
                }, log);
            }
            catch (ArgumentException ae)
            {
                sbInfo.Append(ae.Message);
            }
            catch (Exception)
            {
                sbInfo.Append("ERROR");
            }
            ok = okIntern;
            return sbInfo.ToString();
        }

        public string Process_Registration(NiceSystemInfo niceSystem, out bool fileArleadyUsed, bool sendActivationEmail, string UserName, string MoblieAllNumbers, string Email, string Password, string CreationIp, string WhereHeardText, IMyLog log, LogForEmailSend log4Email)
        {
            StringBuilder sbInfo = new StringBuilder();
            fileArleadyUsed = true;
            try
            {
                timeNow = DateTime.UtcNow;
                Data_AppUserFile ud = Data_AppUserFile.Create(
                    UserName, true, MoblieAllNumbers, Email, Password, CreationIp);
                sbInfo.Append(ud.ApiGuId);
                DSSwitch.appUser().StoreNew(ud, out fileArleadyUsed, log);
                if (!fileArleadyUsed)
                {
                    if (sendActivationEmail) EMail.SendRegisterActivation(ud, log4Email);
                    if (log != null) log.Info(ud.Email + " account created");
                    if (log != null) log.Info("WhereHeard " + WhereHeardText);
                }
            }
            catch (ArgumentException ae)
            {
                sbInfo.Append(ae.Message);
            }
            catch (Exception)
            {
                sbInfo.Append("ERROR");
            }
            return sbInfo.ToString();
        }

        private eTelNum_Instruction castInstractionOrThrow(string XAPIInstruction)
        {
            if (XAPIInstruction == eTelNum_Instruction.Add.ToString())
            {
                return eTelNum_Instruction.Add;
            }
            else if (XAPIInstruction == eTelNum_Instruction.Remove.ToString())
            {
                return eTelNum_Instruction.Remove;
            }
            else if (XAPIInstruction == eTelNum_Instruction.ShowConfirmed.ToString())
            {
                return eTelNum_Instruction.ShowConfirmed;
            }
            else if (XAPIInstruction == eTelNum_Instruction.ShowUnconfirmed.ToString())
            {
                return eTelNum_Instruction.ShowUnconfirmed;
            }
            else if (XAPIInstruction == eTelNum_Instruction.Show.ToString())
            {
                return eTelNum_Instruction.Show;
            }
            else
            {
                throw new ArgumentException("X-APIInstruction: missing or wrong");
            }
        }
    }
}