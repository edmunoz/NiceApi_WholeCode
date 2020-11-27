using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiceApiLibrary_low
{
    public interface IDisplayTextControllerPart
    {
        void AddLine(string text);
        List<TextAndTimestamp> GetCurrent();
        TrayStatus GetCurrentAsTrayStatus();
        void Clear();
    }

    public class TextAndTimestamp
    {
        public string Text;
        public DateTime Timestamp;

        public TextAndTimestamp(string text, DateTime stamp)
        {
            Text = text;
            Timestamp = stamp;
        }
        public TextAndTimestamp(string text)
        {
            Text = text;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Timestamp} {Text}";
        }
    }

    internal class DisplayTextControllerPart : IDisplayTextControllerPart
    {
        private List<TextAndTimestamp> list = new List<TextAndTimestamp>();
        private int maxLines;
        private eDisplayItem itemId;

        public DisplayTextControllerPart(int maxLines, eDisplayItem itemId)
        {
            this.maxLines = maxLines;
            this.itemId = itemId;
            for (int i = 0; i < maxLines; i++)
            {
                list.Add(new TextAndTimestamp("", DateTime.MinValue));
            }
        }
        public void AddLine(string text)
        {
            // 1) add
            list.Add(new TextAndTimestamp(text));

            // 2) trim
            while (list.Count > maxLines)
            {
                list.RemoveAt(0);
            }
        }
        public List<TextAndTimestamp> GetCurrent()
        {
            return list;
        }
        public TrayStatus GetCurrentAsTrayStatus()
        {
            DateTime maxTime = list.Max(e => e.Timestamp);
            TrayStatus r = new TrayStatus();
            foreach (TextAndTimestamp l1 in list)
            {
                r.__Add(maxTime, itemId, $"{maxTime} {l1.Text}");
            }
            return r;
        }
        public void Clear()
        {
            list.Clear();
        }
    }

    /// <summary>
    /// One line, multiple infos
    /// </summary>
    internal class DisplayTextControllerPart_MultiLine : IDisplayTextControllerPart
    {
        private int MaxParts;
        private eDisplayItem ItemId;
        public string[] Texts;
        public DateTime Timestamp;

        public DisplayTextControllerPart_MultiLine(int maxParts, eDisplayItem itemId)
        {
            this.MaxParts = maxParts;
            this.ItemId = itemId;
            Texts = new string[MaxParts];
            for (int i = 0; i < MaxParts; i++)
            {
                Texts[i] = "";
            }
        }

        public void AddLine(string text)
        {
            if (text.Length >= 3)
            {
                int index = text.IndexOf("__");
                if (index > 0)
                {
                    string actualText = text.Substring(index + 2);
                    string pos = text.Substring(0, index);
                    int posI;
                    if (int.TryParse(pos, out posI))
                    {
                        if (posI < MaxParts)
                        {
                            Texts[posI] = actualText;
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < MaxParts; i++)
            {
                Texts[i] = "";
            }
        }

        public List<TextAndTimestamp> GetCurrent()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < MaxParts; i++)
            {
                sb.Append(Texts[i] + " ");
            }
            TextAndTimestamp ret = new TextAndTimestamp(sb.ToString(), Timestamp);
            List<TextAndTimestamp> ret_ = new List<TextAndTimestamp>() { ret };
            return ret_;
        }

        public TrayStatus GetCurrentAsTrayStatus()
        {
            TrayStatus r = new TrayStatus();
            StringBuilder sb = new StringBuilder();
            foreach (string t1 in Texts)
            {
                sb.Append(t1 + " ");
            }
            r.__Add(Timestamp, this.ItemId, sb.ToString());
            return r;
        }
    }

    public class DisplayTextController
    {
        private SortedDictionary<eDisplayItem, IAddLine> IAddLines;

        public static DisplayTextController _globInstance = null;
        public DisplayTextController(bool registerAsGlobal)
        {
            if (registerAsGlobal)
            {
                _globInstance = this;
            }
            IAddLines = new SortedDictionary<eDisplayItem, IAddLine>(new MyComparers());

            Register(new DetailedData_Version("29.10.2020 a"), false);
            Register(new DetailedData_AndroidCommunication(false));
            Register(new DetailedData_AndroidCommunication_BT(DetailedData_AndroidCommunication_BT.eType.eFirstInit));
            Register(new DetailedData_NiceServerSend());
            Register(new DetailedData_NiceServerReceive());
            Register(new DetailedData_ProcessingId(0));
            Register(new DetailedData_LastGoodProcess(0, TimeSpan.MinValue));
            Register(new DetailedData_LastBadProcess(0, TimeSpan.MinValue));
            Register(new DetailedData_LastGoodOrBad());
            Register(new DetailedData_Error(""));

        }

        private void Register(dIAddLine line, bool notSetYet = true)
        {
            line.NotSetYet = notSetYet;
            IAddLines.Add(line.DisplayItem, line);
        }

        public TrayStatus GetStatus()
        {
            lock (this)
            {
                TrayStatus r = new TrayStatus();

                foreach (KeyValuePair<eDisplayItem, IAddLine> i in IAddLines)
                {
                    r.Add(i.Value);
                }
                return r;
            }
        }

        public void AddLineAndUpdate3(IAddLine iFace, out string allOut, StringBuilder debug)
        {
            lock (this)
            {
                if (iFace == null)
                {
                    throw new Exception("iFace == null");
                }
                if (IAddLines == null)
                {
                    throw new Exception("IAddLines == null");
                }

                if (iFace.MergeSupported)
                {
                    IAddLine old = IAddLines.ContainsKey(iFace.DisplayItem) ? IAddLines[iFace.DisplayItem] : null;
                    IAddLine newIf = iFace.MergeAddLine(old);

                    if (newIf == null)
                    {
                        throw new Exception("newIf == null");
                    }

                    if (old == null)
                    {
                        IAddLines.Add(newIf.DisplayItem, newIf);
                    }
                    else
                    {
                        IAddLines[newIf.DisplayItem] = newIf;
                    }
                }
                else if (IAddLines.ContainsKey(iFace.DisplayItem))
                {
                    IAddLines[iFace.DisplayItem] = iFace;
                }
                else
                {
                    IAddLines.Add(iFace.DisplayItem, iFace);
                }

                foreach (eDisplayItem report in iFace.ReportTo)
                {
                    IAddLine other = IAddLines.FirstOrDefault(i => i.Key == report).Value;
                    other?.IncomingReport(iFace);
                }

                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<eDisplayItem, IAddLine> i in IAddLines)
                {
                    try
                    {
                        sb.AppendLine(i.Value.ToInfoLine());
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine("--Exception --");
                    }
                }

                allOut = sb.ToString();
            }
        }

        public void AddLineAndUpdate3_error(IAddLine iFace, out string allOut)
        {
            lock (this)
            {
                if (iFace == null)
                {
                    throw new Exception("iFace == null");
                }
                if (IAddLines == null)
                {
                    throw new Exception("IAddLines == null");
                }

                if (iFace.MergeSupported)
                {
                    IAddLine old = IAddLines.ContainsKey(iFace.DisplayItem) ? IAddLines[iFace.DisplayItem] : null;
                    IAddLine newIf = iFace.MergeAddLine(old);

                    if (old == null)
                    {
                        IAddLines.Add(newIf.DisplayItem, newIf);
                    }
                    else
                    {
                        IAddLines[newIf.DisplayItem] = newIf;
                    }
                }
                else if (IAddLines.ContainsKey(iFace.DisplayItem))
                {
                    IAddLines[iFace.DisplayItem] = iFace;
                }
                else
                {
                    IAddLines.Add(iFace.DisplayItem, iFace);
                }

                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<eDisplayItem, IAddLine> i in IAddLines)
                {
                    try
                    {
                        sb.AppendLine(i.Value.ToInfoLine());
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine("--Exception --");
                    }
                }

                allOut = sb.ToString();
            }
        }

        //public void AddLineAndUpdate2(eDisplayItem part, string text, out string allOut)
        //{
        //    lock (this)
        //    {
        //        // 1) add
        //        switch (part) //todo
        //        {
        //            //case eDisplayItem.AndroidCommunication: AndroidCommunication.AddLine(text); break;
        //            //case eDisplayItem.TypeAndAndroidEP: TypeAndAndroidEP.AddLine(text); break;
        //            //case eDisplayItem.NiceServerSend: NiceServerSend.AddLine(text); break;
        //            //case eDisplayItem.NiceServerReceive: NiceServerReceive.AddLine(text); break;
        //            //case eDisplayItem.ProcessingId: ProcessingId.AddLine(text); break;
        //            //case eDisplayItem.LastGoodProcess: LastGoodProcess.AddLine(text); break;
        //            //case eDisplayItem.LastBadProcess: LastBadProcess.AddLine(text); break;
        //            //case eDisplayItem.OtherGuiText: Rest.AddLine(text); break;
        //        }

        //        List<TextAndTimestamp> all = new List<TextAndTimestamp>();
        //        //all.AddRange(AndroidCommunication.GetCurrent());
        //        //all.AddRange(TypeAndAndroidEP.GetCurrent());
        //        //all.AddRange(NiceServerSend.GetCurrent());
        //        //all.AddRange(NiceServerReceive.GetCurrent());
        //        //all.AddRange(ProcessingId.GetCurrent());
        //        //all.AddRange(LastGoodProcess.GetCurrent());
        //        //all.AddRange(LastBadProcess.GetCurrent());
        //        //all.AddRange(Rest.GetCurrent());

        //        StringBuilder sb = new StringBuilder();
        //        foreach (TextAndTimestamp l in all)
        //        {
        //            sb.AppendLine(l.ToString());
        //        }
        //        allOut = sb.ToString();
        //    }
        //}

        public void Clear()
        {
            lock (this)
            {
                (IAddLines.FirstOrDefault(l => l.Key == eDisplayItem.__OtherGuiText).Value as DetailedData_ManyGuiLine)?.Lines.Clear();
            }
        }
    }

    public interface IAddLine
    {
        string ToInfoLine();
        string ToFullLine();

        eDisplayItem DisplayItem { get; }

        string LineName { get; }

        bool MergeSupported { get; }
        List<eDisplayItem> ReportTo { get; }

        IAddLine MergeAddLine(IAddLine existing);
        void IncomingReport(IAddLine info);
        void ToIntermapperInfo(StringBuilder sb);
    }

    public abstract class dIAddLine : IAddLine
    {
        public DateTime TimeStamp { get; set; }

        public eDisplayItem DisplayItem { get; private set; }
        public string LineName { get; private set; }
        public bool NotSetYet { get; set; }
        public bool MergeSupported { get; set; }
        public List<eDisplayItem> ReportTo { get; set; }
        public virtual IAddLine MergeAddLine(IAddLine existing)
        {
            return existing;
        }

        public dIAddLine(eDisplayItem item)
        {
            TimeStamp = DateTime.Now;
            DisplayItem = item;
            LineName = item.ToLineName();
            ReportTo = new List<eDisplayItem>();
        }

        public virtual void IncomingReport(IAddLine info)
        {

        }

        public abstract string ToInfoLine();
        public virtual string ToFullLine()
        {
            return $"{LineName.PadRight(25)}: {ToInfoLine()}";
        }

        protected string _ToInfoLine(string infoPart)
        {
            if (NotSetYet)
            {
                return $"NotSetYet ({LineName})";
            }
            else
            {
                return $"{TimeStamp} {infoPart}";
            }
        }

        protected void _ToIntermapperLineWithTime(StringBuilder sb, string item, string val)
        {

        }

        private readonly List<string> DontShow = new List<string>()
            {
                "MemCPU.UpdateAgeSec",
                "MemCPU.UpdateTime",
                "MemCPU.AdditionalInfo",
                "MemCPU.Max",
                "MemCPU.Used",
                "MemRAM.UpdateAgeSec",
                "MemRAM.UpdateTime",
                "MemRAM.AdditionalInfo",
                "MemRAM.Max",
                "MemRAM.Used",
                "MemCDrive.UpdateAgeSec",
                "MemCDrive.UpdateTime",
                "MemCDrive.AdditionalInfo",
                "MemCDrive.Max",
                "MemCDrive.Used",
                "MemBMPs.UpdateAgeSec",
                "MemBMPs.UpdateTime",
                "MemBMPs.AdditionalInfo",
                "MemBMPS.Max",
                "MemBMPs.User",
                "Version.UpdateTime",
                "Version.UpdateAgeSec",
                "MemLogFile.UpdateTime",
                "MemLogFile.UpdateAgeSec",
                "AndroidCommunicationBT.NextRun",
                "NiceServerSend.UpdateTime",
                "NiceServerReceive.UpdateTime",
                "ProcessingId.UpdateTime",
                "LastGoodProcess.UpdateTime",
                "LastGoodProcess.Duration",
                "LastBadProcess.UpdateTime",
                "LastBadProcess.Duration",
                "LastGoodOrBad.UpdateTime",
            };


        private void ToIntermapperInfo_FilterOrAdd(StringBuilder sb, string key, string val)
        {
            if (!DontShow.Contains(key))
            {
                key = key.Replace(".", "_");
                sb.AppendLine($"{key}: {val}");
            }
            else
            {
                //                sb.AppendLine($"---{_key}: {_val}");
            }

        }

        public virtual void ToIntermapperInfo(StringBuilder sb)
        {
            System.Reflection.PropertyInfo[] allProps = GetType().GetProperties();

            System.Reflection.PropertyInfo id = GetType().GetProperty("LineName");
            string idVal = (string)id.GetValue(this, null);

            System.Reflection.PropertyInfo time = GetType().GetProperty("TimeStamp");
            DateTime timeVal = (DateTime)time.GetValue(this, null);

            System.Reflection.PropertyInfo notSet = GetType().GetProperty("NotSetYet");
            bool notSetVal = (bool)notSet?.GetValue(this, null);


            if (NotSetYet)
            {
                sb.AppendLine($"{idVal} NotSetYet");
            }
            else
            {
                ToIntermapperInfo_FilterOrAdd(sb, $"{idVal}.UpdateTime", $"{timeVal}");
                TimeSpan age = DateTime.Now - timeVal;
                ToIntermapperInfo_FilterOrAdd(sb, $"{idVal}.UpdateAgeSec", $"{(int)age.TotalSeconds}");

                foreach (var p in allProps)
                {
                    if ((p.Name != "DisplayItem") && (p.Name != "TimeStamp") && (p.Name != "LineName") && (p.Name != "NotSetYet") && (p.Name != "MergeSupported") && (p.Name != "ReportTo"))
                    {
                        string _key = "";
                        string _val = "";
                        object val = null;

                        switch (p.PropertyType.ToString())
                        {
                            case "System.String":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Boolean":
                                val = p.GetValue(this, null);
                                _key = $"{idVal}.{p.Name}";
                                _val = $"{val}";
                                break;

                            case "System.TimeSpan":
                                val = p.GetValue(this, null);
                                _key = $"{idVal}.{p.Name}";
                                _val = $"{(int)(((System.TimeSpan)val).TotalSeconds)}";
                                break;

                            default:
                                System.Diagnostics.Debug.WriteLine($"{p.Name} {p.PropertyType} not supported");
                                _val = $"{p.Name} {p.PropertyType} not supported";
                                break;
                        }

                        ToIntermapperInfo_FilterOrAdd(sb, _key, _val);
                    }
                }
            }
        }
    }

    public class DetailedData_NiceServerSend : dIAddLine
    {
        public int Send_Ok { get; set; }
        public int Send_Failed { get; set; }
        public int Send_ScreenShotResult { get; set; }
        public int Send_CheckTel { get; set; }
        public DetailedData_NiceServerSend() : base(eDisplayItem.__NiceServerSend) { }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(
                String.Format("T:{0} Ok:{1} F:{2} SC:{3} CH:{4}",
                Send_Ok + Send_Failed + Send_ScreenShotResult + Send_CheckTel,
                Send_Ok,
                Send_Failed,
                Send_ScreenShotResult,
                Send_CheckTel));
        }
    }

    public class DetailedData_NiceServerReceive : dIAddLine
    {
        public int Receive_NormalMessage { get; set; }
        public int Receive_ScreenShotRequest { get; set; }
        public int Receive_CheckTel { get; set; }

        public DetailedData_NiceServerReceive() : base(eDisplayItem.__NiceServerReceive) { }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(
                String.Format("T:{0} N:{1} SC:{2} CK:{3}",
                Receive_NormalMessage + Receive_ScreenShotRequest + Receive_CheckTel,
                Receive_NormalMessage,
                Receive_ScreenShotRequest,
                Receive_CheckTel));
        }
    }

    public class DetailedData_AndroidCommunication : dIAddLine
    {
        public bool AndroidOk { get; set; }
        public DetailedData_AndroidCommunication(bool androidOk) : base(eDisplayItem.__AndroidCommunication) => AndroidOk = androidOk;

        public override string ToInfoLine()
        {
            return this._ToInfoLine(AndroidOk ? "Android Ok" : "****** Android DOWN ******");
        }
    }

    public class DetailedData_AndroidCommunication_BT : dIAddLine
    {
        public enum eType
        {
            eFirstInit,
            eStart,
            eDone, // with status, and next run time
            eReportEndPoint,
            eReportTelNumbers,
            eReportTelNumberMerge,
        }

        public DetailedData_AndroidCommunication_BT(eType eType) : base(eDisplayItem.__AndroidCommunication_BT)
        {
            MergeSupported = true;
            switch (eType)
            {
                case eType.eStart:
                    currentGo = new oneGo();
                    currentGo.start = DateTime.UtcNow;
                    break;
                case eType.eFirstInit:
                    break;
                default:
                    throw new Exception("UPS");
            }
        }

        public DetailedData_AndroidCommunication_BT(eType eType, bool statusOk, DateTime nextRun) : base(eDisplayItem.__AndroidCommunication_BT)
        {
            MergeSupported = true;
            switch (eType)
            {
                case eType.eDone:
                    currentGo = new oneGo();
                    currentGo.end = DateTime.UtcNow;
                    currentGo.ok = statusOk;
                    NextRun = nextRun;
                    break;
                default:
                    throw new Exception("UPS");
            }
        }

        public DetailedData_AndroidCommunication_BT(eType eType, string endPoint) : base(eDisplayItem.__AndroidCommunication_BT)
        {
            MergeSupported = true;
            switch (eType)
            {
                case eType.eReportEndPoint:
                    AndroidEndPoint = endPoint;
                    break;
                default:
                    throw new Exception("UPS");
            }
        }

        public DetailedData_AndroidCommunication_BT(eType eType, int telNoServer, int telNoAndroid) : base(eDisplayItem.__AndroidCommunication_BT)
        {
            MergeSupported = true;
            switch (eType)
            {
                case eType.eReportTelNumbers:
                    TelNoServer = telNoServer;
                    TelNoAndroid = telNoAndroid;
                    break;
                default:
                    throw new Exception("UPS");
            }
        }

        public DetailedData_AndroidCommunication_BT(eType eType, TelListController.VerifyResult verRes) : base(eDisplayItem.__AndroidCommunication_BT)
        {
            MergeSupported = true;
            switch (eType)
            {
                case eType.eReportTelNumberMerge:
                    VerifyResult_OnBothAndroidAndServer = verRes.OnBothAndroidAndServer.Count;
                    VerifyResult_OnlyOnAndroid = verRes.OnlyOnAndroid.Count;
                    VerifyResult_OnlyOnServer = verRes.OnlyOnServer.Count;
                    VerifyResult_UserMoreThanOnceOnServer = verRes.UserMoreThanOnceOnServer.Count;
                    break;
                default:
                    throw new Exception("UPS");
            }
        }
        

        internal class oneGo
        {
            public DateTime start;
            public DateTime end;
            public bool ok;

            public bool Running
            {
                get
                {
                    return ((start.Ticks != 0) && (end.Ticks == 0));
                }
            }

            public override string ToString()
            {
                string ret;
                if (Running)
                {
                    ret = $"Running since {start.ToSwissTime(true)}";
                }
                else
                {
                    ret = $"{(ok ? "Good." : "***FAILED*** ")} Done at {end.ToSwissTime(true)} in {(end - start).ToReadableString()}";
                }
                return ret;
            }

            public void Merge(oneGo newData)
            {
                if (newData != null)
                {
                    start = newData.start.Ticks != 0 ? newData.start : start;
                    end = newData.end.Ticks != 0 ? newData.end : end;
                    ok = newData.end.Ticks != 0 ? newData.ok : ok;
                }
            }
        }

        private oneGo currentGo;
        private oneGo lastGo;

        public string AndroidEndPoint { get; set; }
        public int TelNoServer { get; set; }
        public int TelNoAndroid { get; set; }
        public int VerifyResult_OnBothAndroidAndServer { get; set; }
        public int VerifyResult_OnlyOnAndroid { get; set; }
        public int VerifyResult_OnlyOnServer { get; set; }
        public int VerifyResult_UserMoreThanOnceOnServer { get; set; }
        public DateTime NextRun { get; set; }

        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (currentGo != null)
                {
                    sb.Append($" Current:{currentGo}");
                }
                if (lastGo != null)
                {
                    sb.Append($" Last:{lastGo}");
                }
                if (NextRun.Ticks != 0)
                {
                    sb.Append($" NextRun:{NextRun}");
                }
                if (AndroidEndPoint != null)
                {
                    sb.Append($" EndPoint:{AndroidEndPoint}");
                }
                if (TelNoAndroid > 0)
                {
                    sb.Append($" TelAndroid:{TelNoAndroid}");
                }
                if (TelNoServer > 0)
                {
                    sb.Append($" TelServer:{TelNoServer}");
                }
                return sb.ToString();
            }
        }

        public override IAddLine MergeAddLine(IAddLine existing)
        {
            if (existing == null)
            {
                // no existing yet
                return this;
            }
            else
            {
                // update
                DetailedData_AndroidCommunication_BT ex = existing as DetailedData_AndroidCommunication_BT;
                ex.NotSetYet = false;

                ex.AndroidEndPoint = AndroidEndPoint != null ? AndroidEndPoint : ex.AndroidEndPoint;

                if (currentGo?.Running ?? false)
                {
                    ex.lastGo = ex.currentGo;
                    ex.currentGo = null;
                }

                ex.currentGo = ex.currentGo != null ? ex.currentGo : currentGo;
                ex.currentGo?.Merge(currentGo);
                ex.NextRun = NextRun.Ticks != 0 ? NextRun : ex.NextRun;
                ex.TelNoAndroid = TelNoAndroid != 0 ? TelNoAndroid : ex.TelNoAndroid;
                ex.TelNoServer = TelNoServer != 0 ? TelNoServer : ex.TelNoServer;
                ex.VerifyResult_OnBothAndroidAndServer = VerifyResult_OnBothAndroidAndServer != 0 ? VerifyResult_OnBothAndroidAndServer : ex.VerifyResult_OnBothAndroidAndServer;
                ex.VerifyResult_OnlyOnAndroid = VerifyResult_OnlyOnAndroid != 0 ? VerifyResult_OnlyOnAndroid : ex.VerifyResult_OnlyOnAndroid;
                ex.VerifyResult_OnlyOnServer = VerifyResult_OnlyOnServer != 0 ? VerifyResult_OnlyOnServer : ex.VerifyResult_OnlyOnServer;
                ex.VerifyResult_UserMoreThanOnceOnServer = VerifyResult_UserMoreThanOnceOnServer != 0 ? VerifyResult_UserMoreThanOnceOnServer : ex.VerifyResult_UserMoreThanOnceOnServer;

            }
            return existing;
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(Text);
        }
    }


    public class DetailedData_TypeAndAndroidEP : dIAddLine
    {
        public string TrayType { get; set; }
        public string LocalIPAddress { get; set; }
        public string Endpoint { get; set; }

        public DetailedData_TypeAndAndroidEP() : base(eDisplayItem.__TypeAndAndroidEP)
        {
            MergeSupported = true;
        }

        public override IAddLine MergeAddLine(IAddLine existing)
        {
            if (existing == null)
            {
                // first
                return this;
            }
            else
            {
                // update
                DetailedData_TypeAndAndroidEP ex = existing as DetailedData_TypeAndAndroidEP;
                ex.TrayType = TrayType != null ? TrayType : ex.TrayType;
                ex.LocalIPAddress = LocalIPAddress != null ? LocalIPAddress : ex.LocalIPAddress;
                ex.Endpoint = Endpoint != null ? Endpoint : ex.Endpoint;
                ex.NotSetYet = false;
                return ex;
            }
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine($"{TrayType} {LocalIPAddress} {Endpoint}");
        }
    }

    public class DetailedData_ManyGuiLine : dIAddLine
    {
        public List<DetailedData_OneOtherGuiLine> Lines { get; set; }

        public DetailedData_ManyGuiLine() : base(eDisplayItem.__OtherGuiText)
        {
            Lines = new List<DetailedData_OneOtherGuiLine>();
        }

        public override string ToInfoLine()
        {
            StringBuilder sb = new StringBuilder();
            Lines.ForEach(l => sb.AppendLine(l.ToInfoLine()));
            return sb.ToString();
        }

        public override string ToFullLine()
        {
            StringBuilder sb = new StringBuilder();
            Lines.ForEach(l => sb.AppendLine($"{LineName.PadRight(25)}: {l.ToInfoLine()}"));
            return sb.ToString();
        }

        public override void ToIntermapperInfo(StringBuilder sb)
        {
        }
    }

    public class DetailedData_OneOtherGuiLine : dIAddLine
    {
        public string Line { get; set; }

        public DetailedData_OneOtherGuiLine() : base(eDisplayItem.__OtherGuiText)
        {
            MergeSupported = true;
        }

        public override IAddLine MergeAddLine(IAddLine existing)
        {
            if (existing == null)
            {
                // frist entry
                DetailedData_ManyGuiLine many = new DetailedData_ManyGuiLine();
                many.Lines.Add(this);
                return many;
            }
            else
            {
                // not first entry, update existing
                (existing as DetailedData_ManyGuiLine).Lines.Add(this);
                return existing;
            }
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(Line);
        }

        public override void ToIntermapperInfo(StringBuilder sb)
        {
        }
    }

    public class DetailedData_ProcessingId : dIAddLine
    {
        public int ProcessingId { get; set; }
        public DetailedData_ProcessingId(int id) : base(eDisplayItem.__ProcessingId) => ProcessingId = id;

        public override string ToInfoLine()
        {
            return this._ToInfoLine($"Id: {ProcessingId}");
        }
    }

    public class DetailedData_LastGoodProcess : dIAddLine
    {
        public int ProcessingId { get; set; }
        public TimeSpan Duration { get; set; }

        public int DurationInSec
        {
            get
            {
                return (int)Duration.TotalSeconds;

            }
        }

        public DetailedData_LastGoodProcess(int id, TimeSpan duration) : base(eDisplayItem.__LastGoodProcess)
        {
            ProcessingId = id;
            Duration = duration;
            ReportTo.Add(eDisplayItem.__LastGoodOrBad);
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine($"{ProcessingId} in {Duration.ToReadableString()}");
        }
    }
    public class DetailedData_LastBadProcess : dIAddLine
    {
        public int ProcessingId { get; set; }
        public TimeSpan Duration { get; set; }

        public int DurationInSec
        {
            get
            {
                return (int)Duration.TotalSeconds;
            }
        }

        public DetailedData_LastBadProcess(int id, TimeSpan duration) : base(eDisplayItem.__LastBadProcess)
        {
            ProcessingId = id;
            Duration = duration;
            ReportTo.Add(eDisplayItem.__LastGoodOrBad);
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine($"{ProcessingId} in {Duration.ToReadableString()}");
        }
    }

    public class DetailedData_LastGoodOrBad : dIAddLine
    {
        private DateTime TimeStampGood;
        private DateTime TimeStampBad;

        public int GoodSinceSec
        {
            get
            {
                if (TimeStampGood > TimeStampBad)
                {
                    return (int)((TimeStampGood - TimeStampBad).TotalSeconds);
                }
                return 0;
            }
        }
        public int BadSinceSec
        {
            get
            {
                if (TimeStampGood < TimeStampBad)
                {
                    return (int)((TimeStampBad - TimeStampGood).TotalSeconds);
                }
                return 0;
            }
        }

        public DetailedData_LastGoodOrBad() : base(eDisplayItem.__LastGoodOrBad)
        {
            this.MergeSupported = true;
            TimeStampGood = DateTime.Now;
            TimeStampBad = TimeStampGood;
        }

        public override void IncomingReport(IAddLine info)
        {
            NotSetYet = false;
            var good = info as DetailedData_LastGoodProcess;
            if (good != null)
            {
                TimeStampGood = good.TimeStamp;
            }
            var bad = info as DetailedData_LastBadProcess;
            if (bad != null)
            {
                TimeStampBad = bad.TimeStamp;
            }
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine($"Good:{GoodSinceSec} - Bad:{BadSinceSec}");
        }
    }


    public class DetailedData_Error : dIAddLine
    {
        public string Msg { get; set; }
        public DetailedData_Error(string msg) : base(eDisplayItem.__Error) => Msg = msg;

        public override string ToInfoLine()
        {
            return this._ToInfoLine(Msg);
        }
    }

    public class DetailedData_Other : dIAddLine
    {
        public string Msg { get; set; }
        public DetailedData_Other(DateTime timeStamp, eDisplayItem item, string msg) : base(item)
        {
            TimeStamp = timeStamp;
            Msg = msg;
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(Msg);
        }
    }

    public class DetailedData_Usage : dIAddLine
    {
        public long Max { get; set; }
        public long Used { get; set; }

        public long Percent
        {
            get
            {
                return 100 * Used / Max;
            }
        }
        public string AdditionalInfo { get; set; }

        public DetailedData_Usage(eDisplayItem item, long Max, long Used, string AdditionalInfo, bool invert) : base(item)
        {
            this.Max = Max;
            this.Used = invert ? Max - Used : Used;
            this.AdditionalInfo = AdditionalInfo;
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(string.Format("{0}: {1,4}% used. {2} / {3}. {4}",
                LineName.PadRight(9),
                Percent,
                Used,
                Max,
                AdditionalInfo));
        }
    }

    public class DetailedData_BMPs : dIAddLine
    {
        public int Bmps { get; set; }
        public DetailedData_BMPs(int bmps) : base(eDisplayItem.MemBMPs)
        {
            this.Bmps = bmps;
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(string.Format($"BMPs     : {Bmps} files"));
        }
    }

    public class DetailedData_Logfile : dIAddLine
    {
        public long Size { get; set; }
        public DetailedData_Logfile(long size) : base(eDisplayItem.MemLogFile)
        {
            this.Size = size;
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(string.Format($"LogFileSize: {Size}"));
        }
    }

    public class DetailedData_Version : dIAddLine
    {
        public string Version { get; set; }
        public DetailedData_Version(string version) : base(eDisplayItem.__Version)
        {
            this.Version = version;
        }

        public override string ToInfoLine()
        {
            return this._ToInfoLine(string.Format($"Version: {Version}"));
        }
    }

}
