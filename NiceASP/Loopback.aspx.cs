using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class Loopback : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Admin User (protected)
        NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);

        if (!IsPostBack)
        {
            SessionData sd = ConstantStrings.GetSessionData(Session);
            updateValuse();
        }
    }

    private void updateValuse()
    {
        using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ReadOnly_CreateIfNotThere))
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<pre>");
            sb.AppendLine("TimeNow:                      " + DateTime.UtcNow.ToSwissTime(false) + " " + DateTime.UtcNow.ToString());
            sb.AppendLine("ASP build time (utc):         " + ld.ASP_Rebuild_Time.ToSwissTime(true));
            sb.AppendLine("LastError:                    " + ld.LastError);
            sb.AppendLine("Entries.Count:                " + ld.GetEntryCount());
            foreach (System.Collections.Generic.KeyValuePair<string, DataFile_Loopback_Entry> e_ in ld.GetAllEntries())
            {
                DataFile_Loopback_Entry e = e_.Value;
                sb.AppendLine("  ID:                    " + e.EntryId);
                sb.AppendLine("  Last Tray Connection:  " + e.lastTrayConnection.ToSwissTime(true));
                sb.AppendLine("  Last Tray Id:          " + e.lastTrayId);
                sb.AppendLine("  Last Tray out:         " + e.lastToTray.ToSwissTime(true));
                sb.AppendLine("  Last Tray out:         " + e.lastToTrayData);
                sb.AppendLine("  trayTypeAndIp:         " + e.trayTypeAndIp);
                sb.AppendLine("  debugStr:              " + e.debugStr);
                sb.AppendLine("  Last DirectTel Get:    " + e.lastDirectTelGet.ToSwissTime(true));
                sb.AppendLine("  Last DirectTel Ack:    " + e.lastDirectTelAck.ToSwissTime(true));
                sb.AppendLine("-----------------------------------------------");
            }
            sb.AppendLine("</pre>");
            StatusText.InnerHtml = sb.ToString();
        }
    }

    protected void Click_AddScreenshot(object sender, EventArgs e)
    {
        Data_Net__02ScreenshotRequest msg = new Data_Net__02ScreenshotRequest();
        DSSwitch.msgFile02().Store(SessionData.SessionsSystem_Get(Session), msg, MyLog.GetLogger("Loopback"));

        using (DataFile_Loopback ld = new DataFile_Loopback(DataFile_Base.OpenType.ForUpdate_CreateIfNotThere))
        {
            //ld.lastScreenshotAdded = DateTime.UtcNow;
        }
        updateValuse();
    }

    protected void Click_Refresh(object sender, EventArgs e)
    {
        updateValuse();
    }
}