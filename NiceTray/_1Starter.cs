using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Configuration;
using System.IO;
using System.Reflection;

using NiceApiLibrary_low;

namespace NiceTray
{
    public interface I1Starter
    {
        void Start();
        void StopByJoin();
        void StopByForce();
    }

    class _1Starter : I1Starter
    {
        public static I1Starter s_Global;
        I2_InfoDisplay m_2iInfoDisplay;
        I3_GetData m_3iGetData;
        I4_GetScreen m_4Screen;
        I5_MouseAndKeyboard m_5Mouse;
        I6_WhatsAppProcess m_6Process;
        I8_UpdateCommunicator m_8Updater;

        ManualResetEvent m_wantToStop;
        Thread m_workerThread;

        public static void Main()
        {
            try
            {
                s_Global = new _1Starter();
                s_Global.Start();
            }
            catch (SystemException se)
            {
                Console.WriteLine(se.GetType());
                Console.WriteLine(se.Message);
                Console.WriteLine(se.StackTrace);
                Console.ReadLine();
            }
        }

        private _1Starter()
        {
        }

        public void Start()
        {
            m_wantToStop = new ManualResetEvent(false);
            m_workerThread = new Thread(new ThreadStart(internalThread));
            List<string> dspWarning = new List<string>();
            StringBuilder logInfo = new StringBuilder();

            // I2
            Console.WriteLine("I2...");
            string sI2 = ConfigurationManager.AppSettings["_2InfoDisplay"];
            logInfo.Append(" _2InfoDisplay:" + sI2);
            switch (sI2)
            {
                case "DebugOut":
                    m_2iInfoDisplay = new _2InfoDisplay_DebugOut(this.m_wantToStop);
                    dspWarning.Add("_2InfoDisplay.DebugOut");
                    break;
                case "DataLogger":
                    m_2iInfoDisplay = new _2InfoDisplay_DataLogger();
                    dspWarning.Add("_2InfoDisplay.DataLogger");
                    break;
                case "FromApp":
                    m_2iInfoDisplay = new _2InfoDisplay_FromApp(this.m_wantToStop);
                    break;

                default:
                    throw new ArgumentException("Config I2");
            }

            // I3
            Console.WriteLine("I3...");
            string sI3 = ConfigurationManager.AppSettings["_3GetData"];
            logInfo.Append(" _3GetData:" + sI3);
            switch (sI3)
            {
                case "DebugHardcoded":
                    m_3iGetData = new _3GetData_DebugHardcoded();
                    dspWarning.Add("_3GetData.DebugHardcoded");
                    break;

                case "Server":
                    m_3iGetData = new _3GetData_Server(MyUrls.trayASPURL_LiveNoSSL);
                    break;

                case "None":
                    m_3iGetData = new _3GetData_None(MyUrls.trayASPURL_LiveNoSSL);
                    dspWarning.Add("_3GetData.None");
                    break;

                default:
                    throw new ArgumentException("Config I3");
            }

            // I4
            Console.WriteLine("I4...");
            string sI4 = ConfigurationManager.AppSettings["_4GetScreen"];
            logInfo.Append(" _4GetScreen:" + sI4);
            switch (sI4)
            {
                case "DebugReadTemplateFile":
                    m_4Screen = new _4GetScreen_DebugReadTemplateFile(
                        ConfigurationManager.AppSettings["_4GetScreen_DebugReadTemplateFile_Path"]);
                    dspWarning.Add("_4GetScreen.DebugReadTemplateFile");
                    break;

                case "Real":
                    m_4Screen = new _4GetScreen_Real();
                    break;

                default:
                    throw new ArgumentException("Config I4");
            }

            // I5
            Console.WriteLine("I5...");
            string sI5 = ConfigurationManager.AppSettings["_5MouseAndKeyboard"];
            logInfo.Append(" _5MouseAndKeyboard:" + sI5);
            switch (sI5)
            {
                case "Dummy":
                    m_5Mouse = new _5MouseAndKeyboard_Dummy();
                    dspWarning.Add("_5MouseAndKeyboard.Dummy");
                    break;

                case "Real":
                    m_5Mouse = new _5MouseAndKeyboard_Real(m_2iInfoDisplay);
                    break;

                default:
                    throw new ArgumentException("Config I5");
            }

            // I6
            // test s
            //_2InfoDisplay_FromApp d = new _2InfoDisplay_FromApp(new ManualResetEvent(false));
            //d.Start();
            //_5MouseAndKeyboard_Real m = new _5MouseAndKeyboard_Real(d);
            //Ix i13 = new Ix();
            //i13.iMouse = m;
            //List<int> xx = new List<int> { 0xf0, 0x9f, 0x8d, 0x8e };
            //string s = "";
            //foreach (var xxx in xx)
            //{
            //    s += (char)xxx;
            //}
            //_6WhatsAppProcess_Real.EnterTextAndCheckForMouseMove(s, true, i13, null);
            // test e
            Console.WriteLine("I6...");
            string sI6 = ConfigurationManager.AppSettings["_6WhatsAppProcessTree"];
            logInfo.Append(" _6WhatsAppProcessTree:" + sI6);
            string[] sI6Parts = sI6.Split(new char[] { ',' });
            I6_WhatsAppProcess currentI6Child = null;
            for (int i6Id = 0; i6Id < sI6Parts.Length; i6Id++)
            {
                string next = sI6Parts[sI6Parts.Length - i6Id - 1];
                I6_WhatsAppProcess nextI6 = null;
                switch (next)
                {
                    case "Real":
                        nextI6 = new _6WhatsAppProcess_Real();
                        break;

                    case "Simulator":
                        nextI6 = new _6WhatsAppProcess_Simulator();
                        dspWarning.Add("_6WhatsAppProcess.Simulator");
                        break;

                    case "PreTelControll":
                        nextI6 = new _6WhatsAppProcess_Pre_TelControll(this.m_wantToStop);
                        break;

                    case "PreDeleteBmps":
                        nextI6 = new _6WhatsAppProcess_Pre_DeleteBmps();
                        break;

                    case "PreFoxStarter":
                        nextI6 = new _6WhatsAppProcess_Pre_FoxStarter();
                        break;

                    case "PreFocusFox":
                        nextI6 = new _6WhatsAppProcess_Pre_FocusFox();
                        break;

                    default:
                        throw new ArgumentException("Config I6");
                } // end switch
                nextI6.SetUp(currentI6Child);
                currentI6Child = nextI6;
            }
            m_6Process = currentI6Child;

            // I7 was firefox

            // I8
            Console.WriteLine("I8...");
            string sI8 = ConfigurationManager.AppSettings["_8UpdateCommunicator"];
            logInfo.Append(" _8UpdateCommunicator:" + sI8);
            switch (sI8)
            {
                case "Real":
                    m_8Updater = new _8UpdateCommunicator_Real();
                    break;

                case "Dummy":
                    m_8Updater = new _8UpdateCommunicator_Dummy();
                    dspWarning.Add("_8UpdateCommunicator.Dummy");
                    break;

                default:
                    throw new ArgumentException("Config I8");
            }
            /////////////////////////////////////////
            Console.WriteLine("m_2iInfoDisplay...");
            m_2iInfoDisplay.Start();
            m_2iInfoDisplay.FileLog_Info(logInfo.ToString());

            foreach (String line in dspWarning)
            {
                m_2iInfoDisplay.AddLine("*** " + line);
                m_2iInfoDisplay.FileLog_Info("*** " + line);
            }

            Console.WriteLine("http...");
            HttpServer http = new HttpServer();
            http.GoAndReturn();
            Thread.Sleep(1000);
            m_2iInfoDisplay.FileLog_Info("HTTP: " + http.Result.ToString());

            if ("HTTP.OnlyServer".IsConfiguredAndTRUE())
            {
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
            else
            {
                m_workerThread.Start();
            }
        }
        public void StopByJoin()
        {
            m_wantToStop.Set();
            m_workerThread.Join();
        }
        public void StopByForce()
        {
            m_workerThread.Abort();
        }

        void internalThread()
        {
            m_2iInfoDisplay.FileLog_Debug("internalThread start");
            m_2iInfoDisplay.FileLog_Debug(Assembly.GetAssembly(typeof(IMyLog)).WriteAssemblyVersion());

            m_2iInfoDisplay.ReturnWhenReady();

            Ix iAll = new Ix(m_2iInfoDisplay, m_4Screen, m_5Mouse, m_6Process, m_8Updater, new C9_CPUSlowdown());
            iAll.iSlowdown.Slowdown(iAll);

            processEmptyWakeUpTrigger(iAll);

            while (m_wantToStop.WaitOne(10, false) == false)
            {
                try
                {
                    iAll.iSlowdown.Slowdown(iAll);
                    m_2iInfoDisplay.LoopStart();
                    m_3iGetData.ExchangeDataWithServer(m_2iInfoDisplay, m_6Process);
                    List<ASPTrayBase> serverFiles = m_3iGetData.GetServerFiles(iAll);
                    if (serverFiles.Count == 0)
                    {
                        processEmptyWakeUpTrigger(iAll);
                    }
                    else
                    {

                        foreach (ASPTrayBase f1 in serverFiles)
                        {
                            m_2iInfoDisplay.FileLog_Info("".PadRight(50, '*'));
                            m_2iInfoDisplay.FileLog_Info("processing start : " + f1.GetFileName());
                            iAll.iSlowdown.Slowdown(iAll);
                            ASPTrayBase.eASPtrayType f1e = f1.GetEnumType();
                            switch (f1e)
                            {
                                case ASPTrayBase.eASPtrayType.ScreenShotRequest:
                                    {
                                        Data_Net__02ScreenshotRequest _02 = (Data_Net__02ScreenshotRequest)f1;
                                        m_2iInfoDisplay.FileLog_Info("processing screenshot start: " + _02.MsgTicks.ToString());
                                        Bitmap bAll = ImageCapture.GetAll();

                                        m_3iGetData.AddResultFile(
                                            new Data_Net__03ScreenshotResult(
                                                bAll.BmpToB64_Png(),
                                                _02.GetFileName()));
                                        m_2iInfoDisplay.FileLog_Info("processing done screenshot: " + _02.MsgTicks.ToString());
                                    }
                                    break;

                                case ASPTrayBase.eASPtrayType.NormalMessage:
                                    {
                                        Data_Net__00NormalMessage _00 = (Data_Net__00NormalMessage)f1;
                                        m_2iInfoDisplay.AddLine(_00.DestMobile);
                                        m_2iInfoDisplay.FileLog_Info(_00.DestMobile);
                                        if (_00.FailedCounter != 0)
                                        {
                                            m_2iInfoDisplay.FileLog_Info(String.Format("FailedCounter: {0} / {1}", _00.FailedCounter, _00.DisposeAfterNFailed));
                                        }
                                        m_2iInfoDisplay.FileLog_Info("MSG: " + _00.Msg.MsgForLogFile());
                                        int processId;
                                        bool use;
                                        m_3iGetData.Debug_GetProcessIdOfFile(f1, out processId, out use);
                                        if (use)
                                        {
                                            m_6Process.Debug_AmendProcessId(processId);
                                        }

                                        iAll.TypeOfProcess = Ix.eTypeOfProcess.Normal;
                                        eI6Error whatsAppRes = m_6Process.Process(
                                            _00.DestMobile,
                                            _00.Msg,
                                            iAll);
                                        m_2iInfoDisplay.AddLine(whatsAppRes.ToString());
                                        switch (whatsAppRes)
                                        {
                                            case eI6Error.Success:
                                                m_3iGetData.AddResultFile(new Data_Net__01NormalMessageResult(
                                                    _00.GetFileName(),
                                                    true));
                                                m_2iInfoDisplay.FileLog_Debug("processing done: " + _00.UniqueId());
                                                break;

                                            case eI6Error.FailedButNoLettingHostKnow_TelNotActive:
                                                m_2iInfoDisplay.AddLine("No Tel in Android yet");
                                                m_2iInfoDisplay.FileLog_Info("No Tel in Android yet");
                                                break;

                                            default:
                                                // failed
                                                m_3iGetData.AddResultFile(new Data_Net__01NormalMessageResult(
                                                    _00.GetFileName(),
                                                    false));
                                                m_2iInfoDisplay.FileLog_Debug("processing failed (interaction): " + _00.UniqueId() + " " + whatsAppRes.ToString());
                                                break;
                                        }
                                    }
                                    break;

                                case ASPTrayBase.eASPtrayType.CheckTelNumbers:
                                    {
                                        Data_Net__04CheckTelNumbers _04 = (Data_Net__04CheckTelNumbers)f1;
                                        m_2iInfoDisplay.AddLine("CheckTelNumbers:" + _04.GetFileName());
                                        MobileNoHandler handleOk = new MobileNoHandler(_04.TelListChecked);
                                        MobileNoHandler handleRetryPlease = new MobileNoHandler("");
                                        MobileNoHandler handleNotWorking = new MobileNoHandler("");
                                        foreach (string toBeCheckedNoZap in new MobileNoHandler(_04.TelList).MobileNumberArray)
                                        {
                                            string toBeChecked = toBeCheckedNoZap.Zapi_Add();
                                            m_2iInfoDisplay.AddLine("Checking " + toBeChecked);
                                            m_2iInfoDisplay.FileLog_Info("Checking " + toBeChecked);

                                            int processId;
                                            bool use;
                                            m_3iGetData.Debug_GetProcessIdOfFile(f1, out processId, out use);
                                            if (use)
                                            {
                                                m_6Process.Debug_AmendProcessId(processId);
                                            }
                                            iAll.TypeOfProcess = Ix.eTypeOfProcess.TelNumberChecking;
                                            eI6Error whatsAppRes = m_6Process.Process(
                                                toBeChecked, null, iAll);
                                            m_2iInfoDisplay.AddLine(whatsAppRes.ToString());
                                            switch (whatsAppRes)
                                            {
                                                case eI6Error.Success:
                                                    handleOk.Add(toBeCheckedNoZap);
                                                    break;

                                                case eI6Error.FailedButNoLettingHostKnow_TelNotActive:
                                                    handleRetryPlease.Add(toBeCheckedNoZap);
                                                    break;

                                                default:
                                                    // failed
                                                    handleNotWorking.Add(toBeCheckedNoZap);
                                                    break;
                                            }
                                        }
                                        m_3iGetData.AddResultFile(
                                            new Data_Net__05CheckTelNumbersResult(
                                                _04.GetFileName(),
                                                handleOk.getVal,
                                                handleRetryPlease.getVal,
                                                handleNotWorking.getVal));
                                    }
                                    break;

                                default:
                                    m_2iInfoDisplay.AddLine("UNKOWN FILE TYPE");
                                    m_2iInfoDisplay.FileLog_Error("UNKOWN FILE TYPE");
                                    break;
                            }
                        }
                    }

                    // delay
                    if (m_3iGetData.DoWeHaveDataToSend())
                    {
                        // we have data to send, so dont wait too long
                        m_2iInfoDisplay.Delay(500);
                    }
                    else
                    {
                        m_2iInfoDisplay.Delay(2 * 1000);
                    }
                }
                catch (SystemException se)
                {
                    m_2iInfoDisplay.AddLine("SystemException");
                    m_2iInfoDisplay.AddLine(se.Message);
                    m_2iInfoDisplay.AddLine(se.ToString());
                    if (!se.ToString().Contains("System.Net.WebException: The operation has timed out"))
                    {
                        m_2iInfoDisplay.FileLog_Error(se.Message + " - " + se.ToString());
                    }
                    m_3iGetData.Reset_toASP();
                    m_2iInfoDisplay.Delay(5 * 1000);
                }
            }
        }

        private void processEmptyWakeUpTrigger(Ix iAll)//todo uncomment this produces a crash when something is received from the host
        {
            if ("_1Starter.UseEmptyWakeUpTrigger".IsConfiguredAndTRUE())
            {
                m_2iInfoDisplay.AddLine("processEmptyWakeUpTrigger");
                m_2iInfoDisplay.FileLog_Info("processEmptyWakeUpTrigger");
                iAll.TypeOfProcess = Ix.eTypeOfProcess.EmptyWakeUpTrigger;
                eI6Error whatsAppRes = m_6Process.Process(
                    "", //_00.DestMobile,
                    "", //_00.Msg,
                    iAll);
                m_2iInfoDisplay.AddLine("processEmptyWakeUpTrigger Done");
            }
        }
    }

    public static class Filters
    {
        public static string MsgForLogFile(this string str)
        {
            return str.Replace("\r", "<\\r>").Replace("\n", "<\\n>");
        }
        public static string MsgForLogFileHideSpecial(this string str)
        {
            return str.Replace("\r", "").Replace("\n", "");
        }

        //public static char? KeyboardTranslate(this char c)
        //{
        //    int i = c;
        //    if (c == '\t')
        //    {
        //        // avoid Horizontal Tab
        //        return ' ';
        //    }
        //    else if (c == '\v')
        //    {
        //        // avoid Vertical Tab
        //        return ' ';
        //    }
        //    //else if (i >= 80)
        //    //{
        //    //    // avoid controll characters
        //    //    return null;
        //    //}
        //    //else if ((c == '\r') || (c == '\r'))
        //    //{
        //    //    // allow \r and \n
        //    //    return c;
        //    //}
        //    //else if (c < ' ') 
        //    //{
        //    //    // avoid controll characters
        //    //    return null;
        //    //}
        //    else
        //    {
        //        return c;
        //    }
        //}

    }
}

