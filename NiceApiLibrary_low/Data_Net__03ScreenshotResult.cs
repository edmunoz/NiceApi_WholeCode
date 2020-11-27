using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class Data_Net__03ScreenshotResult : ASPTrayBase
    {
        public string B64ScreenshotData;
        public string FileName;

        public override string GetFileName()
        {
            return FileName;
        }
        public override ASPTrayBase.eFilePriority GetFilePriority()
        {
            return eFilePriority.Low;
        }
        public override string GetNiceStatus()
        {
            return "Data_Net__03ScreenshotResult";
        }
        public override bool IsPriority()
        {
            return true;
        }
        public override int GetFailedCount()
        {
            return 0;
        }

        public Data_Net__03ScreenshotResult(string B64ScreenshotData, string FileName)
        {
            NetIniMembers();
            this.B64ScreenshotData = B64ScreenshotData;
            this.FileName = FileName;
        }
        public override void NetIniMembers()
        {
            B64ScreenshotData = "";
            FileName = "";
        }
        public override void NetTo(BinaryWriter bw)
        {
            base.NetTo(bw);
            bw.Write(B64ScreenshotData);
            bw.Write(FileName);
        }
        public override void NetFrom(BinaryReader br)
        {
            B64ScreenshotData = br.ReadString();
            FileName = br.ReadString();
        }
        public Data_Net__03ScreenshotResult(BinaryReader br)
        {
            NetIniMembers();
            NetFrom(br);
        }
    }
}
