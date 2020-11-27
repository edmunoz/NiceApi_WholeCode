using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public interface IData_Base
    {
        void NetTo(BinaryWriter bw);
        void NetFrom(BinaryReader br);
        void NetIniMembers();
        string GetFileName();
    }

    public enum eDisplayItem
    {
        _start,
        __Version = _start,
        __TypeAndAndroidEP,   // Type like 'ZapXp' __ Android einpoint like '10.41.234.2:6000'
        __AndroidCommunication_BT,
        __AndroidCommunication,
        __NiceServerSend,
        __NiceServerReceive,
        __ProcessingId,
        __LastGoodProcess,
        __LastBadProcess,
        __LastGoodOrBad,
        __OtherGuiText,
        __Error,

        MemCPU,
        MemRAM,
        MemCDrive,
        MemAndroid,
        MemBMPs,
        MemLogFile,
        _end
    }

    //public class DisplayLine
    //{
    //    public eDisplayItem ItemId;
    //    public string Value;

    //    public DisplayLine(eDisplayItem item, string val)
    //    {
    //        ItemId = item;
    //        Value = val;
    //    }
    //}

    public class MyComparers : IComparer<eDisplayItem>, IComparer<IAddLine>
    {
        public int Compare(eDisplayItem x, eDisplayItem y)
        {
            return (int)x - (int)y;
        }

        public int Compare(IAddLine x, IAddLine y)
        {
            return (int)(x.DisplayItem) - (int)(y.DisplayItem);
        }

    }

    public class TrayStatus// : List<IAddLine>
    {
        public List<IAddLine> TheList = new List<IAddLine>();

        public void Add(IAddLine it)
        {
            TheList.Add(it);
            TheList.Sort(new MyComparers());
        }
        public void AddRange(TrayStatus other)
        {
            this.TheList.AddRange(other.TheList);
        }

        static public TrayStatus Error(string err)
        {
            TrayStatus r = new TrayStatus();
            r.Add(new DetailedData_Error(err));
            return r;
        }

        public void __Add(DateTime timeStamp, eDisplayItem item, string val)
        {
            DetailedData_Other o = new DetailedData_Other(timeStamp, item, val);

//            this.Add(new DisplayLine(item, val));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IAddLine d in this.TheList)
            {
                sb.AppendLine(d.ToFullLine());
            }
            return sb.ToString();
        }
    }
}

