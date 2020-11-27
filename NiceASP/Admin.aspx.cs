using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;
using NiceASP;

public partial class Admin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Admin User (protected)
        try
        {
            NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
            StringBuilder sb = new StringBuilder();
            aRef(sb, "https://www.google.com/webmasters/tools/sitemap-list?hl=en", "Google sitemap-list", true);
            aRef(sb, "https://www.google.com/webmasters/tools/testing-tools-links?hl=en", "Google testing tools", true);
            aRef(sb, "https://www.google.com/webmasters/tools/home?hl=en", "Google home", true);
            aRef(sb, "ItemX?id=LibVer", null, true);
            aRef(sb, "ItemX?id=DSSwitch", null, true);
            aRef(sb, "Test?Id=Queue", "Queue", true);
            aRef(sb, "ItemX?id=AllValues", null, true);
            aRef(sb, "Test?Id=ShowUsers&Sort=Date", null, true);
            aRef(sb, "Test?Id=ShowUsers&Sort=State", null, true);
            aRef(sb, "Test?Id=ShowUsers&Sort=Email", null, true);
            aRef(sb, "ItemX?id=GetAllTelNumbers&noSync=1", "TelNoAsPhone", true);
            aRef(sb, "ItemX?id=GetTelNumbers", "TelNoAll", true);
            aRef(sb, "ItemX?id=GetTelNumbersBlockedUsers", "BlockedUsers Tel", true);
            aRef(sb, "https://webmail.NiceApi.net/Login.aspx", "webmail", true);
            aRef(sb, "Loopback", null, false);
            aRef(sb, "AdminImages", null, false);
            aRef(sb, "ItemX?id=ShowSubSystem", null, true);
            infoLine(sb, "CurrentSubSystem: " + SessionData.SessionsSystem_Get(Session).Name, true);
            int subSystemCounter = 0;
            foreach (var sub in DSSwitch.full().GetSystems(false))
            {
                aRef(sb, "ItemX?id=SetSubSystem&val=" + sub.Name, string.Format("SetSubSystem-{0}-{1}", ++subSystemCounter, sub.Name), false);
            }
            infoLine(sb, "SendEmailToDataport".IsAppSettingsTrue() ? "SendEmailToDataport is on" : "SendEmailToDataport is off");
            infoLine(sb, "Logger: " + MyLog.GetLogger("Register").GetLoggerInfo());



            theLinks.InnerHtml = sb.ToString();
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    private static void aRef(StringBuilder sb, string link, string text, bool newWindow)
    {
        text = text == null ? link : text;
        sb.AppendLine("<a href=\"" + link + "\" " + (newWindow ? "target=\"_blank\"" : "") + ">" + text + "</a><br />");
    }

    private static void infoLine(StringBuilder sb, string text, bool simple = false)
    {
        if (simple)
        {
            sb.AppendLine(text + "<br />");
        }
        else
        {
            sb.AppendLine("<pre>" + text + "</pre><br />");
        }
    }
}