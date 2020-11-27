using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{

    internal delegate void d_AddCommentLine(string text, bool withTime);
    public abstract class Data_AppUserFile_CheckerBase
    {
        public abstract string Info(Data_AppUserFile user);
        public abstract void CommitOrThrow_Send(MobileNoHandler storedNumbers, string[] telList, bool isWelcomeMessage, int MessageLength, ref Data_AppUserFile.eUserStatus accountStatus, out bool sendFooter);
        public abstract void CommitOrThrow_TelNumberAdd(MobileHandleConfUnconfList existing, MobileNoHandler noToAdd);
        public abstract void CommitOrThrow_TelNumberRemove(MobileHandleConfUnconfList existing, MobileNoHandler noToRemove);
        public abstract void OnProcessed(Data_AppUserFile user, OnProcessedHandler args);
        public abstract void OnPostProcessed(OnProcessedHandler args);
        public abstract int UsedInPercent();
        public abstract int AccountImportance1();
        public abstract int AccountImportance2();
        public abstract Int64 SentCount();
        public abstract Int64 QueuedCount();

        public abstract bool FundManagement_CommitAddOneNumber(string telNumber);
    }

    class  Data_AppUserFile_Checker_Default : Data_AppUserFile_CheckerBase
    {
        public override string Info(Data_AppUserFile user)
        {
            return $"Default {Data_AppUserFile.GetNiceStatusText(user.AccountStatus)} {user.AccountStatus.Importance()}";
        }
        public override void CommitOrThrow_Send(MobileNoHandler storedNumbers, string[] telList, bool isWelcomeMessage, int MessageLength, ref Data_AppUserFile.eUserStatus accountStatus, out bool sendFooter) { sendFooter = false; }
        public override void CommitOrThrow_TelNumberAdd(MobileHandleConfUnconfList existing, MobileNoHandler noToAdd) { }
        public override bool FundManagement_CommitAddOneNumber(string telNumber)
        {
            // not allowed on unknown accounts
            return false;
        }
        public override void CommitOrThrow_TelNumberRemove(MobileHandleConfUnconfList existing, MobileNoHandler noToRemove) { }
        public override void OnProcessed(Data_AppUserFile user, OnProcessedHandler args) { }
        public override void OnPostProcessed(OnProcessedHandler args) { }
        public override int UsedInPercent() { return 0; }
        public override Int64 SentCount() { return 0; }
        public override Int64 QueuedCount() { return 0; }
        public override int AccountImportance1()
        {
            return 1;
        }
        public override int AccountImportance2()
        {
            return 1;
        }
    }

    public class OnProcessedHandler
    {
        public int noOfBytes;
        public LogForEmailSend emailLog;
        public bool updateCounters;
        public Data_AppUserFile_CheckerBase checker;

        public bool free_doPostSendUpgradeRequestOnHighPercent;
        public Data_AppUserFile free_User;

        public OnProcessedHandler(int noOfBytes, LogForEmailSend emailLog, bool updateCounters)
        {
            this.noOfBytes = noOfBytes;
            this.emailLog = emailLog;
            this.updateCounters = updateCounters;
        }

        private static OnProcessedHandler CastArgsOrThrow(Object args)
        {
            if (args.GetType() != typeof(OnProcessedHandler))
            {
                throw new ArgumentException("args wrong type");
            }
            return (OnProcessedHandler)args;
        }

        public static void OnProcessed(Data_AppUserFile user, Object args)
        {
            OnProcessedHandler args2 = CastArgsOrThrow(args);
            args2.checker = user.GetCheckerBase();
            if (args2.checker != null)
            {
                args2.checker.OnProcessed(user, args2);
            }
        }

        public static void PostProcess(Object args)
        {
            OnProcessedHandler args2 = CastArgsOrThrow(args);
            if (args2.checker != null)
            {
                args2.checker.OnPostProcessed(args2);
            }
        }
    }
}