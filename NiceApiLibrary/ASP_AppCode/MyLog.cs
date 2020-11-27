using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceApiLibrary.ASP_AppCode
{
    public class MyLog
    {
        static Dictionary<string, MyLogBoth> s_BothList = new Dictionary<string, MyLogBoth>();
        static MyVoidLogger s_Void = new MyVoidLogger();
        public static IMyLog GetLogger(string name)
        {
            if (s_BothList.ContainsKey(name))
            {
                MyLogBoth b = s_BothList[name];
                return b;
            }
            MyLogBoth bn = new MyLogBoth(
                log4net.LogManager.GetLogger(name),
                new MyLogToMyFile());
            s_BothList.Add(name, bn);
            return bn;
        }
        public static IMyLog GetVoidLogger()
        {
            return s_Void;
        }
    }

    public class MyLogBoth : IMyLog
    {
        log4net.ILog log4;
        MyLogToMyFile logMy;
        public MyLogBoth(log4net.ILog log4, MyLogToMyFile logMy)
        {
            this.log4 = log4;
            this.logMy = logMy;
        }

        public override string ToString()
        {
            return GetLoggerInfo();
        }

        public string GetLoggerInfo()
        {
            return "Both: " + logMy.GetLoggerInfo() + " and log4et";
        }

        public void Debug(string str)
        {
            log4.Debug(str);
            logMy.Debug(str);
        }
        public void Error(string str)
        {
            log4.Error(str);
            logMy.Error(str);
        }
        public void Info(string str)
        {
            log4.Info(str);
            logMy.Info(str);
        }
        public void SqlStatement(string str)
        {
        }
    }

    public class MyLogToLog4Net : IMyLog
    {
        private log4net.ILog log4;
        public MyLogToLog4Net(string logName)
        {
            log4 = log4net.LogManager.GetLogger(logName);
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

    public class MyLogToMyFile : IMyLog
    {
        private static Object MyLock = new Object();
        private static readonly string s_FileName = "MyLog.txt";
        private static void Append(string wholeLine)
        {
            string path = FolderNames.GetMachineRoot() + s_FileName/*"MyLog.txt"*/;
            System.Threading.Monitor.Enter(MyLock);
            try
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(wholeLine);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"MyLogToMyFile.Append(wholeLine:'{wholeLine}', path:'{path}')", ex);
            }
            finally
            {
                System.Threading.Monitor.Exit(MyLock);
            }
        }
        private static string ToNiceLine(string type, string text)
        {
            string r = "";
            r += DateTime.UtcNow.ToUkTime(false) + " ";
            r += type + " " + text;
            return r;
        }

        private bool logSql = false;

        public MyLogToMyFile()
        {
            try
            {
                logSql = "MyLogToMyFile.LogSQL".IsAppSettingsTrue();
            }
            catch { }
        }
        public override string ToString()
        {
            return GetLoggerInfo();
        }

        public string GetLoggerInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("MyLogToMyFile: ");
            sb.Append(s_FileName);
            if (logSql)
            {
                sb.Append(" SQL_log_is_on");
            }
            return sb.ToString();
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
            if (logSql)
            {
                Append(ToNiceLine("Sql", str));
            }
        }
    }
}

