using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class Data_Net__04CheckTelNumbers : ASPTrayBaseWithUniqueFileName
    {
        public string TelList;
        public string TelListChecked;

        public override string ToString()
        {
            return string.Format("04: {0} {3}/{4} {1} ({2})",
                GetFileName(),
                TelList, 
                TelListChecked,
                new MobileNoHandler(TelList).MobileNumbersCount,
                new MobileNoHandler(TelListChecked).MobileNumbersCount
                );
        }

        public Data_Net__04CheckTelNumbers()
        {
            NetIniMembers();
            this.MsgTicks = DateTime.UtcNow.Ticks;
        }

        public Data_Net__04CheckTelNumbers(string UserId, Int64 MsgTicks, string TelList, string TelListChecked)
        {
            NetIniMembers();
            base.SetMembers(UserId, MsgTicks);
            this.TelList = TelList;
            this.TelListChecked = TelListChecked;
        }
        public override void NetIniMembers()
        {
            base.NetIniMembers();
            TelList = "";
            TelListChecked = "";
        }
        public override void NetTo(BinaryWriter bw)
        {
            base.NetTo(bw);
            bw.Write(TelList);
            bw.Write(TelListChecked);
        }
        public override void NetFrom(BinaryReader br)
        {
            base.NetFrom(br);
            TelList = br.ReadString();
            TelListChecked = br.ReadString();
        }
        public Data_Net__04CheckTelNumbers(BinaryReader br)
            :base()
        {
            NetIniMembers();
            NetFrom(br);
        }
        public override eFilePriority GetFilePriority()
        {
            return eFilePriority.Normal;
        }
        public override string GetNiceStatus()
        {
            return $"04CheckNumbers: {ToString()} {this.MsgTicks.ToSwissTime(true)}";
        }
    }

    public static class Data_Net__04CheckTelNumbersEx
    {
    }
}
