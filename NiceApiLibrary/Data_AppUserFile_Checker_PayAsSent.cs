using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    class Data_AppUserFile_Checker_PayAsSent : Data_AppUserFile_CheckerBase
    {
        Data_AppUserFile.payAsSentAccount payAsSent;

        public Data_AppUserFile_Checker_PayAsSent(Data_AppUserFile.payAsSentAccount payAsSent)
        {
            this.payAsSent = payAsSent;
        }

        public override string Info(Data_AppUserFile user)
        {
            return String.Format("Ps: {0} {1}",
                payAsSent.payAsSent_CurrentCredit.ToString(),
                payAsSent.payAsSent_LastMsgQueued.ToSwissTime(false));
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
                DateTime NotBefore = new DateTime(payAsSent.payAsSent_LastMsgQueued, DateTimeKind.Utc).AddSeconds(payAsSent.payAsSent_MinDelayInSeconds);
                if (DateTime.UtcNow < NotBefore)
                {
                    throw new ArgumentException("wait");
                }

                //3) check fonds
                Data_AppUserFile.niceMoney totalCost = payAsSent.payAsSent_CostPerMessage.MultiplyBy(telList.Length);
                if (payAsSent.payAsSent_CurrentCredit.ValueInUsCent < totalCost.ValueInUsCent)
                {
                    throw new ArgumentException("Not enouth fonds on account");
                }
                payAsSent.payAsSent_CurrentCredit.ValueInUsCent -= totalCost.ValueInUsCent;

                //4) update counters
                payAsSent.payAsSent_MsgSent += telList.Length;
                payAsSent.payAsSent_LastMsgQueued = DateTime.UtcNow.Ticks;
            }
        }

        public override bool FundManagement_CommitAddOneNumber(string telNumber)
        {
            Data_AppUserFile.niceMoney totalCost = payAsSent.payAsSent_CostPerNumber.MultiplyBy(1);
            if (payAsSent.payAsSent_CurrentCredit.ValueInUsCent < totalCost.ValueInUsCent)
            {
                // "Not enouth fonds on account"
                return false;
            }
            payAsSent.payAsSent_CurrentCredit.ValueInUsCent -= totalCost.ValueInUsCent;
            return true;
        }

        public override void CommitOrThrow_TelNumberAdd(MobileHandleConfUnconfList existing, MobileNoHandler noToAdd)
        {
            Data_AppUserFile.niceMoney totalCost = payAsSent.payAsSent_CostPerNumber.MultiplyBy(noToAdd.MobileNumbersCount);
            if (payAsSent.payAsSent_CurrentCredit.ValueInUsCent < totalCost.ValueInUsCent)
            {
                throw new ArgumentException("Not enouth fonds on account");
            }
            payAsSent.payAsSent_CurrentCredit.ValueInUsCent -= totalCost.ValueInUsCent;
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
            return payAsSent.payAsSent_MsgSent;
        }

        public override Int64 QueuedCount()
        {
            return 0;
        }

        public override int AccountImportance1()
        {
            int ret = (int)((-1) * payAsSent.payAsSent_CurrentCredit.ValueInUsCent);
            return ret;
        }
        public override int AccountImportance2()
        {
            return AccountImportance1();
        }
    }
}
