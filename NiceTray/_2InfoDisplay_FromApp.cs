using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using NiceApiLibrary_low;

namespace NiceTray
{
    public class _2InfoDisplay_FromApp : PreTextHandler, I2_InfoDisplay
    {
        private eI2LogLevel logLevel;
        private ManualResetEvent AreWeReady = new ManualResetEvent(false);
        public static _2InfoDisplay_FromApp_TheForm theForm;
        public static _2InfoDisplay_FromApp TheApp;

        private ManualResetEvent StopRequest;
        private DisplayTextController textController;

        public void Start()
        {
            logLevel = _2InfoDisplay_DebugOut.GetConfiguredLogLevel();
            if (theForm != null)
            {
                throw new ArgumentException("theForm != null");
            }
            theForm = _2InfoDisplay_FromApp_TheForm.CreateAndStart(AreWeReady);
            TheApp = this;
        }

        public _2InfoDisplay_FromApp(ManualResetEvent stopRequest)
        {
            StopRequest = stopRequest;
            textController = new DisplayTextController(true);
        }

        #region log
        private void GenFileLog(string str, eI2LogLevel thisLevel)
        {
            if (logLevel <= thisLevel)
            {
                KnownFiles.ioAppend(KnownFiles.eKnownFiles.Log, _2InfoDisplay_DebugOut.PreForLevel(thisLevel) + FileLog_GetPreText() + str);
            }
        }

        public void FileLog_Debug(string str)
        {
            GenFileLog(str, eI2LogLevel.Debug_0);
        }
        public void FileLog_Info(string str)
        {
            GenFileLog(str, eI2LogLevel.Info_1);
        }
        public void FileLog_Error(string str)
        {
            GenFileLog(str, eI2LogLevel.Error_2);
        }
        public void FileLog_TelStatus(List<string> val)
        {
            KnownFiles.ioWriteAllLines(KnownFiles.eKnownFiles.TelStatus, val.ToArray());
        }
        #endregion

        public void LoopStart() 
        {
            Clear();
        }
        public void ReturnWhenReady()
        {
            AreWeReady.WaitOne();
        }
        public void AddLine(string line)
        {
            DetailedData_OneOtherGuiLine a = new DetailedData_OneOtherGuiLine() { Line = line };
            string allText;
            StringBuilder debug = new StringBuilder();
            textController.AddLineAndUpdate3(a, out allText, debug);
            theForm.UserAction_DisplayText(new Data_DisplayText(true, allText, this.StopRequest));
        }

        public void AddLine2(IAddLine iFace)
        {
            string allText;
            StringBuilder debug = new StringBuilder();
            try
            {
                textController.AddLineAndUpdate3(iFace, out allText, debug);
                FileLog_Debug(debug.ToString());
            }
            catch (Exception) 
            {
                FileLog_Debug(debug.ToString());
                throw;
            }
            theForm.UserAction_DisplayText(new Data_DisplayText(true, allText, this.StopRequest));
        }

        public void Clear()
        {
            textController.Clear();
//            theForm.UserAction_DisplayText(new Data_DisplayText(true, "", this.StopRequest));
        }
        public void Delay(int ms)
        {
            Thread.Sleep(ms);
        }
        public bool IsRealDisplay()
        {
            return true;
        }

        public long FileSize()
        {
            return KnownFiles.ioSize(KnownFiles.eKnownFiles.Log);
        }
    }
}

