using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.IO;
using System.Text;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class TrayApp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the TrayApp

        IMyLog trayLog = MyLog.GetLogger("TrayApp");
        LogForEmailSend log4Email = new LogForEmailSend(MyLog.GetLogger("Email"));
        try
        {
            // 1) read in incoming object
            //log.Info("incoming msg");
            Data_Net_Tray2ASP fromTray = new Data_Net_Tray2ASP();
            Stream streamB64 = Request.InputStream;

            if (streamB64.Length == 0)
            {
                using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
                {
                    ld.GetEntry_CreateIfNotThere(NiceSystemInfo.DEFAULT_STR).debugStr = "Zero is nice at " + DateTime.UtcNow.Ticks.ToUkTime(false);
                }
                Response.ContentType = "text/plain";
                Response.Write("Zero is nice!");
                return;
            }

            BinBase64StreamHelper.Tray2ASP_FromB64Stream(ref fromTray, streamB64);
            ASPTray_ObjectList.Counters fromTrayCounter = fromTray.GetCounters();
            if (fromTrayCounter.Total_Results() != 0)
            {
                trayLog.Debug("X " + fromTrayCounter.Total_ResultsString());
            }

            //trayLog.Debug("TrayApp request from " + fromTray.TrayType);
            NiceSystemInfo subSystem = fromTray.TrayType.GetSystemInfoFromTrayType();
            if (subSystem == null)
            {
                using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
                {
                    ld.LastError = string.Format("incomming subSystem '{0}' not recognised. {1}",
                        fromTray.TrayType,
                        DateTime.UtcNow.Ticks.ToUkTime(false));
                }
                throw new SystemException("Unknown System: " + fromTray.TrayType);
            }
            //trayLog.Debug("TrayApp loaded " + subSystem.Name);

            // Update loopback file (with the data from the incoming object)
            using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
            {
                DataFile_Loopback_Entry sub = ld.GetEntry_CreateIfNotThere(subSystem.Name);
                sub.lastTrayConnection = DateTime.UtcNow;
                sub.lastTrayId = fromTrayCounter.Total_ResultsString();
                sub.trayTypeAndIp = fromTray.TrayType + " - " + Request.UserHostAddress + " - " + (Request.IsSecureConnection ? "https" : "httpONLY");
            }

            // 3) Process each file from the incoming object
            MessageProcessing_TrayFrom proc = new MessageProcessing_TrayFrom(subSystem, onScreenshot, trayLog, log4Email);
            proc.Process_TrayFrom(fromTray, true);

            // 5) prepare the object to be sent
            Data_Net_ASP2Tray toTray = new Data_Net_ASP2Tray();
            DateTime dtGo = DateTime.UtcNow.AddSeconds(3);
            while ((toTray.ObjectList.Count == 0) && (dtGo > DateTime.UtcNow))
            {
                // no data yet to send and no timeout
                toTray.ObjectList = MessageProcessing_TrayTo.GetFilesToSendToTray_ConsiderPriority(subSystem, 5, trayLog);
                if (toTray.ObjectList.Count == 0)
                {
                    Thread.Sleep(500);
                }
            }

            // 6) update the loopback file with the sent date
            ASPTray_ObjectList.Counters toTrayCounter = toTray.GetCounters();
            using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
            {
                DataFile_Loopback_Entry sub = ld.GetEntry_CreateIfNotThere(subSystem.Name);
                sub.lastToTray = DateTime.UtcNow;
                sub.lastToTrayData = "FilesToProcess: " + toTrayCounter.Total_RequestsString();
            }

            // 7) send the object to be sent
            //            Response.ContentType = "application/octet-stream";
            Response.ContentType = "text/plain";
            BinBase64StreamHelper.ASP2Tray_ToB64Stream(ref toTray, Response.OutputStream);
        }
        catch (ThreadAbortException)
        {
            trayLog.Debug("ThreadAbortException");
        }
        catch (Exception se)
        {
            trayLog.Error("TrayApp " + se.Message + " " + se.ToString());
            Response.ContentType = "text/plain";
            Response.Write("Exception");
            Response.Write(se.Message);
            Response.Write(se.ToString());
        }
    }

    private void onScreenshot(string b64Data, NiceSystemInfo subSystem)
    {
        // 2) Update the loopback file
        //todo using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
        //{
        //    ld.processedLoopbackStartTime = 
        //    ld.processedLoopbackDoneTime = DateTime.UtcNow;
        //    ld.processedScreenshotStartTime = 
        //    ld.processedScreenshotDoneTime = DateTime.UtcNow;
        //}
        if (!String.IsNullOrEmpty(b64Data))
        {
            DataFile_ScreenShot.Update(subSystem, b64Data);
        }
    }
}