using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NiceApiLibrary_low;

namespace NiceTray
{

    class MyLog : IMyLog
    {
        private static MyLog s_TheOnlyOne = new MyLog();
        public static IMyLog GetLogger()
        {
            return s_TheOnlyOne;
        }
        private MyLog()
        {

        }

        public string GetLoggerInfo()
        {
            return GetLogger().GetLoggerInfo();
        }

        private static void Append(string wholeLine)
        {
            using (StreamWriter sw = File.AppendText("MyTrayLog.txt"))
            {
                sw.WriteLine(wholeLine);
            }
        }

        private static string ToNiceLine(string type, string text)
        {
            string r = "";
            r += DateTime.Now.ToString() + " ";
            r += type + " " + text;
            return r;

        }

        public void Debug(string str)
        {
            Append(ToNiceLine("Debug", str));
        }
        public void Error(string str)
        {
            Append(ToNiceLine("Error", str));
        }
        public void Info(string str)
        {
            Append(ToNiceLine("Info", str));
        }
        public void SqlStatement(string str)
        {
        }
    }
}
