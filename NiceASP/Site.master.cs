using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;
using NiceASP;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        // check for https
        if (!FolderNames.IsDevMachine())
        {
            if ((Request.ServerVariables["HTTPS"] == "off") && (Request.ServerVariables["SERVER_PORT"] == "80"))
            {
                Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + Request.ServerVariables["UNENCODED_URL"]);
            }
            if (Request.ServerVariables["HTTP_HOST"].ToUpper().StartsWith("WWW"))
            {
                Response.Redirect("https://niceapi.net" + Request.ServerVariables["URL"]);
            }
        }

        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // first page load
            SessionData sd = ConstantStrings.GetSessionData(Session);

            string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");

            if (sd.LoggonOnUserIsAdmin)
            {
                // admin
                User_Admin.Text = User_Admin.Text
                    .Replace("{_TheUserName_}", sd.LoggedOnUserName)
                    .Replace("{_root_}", root);

                User_LoggedIn.Visible = false;
                User_Unknown.Visible = false;
                User_Admin.Visible = true;
                TheHeader.Style.Add(HtmlTextWriterStyle.BackgroundColor, "red");
            }
            else if (sd.LoggedOnUserEmail != null)
            {
                // user is logged on
                User_LoggedIn.Text = User_LoggedIn.Text
                    .Replace("{_TheUserName_}", sd.LoggedOnUserName)
                    .Replace("{_root_}", root);

                User_LoggedIn.Visible = true;
                User_Unknown.Visible = false;
                User_Admin.Visible = false;
            }
            else
            {
                // no user logged on
                User_Unknown.Text = User_Unknown.Text.Replace("{_root_}", root);

                User_LoggedIn.Visible = false;
                User_Unknown.Visible = true;
                User_Admin.Visible = false;
            }

            // myMessage
            if (!String.IsNullOrEmpty(sd.QuickMessage))
            {
                // message exists
                if (sd.QuickMessageGood)
                {
                    XmlHelper xh = new XmlHelper();
                    xh.AddRootElemet("br", "", "");
                    xh.AddRootElemet("div", "class", "text-primary").InnerXml = "<h3>" + sd.QuickMessage + "</h3>";
                    myMessage.Text = xh.ToString();
                    myMessage.Visible = true;
                }
                else
                {
                    XmlHelper xh = new XmlHelper();
                    xh.AddRootElemet("br", "", "");
                    xh.AddRootElemet("div", "class", "text-danger").InnerXml = "<h3>" + sd.QuickMessage + "</h3>";
                    myMessage.Text = xh.ToString();
                    myMessage.Visible = true;
                }
                sd.QuickMessage = null;
            }
            else
            {
                // No quick message
                myMessage.Text = "";
                myMessage.Visible = false;
            }

        }
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }
}