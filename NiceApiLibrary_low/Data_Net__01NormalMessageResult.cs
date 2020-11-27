using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class Data_Net__01NormalMessageResult : ASPTrayBase
    {
        public string FileName;
        public bool Success;

        public override string GetFileName()
        {
            return FileName;
        }
        public override bool IsPriority()
        {
            return false;
        }
        public override ASPTrayBase.eFilePriority GetFilePriority()
        {
            return eFilePriority.Low;
        }
        public override string GetNiceStatus()
        {
            return "Data_Net__01NormalMessageResult";
        }
        public override int GetFailedCount()
        {
            return 0;
        }

        public Data_Net__01NormalMessageResult(string FileName, bool Success)
        {
            NetIniMembers();
            this.FileName = FileName;
            this.Success = Success;
        }
        public override void NetIniMembers()
        {
            FileName = "";
            Success = false;
        }
        public override void NetTo(BinaryWriter bw)
        {
            base.NetTo(bw);
            bw.Write(FileName);
            bw.Write(Success);
        }
        public override void NetFrom(BinaryReader br)
        {
            FileName = br.ReadString();
            Success = br.ReadBoolean();
        }
        public Data_Net__01NormalMessageResult(BinaryReader br)
        {
            NetIniMembers();
            NetFrom(br);
        }
    }
}
