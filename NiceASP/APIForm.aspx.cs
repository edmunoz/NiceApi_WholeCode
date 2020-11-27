using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class APIForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from a logged in user
        try
        {
            NiceASP.SessionData.LoggedOnOrRedirectToLogin(Session, Response, Request);

            if (!IsPostBack)
            {
                SessionData sd = ConstantStrings.GetSessionData(Session);
                // refresh from file
                Data_AppUserFile user = DSSwitch.appUser().RetrieveOne(sd.LoggedOnUserEmail, MyLog.GetLogger("APIForm"));
                rbTel1.Checked = true;
                setTelNo(labTel1, rbTel1, user.MobileNumberX(0));
                setTelNo(labTel2, rbTel2, user.MobileNumberX(1));
                setTelNo(labTel3, rbTel3, user.MobileNumberX(2));
                setTelNo(labTel4, rbTel4, user.MobileNumberX(3));
                setTelNo(labTel5, rbTel5, user.MobileNumberX(4));
                MessageText.Text = "What a great day.";
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    private static void setTelNo(Label l, RadioButton b, string tel)
    {
        if (!string.IsNullOrEmpty(tel))
        {
            l.Text = tel;
        }
        else
        {
            l.Visible = false;
            b.Visible = false;
        }
    }

    void getTextDependOnRadiobox(RadioButton r, string inText, ref string outText, ref bool found)
    {
        if (r.Checked)
        {
            if (!String.IsNullOrEmpty(inText))
            {
                outText = inText;
                found = true;
            }
        }
    }

    protected void SendMessage_Click(object sender, EventArgs e)
    {
        MyLog.GetLogger("APIForm").Info("APIForm send message");
        SessionData sd = ConstantStrings.GetSessionData(Session);
        Data_AppUserFile user = null;
        try
        {
            user = DSSwitch.appUser().RetrieveOne(sd.LoggedOnUserEmail, MyLog.GetLogger("APIForm"));
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
        bool bFound = false;
        string telText = "";
        getTextDependOnRadiobox(rbTel1, user.MobileNumberX(0), ref telText, ref bFound);
        getTextDependOnRadiobox(rbTel2, user.MobileNumberX(1), ref telText, ref bFound);
        getTextDependOnRadiobox(rbTel3, user.MobileNumberX(2), ref telText, ref bFound);
        getTextDependOnRadiobox(rbTel4, user.MobileNumberX(3), ref telText, ref bFound);
        getTextDependOnRadiobox(rbTel5, user.MobileNumberX(4), ref telText, ref bFound);

        if (bFound)
        {
            string sRet = new APIActualSending(sd.LoggedOnUsersNiceSystemInfo).SendWhatsApp(user.ApiGuId, telText, MessageText.Text, MyLog.GetLogger("APIForm"));
            MyLog.GetLogger("APIForm").Info("APIForm " + sRet);
            State1.Visible = false;
            State2.Visible = true;
            sent2.InnerHtml = APIActualSending.fakeRequest(user.ApiGuId, telText, MessageText.Text);
            received2.InnerHtml = APIActualSending.fakeResponse(sRet);
        }
    }
}