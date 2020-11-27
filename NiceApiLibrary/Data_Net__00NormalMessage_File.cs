using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using NiceApiLibrary_low;


namespace NiceApiLibrary
{
    class Data_Net__00NormalMessage_File : IData_Net__00NormalMessage_store
    {
        public String GetInfo(NiceSystemInfo niceSystem)
        {
            return FolderNames.GetFolder(niceSystem, MyFolders.ASP_QueuedMessages_);
        }
        public void Store(NiceSystemInfo niceSystem, Data_Net__00NormalMessage msg, Data_Net__00NormalMessage.eLocation location, IMyLog log)
        {
            try
            {
                eLocationToMyFolder(location).WriteToFile(niceSystem, msg, log);
            }
            catch (SystemException se)
            {
                log.Error("*** SystemException ***");
                log.Error(se.Message);
            }
        }

        public void Delete(NiceSystemInfo niceSystem, string fileName, Data_Net__00NormalMessage.eLocation location, IMyLog log)
        {
            try
            {
                WithAndWithoutUnderline ww = new WithAndWithoutUnderline(
                    FolderNames.GetFolder(niceSystem, eLocationToMyFolder(location)),
                    fileName);
                File.Delete(ww.Existing);
            }
            catch (SystemException se)
            {
                log.Error("*** SystemException ***");
                log.Error(se.Message);
            }
        }

        public Data_Net__00NormalMessage ReadOne(NiceSystemInfo niceSystem, string fileName, Data_Net__00NormalMessage.eLocation location, IMyLog log)
        {
            try
            {
                WithAndWithoutUnderline ww = new WithAndWithoutUnderline(
                    FolderNames.GetFolder(niceSystem, eLocationToMyFolder(location)),
                    fileName);

                Data_Net__00NormalMessage o = null;

                Stream stream = OpenFile.ForRead(ww.Existing, false, false, log);
                if (stream == null)
                {
                    return null;
                }
                using (BinaryReader br = new BinaryReader(stream))
                {
                    ASPTrayBase ox = ASPTrayBase.ReadOne(br);
                    if (ox.GetEnumType() == ASPTrayBase.eASPtrayType.NormalMessage)
                    {
                        o = (Data_Net__00NormalMessage)ox;
                    }
                }
                return o;
            }
            catch (SystemException se)
            {
                log.Error("*** SystemException ***");
                log.Error(se.Message);
            }
            return null;
        }

        public int GetNoOfQueuedItems(NiceSystemInfo niceSystem, IMyLog log)
        {
            return Directory.GetFiles(FolderNames.GetFolder(niceSystem, MyFolders.ASP_QueuedMessages_)).Length;
        }

        public void ForEach(NiceSystemInfo niceSystem, Data_Net__00NormalMessage.eLocation location, IMyLog log, dProcess_Data_Net__00NormalMessage cb)
        {
            forEach(niceSystem, DateTime.MinValue, null, eLocationToMyFolder(location), log, cb);
        }

        public void ForEach(NiceSystemInfo niceSystem, DateTime newerThan, string containsUser, Data_Net__00NormalMessage.eLocation location, IMyLog log, dProcess_Data_Net__00NormalMessage cb)
        {
            forEach(niceSystem, newerThan, containsUser, eLocationToMyFolder(location), log, cb);
        }

        private static MyFolders eLocationToMyFolder(Data_Net__00NormalMessage.eLocation location)
        {
            switch (location)
            {
                case Data_Net__00NormalMessage.eLocation.Queued:
                    return MyFolders.ASP_QueuedMessages_;
                case Data_Net__00NormalMessage.eLocation.Processed:
                    return MyFolders.ASP_ProcessedMessages_;
                case Data_Net__00NormalMessage.eLocation.Disposed:
                    return MyFolders.ASP_DisposedMessages_;
                default:
                    throw new ArgumentException("eLocationToMyFolder " + location.ToString());
            }
        }

        private void forEach(NiceSystemInfo niceSystem, DateTime newerThan, string containsUser, MyFolders folder, IMyLog log, dProcess_Data_Net__00NormalMessage cb)
        {
            foreach (var f1 in Directory.GetFiles(FolderNames.GetFolder(niceSystem, folder)))
            {
                if ((containsUser != null) && (!f1.Contains(containsUser)))
                {
                    // wrong user
                    continue;
                }

                DateTime fileTime = ASPTrayBase.s_MsgFile_GetDateTimeOfMessageFile(f1);
                if (fileTime >= newerThan)
                {
                    if (!ASPTrayBase.s_MsgFile_IsOld(f1))
                    {
                        ASPTrayBase d1 = ASPTrayBase.ReadOne(f1, log);
                        if ((d1 != null) && (d1.GetEnumType() == ASPTrayBase.eASPtrayType.NormalMessage))
                        {
                            cb((Data_Net__00NormalMessage)d1);
                        }
                    }
                }
            }
        }
    }

    internal class WithAndWithoutUnderline
    {
        public string With;
        public string Without;

        public string Existing
        {
            get
            {
                if (WithExists)
                {
                    return With;
                }
                else
                {
                    return Without;
                }
            }
        }

        public bool WithExists
        {
            get
            {
                return File.Exists(With);
            }
        }
        public bool WithoutExists
        {
            get
            {
                return File.Exists(Without);
            }
        }
        public WithAndWithoutUnderline(string path, string file)
        {
            string noEnd = "";
            if (file.EndsWith("_.txt"))
            {
                noEnd = file.Substring(0, file.Length - 5);
            }
            else if (file.EndsWith(".txt"))
            {
                noEnd = file.Substring(0, file.Length - 4);
            }
            With = path + Path.DirectorySeparatorChar + noEnd + "_.txt";
            Without = path + Path.DirectorySeparatorChar + noEnd + ".txt";
        }
    }

}
