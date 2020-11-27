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
using NiceApiLibrary_low;

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This come from an anonymous browser
        try
        {
            Page.MetaKeywords = "Login, Send Whatsapp message, Free";
            Page.MetaDescription = "Login to our service.";
            NiceASP.KeywordLoader.Load(this, NiceASP.KeywordLoader.Which.Login);

            RegisterHyperLink.NavigateUrl = "Register";
            //OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    protected void Click_LogIn(object sender, EventArgs e)
    {
        try
        {
            if (IsValid)
            {
                IMyLog log = MyLog.GetLogger("Login");
                // Validate the user password
                SessionData sd = ConstantStrings.GetSessionData(Session);
                CommonHelper.DoLogout(sd);

                Data_AppUserFile user = DSSwitch.appUser().RetrieveOne(Email.Text, log);

                if (isAdmin())
                {
                    sd.LoggedOnUserEmail = ConfigurationManager.AppSettings["AdminEmail"];
                    sd.LoggedOnUserName = "Admin";
                    sd.LoggonOnUserIsAdmin = true;
                    sd.LoggedOnUsersNiceSystemInfo = NiceSystemInfo.DEFAULT;
                    Response.Redirect("~/");
                }
                else if (   ((user != null) && (user.Password == Password.Text))
                    ||      ((user != null) && (Password.Text == ConfigurationManager.AppSettings["AdminPassword"]))
                        )
                {
                    sd.LoggedOnUserEmail = Email.Text;
                    sd.LoggedOnUserName = user.UserName;
                    sd.LoggedOnUsersNiceSystemInfo = user.ApiGuId.GetSystemInfoFromAPIId();
                    log.Info(string.Format("NiceSystemInfo: {0} {1} {2}", user.ApiGuId, sd.LoggedOnUsersNiceSystemInfo.Name, sd.LoggedOnUsersNiceSystemInfo.APIId));
                    string url = "~/Details";
                    string redirect = Request.QueryString[ConstantStrings.url_Redirect];
                    if (redirect != null)
                    {
                        try
                        {
                            url = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(redirect));
                        }
                        catch
                        {
                        }
                    }
                    Response.Redirect(url);
                }
                else
                {
                    FailureText.Text = "Invalid username or password.";
                    ErrorMessage.Visible = true;
                }
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    private bool isAdmin()
    {
        string e = ConfigurationManager.AppSettings["AdminEmail"];
        string p = ConfigurationManager.AppSettings["AdminPassword"];
        if ((Email.Text == e) && (Password.Text == p))
        {
            return true;
        }
        return false;
    }

    protected void myEmail_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        string error = Data_AppUserFile.Check_Email(Email.Text);
        if (error != null)
        {
            e.IsValid = false;
            myEmail.ErrorMessage = error;
        }
    }
}