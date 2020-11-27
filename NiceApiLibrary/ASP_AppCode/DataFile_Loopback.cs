using NiceApiLibrary_low;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceApiLibrary.ASP_AppCode
{
    public class DataFile_Loopback : DataFile_Base, IDataFile_User
    {
        public DateTime ASP_Rebuild_Time;
        public string LastError;
        private Dictionary<string, DataFile_Loopback_Entry> Entries;

        public DataFile_Loopback(DataFile_Base.OpenType openType)
            : base(NiceSystemInfo.DEFAULT, openType)
        {
        }

        public int GetEntryCount()
        {
            return Entries.Count;
        }

        public Dictionary<string, DataFile_Loopback_Entry> GetAllEntries()
        {
            return Entries;
        }

        public DataFile_Loopback_Entry GetEntry_CreateIfNotThere(string entryId)
        {
            if (!Entries.ContainsKey(entryId))
            {
                Entries.Add(entryId, new DataFile_Loopback_Entry(entryId));
            }
            return Entries[entryId];
        }

        public override void NetIniMembers()
        {
            ASP_Rebuild_Time = DateTime.MinValue;
            LastError = "";
            Entries = new Dictionary<string, DataFile_Loopback_Entry>();
        }

        public override void NetFrom(BinaryReader br)
        {
            try
            {
                NetIniMembers();

                // 1 
                // 2    02/05/2018  ASP_Rebuild_Time added
                // 3    30/08/2018  Loopback replaced with screenshot on code rewrite for ASP file exchange
                // 4    17/03/2020  trayTypeAndIp
                // 5    23.03.2020      with subsystem
                // 6    27.03.2020      added LastError

                int Version = br.ReadInt32();
                if (Version == 5) { NetFrom_V5(br); }
                if (Version == 6) { NetFrom_V6(br); }
            }
            catch (Exception _)
            {
            }
        }
        private void NetFrom_V6(BinaryReader br)
        {
            NetFrom_V5(br);
            LastError = br.ReadString();
        }

        private void NetFrom_V5(BinaryReader br)
        {
            ASP_Rebuild_Time = new DateTime(br.ReadInt64(), DateTimeKind.Utc);
            int arraySize = br.ReadInt32();
            Entries.Clear();
            for (int i = 0; i < arraySize; i++)
            {
                DataFile_Loopback_Entry e = new DataFile_Loopback_Entry("");
                e.NetFrom(br);
                Entries.Add(e.EntryId, e);
            }
        }

        public override void NetTo(BinaryWriter bw)
        {
            try
            {
                bw.Write(6);
                bw.Write(ASP_Rebuild_Time.Ticks);
                int arraySize = Entries.Count;
                bw.Write(arraySize);
                foreach (KeyValuePair<string, DataFile_Loopback_Entry> e in Entries)
                {
                    e.Value.NetTo(bw);
                }
                bw.Write(LastError);
            }
            catch { }
        }

        public override string GetFullPath()
        {
            return FolderNames.GetFolder(niceSystem, MyFolders.ASP_ServerStateFolder) + "\\" + GetFileName();
        }

        public override string GetFileName()
        {
            return "LoopbackVb.bin";
        }
    }


    /// <summary>
    /// Summary description for LoopbackData
    /// </summary>
    public class DataFile_Loopback_Entry
    {
        public String EntryId;
        public DateTime lastTrayConnection;
        public String lastTrayId;
        public DateTime lastToTray;
        public String lastToTrayData;
        public String trayTypeAndIp;
        public String debugStr;
        public DateTime lastDirectTelGet;
        public DateTime lastDirectTelAck;

        public DataFile_Loopback_Entry(string entryId)
        {
            NetIniMembers(entryId);
        }

        public void NetIniMembers(string entryId)
        {
            this.EntryId = entryId;
            lastTrayConnection = DateTime.MinValue;
            lastTrayId = "N/A";
            lastToTray = DateTime.MinValue;
            lastToTrayData = "N/A";
            trayTypeAndIp = "";
            debugStr = "";
            lastDirectTelGet = DateTime.MinValue;
            lastDirectTelAck = DateTime.MinValue;
        }

        public void NetFrom(BinaryReader br)
        {
            try
            {
                NetIniMembers("");
                int Version = br.ReadInt32();
                if (Version == 1) { NetFrom_V1(br); }
                if (Version == 2) { NetFrom_V2(br); }
            }
            catch { }
        }
        private void NetFrom_V2(BinaryReader br)
        {
            NetFrom_V1(br);
            lastDirectTelGet = new DateTime(br.ReadInt64());
            lastDirectTelAck = new DateTime(br.ReadInt64());
        }

        private void NetFrom_V1(BinaryReader br)
        {
            EntryId = br.ReadString();
            lastTrayConnection = new DateTime(br.ReadInt64());
            lastTrayId = br.ReadString();
            lastToTray = new DateTime(br.ReadInt64());
            lastToTrayData = br.ReadString();
            trayTypeAndIp = br.ReadString();
            debugStr = br.ReadString();
        }

        public void NetTo(BinaryWriter bw)
        {
            try
            {
                // Version
                // 1    23.03.2020  Initial version
                // 2    28.09.2020  With lastDirectTelGet and lastDirectTelAck
                bw.Write(2);
                bw.Write(EntryId);
                bw.Write(lastTrayConnection.Ticks);
                bw.Write(lastTrayId == null ? "null" : lastTrayId);
                bw.Write(lastToTray.Ticks);
                bw.Write(lastToTrayData);
                bw.Write(trayTypeAndIp == null ? "" : trayTypeAndIp);
                bw.Write(debugStr == null ? "" : debugStr);
                bw.Write(lastDirectTelGet.Ticks);
                bw.Write(lastDirectTelAck.Ticks);
            }
            catch { }
        }
    }
}

