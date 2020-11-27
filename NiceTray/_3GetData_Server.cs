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
    public class _3GetData_Server : I3_GetData
    {
        private Data_Net_Tray2ASP toASP;
        private Data_Net_ASP2Tray fromASP;
        private MyUrls urlToUse;

        public _3GetData_Server(MyUrls urlToUse)
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
            d.AddLine("exchangeDataWithASP");
            d.FileLog_Debug("exchangeDataWithASP");

            toASP.TrayType = ConfigurationManager.AppSettings["_3GetData.Server.TrayType"];
            d.AddLine2(new DetailedData_TypeAndAndroidEP() { TrayType = toASP.TrayType });
            d.AddLine2(new DetailedData_TypeAndAndroidEP() { LocalIPAddress = Extensions.GetLocalIPAddress() });

            if (String.IsNullOrEmpty(toASP.TrayType))
            {
                throw new ConfigurationException("_3GetData.Server.TrayType is missing!");
            }
            ASPTray_ObjectList.Counters toASPCount = toASP.GetCounters();
            if (toASPCount.Total_Results() > 0)
            {
                d.FileLog_Info(toASPCount.Total_ResultsString() + " up msg");
            }
            d.AddLine2(toASPCount.AddLineIFace_Send());

            string url = p.Debug_AmendUrl(FolderNames.GetUrl(this.urlToUse), toASP.GetCounters().Total_ResultsString());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ReadWriteTimeout = FolderNames.GetValue(MyValues.Tray_TcpTimeoutSec) * 1000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream requestStream = request.GetRequestStream();
            BinBase64StreamHelper.Tray2ASP_ToB64Stream(ref toASP, requestStream);
            requestStream.Close();

            // GET RESPONSE ****************************************************
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            d.AddLine("GetResponse done");
            toASP = new Data_Net_Tray2ASP();
            fromASP = new Data_Net_ASP2Tray();
            Stream stream = response.GetResponseStream();
            BinBase64StreamHelper.ASP2Tray_FromB64Stream(ref fromASP, stream);
            response.Close();
            ASPTray_ObjectList.Counters fromASPCount = fromASP.GetCounters();
            if (fromASPCount.Total_Requests() > 0)
            {
                d.FileLog_Info(fromASPCount.Total_RequestsString() + " down msg");
            }
            d.AddLine2(fromASPCount.AddLineIFace_Receive());
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
