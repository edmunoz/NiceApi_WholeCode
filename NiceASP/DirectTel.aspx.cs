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
using Newtonsoft.Json;

public partial class DirectTel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Android direct

        IMyLog trayLog = MyLog.GetLogger("TrayApp");
        trayLog.Info("Incoming 17:00");
        string inBody = "notSet";

        try
        {
            LogForEmailSend log4Email = new LogForEmailSend(MyLog.GetLogger("Email"));
            Stream inStream = Request.InputStream;

            if (inStream.Length == 0)
            {
                Response.ContentType = "text/plain";
                Response.Write("DirectTel: Zero!");
                return;
            }

            DirectTel_InJson inJson = null;
            using (var sr = new StreamReader(inStream))
            {
                inBody = sr.ReadToEnd();
                inJson = JsonConvert.DeserializeObject<DirectTel_InJson>(inBody);
            }
            trayLog.Debug("In Json: ");
            trayLog.Debug(JsonConvert.SerializeObject(inJson));

            DirectTel_OutJson outJson = DirectTel_Processing.ProcessDirectTel(inJson, trayLog, log4Email, 
                delegate (NiceSystemInfo subSystem) 
                {
                    //OnGet
                    // Update loopback file (with the data from the incoming object)
                    using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
                    {
                        DataFile_Loopback_Entry sub = ld.GetEntry_CreateIfNotThere(subSystem.Name);
                        sub.lastDirectTelGet = DateTime.UtcNow;
                    }
                }, 
                delegate (NiceSystemInfo subSystem) 
                {
                    //OnAck
                    // Update loopback file (with the data from the incoming object)
                    using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
                    {
                        DataFile_Loopback_Entry sub = ld.GetEntry_CreateIfNotThere(subSystem.Name);
                        sub.lastDirectTelAck = DateTime.UtcNow;
                    }
                });

            trayLog.Debug("Out Json: ");
            trayLog.Debug(JsonConvert.SerializeObject(outJson));
            Tx(outJson);
        }
        catch (ThreadAbortException)
        {
            trayLog.Debug("ThreadAbortException");
        }
        catch (Exception se)
        {
            trayLog.Error("DirectTel " + se.Message + " " + se.ToString());
            trayLog.Error("DirectTel " + inBody);
            Response.ContentType = "text/plain";
            Response.Write("Exception");
            Response.Write(se.Message);
            Response.Write(se.ToString());
        }
    }

    private void Tx(DirectTel_OutJson outJson)
    {
        Response.ContentType = "application/json";
        using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
        {
            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, outJson);
                jsonWriter.Flush();
            }
        }
    }
}

