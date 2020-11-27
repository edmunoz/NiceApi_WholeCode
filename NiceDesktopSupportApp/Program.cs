using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

using NiceApiLibrary;
using NiceApiLibrary_low;

namespace NiceDesktopSupportApp
{
    class Program
    { 
        static MyDebugOutLogger s_Log = new MyDebugOutLogger();
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(OperatingSystemInfo.Get().ToString());
            DSSwitch.NiceApiLibrary_StartUp(s_Log, true);
            s_Log.logSQL = true;

            bool go = true;
            Question ques = new Question();
            ques.Add(new QuestionOption("Email templates", EmailTemplates.Go));
            ques.Add(new QuestionOption("Add Tel# to free account...", NumberManager.AddToFree));
            ques.Add(new QuestionOption("Add Tel# to COMMERCIAL account...", NumberManager.Manager));
            ques.Add(new QuestionOption("Get Credit", WhatsappMessages.CheckCredit));
            ques.Add(new QuestionOption("EmailFromAPpiKey", Tests.EmailFromAPpiKey));

            // ques.Add(new QuestionOption("Send Moved emails", ); rrt//SendMovedEmail.Go(s_LogZap); break;
            //ques.Add(new QuestionOption("Send Nice test mail", SendNiceTestMail.Go));
            ques.Add(new QuestionOption("show Lib Version", Others.LibVersion));
            //            ques.Add(new QuestionOption("Just a log message", Others.JustALogMessage));
            ques.Add(new QuestionOption("Send Whatsapp messages...", WhatsappMessages.Go));
            ques.Add(new QuestionOption("Send Whatsapp messages Sys01", WhatsappMessages.GoSys01));
            ques.Add(new QuestionOption("Send Whatsapp messages Sys02", WhatsappMessages.GoSys02));
            ques.Add(new QuestionOption("Send Whatsapp messages Sys03", WhatsappMessages.GoSys03));
            ques.Add(new QuestionOption("Send Whatsapp messages Sys03 group", WhatsappMessages.GoSys03group));
            ques.Add(new QuestionOption("Send Whatsapp messages Sys04", WhatsappMessages.GoSys04));
            ques.Add(new QuestionOption("Send Whatsapp messages SysTest", WhatsappMessages.GoSysTest));
            //            ques.Add(new QuestionOption("Send ANY email", SendEmail.All));
            ques.Add(new QuestionOption("Show Logs", ShowLogs.Go));
            //ques.Add(new QuestionOption("Test SQL Copy", SQLCopy.Go));
            ques.Add(new QuestionOption("IIS Log All (dataLogger output)", IISLog.All));
            ques.Add(new QuestionOption("IIS Log Day Sum (dataLogger output)", IISLog.DaySummary));
            ques.Add(new QuestionOption("Tel# Analyser", TelNumberAnalyser.Analyse));
//            ques.Add(new QuestionOption("Test commercial code changes", CommercialCodeChange.Test));
            //ques.Add(new QuestionOption("Send Tel Check...", NumberManager.SendTelCheck));
            //ques.Add(new QuestionOption("Send Tel Sync (Dangerous)...", NumberManager.SendTelSync));
            ques.Add(new QuestionOption("Show Data_MessageFile content ...", ShowData_MessageFileContent.ShowAll));
            ques.Add(new QuestionOption("Get new APIId ...", Others.GetNewAPIId));
            //ques.Add(new QuestionOption("Test LibAPI...", Tests.LibAPI));
            ques.Add(new QuestionOption("Show Wallet contents", ShowWallet.Go));
            ques.Add(new QuestionOption("Ftp backup", FtpBackup.Go));
//            ques.Add(new QuestionOption("Sql Tester", SqlTesterForm.Show));
            ques.Add(new QuestionOption("Set LowLevelHttpDumping", delegate (IMyLog log, QuestionOption it) { LowLevelHttpDumper.Enabled = true; }));
            //ques.Add(new QuestionOption("500 Error Test", WhatsappMessages.Error500));
            //            ques.Add(new QuestionOption("Tests.TestQueuePriority", Tests.TestQueuePriority));
            //ques.Add(new QuestionOption("MultySystem Debug", Tests.MultySystemDebugs));
            //ques.Add(new QuestionOption("cUrl Command", cUrl_Command.Go));
            //ques.Add(new QuestionOption("Debug Loopback process...", Tests.DebugLoopback));
            //ques.Add(new QuestionOption("Debug DisplayTextControllerPart_MultiLine", Tests.DebugDisplayTextControllerPart_MultiLine));
            //ques.Add(new QuestionOption("Debug Android Sync in low lib", Tests.DebugAndroidSyncInLowLib));
            //ques.Add(new QuestionOption("Debug Debug_DirectTel", Tests.Debug_DirectTel));
            


            while (go)
            {
                ques.AskAndAct("What to do (SQLite Version", s_Log);
            }
        }
    }
}
