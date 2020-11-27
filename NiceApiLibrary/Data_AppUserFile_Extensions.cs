using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public static class Data_AppUserFile_high
    {
        public static void AddCommentLine(this Data_AppUserFile appUserFile, string text, bool withTime)
        {
            if (appUserFile.Comment.Length > 0)
            {
                appUserFile.Comment += "\r\n";
            }
            if (withTime)
            {
                appUserFile.Comment += DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss: ");
            }
            appUserFile.Comment += text;
        }

        public static Data_AppUserFile_CheckerBase GetCheckerBase(this Data_AppUserFile appUserFile, bool returnDefaultChecker = false)
        {
            switch (appUserFile.AccountStatus)
            {
                case Data_AppUserFile.eUserStatus.verified_welcome_No_sent:
                case Data_AppUserFile.eUserStatus.verified_welcome_queued:
                case Data_AppUserFile.eUserStatus.free_account:
                    return new Data_AppUserFile_Checker_Free(appUserFile.FreeAccount);
                case Data_AppUserFile.eUserStatus.verified_checkingTelNumbers:
                    return new Data_AppUserFile_Checker_Default();
                case Data_AppUserFile.eUserStatus.commercial_monthly:
                    return new Data_AppUserFile_Checker_Monthly(appUserFile.MonthlyAccount, appUserFile.AddCommentLine);
                case Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice:
                    return new Data_AppUserFile_Checker_MonthlyDifPrice(appUserFile.MonthlyDifPriceAccount, appUserFile.AddCommentLine);
                case Data_AppUserFile.eUserStatus.commercial_payassent:
                    return new Data_AppUserFile_Checker_PayAsSent(appUserFile.PayAsSentAccount);
                case Data_AppUserFile.eUserStatus.commercial_systemDuplication:
                    return new Data_AppUserFile_Checker_SystemDuplication(appUserFile.SystemDuplicationAccount, appUserFile.AddCommentLine);
                default:
                    if (returnDefaultChecker)
                    {
                        return new Data_AppUserFile_Checker_Default();
                    }
                    else
                    {
                        throw new ArgumentException("Account not active (2). " + appUserFile.AccountStatusExplained());
                    }
            }
        }

        public static string CommitOrThrow_TelNumberAdd(this Data_AppUserFile appUserFile, string TelList)
        {
            Data_AppUserFile_CheckerBase checker = appUserFile.GetCheckerBase();
            if (checker == null)
            {
                throw new ArgumentException("Account not active (3). " + appUserFile.AccountStatusExplained());
            }
            MobileNoHandler noToAdd = new MobileNoHandler(TelList);
            MobileHandleConfUnconfList existing = new MobileHandleConfUnconfList();
            existing.Add(appUserFile.MobileNumbers_AllConfirmed__.getVal, true);
            existing.Add(appUserFile.MobileNumbers_AllUnConfirmed__.getVal, false);
            checker.CommitOrThrow_TelNumberAdd(existing, noToAdd);

            existing.Add(TelList, false);
            string retString;
            existing.SortAndReturn(out appUserFile.MobileNumbers_AllConfirmed__, out appUserFile.MobileNumbers_AllUnConfirmed__, out retString);
            return retString;
        }

        public static string CommitOrThrow_TelNumberRemove(this Data_AppUserFile appUserFile, string TelList)
        {
            Data_AppUserFile_CheckerBase checker = appUserFile.GetCheckerBase();
            if (checker == null)
            {
                throw new ArgumentException("Account not active (4). " + appUserFile.AccountStatusExplained());
            }
            MobileNoHandler noToRemove = new MobileNoHandler(TelList);
            MobileHandleConfUnconfList existing = new MobileHandleConfUnconfList();
            existing.Add(appUserFile.MobileNumbers_AllConfirmed__.getVal, true);
            existing.Add(appUserFile.MobileNumbers_AllUnConfirmed__.getVal, false);
            checker.CommitOrThrow_TelNumberRemove(existing, noToRemove);

            existing.Remove(TelList);
            string retString;
            existing.SortAndReturn(out appUserFile.MobileNumbers_AllConfirmed__, out appUserFile.MobileNumbers_AllUnConfirmed__, out retString);
            return retString;
        }

        public static void CommitOrThrow_Send(this Data_AppUserFile appUserFile, string[] telList, bool isWelcomeMessage, int MessageLength, ref Data_AppUserFile.eUserStatus accountStatus, out bool sendFooter)
        {
            sendFooter = true;
            Data_AppUserFile_CheckerBase checker = appUserFile.GetCheckerBase();
            if (checker == null)
            {
                throw new ArgumentException("Account not active (5). " + appUserFile.AccountStatusExplained());
            }
            checker.CommitOrThrow_Send(appUserFile.MobileNumbers_AllConfirmed__, telList, isWelcomeMessage, MessageLength, ref appUserFile.AccountStatus, out sendFooter);
        }

        public static String AccountStatusExplained(this Data_AppUserFile appUserFile)
        {
            switch (appUserFile.AccountStatus)
            {
                case Data_AppUserFile.eUserStatus.email_sent_for_verification:
                    return "An email has beed sent to you to verify your email address.";
                case Data_AppUserFile.eUserStatus.verified_welcome_No_sent:
                    return "Your email address is verified. Please allow us 48 hours to activate your account.";
                case Data_AppUserFile.eUserStatus.verified_welcome_queued:
                    return "Your email address is verified. We have queued a welcome message for you.";
                case Data_AppUserFile.eUserStatus.verified_checkingTelNumbers:
                    return "Your email address is verified. We are verifying your numbers.";
                case Data_AppUserFile.eUserStatus.blocked:
                    return "Your account has been blocked. Please contact support to resolve the issue.";
                case Data_AppUserFile.eUserStatus.free_account:
                    return "Your account is active";
                case Data_AppUserFile.eUserStatus.commercial_monthly:
                    return "Your commercial account is active";
                case Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice:
                    return "Your commercial account is active";
                case Data_AppUserFile.eUserStatus.commercial_payassent:
                    return "Your commercial account is active";
                case Data_AppUserFile.eUserStatus.commercial_systemDuplication:
                    return "Your commercial account is active";
                default:
                    return "Account not active (6)";
            }
        }

        public static bool IsAccountActive(this Data_AppUserFile appUserFile, string whatToSend)
        {
            if (appUserFile.AccountStatus == Data_AppUserFile.eUserStatus.free_account)
            {
                return true;
            }
            if (appUserFile.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthly)
            {
                return true;
            }
            if (appUserFile.AccountStatus == Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice)
            {
                return true;
            }
            if (appUserFile.AccountStatus == Data_AppUserFile.eUserStatus.commercial_payassent)
            {
                return true;
            }
            if (appUserFile.AccountStatus == Data_AppUserFile.eUserStatus.commercial_systemDuplication)
            {
                return true;
            }
            if ((appUserFile.AccountStatus == Data_AppUserFile.eUserStatus.verified_welcome_No_sent) || (appUserFile.AccountStatus == Data_AppUserFile.eUserStatus.verified_welcome_queued))
            {
                if (whatToSend.Contains("Welcome"))
                {
                    return true;
                }
            }
            return false;
        }

        // use as default the confirmed numbers
        public static string MobileNumberX(this Data_AppUserFile appUserFile, int id)
        {
            return appUserFile.MobileNumbers_AllConfirmed__.MobileNumberX(id);
        }

        public static int MobileNumbersCount(this Data_AppUserFile appUserFile)
        {
            return appUserFile.MobileNumbers_AllConfirmed__.MobileNumbersCount;
        }

        public static string[] MobileNumberArray(this Data_AppUserFile appUserFile)
        {
            return appUserFile.MobileNumbers_AllConfirmed__.MobileNumberArray;
        }

        public static string MainTelNo(this Data_AppUserFile appUserFile)
        {
            if (appUserFile.MobileNumbers_AllConfirmed__.MobileNumbersCount > 0)
            {
                return appUserFile.MobileNumbers_AllConfirmed__.MainTelNo;
            }
            return appUserFile.MobileNumbers_AllUnConfirmed__.MainTelNo;
        }

        public static string CounteyCode(this Data_AppUserFile appUserFile)
        {
            if (appUserFile.MobileNumbers_AllConfirmed__.MobileNumbersCount > 0)
            {
                return appUserFile.MobileNumbers_AllConfirmed__.CounteyCode;
            }
            return appUserFile.MobileNumbers_AllUnConfirmed__.CounteyCode;
        }

        public static string CreationDate_Utc(this Data_AppUserFile appUserFile)
        {
            DateTime dtU = new DateTime(appUserFile.CreationDate, DateTimeKind.Utc);
            string r = dtU.ToString("dd/MM/yyyy HH:mm:ss");
            return r;
        }

        public static string CreationDate_Local(this Data_AppUserFile appUserFile)
        {
            DateTime dtU = new DateTime(appUserFile.CreationDate, DateTimeKind.Utc);
            string r = dtU.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
            return r;
        }

        public static string CreationDate_Br(this Data_AppUserFile appUserFile)
        {
            DateTime dtU = new DateTime(appUserFile.CreationDate, DateTimeKind.Utc);
            string r = dtU.AddHours(-3).ToString("dd/MM/yyyy HH:mm:ss");
            return r;
        }

        public static string CreationDate_Br_Short(this Data_AppUserFile appUserFile)
        {
            DateTime dtU = new DateTime(appUserFile.CreationDate, DateTimeKind.Utc);
            string r = dtU.AddHours(-3).ToString("dd/MM/yyyy");
            return r;
        }

        public static int UsedInPercent(this Data_AppUserFile appUserFile)
        {
            try
            {
                return appUserFile.GetCheckerBase(true).UsedInPercent();
            }
            catch
            {
                return 0;
            }
        }

        public static Int64 SentCount(this Data_AppUserFile appUserFile)
        {
            try
            {
                return appUserFile.GetCheckerBase().SentCount();
            }
            catch
            {
                return 0;
            }
        }

        public static Int64 QueuedCount(this Data_AppUserFile appUserFile)
        {
            try
            {
                return appUserFile.GetCheckerBase().QueuedCount();
            }
            catch
            {
                return 0;
            }
        }
    }
}
 