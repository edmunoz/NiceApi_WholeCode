using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Diagnostics;

using NiceApiLibrary_low;

namespace NiceTray
{
    public class _2InfoDisplay_DebugOut : PreTextHandler, I2_InfoDisplay
    {
        private eI2LogLevel logLevel;
        //private DisplayTextController textController;

        public _2InfoDisplay_DebugOut(ManualResetEvent stopRequest)
        {
            //textController = new DisplayTextController(0, 10, stopRequest);
        }

        private void Out(string text)
        {
            // dummy to file
            System.Diagnostics.Debug.WriteLine("I2Dsp: " + text);
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
                //mg Out("TelStatus: " + line);
            }
        }

        public void AddLine(string line)
        {
            // dummy to screen
//mg            Debug.WriteLine("AddLine: " + line);
        }

        public void AddLine2(IAddLine iFace)
        {

        }
        public void Clear()
        {
            //textController.Clear();
        }
        public void LoopStart()
        {
            Clear();
        }
        public void ReturnWhenReady() { }


        public void Delay(int ms)
        {
            //Out(String.Format(".Delay({0}) ignored", ms));
        }

        public bool IsRealDisplay()
        {
            return false;
        }


        public static eI2LogLevel GetConfiguredLogLevel()
        {
            eI2LogLevel ret = eI2LogLevel.Error_2;
            string cnf = ConfigurationManager.AppSettings["_2InfoDisplay.LogLevel"];
            if (cnf != null)
            {
                switch (cnf)
                {
                    case "Debug": ret = eI2LogLevel.Debug_0; break;
                    case "Info": ret = eI2LogLevel.Info_1; break;
                    case "Error": default: ret = eI2LogLevel.Error_2; break;
                }
            }
            return ret;
        }
    }
}
