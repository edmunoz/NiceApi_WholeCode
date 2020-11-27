using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    class Data_Net__02ScreenshotRequest_File : IData_Net__02ScreenshotRequest_store
    {
        public String GetInfo(NiceSystemInfo niceSystem)
        {
            return FolderNames.GetFolder(niceSystem, MyFolders.ASP_ServerStateFolder) + " ScreenshotRequest";
        }

        public void Store(NiceSystemInfo niceSystem, Data_Net__02ScreenshotRequest msg, IMyLog log)
        {
            try
            {
                MyFolders.ASP_QueuedMessages_.WriteToFile(niceSystem, msg, log);
            }
            catch (SystemException se)
            {
                log.Error("*** SystemException ***");
                log.Error(se.Message);
            }
        }

        public void ForEach(NiceSystemInfo niceSystem, IMyLog log, dProcess_Data_Net__02ScreenshotRequest cb)
        {
            try
            {
                foreach (var f1 in Directory.GetFiles(FolderNames.GetFolder(niceSystem, MyFolders.ASP_QueuedMessages_)))
                {
                    if (!ASPTrayBase.s_MsgFile_IsOld(f1))
                    {
                        ASPTrayBase d1 = ASPTrayBase.ReadOne(f1, log);
                        if ((d1 != null) && (d1.GetEnumType() == ASPTrayBase.eASPtrayType.ScreenShotRequest))
                        {
                            cb((Data_Net__02ScreenshotRequest)d1);
                        }
                    }
                }
            }
            catch (SystemException se)
            {
                log.Error("*** SystemException ***");
                log.Error(se.Message);
            }
        }

        public Data_Net__02ScreenshotRequest ReadOne(NiceSystemInfo niceSystem, string fileName, IMyLog log)
        {
            try
            {
                WithAndWithoutUnderline ww = new WithAndWithoutUnderline(
                    FolderNames.GetFolder(niceSystem, MyFolders.ASP_QueuedMessages_),
                    fileName);
                Data_Net__02ScreenshotRequest o = null;

                Stream stream = OpenFile.ForRead(ww.Existing, false, false, log);
                if (stream == null)
                {
                    return null;
                }
                using (BinaryReader br = new BinaryReader(stream))
                {
                    ASPTrayBase ox = ASPTrayBase.ReadOne(br);
                    if (ox.GetEnumType() == ASPTrayBase.eASPtrayType.ScreenShotRequest)
                    {
                        o = (Data_Net__02ScreenshotRequest)ox;
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

        public void Delete(NiceSystemInfo niceSystem, string fileName, IMyLog log)
        {
            try
            {
                WithAndWithoutUnderline ww = new WithAndWithoutUnderline(
                    FolderNames.GetFolder(niceSystem, MyFolders.ASP_QueuedMessages_),
                    fileName);
                File.Delete(ww.Existing);
            }
            catch (SystemException se)
            {
                log.Error("*** SystemException ***");
                log.Error(se.Message);
            }
        }
    }
}
