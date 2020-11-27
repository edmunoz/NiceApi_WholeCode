using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;

public partial class Logout : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SessionData sd = ConstantStrings.GetSessionData(Session);
        CommonHelper.DoLogout(sd);
        sd.QuickMessage = "Your successfully logged out.";
        sd.QuickMessageGood = true;

        Response.Redirect(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/"));
    }
}