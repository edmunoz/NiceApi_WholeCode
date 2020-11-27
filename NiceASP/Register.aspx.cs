using Microsoft.AspNet.Identity;
using System.Web.UI.WebControls;
using System;
using System.Linq;
using System.Web.UI;
using System.Configuration;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class Register : Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        // This come from an anonymous browser
        try
        {
            Page.MetaKeywords = "Register, Send Whatsapp message, Free";
            Page.MetaDescription = "Register to use the service.";
            NiceASP.KeywordLoader.Load(this, NiceASP.KeywordLoader.Which.Register);

            string query = Request.QueryString["ApiGuId"];
            IMyLog log = MyLog.GetLogger("Register");
            LogForEmailSend log4Email = new LogForEmailSend(MyLog.GetLogger("Email"));

            if (query != null)
            {
                // email validation callback
                query = EMail.Base64_URLDecoding(query);
                MessageProcessing_API api = new MessageProcessing_API(query);
                bool good;
                api.Process_Registration_JustVerified(NiceSystemInfo.DEFAULT, out good, false, true, log, log4Email);
                if (good)
                {
                    // good, 
                    SessionData sd = ConstantStrings.GetSessionData(Session);
                    sd.QuickMessage = "Your email is now verified. Please wait 48 hour for your account to become active.";
                    sd.QuickMessageGood = true;
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    // bad
                    SessionData sd = ConstantStrings.GetSessionData(Session);
                    sd.QuickMessage = "This token is expired.";
                    sd.QuickMessageGood = false;
                    Response.Redirect("~/");
                }
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    protected void CreateUser_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            if (Page.IsValid)
            {
                IMyLog log = MyLog.GetLogger("Register");
                LogForEmailSend log4Email = new LogForEmailSend(MyLog.GetLogger("Email"));

                MessageProcessing_API api = new MessageProcessing_API(null);
                bool fileArleadyUsed;
                api.Process_Registration(
                    NiceSystemInfo.DEFAULT,
                    out fileArleadyUsed,
                    true,
                    UserName.Text,
                    Tel1.Text + Tel2.Text + Tel3.Text + Tel4.Text + Tel5.Text,
                    Email.Text,
                    Password.Text,
                    GetIPAddress(),
                    WhereHeard.Text,
                    log,
                    log4Email);

                if (fileArleadyUsed)
                {
                    SessionData sd = ConstantStrings.GetSessionData(Session);
                    sd.QuickMessage = "Your registratin failed. Please try again later.";
                    sd.QuickMessageGood = false;
                    Response.Redirect("~/Register.aspx");
                }
                else
                {
                    // ok
                    SessionData sd = ConstantStrings.GetSessionData(Session);
                    sd.QuickMessage = "Verify your email: We sent a verification email to " + Email.Text + " .Click the link in the email to get started!";
                    sd.QuickMessageGood = true;
                    Response.Redirect("~/Login.aspx");
                }
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    protected string GetIPAddress()
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;
        string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length != 0)
            {
                return addresses[0];
            }
        }

        return context.Request.ServerVariables["REMOTE_ADDR"];
    }

    #region ServerValidate
    private void setError(Label label, string text, ref bool bOk)
    {
        label.Visible = true;
        label.Text = text;

        errorSummary.Visible = true;
        if (errorSummary.Text.Length > 0)
        {
            errorSummary.Text += "<br>";
        }
        errorSummary.Text += text;

        bOk = false;
    }
    private void resetError(Label label)
    {
        label.Text = "";
        label.Visible = false;
    }
    protected void allFieldsValidation(object sender, ServerValidateEventArgs e)
    {
        bool bOk = true;
        errorSummary.Text = "";
        errorSummary.Visible = false;

        // basic checks
        checkUserName_ServerValidate(ref bOk);
        checkTelx_ServerValidate(ref bOk, 1);
        checkTelx_ServerValidate(ref bOk, 2);
        checkTelx_ServerValidate(ref bOk, 3);
        checkTelx_ServerValidate(ref bOk, 4);
        checkTelx_ServerValidate(ref bOk, 5);
        checkEmail_ServerValidate(ref bOk);
        checkPasswordWithConfirm_ServerValidate(ref bOk);

        if (bOk)
        {
            IMyLog log = MyLog.GetLogger("Register");

            // basic test ok, see if the numbers are already used
            TelListController currentList = DSSwitch.appUser().GetCurrentTelList(log);
            using (var _lock = currentList.GetLock())
            {
                checkTelx_AlreadyRegistred(ref bOk, 1, _lock.Locked);
                checkTelx_AlreadyRegistred(ref bOk, 2, _lock.Locked);
                checkTelx_AlreadyRegistred(ref bOk, 3, _lock.Locked);
                checkTelx_AlreadyRegistred(ref bOk, 4, _lock.Locked);
                checkTelx_AlreadyRegistred(ref bOk, 5, _lock.Locked);
            }
        }

        e.IsValid = bOk;
    }

    protected void checkUserName_ServerValidate(ref bool bOk)
    {
        resetError(errorUserName);
        string error = Data_AppUserFile.Check_UserName(UserName.Text);
        if (error != null)
        {
            setError(errorUserName, error, ref bOk);
        }
    }

    protected void myNoSpam_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        if (!NoSpam.Checked)
        {
            e.IsValid = false;
        }
    }

    protected void checkTelx_AlreadyRegistred(ref bool bOk, int id, TelListController.LockedTelListController current)
    {
        string userInput = null;
        Label errorLabel = null;
        switch (id)
        {
            case 1: userInput = Tel1.Text; errorLabel = errorTel1; resetError(errorLabel); break;
            case 2: userInput = Tel2.Text; errorLabel = errorTel2; resetError(errorLabel); break;
            case 3: userInput = Tel3.Text; errorLabel = errorTel3; resetError(errorLabel); break;
            case 4: userInput = Tel4.Text; errorLabel = errorTel4; resetError(errorLabel); break;
            case 5: userInput = Tel5.Text; errorLabel = errorTel5; resetError(errorLabel); break;
            default: return;
        }

        if (current.GetEntry(userInput) != null)
        {
            if (!"RegisterAllowTwice".IsAppSettingsSame(userInput))
            {
                // already registred
                setError(errorLabel, "This Mobile number is already registered . <br>Please  <a href=\"mailto:support@NiceApi.net?Subject=DoubleRegistration\">contact us</a> if this number belongs to you and you have not registred it already using a different email address.", ref bOk);
            }
        }
    }

    protected void checkTelx_ServerValidate(ref bool bOk, int id)
    {
        string userInput = null;
        Label errorLabel = null;
        switch (id)
        {
            case 1: 
                userInput = Tel1.Text; errorLabel = errorTel1; resetError(errorLabel);

                if (Tel1.Text.Length == 0)
                {
                    setError(errorTel1, "MobileNumber 1 must be set", ref bOk);
                }
                break;
            case 2: userInput = Tel2.Text; errorLabel = errorTel2; resetError(errorLabel); break;
            case 3: userInput = Tel3.Text; errorLabel = errorTel3; resetError(errorLabel); break;
            case 4: userInput = Tel4.Text; errorLabel = errorTel4; resetError(errorLabel); break;
            case 5: userInput = Tel5.Text; errorLabel = errorTel5; resetError(errorLabel); break;
            default: return;
        }
        string error = Data_AppUserFile.Check_MobileNumber(userInput, id);
        if (error != null)
        {
            setError(errorLabel, error, ref bOk);
        }
    }

    protected void checkEmail_ServerValidate(ref bool bOk)
    {
        IMyLog log = MyLog.GetLogger("Register");
        resetError(errorEmail);
        string error = Data_AppUserFile.Check_Email(Email.Text);
        if (error != null)
        {
            setError(errorEmail, error, ref bOk);
        }
        else if (DSSwitch.appUser().HasAccount(Email.Text, log))
        {
            setError(errorEmail, "Email is already registred.", ref bOk);
        }
        else if (isOnBlacklist(Email.Text))
        {
            log.Debug("Blacklisted " + Email.Text + " refused");
            setError(errorEmail, "Email is invalid", ref bOk);
        }
    }
    private bool isOnBlacklist(string email)
    {
        try
        { 
            foreach (string black1 in ConfigurationManager.AppSettings["EmailBlackList"].Split(new char[] {','}))
            {
                if (email.Contains(black1))
                {
                    return true;
                }
            }
        }
        catch (Exception)
        {

        }
        return false;
    }

    protected void checkPasswordWithConfirm_ServerValidate(ref bool bOk)
    {
        resetError(errorConfirmPassword);
        resetError(errorPassword);
        if (Password.Text != ConfirmPassword.Text)
        {
            setError(errorConfirmPassword, "The password and confirm password do not match.", ref bOk);
        }
        else
        {
            string error = Data_AppUserFile.Check_Password(Password.Text);
            if (error != null)
            {
                setError(errorPassword, error, ref bOk);
            }
        }
    }
    #endregion ServerValidate

    protected void NoSpam_CheckedChanged(object sender, EventArgs e)
    {
        if (NoSpam.Checked)
        {
            Email.Enabled = Password.Enabled = ConfirmPassword.Enabled = WhereHeard.Enabled = CreateButton.Enabled =
            Tel1.Enabled = Tel2.Enabled = Tel3.Enabled = Tel4.Enabled = Tel5.Enabled = true;
        }
        else
        {
            Email.Enabled = Password.Enabled = ConfirmPassword.Enabled = WhereHeard.Enabled = CreateButton.Enabled =
            Tel1.Enabled = Tel2.Enabled = Tel3.Enabled = Tel4.Enabled = Tel5.Enabled = false;
        }
    }
}