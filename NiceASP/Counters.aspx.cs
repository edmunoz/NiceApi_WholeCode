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

public partial class Counters : System.Web.UI.Page
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
                Data_AppUserFile user = DSSwitch.appUser().RetrieveOne(sd.LoggedOnUserEmail, MyLog.GetLogger("Counters"));

                if (user != null)
                {
                    switch (user.AccountStatus)
                    {
                        case Data_AppUserFile.eUserStatus.free_account:
                            sectionFree.Visible = true;
                            F_Tel1.Text = user.MobileNumbers_AllConfirmed__.MobileNumberX(0);
                            F_Tel2.Text = user.MobileNumbers_AllConfirmed__.MobileNumberX(1);
                            F_Tel3.Text = user.MobileNumbers_AllConfirmed__.MobileNumberX(2);
                            F_Tel4.Text = user.MobileNumbers_AllConfirmed__.MobileNumberX(3);
                            F_Tel5.Text = user.MobileNumbers_AllConfirmed__.MobileNumberX(4);
                            F_LastMsgQueued.Text = user.FreeAccount.free_LastMsgQueued.ToUkTime(false);
                            F_MsgSent.Text = (user.FreeAccount.free_MsgQueued + user.FreeAccount.free_MsgSent).ToString();
                            F_MsgLeft.Text = user.FreeAccount.free_MsgLeft.ToString();
                            F_MinDelayInSeconds.Text = user.FreeAccount.free_MinDelayInSeconds.ToString() + " seconds";
                            break;

                        case Data_AppUserFile.eUserStatus.commercial_monthly:
                            sectionMonthly.Visible = true;
                            M_Tels.Text = user.MobileNumbers_AllConfirmed__.getVal;
                            M_ActiveUntil.Text = user.MonthlyAccount.monthly_PaidUntil.ToUkTime(false);
                            if (user.MonthlyAccount.monthly_PaidUntil < DateTime.UtcNow.Ticks)
                            {
                                M_ActiveUntil.BackColor = System.Drawing.Color.Red;
                            }

                            M_LastQueued.Text = user.MonthlyAccount.monthly_LastMsgQueued.ToUkTime(false);
                            M_CurrentCredit.Text = user.MonthlyAccount.monthly_CurrentCredit.ToString();
                            M_CostPerNumber.Text = user.MonthlyAccount.monthly_CostPerNumber.ToString();
                            M_MsgSent.Text = user.MonthlyAccount.monthly_MsgSent.ToString();
                            M_MinDelayInSeconds.Text = user.MonthlyAccount.monthly_MinDelayInSeconds.ToString() + " seconds";
                            break;

                        case Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice:
                            sectionMonthlyDifPrice.Visible = true;
                            M2_Tels.Text = user.MobileNumbers_AllConfirmed__.getVal;
                            M2_ActiveUntil.Text = user.MonthlyDifPriceAccount.PaidUntil().Ticks.ToUkTime(true);
                            if (user.MonthlyDifPriceAccount.PaidUntil() < DateTime.UtcNow)
                            {
                                M2_ActiveUntil.BackColor = System.Drawing.Color.Red;
                            }
                            M2_LastQueued.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_LastMsgQueued.ToUkTime(false);
                            M2_CurrentCredit.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_CurrentCredit.ToString();
                            M2_CostPerNumber.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_CostPerNumber.ToString();
                            M2_TotalMsgSent.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_TotalMsgSent.ToString();
                            M2_MsgSentThisPeriod.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_ThisMonthMsgSent.ToString();
                            break;

                        case Data_AppUserFile.eUserStatus.commercial_payassent:
                            sectionPayAsSent.Visible = true;
                            P_Tels.Text = user.MobileNumbers_AllConfirmed__.getVal;
                            P_CurrentCredit.Text = user.PayAsSentAccount.payAsSent_CurrentCredit.ToString();
                            P_CostPerNumber.Text = user.PayAsSentAccount.payAsSent_CostPerNumber.ToString();
                            P_CostPerMessage.Text = user.PayAsSentAccount.payAsSent_CostPerMessage.ToString();
                            P_LastQueued.Text = user.PayAsSentAccount.payAsSent_LastMsgQueued.ToUkTime(false);
                            P_MsgSent.Text = user.PayAsSentAccount.payAsSent_MsgSent.ToString();
                            P_MinDelayInSeconds.Text = user.PayAsSentAccount.payAsSent_MinDelayInSeconds.ToString() + " seconds";
                            break;

                        case Data_AppUserFile.eUserStatus.commercial_systemDuplication:
                            sectionSystemDuplication.Visible = true;
                            D_ActiveUntil.Text = user.SystemDuplicationAccount.systemDuplication_PaidUntil.ToUkTime(false);
                            if (user.SystemDuplicationAccount.systemDuplication_PaidUntil < DateTime.UtcNow.Ticks)
                            {
                                D_ActiveUntil.BackColor = System.Drawing.Color.Red;
                            }

                            D_LastQueued.Text = user.SystemDuplicationAccount.systemDuplication_LastMsgQueued.ToUkTime(false);
                            D_MsgSent.Text = user.SystemDuplicationAccount.systemDuplication_MsgSent.ToString();
                            break;

                        default:
                            sectionWrong.Visible = true;
                            X_Status.Text = Data_AppUserFile.GetNiceStatusText(user.AccountStatus);
                            break;

                    }
                }
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }
}