using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NiceApiLibrary_low;
using NiceDesktopSupportApp;

namespace NiceTray
{
    /// <summary>
    /// Simulator with user interaction (questions)
    /// </summary>
    class _6WhatsAppProcess_Simulator : I6_WhatsAppProcess
    {
        public void SetUp(I6_WhatsAppProcess p)
        { }
        public eI6Error Process(string destMobile, string msg, Ix iAll)
        {
            using (var x = new LogPreText("Simu", iAll))
            {
                iAll.iDsp.FileLog_Debug(destMobile);
                eI6Error ret = eI6Error.Success;
                Console.Clear();
                if (iAll.TypeOfProcess == Ix.eTypeOfProcess.TelNumberChecking) Console.WriteLine("TelNumberChecking");
                Console.WriteLine(destMobile);
                Console.WriteLine(msg);
                Console.WriteLine();
                Question q = new Question();
                q.Add(new QuestionOption("Success", delegate(IMyLog log, QuestionOption it) { ret = eI6Error.Success; }));
                q.Add(new QuestionOption("TelNotActive", delegate(IMyLog log, QuestionOption it) { ret = eI6Error.FailedButNoLettingHostKnow_TelNotActive; }));
                q.Add(new QuestionOption("Step2Failed", delegate(IMyLog log, QuestionOption it) { ret = eI6Error.Step2Failed; }));
                q.AskAndAct("Action: ", null);

                return ret;
            }
        }
        public void Debug_AmendProcessId(int newProcessId)
        {

        }
        public string Debug_AmendUrl(string suggestedUrl, string additionalInfo)
        {
            Console.WriteLine(additionalInfo);
            string ret = suggestedUrl;
            Question q = new Question();
            q.Add(new QuestionOption("Leave it", delegate(IMyLog log, QuestionOption it){}));
            q.Add(new QuestionOption("Change to http://localhost:60257/TrayApp", delegate(IMyLog log, QuestionOption it) { ret = "http://localhost:60257/TrayApp"; }));
            q.Add(new QuestionOption("Change to http://localhost/LocalNiceApi/TrayApp", delegate(IMyLog log, QuestionOption it) { ret = "http://localhost/LocalNiceApi/TrayApp"; }));
            q.Add(new QuestionOption("Change to http://localhost/Nice_WithSql/TrayApp", delegate(IMyLog log, QuestionOption it) { ret = "http://localhost/Nice_WithSql/TrayApp"; }));
            q.AskAndAct("Url: " + suggestedUrl, null);

            return ret;
        }
    }
}
