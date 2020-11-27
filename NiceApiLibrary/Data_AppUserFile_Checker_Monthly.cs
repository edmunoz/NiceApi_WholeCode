using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    class Data_AppUserFile_Checker_Monthly : Data_AppUserFile_CheckerBase
    {
        Data_AppUserFile.monthlyAccount monthly;
        d_AddCommentLine commentLog;

        public Data_AppUserFile_Checker_Monthly(Data_AppUserFile.monthlyAccount monthly, d_AddCommentLine commentLog)
        {
            this.monthly = monthly;
            this.commentLog = commentLog;
        }

        public override string Info(Data_AppUserFile user)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Mo: ");
            if (DateTime.UtcNow.Ticks > user.MonthlyAccount.monthly_PaidUntil)
            {
                // expired
                TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - user.MonthlyAccount.monthly_PaidUntil);
                sb.AppendFormat("Expired {0:0}Days ", ts.TotalDays);
            }
            else
            {
                // active
                TimeSpan ts = new TimeSpan(user.MonthlyAccount.monthly_PaidUntil - DateTime.UtcNow.Ticks);
                sb.AppendFormat("{0:0.0}DaysLeft ", ts.TotalDays);
            }
            sb.Append(user.MonthlyAccount.monthly_CurrentCredit.ToString() + " ");
            sb.Append(user.MonthlyAccount.monthly_LastMsgQueued.ToSwissTime(false));
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
                //2) checkTimeDelay()
                DateTime NotBefore = new DateTime(monthly.monthly_LastMsgQueued, DateTimeKind.Utc).AddSeconds(monthly.monthly_MinDelayInSeconds);
                if (DateTime.UtcNow < NotBefore)
                {
                    throw new ArgumentException("wait");
                }

                //3) check still active
                DateTime NotAfter = new DateTime(monthly.monthly_PaidUntil, DateTimeKind.Utc);
                if (DateTime.UtcNow > NotAfter)
                {
                    throw new ArgumentException("Please arrange a top up");
                }

                //4) update counters
                monthly.monthly_MsgSent += telList.Length;
                monthly.monthly_LastMsgQueued = DateTime.UtcNow.Ticks;
            }
        }

        public override bool FundManagement_CommitAddOneNumber(string telNumber)
        {
            Data_AppUserFile.niceMoney totalCost = monthly.monthly_CostPerNumber.MultiplyBy(1);
            if (monthly.monthly_CurrentCredit.ValueInUsCent < totalCost.ValueInUsCent)
            {
                // "Not enouth fonds on account"
                return false;
            }
            monthly.monthly_CurrentCredit.ValueInUsCent -= totalCost.ValueInUsCent;
            this.commentLog("Adding " + telNumber, true);
            return true;
        }

        public override void CommitOrThrow_TelNumberAdd(MobileHandleConfUnconfList existing, MobileNoHandler noToAdd)
        {
            Data_AppUserFile.niceMoney totalCost = monthly.monthly_CostPerNumber.MultiplyBy(noToAdd.MobileNumbersCount);
            if (monthly.monthly_CurrentCredit.ValueInUsCent < totalCost.ValueInUsCent)
            {
                throw new ArgumentException("Not enouth fonds on account");
            }
            monthly.monthly_CurrentCredit.ValueInUsCent -= totalCost.ValueInUsCent;
        }
        public override void CommitOrThrow_TelNumberRemove(MobileHandleConfUnconfList existing, MobileNoHandler noToRemove)
        {

        }


        public override int UsedInPercent()
        {
            return 99;
        }

        public override Int64 SentCount()
        {
            return monthly.monthly_MsgSent;
        }

        public override Int64 QueuedCount()
        {
            return 0;
        }

        public override int AccountImportance1()
        {
            int ret = 0;
            if (DateTime.UtcNow.Ticks > this.monthly.monthly_PaidUntil)
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
            if (DateTime.UtcNow.Ticks > this.monthly.monthly_PaidUntil)
            {
                // expired
                TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - this.monthly.monthly_PaidUntil);
                ret = (int)((1) * ts.TotalHours);
            }
            else
            {
                // active
                TimeSpan ts = new TimeSpan(this.monthly.monthly_PaidUntil - DateTime.UtcNow.Ticks);
                ret = (int)((-1) * ts.TotalHours);
            }
            return ret;
        }
    }
}
