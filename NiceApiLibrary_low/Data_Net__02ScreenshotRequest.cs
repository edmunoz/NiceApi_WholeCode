using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class Data_Net__02ScreenshotRequest : ASPTrayBase
    {
        public Int64 MsgTicks;

        public override string GetFileName()
        {
            throw new NotImplementedException("not suitable for publication!");
        }
        public override bool IsPriority()
        {
            return true;
        }
        public override int GetFailedCount()
        {
            return 0;
        }
        public override ASPTrayBase.eFilePriority GetFilePriority()
        {
            return eFilePriority.High;
        }
        public override string GetNiceStatus()
        {
            return "Data_Net__02ScreenshotRequest";
        }

        public Data_Net__02ScreenshotRequest()
        {
            NetIniMembers();
            this.MsgTicks = DateTime.UtcNow.Ticks;
        }
        public override void NetIniMembers()
        {
            MsgTicks = 0;
        }
        public override void NetTo(BinaryWriter bw)
        {
            base.NetTo(bw);
            bw.Write(MsgTicks);
        }
        public override void NetFrom(BinaryReader br)
        {
            MsgTicks = br.ReadInt64();
        }
        public Data_Net__02ScreenshotRequest(BinaryReader br)
        {
            NetIniMembers();
            NetFrom(br);
        }
    }
}
