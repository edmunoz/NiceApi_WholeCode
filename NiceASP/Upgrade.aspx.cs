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

public partial class Upgrade : System.Web.UI.Page
{
    SessionData sd;

    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from a logged in user

        NiceASP.SessionData.LoggedOnOrRedirectToLogin(Session, Response, Request);

        sd = ConstantStrings.GetSessionData(Session);

        if (!IsPostBack)
        {
            // check if a committed request exists, if so display it
            Data_AppUserWallet existingWallet = DSSwitch.appWallet().RetrieveOne(
                sd.LoggedOnUserEmail, MyLog.GetVoidLogger());
            Data_AppUserFile user = DSSwitch.appUser().RetrieveOne(sd.LoggedOnUserEmail, MyLog.GetVoidLogger());


            if ((user != null) && (user.AccountStatus != Data_AppUserFile.eUserStatus.free_account))
            {
                MainSection_Normal.Visible = false;
                TitleId.Text = "Upgrade Request";
                string niceName = Data_AppUserFile.GetNiceStatusText(user.AccountStatus);
                Literal1.Text = "You currently hold a " + niceName + " account. Please contact us to do the upgrade.";
            }
            else if ((existingWallet != null) && (existingWallet.HasUpgradeRequest()))
            {
                // display existing request
                showStoredData(existingWallet);
            }
            else
            {
                // no commit yet
                UpdateInfo priceInfo = getHardcodedPriceInfoOrGoBackToPricePage();
                TitleId.Text = priceInfo.Title + " - Upgrade Request";
                userName.Text = sd.LoggedOnUserName;
                InfoText.Text = new UpgradeTextList(priceInfo.Info).GetAsHTML;
                alterStep1DivVisibility(true, priceInfo, 0, 0, 0, 0);
                CalculateButton.Focus();
            }
        }
    }

    private UpdateInfo getHardcodedPriceInfoOrGoBackToPricePage()
    {
        try
        {
            string type = Request["Type"];
            UpdateInfo priceInfo = UpdateInfo.GetInfo(UpdateInfo.ParseType(type));
            if (priceInfo == null)
            {
                throw new ArgumentException();
            }
            return priceInfo;
        }
        catch { }
        Response.Redirect("~/Price");
        return null;
    }

    protected void Calculate_Click(object sender, EventArgs e)
    {
        IMyLog logUpgrade = MyLog.GetLogger("Upgrade");

        try
        {
            UpdateInfo priceInfo = getHardcodedPriceInfoOrGoBackToPricePage();

            int howManyNumbers = 0;
            int.TryParse(HowManyNumbers.SelectedItem.Value, out howManyNumbers);

            int howManyMessages = 0;
            int.TryParse(HowManyMessages.SelectedItem.Value, out howManyMessages);

            int howManyTopups = 0;
            int.TryParse(HowManyTopups.SelectedItem.Value, out howManyTopups);

            int fullpayment = 0;
            int.TryParse(FullPayment.SelectedItem.Value, out fullpayment);

            Data_AppUserWallet wal = new Data_AppUserWallet(
                priceInfo.Type,
                new AmountAndPrice(howManyNumbers, GetPrice(priceInfo.Number)),
                new AmountAndPrice(howManyMessages, GetPrice(priceInfo.Message)),
                new AmountAndPrice(howManyTopups, GetPrice(priceInfo.Month)),
                new AmountAndPrice(1, GetPrice(priceInfo.OneTimeSetup)),
                new AmountAndPrice(fullpayment, GetPrice(priceInfo.FullPayment)));
            CalcInfo.Text = wal.CalucateCost().Explained;
            alterStep1DivVisibility(false, priceInfo,
                HowManyNumbers.SelectedIndex,
                HowManyMessages.SelectedIndex,
                HowManyTopups.SelectedIndex,
                FullPayment.SelectedIndex
                );
            CalcInfoDiv.Visible = true;
            CommitDiv.Visible = true;
            CommitButton.Focus();
        }
        catch (Exception Ex)
        {
            logUpgrade.Debug(Ex.Message);
        }
    }

    private static decimal GetPrice(NiceApiLibrary.UpdateInfo.UpdateInfoSection infoSection)
    {
        if (infoSection == null)
        {
            return 0M;
        }
        return infoSection.CostPerUnit;
    }

    protected void Recalculate_Click(object sender, EventArgs e)
    {
        alterStep1DivVisibility(true, getHardcodedPriceInfoOrGoBackToPricePage(), 0, 0, 0, 0);
        CalcInfoDiv.Visible = false;
        CommitDiv.Visible = false;
        CalculateButton.Focus();
    }

    protected void Commit_Click(object sender, EventArgs e)
    {
        IMyLog logUpgrade = MyLog.GetLogger("Upgrade");
        try
        {
            UpdateInfo priceInfo = getHardcodedPriceInfoOrGoBackToPricePage();

            int howManyNumbers = 0;
            int.TryParse(HowManyNumbers.SelectedItem.Value, out howManyNumbers);

            int howManyMessages = 0;
            int.TryParse(HowManyMessages.SelectedItem.Value, out howManyMessages);

            int howManyTopups = 0;
            int.TryParse(HowManyTopups.SelectedItem.Value, out howManyTopups);

            int fullpayment = 0;
            int.TryParse(FullPayment.SelectedItem.Value, out fullpayment);

            logUpgrade.Info("Commit");

            Data_AppUserWallet wal = Data_AppUserWallet.Create(
                sd.LoggedOnUserEmail,
                TitleId.Text,
                priceInfo.Info,
                priceInfo.Type,
                new AmountAndPrice(howManyNumbers, GetPrice(priceInfo.Number)),
                new AmountAndPrice(howManyMessages, GetPrice(priceInfo.Message)),
                new AmountAndPrice(howManyTopups, GetPrice(priceInfo.Month)),
                new AmountAndPrice(1, GetPrice(priceInfo.OneTimeSetup)),
                new AmountAndPrice(fullpayment, GetPrice(priceInfo.FullPayment)));

            string emailBody = wal.GetEmailBody(sd.LoggedOnUserName, sd.LoggedOnUserEmail);

            bool alreadyThere;
            DSSwitch.appWallet().StoreNew(wal, out alreadyThere, logUpgrade);

            EMail.SendGeneralEmail(sd.LoggedOnUserEmail, true, wal.Title, emailBody, new LogForEmailSend(MyLog.GetLogger("Email")));

            showStoredData(wal);
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
        catch (Exception ex)
        {
            logUpgrade.Debug(ex.Message);
        }
    }

    protected void CancelCommit_Click(object sender, EventArgs e)
    {
        IMyLog logUpgrade = MyLog.GetLogger("Upgrade");
        try
        {
            logUpgrade.Info("CancelCommit_Click");

            sd.QuickMessage = "You have just cancelled your upgrade request";
            sd.QuickMessageGood = true;

            DSSwitch.appWallet().DeleteOne(sd.LoggedOnUserEmail, logUpgrade);

            Response.Redirect("~/Price");
        }
        catch (DataUnavailableException)
        {
            DSSwitch.OnDataUnavailableException(this);
        }
        catch (Exception ex)
        {
            logUpgrade.Debug(ex.Message);
        }
    }

    private void alterStep1DivVisibility(bool visible, UpdateInfo priceInfo,
        int selectedIndexNumbers, int selectedIndexMessages, int selectedIndexTopups, int selectadIndexFullPayment)
    {
        Step1Div7.Visible = visible;

        populateDropDowns(visible, priceInfo == null ? null : priceInfo.Number, selectedIndexNumbers, HowManyNumbers, Step_HowManyNumbers_Div1, Step_HowManyNumbers_Div2);
        populateDropDowns(visible, priceInfo == null ? null : priceInfo.Message, selectedIndexMessages, HowManyMessages, Step_HowManyMessages_Div1, Step_HowManyMessages_Div2);
        populateDropDowns(visible, priceInfo == null ? null : priceInfo.Month, selectedIndexTopups, HowManyTopups, Step_HowManyMonth_Div1, Step_HowManyMonth_Div2);
        populateDropDowns(visible, priceInfo == null ? null : priceInfo.FullPayment, selectadIndexFullPayment, FullPayment, Step_FullPayment_Div1, Step_FullPayment_Div2);
    }

    private static void populateDropDowns(
        bool visible, 
        NiceApiLibrary.UpdateInfo.UpdateInfoSection infoSection,
        int selectedIndex,
        DropDownList dropDownList,
        System.Web.UI.HtmlControls.HtmlGenericControl div1,
        System.Web.UI.HtmlControls.HtmlGenericControl div2)
    {
        if (infoSection == null)
        {
            div1.Visible = false;
            div2.Visible = false;
            return;
        }

        div1.Visible = visible ? infoSection.Active : false;
        div1.InnerText = infoSection.PromptText;
        div2.Visible = visible ? infoSection.Active : false;

        if (infoSection.Active)
        {
            dropDownList.Items.Clear();
            dropDownList.Items.Add("Please Select...");
            if (infoSection.Options != null)
            {
                foreach (UpdateInfo.UpdateInfoOption entry in infoSection.Options)
                {
                    dropDownList.Items.Add(new ListItem(entry.Text, entry.Factor.ToString()));
                }
            }
            dropDownList.SelectedIndex = selectedIndex;
        }
    }

    private void showStoredData(Data_AppUserWallet wal)
    {
        PriceAndText price = wal.CalucateCost();

        TitleId.Text = wal.Title;
        userName.Text = sd.LoggedOnUserName;
        InfoText.Text = wal.DisplayLinesAsHTML();
        CalcInfo.Text = price.Explained;

        commitTextDiv.Visible = true;
        CalcInfoDiv.Visible = true;

        alterStep1DivVisibility(false, null, 0, 0, 0, 0);
        CalcInfoDiv.Visible = true;
        CommitDiv.Visible = false;
        PaymentInstruction.Visible = true;
        paypalAmount.Text = String.Format("{0:0.00} USD", price.FinalPrice);
        paypalLink.HRef = String.Format("https://PayPal.me/NiceAPI/{0:0.00}usd", price.FinalPrice);

        try
        {
            BitcoinPrice.BitcoinPriceInfos currentPrice = BitcoinPrice.PriceFromGecko();
            btcAmount.Text = String.Format("{0:0.00000} BTC", price.FinalPrice / currentPrice.btc.PriceInUSD);
            bchAmount.Text = String.Format("{0:0.00000} BCH", price.FinalPrice / currentPrice.bch.PriceInUSD);
        }
        catch
        { }
    }
}