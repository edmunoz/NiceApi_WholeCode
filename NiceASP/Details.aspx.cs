using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;

public partial class Data : System.Web.UI.Page
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
                IMyLog log = MyLog.GetLogger("Details");

                Data_AppUserFile user = DSSwitch.appUser().RetrieveOne(sd.LoggedOnUserEmail, log);

                UserName.Text = user.UserName;
                UserEmail.Text = user.Email;
                if (user.MobileNumbersCount() > 5)
                {
                    Tel1.Text = Tel2.Text = Tel3.Text = Tel4.Text = Tel5.Text = "Many";
                }
                else
                {
                    Tel1.Text = user.MobileNumberX(0);
                    Tel2.Text = user.MobileNumberX(1);
                    Tel3.Text = user.MobileNumberX(2);
                    Tel4.Text = user.MobileNumberX(3);
                    Tel5.Text = user.MobileNumberX(4);
                }
                ApiGuId.Text = user.ApiGuId;
                Status.Text = Data_AppUserFile.GetNiceStatusText(user.AccountStatus);
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }
}