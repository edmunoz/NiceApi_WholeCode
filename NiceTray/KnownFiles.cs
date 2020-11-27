using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;

namespace NiceTray
{
    public static class KnownFiles
    {
        public enum eKnownFiles
        {
            Log,
            TelStatus,
            AndroidCom,
            _end
        }

        class eachInfo
        {
            public eKnownFiles Id;
            public string Path;
            public string Name;

            public eachInfo(eKnownFiles Id, string Path, string Name)
            {
                this.Id = Id;
                this.Path = Path;
                this.Name = Name;
            }
        }

        private static List<eachInfo> info = new List<eachInfo>() {
            { new eachInfo(eKnownFiles.Log, "_NiceTray_NewLog.txt", "Log")},
            { new eachInfo(eKnownFiles.TelStatus, "_NiceTray_TelStatus.txt", "TelStatus")},
            { new eachInfo(eKnownFiles.AndroidCom, "_NiceTray_AndroidCom.txt", "AndroidCom")},
        };

        private static string getFilePath(eKnownFiles which)
        {
            return info[(int)which].Path;
        }

        public static List<string> GetAll()
        {
            List<string> ret = new List<string>();
            foreach (var x in info)
            {
                ret.Add(x.Name);
            }
            return ret;
        }

        public static string Get(eKnownFiles which)
        {
            return info[(int)which].Name;
        }

        public static bool IsKnown(string inName, out eKnownFiles which)
        {
            which = eKnownFiles._end;
            foreach (var x in info)
            {
                if (x.Name == inName)
                {
                    which = x.Id;
                    return true;
                }
            }
            return false;
        }


        private static Object allLocher = new object();
        private static void lockAndProcess(Action process)
        {
            Monitor.Enter(allLocher);
            try
            {
                process();
            }
            catch (SystemException _)
            {

            }
            Monitor.Exit(allLocher);
        }

        public static void ioAppend(eKnownFiles which, string text)
        {
            lockAndProcess(delegate() {
                using (StreamWriter sw = File.AppendText(getFilePath(which)))
                {
                    sw.WriteLine(text);
                }
            });
        }

        public static long ioSize(eKnownFiles which)
        {
            long size = -1;
            lockAndProcess(delegate () {
                try
                {
                    size = new System.IO.FileInfo(getFilePath(which)).Length;
                }
                catch (Exception) { }
            });
            return size;
        }

        public static void ioDelete(eKnownFiles which)
        {
            lockAndProcess(delegate() {
                File.Delete(getFilePath(which));
            });
        }

        public static string ioReadAllText(eKnownFiles which)
        {
            string r = null;
            lockAndProcess(delegate() {
                r = File.ReadAllText(getFilePath(which));
            });
            return r;
        }

        public static void ioWriteAllLines(eKnownFiles which, string[] contents)
        {
            lockAndProcess(delegate()
            {
                File.WriteAllLines(getFilePath(which), contents);
            });
        }
    }
}
