using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Net;
using System.Configuration;

using NiceApiLibrary_low;

namespace NiceTray
{
    public class _3GetData_None : I3_GetData
    {
        private Data_Net_Tray2ASP toASP;
        private Data_Net_ASP2Tray fromASP;
        private MyUrls urlToUse;

        public _3GetData_None(MyUrls urlToUse)
        {
            this.urlToUse = urlToUse;
            Reset_toASP();
        }

        public void Reset_toASP()
        {
            this.toASP = new Data_Net_Tray2ASP();
        }

        public void ExchangeDataWithServer(I2_InfoDisplay d, I6_WhatsAppProcess p)
        {
            string url = p.Debug_AmendUrl(FolderNames.GetUrl(this.urlToUse), toASP.GetCounters().Total_ResultsString());

            toASP = new Data_Net_Tray2ASP();
            fromASP = new Data_Net_ASP2Tray();
        }

        public List<ASPTrayBase> GetServerFiles(Ix ix)
        {
            return fromASP.ObjectList;
        }
        public void Debug_GetProcessIdOfFile(ASPTrayBase file, out int processId, out bool useProcessId)
        {
            processId = 0;
            useProcessId = false;
        }

        public bool DoWeHaveDataToSend()
        {
            ASPTray_ObjectList.Counters c = toASP.GetCounters();
            return c.Total_Results() != 0;
        }

        public void AddResultFile(ASPTrayBase file)
        {
            toASP.ObjectList.Add(file);
        }
    }

}
