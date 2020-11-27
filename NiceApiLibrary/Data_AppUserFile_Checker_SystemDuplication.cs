using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    class Data_AppUserFile_Checker_SystemDuplication : Data_AppUserFile_CheckerBase
    {
        Data_AppUserFile.systemDuplicationAccount duplicationAccount;
        d_AddCommentLine commentLog;

        public Data_AppUserFile_Checker_SystemDuplication(Data_AppUserFile.systemDuplicationAccount duplicationAccount, d_AddCommentLine commentLog)
        {
            this.duplicationAccount = duplicationAccount;
            this.commentLog = commentLog;
        }

        public override string Info(Data_AppUserFile user)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SD: ");
            if (DateTime.UtcNow.Ticks > duplicationAccount.systemDuplication_PaidUntil)
            {
                // expired
                TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - duplicationAccount.systemDuplication_PaidUntil);
                sb.AppendFormat("Expired {0:0}Days ", ts.TotalDays);
            }
            else
            {
                // active
                TimeSpan ts = new TimeSpan(duplicationAccount.systemDuplication_PaidUntil - DateTime.UtcNow.Ticks);
                sb.AppendFormat("{0:0.0}DaysLeft ", ts.TotalDays);
            }
            sb.Append(user.MonthlyAccount.monthly_CurrentCredit.ToString() + " ");
            sb.Append(duplicationAccount.systemDuplication_LastMsgQueued.ToSwissTime(false));
            return sb.ToString();
        }

        public override void OnProcessed(Data_AppUserFile user, OnProcessedHandler args) { }
        public override void OnPostProcessed(OnProcessedHandler args) { }

        public override void CommitOrThrow_Send(MobileNoHandler storedNumbers, string[] telList, bool isWelcomeMessage, int MessageLength, ref Data_AppUserFile.eUserStatus accountStatus, out bool sendFooter)
        {
            sendFooter = false;

            //1) 
            Data_AppUserFile_Checker_Free.VerifyAllMobileNumbers(storedNumbers, telList);

            if (isWelcomeMessage)
            {
                throw new ArgumentException("Wrong account");
            }
            else
            {
                //3) check still active
                DateTime NotAfter = new DateTime(duplicationAccount.systemDuplication_PaidUntil, DateTimeKind.Utc);
                if (DateTime.UtcNow > NotAfter)
                {
                    throw new ArgumentException("Please arrange a top up");
                }

                //4) update counters
                duplicationAccount.systemDuplication_MsgSent += telList.Length;
                duplicationAccount.systemDuplication_LastMsgQueued = DateTime.UtcNow.Ticks;
            }
        }

        public override bool FundManagement_CommitAddOneNumber(string telNumber)
        {
            this.commentLog("Adding " + telNumber, true);
            return true;
        }

        public override void CommitOrThrow_TelNumberAdd(MobileHandleConfUnconfList existing, MobileNoHandler noToAdd)
        {
        }
        public override void CommitOrThrow_TelNumberRemove(MobileHandleConfUnconfList existing, MobileNoHandler noToRemove)
        {
        }

        public override int UsedInPercent()
        {
            return 88;
        }

        public override Int64 SentCount()
        {
            return duplicationAccount.systemDuplication_MsgSent;
        }

        public override Int64 QueuedCount()
        {
            return 0;
        }

        public override int AccountImportance1()
        {
            int ret = 0;
            if (DateTime.UtcNow.Ticks > this.duplicationAccount.systemDuplication_PaidUntil)
            {
                // expired
                ret = -1;
            }
            else
            {
                // active
                ret = 1;
            }
            return ret;
        }
        public override int AccountImportance2()
        {
            int ret = 0;
            if (DateTime.UtcNow.Ticks > this.duplicationAccount.systemDuplication_PaidUntil)
            {
                // expired
                TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - this.duplicationAccount.systemDuplication_PaidUntil);
                ret = (int)((1) * ts.TotalHours);
            }
            else
            {
                // active
                TimeSpan ts = new TimeSpan(this.duplicationAccount.systemDuplication_PaidUntil - DateTime.UtcNow.Ticks);
                ret = (int)((-1) * ts.TotalHours);
            }
            return ret;
        }
    }
}
