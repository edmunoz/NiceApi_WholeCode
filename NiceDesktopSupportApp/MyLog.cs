using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

using log4net;
using NiceApiLibrary;
using NiceApiLibrary_low;

namespace NiceDesktopSupportApp
{

    public class MyLogToLog4Net : IMyLog
    {
        private static bool s_isConfigured;
        private static ICollection s_Config;
        private log4net.ILog log4;

        private static void ConfigureIfNeeded()
        {
            if (!s_isConfigured)
            {
                s_isConfigured = true;
                s_Config = log4net.Config.XmlConfigurator.Configure();
            }
        }
        public MyLogToLog4Net(string logName)
        {
            ConfigureIfNeeded();
            log4 = log4net.LogManager.GetLogger(logName);
            ToString();
        }

        public override string ToString()
        {
            return GetLoggerInfo();
        }

        public string GetLoggerInfo()
        {
            string ret = "";
            foreach (log4net.Appender.IAppender appender1 in log4.Logger.Repository.GetAppenders())
            {
                if (typeof(log4net.Appender.RollingFileAppender) == appender1.GetType())
                {
                    log4net.Appender.RollingFileAppender r1 = (log4net.Appender.RollingFileAppender)appender1;
                    ret += "File:" + System.IO.Path.GetFileName(r1.File) + " - ";
                }
                else if (typeof(log4net.Appender.AdoNetAppender) == appender1.GetType())
                {
                    log4net.Appender.AdoNetAppender db1 = (log4net.Appender.AdoNetAppender)appender1;
                    ret += "DB:" + db1.CommandText + " - ";
                }
                else
                {
                    ret += "???"; break;
                }
            }
            return "Logging to Log4Net: " + ret;
        }

        public void Debug(string str)
        {
            log4.Debug(str);
        }
        public void Error(string str)
        {
            log4.Error(str);
        }
        public void Info(string str)
        {
            log4.Info(str);
        }
        public void SqlStatement(string str)
        {
        }
    }

    class MyLogToFile : IMyLog
    {
        private static readonly string s_FileName = "MyDesktopSupportLog.txt";
        private static MyLogToFile s_TheOnlyOne = new MyLogToFile();
        public static IMyLog GetLogger()
        {
            return s_TheOnlyOne;
        }

        private static void Append(string wholeLine)
        {
            using (StreamWriter sw = File.AppendText(s_FileName/*"MyDesktopSupportLog.txt"*/))
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

        public override string ToString()
        {
            return GetLoggerInfo();
        }

        public string GetLoggerInfo()
        {
            return "Logging to file " + s_FileName;
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

    //class MyLog_Zap : ZapZapLibrary.IMyLog
    //{
    //    private static MyLog_Zap s_TheOnlyOne = new MyLog_Zap();
    //    public static ZapZapLibrary.IMyLog GetLogger()
    //    {
    //        return s_TheOnlyOne;
    //    }

    //    private static void Append(string wholeLine)
    //    {
    //        using (StreamWriter sw = File.AppendText("MyDesktopSupportLog_Zap.txt"))
    //        {
    //            sw.WriteLine(wholeLine);
    //        }
    //    }

    //    private static string ToNiceLine(string type, string text)
    //    {
    //        string r = "";
    //        r += DateTime.Now.ToString() + " ";
    //        r += type + " " + text;
    //        return r;

    //    }

    //    public void Debug(string str)
    //    {
    //        Append(ToNiceLine("Debug", str));
    //    }
    //    public void Error(string str)
    //    {
    //        Append(ToNiceLine("Error", str));
    //    }
    //    public void Info(string str)
    //    {
    //        Append(ToNiceLine("Info", str));
    //    }
    //}

}
