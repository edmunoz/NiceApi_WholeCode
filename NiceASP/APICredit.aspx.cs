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


public partial class APICredit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the AdminTool

        try
        {
            Response.ContentType = "text/plain";
            string XAPIId = Request.Headers["X-APIId"];
            string email_ = Data_AppUserFile.API_IdToEmail(XAPIId);
            string email = Data_AppUserFile.EmailToRealEmail(email_);

            Data_AppUserFile user = DSSwitch.appUser().RetrieveOne(email, MyLog.GetLogger("APICredit"));

            switch (user.AccountStatus)
            {
                case Data_AppUserFile.eUserStatus.commercial_monthly:
                    Response.Write(user.MonthlyAccount.monthly_CurrentCredit.ToString());
                    break;

                case Data_AppUserFile.eUserStatus.commercial_payassent:
                    Response.Write(user.PayAsSentAccount.payAsSent_CurrentCredit.ToString());
                    break;

                case Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice:
                    Response.Write(user.MonthlyDifPriceAccount.monthlyDifPrice_CurrentCredit.ToString());
                    break;

                case Data_AppUserFile.eUserStatus.commercial_systemDuplication:
                    Response.Write(user.SystemDuplicationAccount.systemDuplication_PaidUntil.ToUkTime(false));
                    break;

                default:
                    Response.Write(Data_AppUserFile.GetNiceStatusText(user.AccountStatus));
                    break;
            }
        }
        catch (Exception)
        {
            Response.ContentType = "text/plain";
            Response.Write("Fehler");
        }
    }
}