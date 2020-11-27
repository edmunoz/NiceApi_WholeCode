using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    class Data_AppUserFile_Checker_MonthlyDifPrice : Data_AppUserFile_CheckerBase
    {
        Data_AppUserFile.monthlyDifPriceAccount monthlyDifPrice;
        d_AddCommentLine commentLog;

        public Data_AppUserFile_Checker_MonthlyDifPrice(Data_AppUserFile.monthlyDifPriceAccount monthlyDifPrice, d_AddCommentLine commentLog)
        {
            this.monthlyDifPrice = monthlyDifPrice;
            this.commentLog = commentLog;
        }

        public override string Info(Data_AppUserFile user)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Md: ");
            if (user.MonthlyDifPriceAccount.HasExpired())
            {
                // expired
                TimeSpan ts = user.MonthlyDifPriceAccount.ExpiredSince();
                sb.AppendFormat("Expired {0:0}Days ", ts.TotalDays);
            }
            else
            {
                // active
                TimeSpan ts = user.MonthlyDifPriceAccount.TimeLeft();
                sb.AppendFormat("{0:0.0}DaysLeft ", ts.TotalDays);
            }
            sb.Append(user.MonthlyDifPriceAccount.monthlyDifPrice_CurrentCredit.ToString() + " ");
            sb.Append($"L{user.MonthlyDifPriceAccount.monthlyDifPrice_Level} ");
            sb.Append(user.MonthlyDifPriceAccount.monthlyDifPrice_LastMsgQueued.ToSwissTime(false));
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
                // 0) checkTimeDelay()
                DateTime NotBefore = new DateTime(monthlyDifPrice.monthlyDifPrice_LastMsgQueued, DateTimeKind.Utc)
                    .AddSeconds(monthlyDifPrice.monthlyDifPrice_MinDelayInSeconds);
                if (DateTime.UtcNow < NotBefore)
                {
                    throw new ArgumentException("wait");
                }

                PriceLevelHandler levelHandler = new PriceLevelHandler(monthlyDifPrice.monthlyDifPrice_LevelDefinitions);
                if (!levelHandler.Ok)
                {
                    throw new ArgumentException("Please check configuration. (NO)");
                }

                // 0) first initialisation
                if (monthlyDifPrice.monthlyDifPrice_Level == 0)
                {
                    Data_AppUserFile.niceMoney cost1 = levelHandler.Config.FirstOrDefault(_ => _.Level == 1).Cost;
                    if (!monthlyDifPrice.monthlyDifPrice_CurrentCredit.DeductIfEnoughFund(cost1))
                    {
                        throw new ArgumentException("Please arrange a top up. (L0)");
                    }
                    else
                    {
                        monthlyDifPrice.monthlDifPricey_PeriodeStart = DateTime.UtcNow.Ticks;
                        monthlyDifPrice.monthlyDifPrice_ThisMonthMsgSent = 0;
                        monthlyDifPrice.monthlyDifPrice_TotalMsgSent++;
                        monthlyDifPrice.monthlyDifPrice_Level = 1;
                        monthlyDifPrice.monthlyDifPrice_LastMsgQueued = DateTime.UtcNow.Ticks;
                        commentLog($"Deducted {cost1} for initialisation", true);
                    }
                }

                // 1) Check aktive, sonst eine Periode mehr und auf Level 1
                if (monthlyDifPrice.HasExpired())
                {
                    if (!monthlyDifPrice.monthlyDifPrice_AutoRenewMonthPayment)
                    {
                        throw new ArgumentException("Please arrange a top up. (AR)");
                    }
                    Data_AppUserFile.niceMoney cost1 = levelHandler.Config.FirstOrDefault(_ => _.Level == 1).Cost;
                    if (!monthlyDifPrice.monthlyDifPrice_CurrentCredit.DeductIfEnoughFund(cost1))
                    {
                        throw new ArgumentException("Please arrange a top up. (L1)");
                    }
                    else
                    {
                        monthlyDifPrice.monthlDifPricey_PeriodeStart = DateTime.UtcNow.Ticks;
                        monthlyDifPrice.monthlyDifPrice_ThisMonthMsgSent = 0;
                        monthlyDifPrice.monthlyDifPrice_TotalMsgSent++;
                        monthlyDifPrice.monthlyDifPrice_Level = 1;
                        monthlyDifPrice.monthlyDifPrice_LastMsgQueued = DateTime.UtcNow.Ticks;
                        commentLog($"Deducted {cost1} to renew Month", true);
                    }
                }

                // 2) Check messageCount and go a level up if needed
                var curLev = levelHandler.Config.FirstOrDefault(_ => _.Level == monthlyDifPrice.monthlyDifPrice_Level);
                if (curLev == null)
                {
                    // bad config
                    throw new ArgumentException($"Please check configuration. (BL) - {levelHandler}");
                }
                if (curLev.MaxMessages == monthlyDifPrice.monthlyDifPrice_ThisMonthMsgSent)
                {
                    // no more messages left
                    if (!monthlyDifPrice.monthlyDifPrice_AutoInceremntLevel)
                    {
                        throw new ArgumentException("Please arrange a top up. (AI)");
                    }
                    var nextLev = levelHandler.Config.FirstOrDefault(_ => _.Level == (monthlyDifPrice.monthlyDifPrice_Level + 1));
                    if (nextLev == null)
                    {
                        // no more levels
                        throw new ArgumentException("Please arrange a top up. (NL)");
                    }
                    Data_AppUserFile.niceMoney costDif = new Data_AppUserFile.niceMoney(nextLev.Cost.ValueInUsCent - curLev.Cost.ValueInUsCent);
                    if (!monthlyDifPrice.monthlyDifPrice_CurrentCredit.DeductIfEnoughFund(costDif))
                    {
                        throw new ArgumentException("Please arrange a top up. (LN)");
                    }
                    else
                    {
                        monthlyDifPrice.monthlyDifPrice_Level = nextLev.Level;
                        monthlyDifPrice.monthlyDifPrice_ThisMonthMsgSent++;
                        monthlyDifPrice.monthlyDifPrice_TotalMsgSent++;
                        monthlyDifPrice.monthlyDifPrice_LastMsgQueued = DateTime.UtcNow.Ticks;
                        commentLog($"Deducted {costDif} to enter level {nextLev.Level}", true);
                    }
                }
                else
                {
                    //ok to send one more message
                    monthlyDifPrice.monthlyDifPrice_LastMsgQueued = DateTime.UtcNow.Ticks;
                    monthlyDifPrice.monthlyDifPrice_ThisMonthMsgSent++;
                    monthlyDifPrice.monthlyDifPrice_TotalMsgSent++;
                }
            }
        }

        public override bool FundManagement_CommitAddOneNumber(string telNumber)
        {
            if (!monthlyDifPrice.monthlyDifPrice_CurrentCredit.DeductIfEnoughFund(monthlyDifPrice.monthlyDifPrice_CostPerNumber))
            {
                // "Not enouth fonds on account"
                return false;
            }
            this.commentLog("Adding " + telNumber, true);
            return true;
        }

        public override void CommitOrThrow_TelNumberAdd(MobileHandleConfUnconfList existing, MobileNoHandler noToAdd)
        {
            Data_AppUserFile.niceMoney totalCost = monthlyDifPrice.monthlyDifPrice_CostPerNumber.MultiplyBy(noToAdd.MobileNumbersCount);
            if (!monthlyDifPrice.monthlyDifPrice_CurrentCredit.DeductIfEnoughFund(totalCost))
            {
                throw new ArgumentException("Not enouth fonds on account");
            }
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
            return monthlyDifPrice.monthlyDifPrice_TotalMsgSent;
        }

        public override Int64 QueuedCount()
        {
            return 0;
        }

        public override int AccountImportance1()
        {
            int ret = 0;
            if (monthlyDifPrice.HasExpired())
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
            if (monthlyDifPrice.HasExpired())
            {
                // expired
                TimeSpan ts = monthlyDifPrice.ExpiredSince();
                ret = (int)((1) * ts.TotalHours);
            }
            else
            {
                // active
                TimeSpan ts = monthlyDifPrice.TimeLeft();
                ret = (int)((-1) * ts.TotalHours);
            }
            return ret;
        }
    }

}
