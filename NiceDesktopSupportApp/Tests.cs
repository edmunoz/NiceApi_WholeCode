using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;

using NiceApiLibrary;
using NiceApiLibrary_low;
using NiceApiLibrary.ASP_AppCode;
using System.Diagnostics;
using System.Threading;

namespace NiceDesktopSupportApp
{
    public class Tests
    {
        //public static void TestQueuePriority(IMyLog log, QuestionOption it)
        //{
        //    List<ASPTrayBase> test = MessageProcessing_TrayTo.GetFilesToSendToTray_ConsiderPriority(-1, log);
        //    foreach (var one in test)
        //    {
        //        Console.WriteLine(one.GetNiceStatus());
        //    }
        //}

        public static void ToDebug(string str)
        {
            Debug.WriteLine(str);
        }
        public static void DebugAndroidSyncInLowLib(IMyLog log, QuestionOption it)
        {
            //AndroidSynchroniserThread.OnLog = ToDebug;
            //AndroidSynchroniserThread.DoWeRellyWantToRun = true;
            //var x = AndroidSynchroniserThread.I;
            //while (true)
            //{
            //    Thread.Sleep(500);
            //}

        }

        public static void Debug_DirectTel(IMyLog log, QuestionOption it)
        {
            throw new NotImplementedException("not suitable for publication!");
        }

        public static void DebugDisplayTextControllerPart_MultiLine(IMyLog log, QuestionOption it)
        {
            string res;
            StringBuilder sb = new StringBuilder();
            StringBuilder im = new StringBuilder();
            DisplayTextController c = new DisplayTextController(false);
            c.AddLineAndUpdate3(new DetailedData_LastBadProcess(13, new TimeSpan(13)), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_LastBadProcess(13, new TimeSpan(13)), out res, sb); resOut(res);

            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eFirstInit), out res, sb); resOut(res);

            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eStart), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportEndPoint, "TheEndPoint"), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumbers, 0, 31), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumbers, 13, 0), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eDone, true, DateTime.Now.AddSeconds(30)), out res, sb); resOut(res);

            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eStart), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportEndPoint, "TheEndPoint"), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumbers, 113, 131), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eDone, false, DateTime.Now.AddSeconds(30)), out res, sb); resOut(res);

            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eStart), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportEndPoint, "TheEndPoint"), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumbers, 13, 31), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eDone, true, DateTime.Now.AddSeconds(30)), out res, sb); resOut(res);

            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eStart), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportEndPoint, "TheEndPoint"), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eReportTelNumbers, 13, 31), out res, sb); resOut(res);
            c.AddLineAndUpdate3(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eDone, true, DateTime.Now.AddSeconds(30)), out res, sb); resOut(res);

            c.GetStatus().TheList.ForEach(e => e.ToIntermapperInfo(im));
            Debug.WriteLine("-----------------------------");
            Debug.WriteLine(im.ToString());
        }
        private static void resOut(string res)
        {
            Debug.WriteLine("****************************************************");
            Debug.WriteLine(res);

            //foreach (var res1 in res.Split('\r'))
            //{
            //    Debug.WriteLine(res1);
            //}
        }

        public static void DebugLoopback(IMyLog log, QuestionOption it)
        {
            using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
            {
                ld.LastError = "e13";
                ld.GetEntry_CreateIfNotThere(NiceSystemInfo.DEFAULT_STR).debugStr = "Zero is nice at " + DateTime.UtcNow.Ticks.ToUkTime(false);
                ld.GetEntry_CreateIfNotThere("123").debugStr = "only testing";
            }
        }

        public static void LibAPI(IMyLog log, QuestionOption it)
        {
            string GUID = null;
            bool go = true;
            Console.Clear();
            Question ques = new Question();
            ques.Add(new QuestionOption("Exit", delegate (IMyLog dlog, QuestionOption dit)
            {
                go = false;
            }));
            //////////////////////////////////////////////////////////////////
            while (go)
            {
                ques.AskAndAct("What to do", log);
            }
        }

        public static void MultySystemDebugs(IMyLog log, QuestionOption it)
        {
            var x1 = DSSwitch.appUser().GetType();
            var x2 = DSSwitch.full().GetNiceSystemType();
            DSSwitch.appUser().RetrieveAll(Data_AppUserFile.SortType.State, null, log);

        }

        public static void EmailFromAPpiKey(IMyLog log, QuestionOption it)
        {
            var key = Question.Ask("Enter API Key");
            string email = Data_AppUserFile.API_IdToEmail(key);
            email = Data_AppUserFile.EmailToRealEmail(email);
            Console.WriteLine(key);
            Console.WriteLine(email);
            Console.WriteLine("");
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////
        public static void TestAll()
        {
        }

        public static void TestB64Helper()
        {
            try
            {
                Data_Net_ASP2Tray a2t = new Data_Net_ASP2Tray();
                MemoryStream msIn = new MemoryStream(new byte[] { 1, 2, 3 });
                BinBase64StreamHelper.ASP2Tray_FromB64Stream(ref a2t, msIn);
            }
            catch (FormatException fe)
            {

            }
        }
    }
}
