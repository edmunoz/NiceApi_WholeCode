using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using NiceApiLibrary_low;

namespace NiceTray
{
    public class _3GetData_DebugHardcoded : I3_GetData
    {
        private bool mFirstRound = true;
        private Data_Net_Tray2ASP toASP = new Data_Net_Tray2ASP();
        private Dictionary<string, OneListEntry> toProcess;

        public _3GetData_DebugHardcoded()
        {
            throw new NotImplementedException("not suitable for publication!");
        }

        public void ExchangeDataWithServer(I2_InfoDisplay d, I6_WhatsAppProcess p)
        {
            toASP.TrayType = "_3GetData.Server.TrayType".GetConfig();
            ASPTray_ObjectList.Counters toASPCount = toASP.GetCounters();
            if (toASPCount.Total_Results() > 0)
            {
                d.FileLog_Info(toASPCount.Total_ResultsString() + " up msg");
            }
            d.AddLine(toASPCount.Total_ResultsString() + " up msg (b64)");
            d.AddLine2(toASPCount.AddLineIFace_Send());

            d.AddLine("ExchangeDataWithServer dummy");
        }
        public void Reset_toASP()
        {
        }
        public void AddResultFile(ASPTrayBase file)
        {

        }
        public bool DoWeHaveDataToSend()
        {
            return true;
        }

        public List<ASPTrayBase> GetServerFiles(Ix ix)
        {
            if (mFirstRound)
            {
                mFirstRound = false;

                if ("_3GetData.DebugHardcoded.WithDelay".IsConfiguredAndTRUE())
                {
                    ix.iDsp.AddLine("_3GetData_DebugHardcoded: Waiting 30 sec, Bring Firefox to front.");
                    ix.iDsp.Delay(10000);
                    ix.iDsp.AddLine("10 sec...");
                    ix.iDsp.Delay(10000);
                    ix.iDsp.AddLine("_3GetData_DebugHardcoded: Waiting 30 sec, Bring Firefox to front.");
                    ix.iDsp.AddLine("20 sec...");
                }
            }
            else
            {
                throw new ArgumentException("not mFirstRound");
            }

            List<ASPTrayBase> ret = new List<ASPTrayBase>();
            foreach (var e1 in toProcess.Values)
            {
                if (ret.Count > 5)
                {
                    break;
                }
                ASPTrayBase f1 = e1.To_MessageFile();
                ret.Add(f1);
            }
            return ret;
        }

        public void Debug_GetProcessIdOfFile(ASPTrayBase file, out int processId, out bool useProcessId)
        {
            processId = 0;
            useProcessId = false;
            if (file.GetEnumType() == ASPTrayBase.eASPtrayType.NormalMessage)
            {
                Data_Net__00NormalMessage _0 = (Data_Net__00NormalMessage)file;
                if (toProcess.ContainsKey(_0.DestMobile))
                {
                    OneListEntry e1 = toProcess[_0.DestMobile];
                    processId = e1.processId;
                    useProcessId = true;
                }
            }
        }
    }

    class OneListEntry
    {
        public int processId;
        public string tel;
        public string msg;
        public bool doScreenshot;
        public ASPTrayBase alreadyFormated;

        public ASPTrayBase To_MessageFile()
        {
            if (doScreenshot)
            {
                throw new NotImplementedException();
            }
            else if (alreadyFormated != null)
            {
                return alreadyFormated;
            }
            else
            {
        //        // normal case
                Data_Net__00NormalMessage _0 = new Data_Net__00NormalMessage("ich@host.com", "zapi_+" + tel, DateTime.UtcNow.Ticks, msg, 0, -1, false);
                return _0;
        //        f1.DestMobile = "zapi_+" + tel;
            }
            //return null;
        }
        public OneListEntry(int processId, string tel, string msg)
        {
            this.processId = processId;
            this.tel = tel;
            this.msg = msg;
        }
        public OneListEntry(int processId, bool doScreenshot)
        {
            this.processId = processId;
            this.doScreenshot = doScreenshot;
        }
        public OneListEntry(ASPTrayBase alreadyFormated)
        {
            this.alreadyFormated = alreadyFormated;
        }
    }
}
