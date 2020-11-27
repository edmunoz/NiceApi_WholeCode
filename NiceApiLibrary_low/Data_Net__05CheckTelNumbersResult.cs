using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class Data_Net__05CheckTelNumbersResult : ASPTrayBase
    {
        public string RequestFileName;
        public string TelListOk;
        public string TelListDoRetry;
        public string TelListNotWorking;

        public Data_Net__05CheckTelNumbersResult(string RequestFileName,
            string TelListOk, string TelListDoRetry, string TelListNotWorking)
        {
            NetIniMembers();
            this.RequestFileName = RequestFileName;
            this.TelListOk = TelListOk;
            this.TelListDoRetry = TelListDoRetry;
            this.TelListNotWorking = TelListNotWorking;
        }
        public override void NetIniMembers()
        {
            RequestFileName = "";
            TelListOk = "";
            TelListDoRetry = "";
            TelListNotWorking = "";
        }
        public override void NetTo(BinaryWriter bw)
        {
            base.NetTo(bw);
            bw.Write(RequestFileName);
            bw.Write(TelListOk);
            bw.Write(TelListDoRetry);
            bw.Write(TelListNotWorking);
        }
        public override void NetFrom(BinaryReader br)
        {
            RequestFileName = br.ReadString();
            TelListOk = br.ReadString();
            TelListDoRetry = br.ReadString();
            TelListNotWorking = br.ReadString();
        }
        public Data_Net__05CheckTelNumbersResult(BinaryReader br)
            : base()
        {
            NetIniMembers();
            NetFrom(br);
        }

        public override int GetFailedCount()
        {
            return 0;
        }
        public override ASPTrayBase.eFilePriority GetFilePriority()
        {
            return eFilePriority.Low;
        }
        public override string GetNiceStatus()
        {
            return "Data_Net__05CheckTelNumbersResult";
        }

        public override string GetFileName()
        {
            return this.RequestFileName;
        }

    }
}

