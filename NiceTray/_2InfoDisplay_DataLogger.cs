using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NiceDesktopSupportApp;
using NiceApiLibrary_low;

namespace NiceTray
{
    class _2InfoDisplay_DataLogger : PreTextHandler, I2_InfoDisplay
    {
        private eI2LogLevel logLevel;

        public _2InfoDisplay_DataLogger()
        {
            Question.Ask("Please start up DataLogger");
        }

        private void Out(string text)
        {
            CSC.DataLoggerAccess.Send(Encoding.ASCII.GetBytes(text + "\n"));
        }
        public void Start()
        {
            logLevel = _2InfoDisplay_DebugOut.GetConfiguredLogLevel();
            Out(".Start");
        }

        public static string PreForLevel(eI2LogLevel thisLevel)
        {
            DateTime dt = DateTime.Now;
            string s1 = dt.ToString("dd/MM/yyyy HH:mm:ss.fff ");

            switch (thisLevel)
            {
                case eI2LogLevel.Debug_0: return s1 + "Debug: ";
                case eI2LogLevel.Info_1: return s1 + "Info : ";
                case eI2LogLevel.Error_2: return s1 + "Error: ";
            }
            return "???";
        }

        private void GenFileLog(string str, eI2LogLevel thisLevel)
        {
            if (logLevel <= thisLevel)
            {
                Out(PreForLevel(thisLevel) + FileLog_GetPreText() + str);
            }
        }

        public void FileLog_Debug(string str) { GenFileLog(str, eI2LogLevel.Debug_0); }
        public void FileLog_Info(string str) { GenFileLog(str, eI2LogLevel.Info_1); }
        public void FileLog_Error(string str) { GenFileLog(str, eI2LogLevel.Error_2); }

        public void FileLog_TelStatus(List<string> val)
        {
            foreach (string line in val)
            {
                Out("TelStatus: " + line);
            }
        }

        public void AddLine(string line)
        {
            Out(line);
        }
        public void AddLine2(IAddLine iFace)
        {
            Out(iFace.ToInfoLine());
        }
        public void Clear()
        {
        }
        public void LoopStart()
        {
            Clear();
        }
        public void ReturnWhenReady() { }

        public void Delay(int ms)
        {
        }

        public bool IsRealDisplay()
        {
            return false;
        }
    }
}
