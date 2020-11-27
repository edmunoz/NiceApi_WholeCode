using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace NiceApiLibrary_low
{
    public interface IMyLog
    {
        void Debug(string str);
        void Info(string str);
        void Error(string str);
        void SqlStatement(string str);
        string GetLoggerInfo();
    }

    public class LogForEmailSend
    {
        public IMyLog Log;
        public LogForEmailSend(IMyLog log)
        {
            this.Log = log;
        }
    }

    public class MyVoidLogger : IMyLog
    {
        public void Debug(string str) { }
        public void Info(string str) { }
        public void Error(string str) { }
        public void SqlStatement(string str) { }
        public string GetLoggerInfo() { return "MyVoidLogger"; }
    }

    public class MyDebugOutLogger : IMyLog
    {
        public bool logSQL = false;
        public void Debug(string str) { System.Diagnostics.Debug.WriteLine(str); }
        public void Info(string str) { System.Diagnostics.Debug.WriteLine(str); }
        public void Error(string str) { System.Diagnostics.Debug.WriteLine(str); }
        public void SqlStatement(string str) { if (logSQL) { System.Diagnostics.Debug.WriteLine(str); } }
        public string GetLoggerInfo() { return "MyDebugOutLogger"; }
    }

    public class MyMemoryLog : IMyLog
    {
        private List<string> List;
        public MyMemoryLog()
        {
            List = new List<string>();
        }
        public string GetData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            foreach (string value in GetDataArray())
            {
                sb.AppendLine(value);
            }
            return sb.ToString();
        }
        public List<string> GetDataArray()
        {
            return new List<string>(List.ToArray());
        }
        private void Add(string which, string line)
        {
            try
            {
                string niceLine = DateTime.UtcNow.ToString() + " " + which.PadRight(5) + ": " + line;
                List.Insert(0, niceLine);
            }
            catch { }
        }
        public void Debug(string str) 
        { 
            Add("Debug", str);
        }
        public void Info(string str) 
        { 
            Add("Info", str);
        }
        public void Error(string str)
        { 
            Add("Error", str);
        }
        public void SqlStatement(string str) 
        { 
        }
        public string GetLoggerInfo() 
        { 
            return "MyMemoryLog"; 
        }
    }

    public class MyToFileLog : IMyLog
    {
        private string filePath;
        public MyToFileLog(string filePath)
        {
            this.filePath = filePath;
        }
        private void Add(string which, string line)
        {
            try
            {
                string niceLine = DateTime.UtcNow.ToString() + " " + which.PadRight(5) + ": " + line + Environment.NewLine;
                File.AppendAllText(filePath, niceLine);
            }
            catch { }
        }
        public void Debug(string str)
        {
            Add("Debug", str);
        }
        public void Info(string str)
        {
            Add("Info", str);
        }
        public void Error(string str)
        {
            Add("Error", str);
        }
        public void SqlStatement(string str)
        {
        }
        public string GetLoggerInfo()
        {
            long length = -1;
            try
            {
                length = new System.IO.FileInfo(filePath).Length;
            }
            catch { }
            return $"{filePath} - Size:{length}";
        }
    }

    public class MyManyLog : IMyLog
    {
        private List<IMyLog> list;
        public MyManyLog(List<IMyLog> list)
        {
            this.list = list;
        }
        public void Debug(string str)
        {
            foreach (IMyLog l in list)
            {
                l.Debug(str);
            }
        }
        public void Info(string str)
        {
            foreach (IMyLog l in list)
            {
                l.Info(str);
            }
        }
        public void Error(string str)
        {
            foreach (IMyLog l in list)
            {
                l.Error(str);
            }
        }
        public void SqlStatement(string str)
        {
            foreach (IMyLog l in list)
            {
                l.SqlStatement(str);
            }
        }
        public string GetLoggerInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IMyLog l in list)
            {
                sb.Append(l.GetLoggerInfo() + ", ");
            }
            return sb.ToString();
        }
    }
}

