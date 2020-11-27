using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    class Data_AppUserFile_Checker_Free : Data_AppUserFile_CheckerBase
    {
        Data_AppUserFile.freeAccount free;

        public Data_AppUserFile_Checker_Free(Data_AppUserFile.freeAccount free)
        {
            this.free = free;
        }

        public override string Info(Data_AppUserFile user)
        {
            return String.Format("Fr: {0}", UsedInPercent());
        }

        public override void OnProcessed(Data_AppUserFile user, OnProcessedHandler args)
        {
            if (args.updateCounters)
            {
                free.free_MsgQueued--;
                free.free_MsgSent++;
            }

            if (user.AccountStatus == Data_AppUserFile.eUserStatus.verified_welcome_queued)
            {
                free.free_WelcomeCounter--;
                if (free.free_WelcomeCounter == 0)
                {
                    user.AccountStatus = Data_AppUserFile.eUserStatus.free_account;
                }
            }

            if (user.AccountStatus == Data_AppUserFile.eUserStatus.free_account)
            {
                if (
                    (user.FreeAccount.free_MsgSent == 160) ||   // 80%
                    (user.FreeAccount.free_MsgSent == 180) ||   // 90%
                    (user.FreeAccount.free_MsgSent == 200))
                {
                    args.free_doPostSendUpgradeRequestOnHighPercent = true;
                    args.free_User = user;
                }
            }
        }

        public override void OnPostProcessed(OnProcessedHandler args)
        {
            if (args.free_doPostSendUpgradeRequestOnHighPercent)
            {
                NiceApiLibrary.EMail.SendUpgradeRequestOnHighPercent(args.free_User, args.emailLog);
            }
        }

        public override void CommitOrThrow_Send(MobileNoHandler storedNumbers, string[] telList, bool isWelcomeMessage, int MessageLength, ref Data_AppUserFile.eUserStatus accountStatus, out bool sendFooter)
        {
            sendFooter = free.free_SendFooter;

            //1) 
            VerifyAllMobileNumbers(storedNumbers, telList);

            if (isWelcomeMessage)
            {
                if (accountStatus == Data_AppUserFile.eUserStatus.verified_welcome_No_sent)
                {
                    // the first welcome message
                    accountStatus = Data_AppUserFile.eUserStatus.verified_welcome_queued;
                    free.free_WelcomeCounter = (Int16)telList.Length;
                }
                else if (accountStatus == Data_AppUserFile.eUserStatus.verified_welcome_queued)
                {
                    // not the first welcome messages
                    free.free_WelcomeCounter += (Int16)telList.Length;
                }
            }
            else
            {
                //2) checkTimeDelay()
                DateTime NotBefore = new DateTime(free.free_LastMsgQueued, DateTimeKind.Utc).AddSeconds(free.free_MinDelayInSeconds);
                if (DateTime.UtcNow < NotBefore)
                {
                    throw new ArgumentException("wait");
                }

                //3) checkRemainingCounter()
                long need = telList.Length;
                long _is_ = free.free_MsgLeft;
                if (_is_ != -1)
                {
                    if (need > _is_)
                    {
                        throw new ArgumentException("Please arrange an account upgrade");
                    }
                }

                //4) check max message length
                if (MessageLength > 10240)
                {
                    throw new ArgumentException("Message too big");
                }

                //5) update counters
                free.free_MsgLeft -= need;
                free.free_MsgQueued += need;
                free.free_LastMsgQueued = DateTime.UtcNow.Ticks;
            }
        }

        public override void CommitOrThrow_TelNumberAdd(MobileHandleConfUnconfList existing, MobileNoHandler noToAdd)
        {
            throw new ArgumentException("Tel number manipulation not allowed");
        }
        public override void CommitOrThrow_TelNumberRemove(MobileHandleConfUnconfList existing, MobileNoHandler noToRemove)
        {
            throw new ArgumentException("Tel number manipulation not allowed");
        }

        public override bool FundManagement_CommitAddOneNumber(string telNumber)
        {
            // not allowed for free accounts
            return false;
        }

        public override int UsedInPercent()
        {
            try
            {
                decimal sent = free.free_MsgSent;
                decimal remaining = free.free_MsgLeft;
                decimal tot = sent + remaining;
                decimal factor = (sent) / tot;
                factor *= 100;
                return (int)factor;
            }
            catch
            {
                return 0;
            }
        }

        public override Int64 SentCount()
        {
            return free.free_MsgSent;
        }

        public override long QueuedCount()
        {
            return free.free_MsgQueued;
        }


        public static void VerifyAllMobileNumbers(MobileNoHandler storedNumbers, string[] telList)
        {
            MobileHandleConfUnconfList list = new MobileHandleConfUnconfList();
            list.Add(storedNumbers.getVal, true);
            foreach (string reqTel in telList)
            {
                if (!list.Conrtains(reqTel))
                {
                    throw new ArgumentException("X-APIMobile not configured");
                }
            }
        }

        public override int AccountImportance1()
        {
            int iUsage = UsedInPercent();
            if (iUsage == 100)
            {
                iUsage = 0 - iUsage;
            }
            return iUsage;
        }
        public override int AccountImportance2()
        {
            return (int)((DateTime.Now - (new DateTime(free.free_LastMsgQueued))).TotalHours);
        }
    }
}
