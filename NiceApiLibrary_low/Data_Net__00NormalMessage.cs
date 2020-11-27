using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class Data_Net__00NormalMessage : ASPTrayBase
    {
        public string UserId;
        public string DestMobile;
        public Int64 MsgTicks;
        public string Msg;
        public Int32 FailedCounter;
        public Int32 DisposeAfterNFailed; 
        public bool NoCounterUpdate;

        public string MsgTicksSwiss
        {
            get
            {
                return MsgTicks.ToSwissTime(true);
            }
        }

        public enum eLocation
        {
            Queued = 1,
            Processed,
            Disposed,
        }

        public override string ToString()
        {
            return string.Format("00: {0} {1}/{2} {3}", 
                GetFileName(),
                FailedCounter, 
                DisposeAfterNFailed, 
                NoCounterUpdate ? "NoCounterUpdate" : "");
        }

        public override string GetFileName()
        {
            return ASPTrayBase.s_MsgFile_GetFileName(MsgTicks, UserId);
        }
        public override int GetFailedCount()
        {
            return FailedCounter;
        }
        public override ASPTrayBase.eFilePriority GetFilePriority()
        {
            return eFilePriority.Normal;
        }
        public override string GetNiceStatus()
        {
            return $"Data_Net__00NormalMessage F:{FailedCounter} L:{Msg.Length} T:{MsgTicks.ToSwissTime(true)} {DestMobile}";
        }

        public Data_Net__00NormalMessage()
        {
            NetIniMembers();
        }

        public Data_Net__00NormalMessage(string UserId, string DestMobile, Int64 MsgTicks, string Msg, Int32 FailedCounter, Int32 DisposeAfterNFailed, bool NoCounterUpdate)
        {
            NetIniMembers();
            this.UserId = UserId;
            this.DestMobile = DestMobile;
            this.MsgTicks = MsgTicks;
            this.Msg = Msg;
            this.FailedCounter = FailedCounter;
            this.DisposeAfterNFailed = DisposeAfterNFailed;
            this.NoCounterUpdate = NoCounterUpdate;
        }
        public override void NetIniMembers()
        {
            UserId = "";
            DestMobile = "";
            MsgTicks = 0;
            Msg = "";
            FailedCounter = 0;
            DisposeAfterNFailed = 0;
            NoCounterUpdate = false;
        }
        public override void NetTo(BinaryWriter bw)
        {
            base.NetTo(bw);
            bw.Write(UserId);
            bw.Write(DestMobile);
            bw.Write(MsgTicks);
            bw.Write(Msg);
            bw.Write(FailedCounter);
            bw.Write(DisposeAfterNFailed);
            bw.Write(NoCounterUpdate);
        }
        public override void NetFrom(BinaryReader br)
        {
            UserId = br.ReadString();
            DestMobile = br.ReadString();
            MsgTicks = br.ReadInt64();
            Msg = br.ReadString();
            FailedCounter = br.ReadInt32();
            DisposeAfterNFailed = br.ReadInt32();
            NoCounterUpdate = br.ReadBoolean();
        }
        public Data_Net__00NormalMessage(BinaryReader br)
        {
            NetIniMembers();
            NetFrom(br);
        }
    }

    public static class Data_Net__NormalMessageEx
    {
        public static string UniqueId(this Data_Net__00NormalMessage c) 
        { 
            return c.MsgTicks.ToString() + "_" + TickText(c) + "_" + c.UserId; 
        }
        private static string TickText(Data_Net__00NormalMessage c)
        { 
            return (new DateTime(c.MsgTicks)).ToString("ddMMyyyy"); 
        }
        public static List<string> ToFullString(this Data_Net__00NormalMessage c) 
        {
            int pad = 15;
            List<string> r = new List<string>();
            r.Add(String.Format("{0}: {1}", "UserId".PadRight(pad), c.UserId));
            r.Add(String.Format("{0}: {1}", "DestMobile".PadRight(pad), c.DestMobile));
            r.Add(String.Format("{0}: {1}", "MsgTicks".PadRight(pad), c.MsgTicks.ToSwissTime(false)));
            r.Add(String.Format("{0}: {1}", "Msg".PadRight(pad), c.Msg));
            r.Add(String.Format("{0}: {1}", "FailedCounter".PadRight(pad), c.FailedCounter));
            r.Add(String.Format("{0}: {1}", "DisposeAfterNFailed".PadRight(pad), c.DisposeAfterNFailed));
            r.Add(String.Format("{0}: {1}", "NoCounterUpdate".PadRight(pad), c.NoCounterUpdate));
            return r;
        }
        public static TimeSpan AliveSince(this Data_Net__00NormalMessage c)
        {
            return DateTime.UtcNow - new DateTime(c.MsgTicks, DateTimeKind.Utc);
        }
    }
}
