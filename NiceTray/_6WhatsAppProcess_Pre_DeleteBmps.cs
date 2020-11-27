using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceTray
{
    abstract class _6WhatsAppProcess_Pre_Base : I6_WhatsAppProcess
    {
        protected I6_WhatsAppProcess Child;
        
        public virtual void SetUp(I6_WhatsAppProcess child)
        {
            Child = child;
        }
        public abstract eI6Error Process(string destMobile, string msg, Ix iAll);
        public virtual void Debug_AmendProcessId(int newProcessId) 
        { 
                Child.Debug_AmendProcessId(newProcessId);
        }
        public virtual string Debug_AmendUrl(string suggestedUrl, string additionalInfo)
        {
            return Child.Debug_AmendUrl(suggestedUrl, additionalInfo);
        }
    }

    class _6WhatsAppProcess_Pre_DeleteBmps : _6WhatsAppProcess_Pre_Base
    {
        DateTime LastCheck = new DateTime(0);
        TimeSpan CheckInterval = new TimeSpan(0, 10, 0); //once every 10 minutes
        public override eI6Error Process(string destMobile, string msg, Ix iAll)
        {
            using (var x = new LogPreText("DelB", iAll))
            {
                if (DateTime.UtcNow > (LastCheck + CheckInterval))
                {
                    LastCheck = DateTime.UtcNow;
                    int maxFiles = int.Parse("_6WhatsAppProcess_Pre_DeleteBmps.MaxFiles".GetConfig());
                    BmpFileHandler.PathAndDate[] files = new BmpFileHandler(".").GetSortedList();
                    iAll.iDsp.FileLog_Info(String.Format("{0} files on disk",
                        files.Length));
                    if (files.Length > maxFiles)
                    {
                        int toDelete = files.Length - maxFiles;
                        iAll.iDsp.FileLog_Info(String.Format("deleting {0} files",
                            toDelete));
                        for (int i = 0; i < toDelete; i++)
                        {
                            try
                            {
                                if (files[i].Path.Contains("zapi_"))
                                {
                                    File.Delete(files[i].Path);
                                }
                                else
                                {
                                    iAll.iDsp.FileLog_Debug("Not deleting as it does not contain the zapi text");
                                }
                            }
                            catch (SystemException)
                            { }
                        }
                    }
                }

                return Child.Process(destMobile, msg, iAll);
            }
        }
    }

    class _6WhatsAppProcess_Pre_DeleteLogFile : _6WhatsAppProcess_Pre_Base
    {
        DateTime LastCheck = new DateTime(0);
        TimeSpan CheckInterval = new TimeSpan(0, 10, 0); //once every 10 minutes
        public override eI6Error Process(string destMobile, string msg, Ix iAll)
        {
            using (var x = new LogPreText("DelLog", iAll))
            {
                if (DateTime.UtcNow > (LastCheck + CheckInterval))
                {
                    LastCheck = DateTime.UtcNow;

                    long maxSize;
                    if (long.TryParse("_6WhatsAppProcess_Pre_DeleteLogFile.MaxSize".GetConfig(), out maxSize))
                    {
                        long currentSize = KnownFiles.ioSize(KnownFiles.eKnownFiles.Log);
                        if (currentSize > maxSize)
                        {
                            KnownFiles.ioDelete(KnownFiles.eKnownFiles.Log);
                        }
                    }
                }

                return Child.Process(destMobile, msg, iAll);
            }
        }
    }

}
