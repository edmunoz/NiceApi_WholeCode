using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace NiceApiLibrary_low
{
    public class Data_Net_Tray2ASP : ASPTray_ObjectList, IData_Base
    {
        public Int32 _FileVersion;
        public string TrayType;

        public Data_Net_Tray2ASP()
        {
            NetIniMembers();
        }

        public override void NetIniMembers()
        {
            base.NetIniMembers();
            _FileVersion = -1;
            TrayType = "";
        }

        public override void NetTo(BinaryWriter bw)
        {
            // v2   First version with Objec List
            bw.Write((Int32)2);
            bw.Write(this.TrayType);
            base.NetTo(bw);
        }

        private void NetFrom_V2(BinaryReader br)
        {
            this.TrayType = br.ReadString();
            base.NetFrom(br);
        }

        public override void NetFrom(BinaryReader br)
        {
            _FileVersion = br.ReadInt32();
            switch (_FileVersion)
            {
                case 2:
                    NetFrom_V2(br); break;
                default:
                    throw new IOException("Wrong file version in Data_Net_Tray2ASP.");
            }
        }
    }

    public static class ASPTray_ObjectList_Ex
    {
    }

    public class ASPTray_ObjectList : IData_Base
    {
        public Int32 _ObjectListVersion;
        public List<ASPTrayBase> ObjectList;

        public virtual string GetFileName()
        {
            throw new NotSupportedException("Dont call GetFileName on a ASPTray_ObjectList");
        }

        public class Counters
        {
            public int NormalMessage;
            public int NormalMessageResult_Success;
            public int NormalMessageResult_Failed;
            public int CheckTelIn;
            public int CheckTelInDone;
            public int CheckTelOutOk;
            public int CheckTelOutRetry;
            public int CheckTelOutNotWorking;

            public int ScreenShotRequest;
            public int ScreenShotResult;

            public int Total_Requests()
            {
                return
                    NormalMessage +
                    ScreenShotRequest +
                    CheckTelIn;
            }

            public DetailedData_NiceServerReceive AddLineIFace_Receive()
            {
                return new DetailedData_NiceServerReceive()
                {
                    Receive_NormalMessage = NormalMessage,
                    Receive_ScreenShotRequest = ScreenShotRequest,
                    Receive_CheckTel = CheckTelIn
                };
            }

            public string Total_RequestsString()
            {
                string r = String.Format("T:{0} N:{1} SC:{2} CK:{3}",
                    Total_Requests(),
                    NormalMessage,
                    ScreenShotRequest,
                    CheckTelIn);
                return r;
            }

            public int Total_Results()
            {
                return
                    NormalMessageResult_Success + NormalMessageResult_Failed + ScreenShotResult + CheckTelOutOk + CheckTelOutRetry + CheckTelOutNotWorking;
            }

            public DetailedData_NiceServerSend AddLineIFace_Send()
            {
                return new DetailedData_NiceServerSend()
                {
                    Send_Ok = NormalMessageResult_Success,
                    Send_Failed = NormalMessageResult_Failed,
                    Send_ScreenShotResult = ScreenShotResult,
                    Send_CheckTel = CheckTelOutOk + CheckTelOutRetry + CheckTelOutNotWorking
                };
            }

            public string Total_ResultsString()
            {
                string r = String.Format("T:{0} Ok:{1} F:{2} SC:{3} CH:{4}",
                    Total_Results(),
                    NormalMessageResult_Success,
                    NormalMessageResult_Failed,
                    ScreenShotResult,
                    CheckTelOutOk + CheckTelOutRetry + CheckTelOutNotWorking);
                return r;
            }
        }

        public virtual Counters GetCounters()
        {
            Counters r = new Counters();
            foreach (ASPTrayBase l1 in ObjectList)
            {
                ASPTrayBase.eASPtrayType e = l1.GetEnumType();
                switch (e)
                {
                    case ASPTrayBase.eASPtrayType.NormalMessage: r.NormalMessage++; break;
                    case ASPTrayBase.eASPtrayType.NormalMessageResult:
                        Data_Net__01NormalMessageResult _01 = (Data_Net__01NormalMessageResult)l1;
                        if (_01.Success)
                        {
                            r.NormalMessageResult_Success++;
                        }
                        else
                        {
                            r.NormalMessageResult_Failed++;
                        }
                        break;
                    case ASPTrayBase.eASPtrayType.ScreenShotRequest: r.ScreenShotRequest++; break;
                    case ASPTrayBase.eASPtrayType.ScreenShotResult: r.ScreenShotResult++; break;
                    case ASPTrayBase.eASPtrayType.CheckTelNumbers:
                        Data_Net__04CheckTelNumbers _04 = (Data_Net__04CheckTelNumbers)l1;
                        r.CheckTelIn += (new MobileNoHandler(_04.TelList)).MobileNumbersCount;
                        r.CheckTelInDone += (new MobileNoHandler(_04.TelListChecked)).MobileNumbersCount;
                        break;
                    case ASPTrayBase.eASPtrayType.CheckTelNumbersResult:
                        Data_Net__05CheckTelNumbersResult _05 = (Data_Net__05CheckTelNumbersResult)l1;
                        r.CheckTelOutOk += (new MobileNoHandler(_05.TelListOk)).MobileNumbersCount;
                        r.CheckTelOutRetry += (new MobileNoHandler(_05.TelListDoRetry)).MobileNumbersCount;
                        r.CheckTelOutNotWorking += (new MobileNoHandler(_05.TelListNotWorking)).MobileNumbersCount;
                        break;
                    default:
                        throw new SystemException("eASPtrayType " + e.ToString());
                }
            }
            return r;
        }

        public ASPTray_ObjectList()
        {
            NetIniMembers();
        }

        public virtual void NetIniMembers()
        {
            _ObjectListVersion = -1;
            ObjectList = new List<ASPTrayBase>();
        }

        public virtual void NetTo(BinaryWriter bw)
        {
            // v2   First version with Objec List
            bw.Write((Int32)2);
            bw.Write((Int32)ObjectList.Count);
            foreach (var ox in ObjectList)
            {
                ox.NetTo(bw);
            }
        }

        private void NetFrom_V2(BinaryReader br)
        {
            Int32 count = br.ReadInt32();
            for (Int32 i = 0; i < count; i++)
            {
                ASPTrayBase f1 = ASPTrayBase.ReadOne(br);
                if (f1 != null)
                {
                    ObjectList.Add(f1);
                }
            }
        }

        public virtual void NetFrom(BinaryReader br)
        {
            _ObjectListVersion = br.ReadInt32();
            switch (_ObjectListVersion)
            {
                case 2:
                    NetFrom_V2(br); break;
                default:
                    throw new IOException("Wrong file version in Data_Net_Tray2ASP.");
            }
        }
    }
}
