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

// URL http://getcitydetails.geobytes.com/GetCityDetails?fqcn=127.0.0.1

public partial class Data : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Admin User (protected)
        try
        {
            NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
            if (!IsPostBack)
            {
                string user = Request.QueryString["user"];
                if (user != null)
                {

                    I_UserEmail.Text = user;
                    GetData_Click(null, null);
                }
                else
                {
                    sectionEmailInput.Visible = true;
                }
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    protected void GetData_Click(object sender, EventArgs e)
    {
        try
        {
            IMyLog log = MyLog.GetLogger("DataAll");
            Data_AppUserFile user = DSSwitch.appUser().RetrieveOne(I_UserEmail.Text, log);
            if (user == null)
            {
                I_UserEmail.Text = I_UserEmail.Text + " not found";
            }
            else
            {
                sectionEmailInput.Visible = false;

                L_FileVersion.Text = user._FileVersion.ToString();
                L_UserName.Text = user.UserName;
                L_UserEmail.Text = user.Email;
                L_UserPassword.Text = user.Password;
                L_AllTelNumbers.TextMode = TextBoxMode.MultiLine;
                L_AllTelNumbers.Text = user.MobileNumbers_AllConfirmed__.getVal.My_MobileNumbers_AddNewLine();
                L_AllTelNumbers.Rows = 2 + user.MobileNumbers_AllConfirmed__.MobileNumbersCount;
                L_AllUnconfTelNumbers.TextMode = TextBoxMode.MultiLine;
                L_AllUnconfTelNumbers.Text = user.MobileNumbers_AllUnConfirmed__.getVal.My_MobileNumbers_AddNewLine();
                L_AllUnconfTelNumbers.Rows = 2 + user.MobileNumbers_AllUnConfirmed__.MobileNumbersCount;
                L_ApiGuId.Text = user.ApiGuId;
                L_CreationIp.Text = user.CreationIp;
                L_CountryName.Text = HttpUtility.HtmlEncode(CountryListLoader.Lookup(user.MainTelNo()));
                L_CreatedDate.Text = user.CreationDate.ToSwissTime(true);
                L_Status.Text = Data_AppUserFile.GetNiceStatusText(user.AccountStatus);
                L_StatusExplained.InnerHtml = Data_AppUserFile.GetAccountStatusExplanationHtml();
                L_Comment.TextMode = TextBoxMode.MultiLine;
                L_Comment.Text = user.Comment;
                L_Comment.Rows = user.Comment.Split(null).Length + 2;
                L_DeleteOnFailed.Checked = user.DeleteOnFailed;
                L_AddNumberAllowedWithAPI.Checked = user.AddNumber_AllowedWithAPI;
                L_AddNumberActivateOnSyncRequest.Checked = user.AddNumber_ActivateOnSyncRequest;

                Lf_LastMsgQueued.Text = user.FreeAccount.free_LastMsgQueued.ToUkTime(true);
                Lf_MsgSent.Text = user.FreeAccount.free_MsgSent.ToString();
                Lf_MsgLeft.Text = user.FreeAccount.free_MsgLeft.ToString();
                Lf_MinDelayInSeconds.Text = user.FreeAccount.free_MinDelayInSeconds.ToString();
                Lf_SendFooter.Checked = user.FreeAccount.free_SendFooter;
                Lf_WelcomeCounter.Text = user.FreeAccount.free_WelcomeCounter.ToString();
                Lf_MsgQueued.Text = user.FreeAccount.free_MsgQueued.ToString();

                Lm_LastMsgQueued.Text = user.MonthlyAccount.monthly_LastMsgQueued.ToUkTime(true);
                Lm_MsgSent.Text = user.MonthlyAccount.monthly_MsgSent.ToString();
                Lm_PaidUntil.Text = user.MonthlyAccount.monthly_PaidUntil.ToUkTime(false);
                Lm_MinDelayInSeconds.Text = user.MonthlyAccount.monthly_MinDelayInSeconds.ToString();
                Lm_CostPerNumber.Text = user.MonthlyAccount.monthly_CostPerNumber.ToString();
                Lm_CurrentCredit.Text = user.MonthlyAccount.monthly_CurrentCredit.ToString();

                Lm2_LastMsgQueued.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_LastMsgQueued.ToUkTime(true);
                Lm2_TotalMsgSent.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_TotalMsgSent.ToString();
                Lm2_ThisMonthMsgSent.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_ThisMonthMsgSent.ToString();
                Lm2_PeriodeStart.Text = user.MonthlyDifPriceAccount.monthlDifPricey_PeriodeStart.ToUkTime(true);
                Lm2_PeriodeDurationInDays.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_PeriodeDurationInDays.ToString();
                Lm2_MinDelayInSeconds.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_MinDelayInSeconds.ToString();
                Lm2_CostPerNumber.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_CostPerNumber.ToString();
                Lm2_CurrentCredit.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_CurrentCredit.ToString();
                Lm2_LevelDefinitions.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_LevelDefinitions;
                Lm2_Level.Text = user.MonthlyDifPriceAccount.monthlyDifPrice_Level.ToString();
                Lm2_AutoInceremntLevel.Checked = user.MonthlyDifPriceAccount.monthlyDifPrice_AutoInceremntLevel;
                Lm2_AutoRenewMonthPayment.Checked = user.MonthlyDifPriceAccount.monthlyDifPrice_AutoRenewMonthPayment;

                Lp_LastMsgQueued.Text = user.PayAsSentAccount.payAsSent_LastMsgQueued.ToUkTime(true);
                Lp_MsgSent.Text = user.PayAsSentAccount.payAsSent_MsgSent.ToString();
                Lp_MinDelayInSeconds.Text = user.PayAsSentAccount.payAsSent_MinDelayInSeconds.ToString();
                Lp_CostPerNumber.Text = user.PayAsSentAccount.payAsSent_CostPerNumber.ToString();
                Lp_CostPerMessage.Text = user.PayAsSentAccount.payAsSent_CostPerMessage.ToString();
                Lp_CurrentCredit.Text = user.PayAsSentAccount.payAsSent_CurrentCredit.ToString();

                Ld_LastMsgQueued.Text = user.SystemDuplicationAccount.systemDuplication_LastMsgQueued.ToUkTime(true);
                Ld_MsgSent.Text = user.SystemDuplicationAccount.systemDuplication_MsgSent.ToString();
                Ld_PaidUntil.Text = user.SystemDuplicationAccount.systemDuplication_PaidUntil.ToUkTime(true);

                // set visibility
                switch (user.AccountStatus)
                {
                    case Data_AppUserFile.eUserStatus.free_account:
                        sectinDetails.Visible = true;
                        sectionFree.Visible = true;
                        sectionMonthly.Visible = false;
                        sectionMonthlyDifPrice.Visible = false;
                        sectionPayAsSent.Visible = false;
                        sectionSystemDuplication.Visible = false;
                        break;

                    case Data_AppUserFile.eUserStatus.commercial_monthly:
                        sectinDetails.Visible = true;
                        sectionFree.Visible = false;
                        sectionMonthly.Visible = true;
                        sectionMonthlyDifPrice.Visible = false;
                        sectionPayAsSent.Visible = false;
                        sectionSystemDuplication.Visible = false;
                        break;

                    case Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice:
                        sectinDetails.Visible = true;
                        sectionFree.Visible = false;
                        sectionMonthly.Visible = false;
                        sectionMonthlyDifPrice.Visible = true;
                        sectionPayAsSent.Visible = false;
                        sectionSystemDuplication.Visible = false;
                        break;

                    case Data_AppUserFile.eUserStatus.commercial_payassent:
                        sectinDetails.Visible = true;
                        sectionFree.Visible = false;
                        sectionMonthly.Visible = false;
                        sectionMonthlyDifPrice.Visible = false;
                        sectionPayAsSent.Visible = true;
                        sectionSystemDuplication.Visible = false;
                        break;

                    case Data_AppUserFile.eUserStatus.commercial_systemDuplication:
                        sectinDetails.Visible = true;
                        sectionFree.Visible = false;
                        sectionMonthly.Visible = false;
                        sectionMonthlyDifPrice.Visible = false;
                        sectionPayAsSent.Visible = false;
                        sectionSystemDuplication.Visible = true;
                        break;

                    default:
                        sectinDetails.Visible = true;
                        sectionFree.Visible = true;
                        sectionMonthly.Visible = true;
                        sectionMonthlyDifPrice.Visible = true;
                        sectionPayAsSent.Visible = true;
                        sectionSystemDuplication.Visible = true;

                        break;

                }
            }
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
    }

    protected void ShowDetails_Click(object sender, EventArgs e)
    {
        sectinDetails.Visible = true;
        sectionFree.Visible = true;
        sectionMonthly.Visible = true;
        sectionMonthlyDifPrice.Visible = true;
        sectionPayAsSent.Visible = true;
        sectionSystemDuplication.Visible = true;
    }

    protected void ResetStatus_Click(object sender, EventArgs e)
    {
        L_Status_Store.Checked = true;
        L_Status.Text = "1";
        Lf_WelcomeCounter_Store.Checked = true;
        Lf_WelcomeCounter.Text = "0";
    }

    protected void UpdateData_Click(object sender, EventArgs e)
    {
        try
        {
            DSSwitch.appUser().Update_General(I_UserEmail.Text, delegate(Data_AppUserFile user, Object args)
            {
                UpdateIfChanged(L_UserName_Store, L_UserName, ref user.UserName);
                UpdateIfChanged(L_UserEmail_Store, L_UserEmail, ref user.Email);
                UpdateIfChanged(L_UserEmail_Store, L_ApiGuId, ref user.ApiGuId);
                UpdateIfChanged(L_UserPassword_Store, L_UserPassword, ref user.Password);
                UpdateIfChanged_Mob(L_AllTelNumbers_Store, L_AllTelNumbers, ref user.MobileNumbers_AllConfirmed__);
                UpdateIfChanged_Mob(L_AllUnconfTelNumbers_Store, L_AllUnconfTelNumbers, ref user.MobileNumbers_AllUnConfirmed__);
                UpdateIfChanged(L_CreationIp_Store, L_CreationIp, ref user.CreationIp);
                UpdateIfChanged_Stat(L_Status_Store, L_Status, ref user.AccountStatus);
                UpdateIfChanged(L_Comment_Store, L_Comment, ref user.Comment);
                UpdateIfChanged_Bol(L_DeleteOnFailed_Store, L_DeleteOnFailed, ref user.DeleteOnFailed);
                UpdateIfChanged_Bol(L_AddNumberAllowedWithAPI_Store, L_AddNumberAllowedWithAPI, ref user.AddNumber_AllowedWithAPI);
                UpdateIfChanged_Bol(L_AddNumberActivateOnSyncRequest_Store, L_AddNumberActivateOnSyncRequest, ref user.AddNumber_ActivateOnSyncRequest);

                UpdateIfChanged_I64(Lf_MsgSent_Store, Lf_MsgSent, ref user.FreeAccount.free_MsgSent);
                UpdateIfChanged_I64(Lf_MsgLeft_Store, Lf_MsgLeft, ref user.FreeAccount.free_MsgLeft);
                UpdateIfChanged_I32(Lf_MinDelayInSeconds_Store, Lf_MinDelayInSeconds, ref user.FreeAccount.free_MinDelayInSeconds);
                UpdateIfChanged_Bol(Lf_SendFooter_Store, Lf_SendFooter, ref user.FreeAccount.free_SendFooter);
                UpdateIfChanged_I16(Lf_WelcomeCounter_Store, Lf_WelcomeCounter, ref user.FreeAccount.free_WelcomeCounter);
                UpdateIfChanged_I64(Lf_MsgQueued_Store, Lf_MsgQueued, ref user.FreeAccount.free_MsgQueued);

                UpdateIfChanged_I64(Lm_MsgSent_Store, Lm_MsgSent, ref user.MonthlyAccount.monthly_MsgSent);
                UpdateIfChanged_Dt(Lm_PaidUntil_Store, Lm_PaidUntil, ref user.MonthlyAccount.monthly_PaidUntil);
                UpdateIfChanged_I32(Lm_MinDelayInSeconds_Store, Lm_MinDelayInSeconds, ref user.MonthlyAccount.monthly_MinDelayInSeconds);
                UpdateIfChanged_mon(Lm_CostPerNumber_Store, Lm_CostPerNumber, ref user.MonthlyAccount.monthly_CostPerNumber);
                UpdateIfChanged_mon(Lm_CurrentCredit_Store, Lm_CurrentCredit, ref user.MonthlyAccount.monthly_CurrentCredit);

                UpdateIfChanged_I64(Lm2_TotalMsgSent_Store, Lm2_TotalMsgSent, ref user.MonthlyDifPriceAccount.monthlyDifPrice_TotalMsgSent);
                UpdateIfChanged_I64(Lm2_ThisMonthMsgSent_Store, Lm2_ThisMonthMsgSent, ref user.MonthlyDifPriceAccount.monthlyDifPrice_ThisMonthMsgSent);
                UpdateIfChanged_Dt(Lm2_PeriodeStart_Store, Lm2_PeriodeStart, ref user.MonthlyDifPriceAccount.monthlDifPricey_PeriodeStart);
                UpdateIfChanged_I32(Lm2_PeriodeDurationInDays_Store, Lm2_PeriodeDurationInDays, ref user.MonthlyDifPriceAccount.monthlyDifPrice_PeriodeDurationInDays);
                UpdateIfChanged_I32(Lm2_MinDelayInSeconds_Store, Lm2_MinDelayInSeconds, ref user.MonthlyDifPriceAccount.monthlyDifPrice_MinDelayInSeconds);
                UpdateIfChanged_mon(Lm2_CostPerNumber_Store, Lm2_CostPerNumber, ref user.MonthlyDifPriceAccount.monthlyDifPrice_CostPerNumber);
                UpdateIfChanged_mon(Lm2_CurrentCredit_Store, Lm2_CurrentCredit, ref user.MonthlyDifPriceAccount.monthlyDifPrice_CurrentCredit);
                UpdateIfChanged(Lm2_LevelDefinitions_Store, Lm2_LevelDefinitions, ref user.MonthlyDifPriceAccount.monthlyDifPrice_LevelDefinitions);
                UpdateIfChanged_I32(Lm2_Level_Store, Lm2_Level, ref user.MonthlyDifPriceAccount.monthlyDifPrice_Level);
                UpdateIfChanged_Bol(Lm2_AutoInceremntLevel_Store, Lm2_AutoInceremntLevel, ref user.MonthlyDifPriceAccount.monthlyDifPrice_AutoInceremntLevel);
                UpdateIfChanged_Bol(Lm2_AutoRenewMonthPayment_Store, Lm2_AutoRenewMonthPayment, ref user.MonthlyDifPriceAccount.monthlyDifPrice_AutoRenewMonthPayment);

                UpdateIfChanged_I64(Lp_MsgSent_Store, Lp_MsgSent, ref user.PayAsSentAccount.payAsSent_MsgSent);
                UpdateIfChanged_I32(Lp_MinDelayInSeconds_Store, Lp_MinDelayInSeconds, ref user.PayAsSentAccount.payAsSent_MinDelayInSeconds);
                UpdateIfChanged_mon(Lp_CostPerNumber_Store, Lp_CostPerNumber, ref user.PayAsSentAccount.payAsSent_CostPerNumber);
                UpdateIfChanged_mon(Lp_CostPerMessage_Store, Lp_CostPerMessage, ref user.PayAsSentAccount.payAsSent_CostPerMessage);
                UpdateIfChanged_mon(Lp_CurrentCredit_Store, Lp_CurrentCredit, ref user.PayAsSentAccount.payAsSent_CurrentCredit);

                UpdateIfChanged_I64(Ld_MsgSent_Store, Ld_MsgSent, ref user.SystemDuplicationAccount.systemDuplication_MsgSent);
                UpdateIfChanged_Dt(Ld_PaidUntil_Store, Ld_PaidUntil, ref user.SystemDuplicationAccount.systemDuplication_PaidUntil);

            }, null, null, MyLog.GetLogger("DataAll"));

            SessionData sd = ConstantStrings.GetSessionData(Session);
            sd.QuickMessage = I_UserEmail.Text + " updated";
            sd.QuickMessageGood = true;
            Response.Redirect(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/"));
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
        catch (Exception)
        {
        }
    }

    private void UpdateIfChanged(CheckBox cbChanged, TextBox val, ref string res)
    {
        if (cbChanged.Checked)
        {
            res = val.Text;
        }
    }

    private void UpdateIfChanged_Mob(CheckBox cbChanged, TextBox val, ref MobileNoHandler res)
    {
        if (cbChanged.Checked)
        {
            res = new MobileNoHandler(val.Text);
        }
    }

    private void UpdateIfChanged_Stat(CheckBox cbChanged, TextBox val, ref Data_AppUserFile.eUserStatus res)
    {
        if (cbChanged.Checked)
        {
            res = (Data_AppUserFile.eUserStatus)int.Parse(val.Text);
        }
    }

    private void UpdateIfChanged_Bol(CheckBox cbChanged, CheckBox val, ref bool res)
    {
        if (cbChanged.Checked)
        {
            res = val.Checked;
        }
    }

    private void UpdateIfChanged_mon(CheckBox cbChanged, TextBox val, ref Data_AppUserFile.niceMoney res)
    {
        if (cbChanged.Checked)
        {
            res = Data_AppUserFile.niceMoney.Parse(val.Text);
        }
    }

    private void UpdateIfChanged_I64(CheckBox cbChanged, TextBox val, ref Int64 res)
    {
        if (cbChanged.Checked)
        {
            res = Int64.Parse(val.Text);
        }
    }
    private void UpdateIfChanged_I32(CheckBox cbChanged, TextBox val, ref Int32 res)
    {
        if (cbChanged.Checked)
        {
            res = Int32.Parse(val.Text);
        }
    }
    private void UpdateIfChanged_I16(CheckBox cbChanged, TextBox val, ref Int16 res)
    {
        if (cbChanged.Checked)
        {
            res = Int16.Parse(val.Text);
        }
    }
    private void UpdateIfChanged_Dt(CheckBox cbChanged, TextBox val, ref Int64 res)
    {
        if (cbChanged.Checked)
        {
            // 05/09/2018
            String[] sp = val.Text.Replace("UTC:", "").Replace(".", "/").Split(new char[] { '/',' ' });
            if (sp.Length < 3)
            {
                throw new ArgumentException("UpdateIfChanged_Dt");
            }
            DateTime dt = new DateTime(int.Parse(sp[2]), int.Parse(sp[1]), int.Parse(sp[0]), 0, 0, 0, DateTimeKind.Utc);
            res = dt.Ticks;
        }
    }

    private void SetChecked(CheckBox cb)
    {
        cb.Checked = true;
        cb.Focus();
    }

    protected void L_UserName_TextChanged(object sender, EventArgs e) { SetChecked(L_UserName_Store); }
    protected void L_UserEmail_TextChanged(object sender, EventArgs e)
    {
        SetChecked(L_UserEmail_Store);
        L_ApiGuId.Text = Data_AppUserFile.API_ToId(L_UserEmail.Text, Guid.NewGuid());
    }
    protected void L_UserPassword_TextChanged(object sender, EventArgs e) { SetChecked(L_UserPassword_Store); }
    protected void L_AllTelNumbers_TextChanged(object sender, EventArgs e) { SetChecked(L_AllTelNumbers_Store); }
    protected void L_AllUnconfTelNumbers_TextChanged(object sender, EventArgs e) { SetChecked(L_AllUnconfTelNumbers_Store); }
    protected void L_CreationIp_TextChanged(object sender, EventArgs e) { SetChecked(L_CreationIp_Store); }
    protected void L_Status_TextChanged(object sender, EventArgs e) { SetChecked(L_Status_Store); }
    protected void L_Comment_TextChanged(object sender, EventArgs e) { SetChecked(L_Comment_Store); }
    protected void L_DeleteOnFailed_CheckedChanged(object sender, EventArgs e) { SetChecked(L_DeleteOnFailed_Store); }
    protected void L_AddNumberAllowedWithAPI_CheckedChanged(object sender, EventArgs e) { SetChecked(L_AddNumberAllowedWithAPI_Store); }
    protected void L_AddNumberActivateOnSyncRequest_CheckedChanged(object sender, EventArgs e) { SetChecked(L_AddNumberActivateOnSyncRequest_Store); }
    protected void Lf_MsgSent_TextChanged(object sender, EventArgs e) { SetChecked(Lf_MsgSent_Store); }
    protected void Lf_MsgLeft_TextChanged(object sender, EventArgs e) { SetChecked(Lf_MsgLeft_Store); }
    protected void Lf_MinDelayInSeconds_TextChanged(object sender, EventArgs e) { SetChecked(Lf_MinDelayInSeconds_Store); }
    protected void Lf_SendFooter_CheckedChanged(object sender, EventArgs e) { SetChecked(Lf_SendFooter_Store); }
    protected void Lf_WelcomeCounter_TextChanged(object sender, EventArgs e) { SetChecked(Lf_WelcomeCounter_Store); }
    protected void Lf_MsgQueued_TextChanged(object sender, EventArgs e) { SetChecked(Lf_MsgQueued_Store); }
    protected void Lm_MsgSent_TextChanged(object sender, EventArgs e) { SetChecked(Lm_MsgSent_Store); }
    protected void Lm_PaidUntil_TextChanged(object sender, EventArgs e) { SetChecked(Lm_PaidUntil_Store); }
    protected void Lm_MinDelayInSeconds_TextChanged(object sender, EventArgs e) { SetChecked(Lm_MinDelayInSeconds_Store); }
    protected void Lm_CostPerNumber_TextChanged(object sender, EventArgs e) { SetChecked(Lm_CostPerNumber_Store); }
    protected void Lm_CurrentCredit_TextChanged(object sender, EventArgs e) { SetChecked(Lm_CurrentCredit_Store); }

    protected void Lm2_TotalMsgSent_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_TotalMsgSent_Store); }
    protected void Lm2_ThisMonthMsgSent_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_ThisMonthMsgSent_Store); }
    protected void Lm2_PeriodeStart_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_PeriodeStart_Store); }
    protected void Lm2_PeriodeDurationInDays_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_PeriodeDurationInDays_Store); }
    protected void Lm2_MinDelayInSeconds_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_MinDelayInSeconds_Store); }
    protected void Lm2_CostPerNumber_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_CostPerNumber_Store); }
    protected void Lm2_CurrentCredit_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_CurrentCredit_Store); }
    protected void Lm2_LevelDefinitions_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_LevelDefinitions_Store); }
    protected void Lm2_Level_TextChanged(object sender, EventArgs e) { SetChecked(Lm2_Level_Store); }
    protected void Lm2_AutoInceremntLevel_CheckedChanged(object sender, EventArgs e) { SetChecked(Lm2_AutoInceremntLevel_Store); }
    protected void Lm2_AutoRenewMonthPayment_CheckedChanged(object sender, EventArgs e) { SetChecked(Lm2_AutoRenewMonthPayment_Store); }

    
    protected void Lp_MsgSent_TextChanged(object sender, EventArgs e) { SetChecked(Lp_MsgSent_Store); }
    protected void Lp_MinDelayInSeconds_TextChanged(object sender, EventArgs e) { SetChecked(Lp_MinDelayInSeconds_Store); }
    protected void Lp_CostPerNumber_TextChanged(object sender, EventArgs e) { SetChecked(Lp_CostPerNumber_Store); }
    protected void Lp_CostPerMessage_TextChanged(object sender, EventArgs e) { SetChecked(Lp_CostPerMessage_Store); }
    protected void Lp_CurrentCredit_TextChanged(object sender, EventArgs e) { SetChecked(Lp_CurrentCredit_Store); }
    protected void Ld_MsgSent_TextChanged(object sender, EventArgs e) { SetChecked(Ld_MsgSent_Store); }
    protected void Ld_PaidUntil_TextChanged(object sender, EventArgs e) { SetChecked(Ld_PaidUntil_Store); }
}