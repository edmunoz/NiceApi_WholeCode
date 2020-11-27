<%@ Page Title="Upgrade" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Upgrade.aspx.cs" Inherits="Upgrade" %>

<asp:Content ContentPlaceHolderID="HeaderAdditionPlaceHolder" runat="server">
    <link rel="canonical" href="https://niceapi.net/Upgrade" />

    <style>
        upgradeButton {
            background-color: #FF22FF;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
    <h2>
        <asp:Literal runat="server" ID="TitleId" /></h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="Literal1" />
    </p>

    <div runat="server" class="form-horizontal" ID="MainSection_Normal">
        <div class="form-group">
            You are logged in as
            <asp:Literal ID="userName" runat="server"></asp:Literal>.<br />
        </div>
        <div class="form-group" runat="server" id="commitTextDiv" visible="false">
            <h3>You have committed yourself to the following account upgrade:</h3>
            <br />
        </div>
        <div class="form-group">
            <pre><asp:Literal runat="server" ID="InfoText" /></pre>
        </div>

        <div runat="server" id="Step_HowManyNumbers_Div1">How many numbers, approximately, are you planing to add to your account?</div>
        <div runat="server" id="Step_HowManyNumbers_Div2" class="form-group">
            <div class="col-md-10">
                <asp:DropDownList runat="server" ID="HowManyNumbers" Enabled="true" CssClass="form-control">
                    <asp:ListItem>Please Select ...</asp:ListItem>
                    <asp:ListItem Value="10">&lt; 10</asp:ListItem>
                    <asp:ListItem Value="100">10 - 100</asp:ListItem>
                    <asp:ListItem Value="1000">100 - 1000</asp:ListItem>
                    <asp:ListItem Value="2000">&gt; 1000</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div runat="server" id="Step_HowManyMessages_Div1">How many Whatsapp messages, approximately, are you planing to send each month?</div>
        <div runat="server" id="Step_HowManyMessages_Div2" class="form-group">
            <div class="col-md-10">
                <asp:DropDownList runat="server" ID="HowManyMessages" Enabled="true" CssClass="form-control">
                    <asp:ListItem>Please Select ...</asp:ListItem>
                    <asp:ListItem Value="10">&lt; 10</asp:ListItem>
                    <asp:ListItem Value="50">10 - 50</asp:ListItem>
                    <asp:ListItem Value="100">50 - 100</asp:ListItem>
                    <asp:ListItem Value="200">&gt; 100</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div runat="server" id="Step_HowManyMonth_Div1">How often would you like to make a top up payment?</div>
        <div runat="server" id="Step_HowManyMonth_Div2" class="form-group">
            <div class="col-md-10">
                <asp:DropDownList runat="server" ID="HowManyTopups" Enabled="true" CssClass="form-control">
                    <asp:ListItem>Please Select ...</asp:ListItem>
                    <asp:ListItem Value="1">Once each month</asp:ListItem>
                    <asp:ListItem Value="2">Once every 2 month</asp:ListItem>
                    <asp:ListItem Value="6">Once every 6 month</asp:ListItem>
                    <asp:ListItem Value="12">Once a year</asp:ListItem>
                    <asp:ListItem Value="24">Once every 2 year</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div runat="server" id="Step_FullPayment_Div1">Please choose the amount you would like to put on your account.</div>
        <div runat="server" id="Step_FullPayment_Div2" class="form-group">
            <div class="col-md-10">
                <asp:DropDownList runat="server" ID="FullPayment" Enabled="true" CssClass="form-control">
                    <asp:ListItem>Please Select ...</asp:ListItem>
                    <asp:ListItem Value="10">10 USD</asp:ListItem>
                    <asp:ListItem Value="20">20 USD</asp:ListItem>
                    <asp:ListItem Value="50">50 USD</asp:ListItem>
                    <asp:ListItem Value="100">100 USD</asp:ListItem>
                    <asp:ListItem Value="200">200 USD</asp:ListItem>
                    <asp:ListItem Value="500">500 USD</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div runat="server" id="Step1Div7" class="form-group">
            <div class="col-md-offset-0 col-md-10">
                <asp:Button runat="server" ID="CalculateButton" OnClick="Calculate_Click" Text="Next" CssClass="btn btn-default myButton" />
            </div>
        </div>
        <div class="form-group" runat="server" id="CalcInfoDiv" visible="false">
            <pre><asp:Literal runat="server" ID="CalcInfo"  /></pre>
        </div>
        <div class="form-group" runat="server" id="CommitDiv" visible="false">
            <div class="col-md-offset-0 col-md-10">
                <asp:Button runat="server" ID="RecalcButton" OnClick="Recalculate_Click" Text="Back" CssClass="btn btn-default myButton" />
            </div>
            <div class="col-md-offset-0 col-md-10">
                <asp:Button runat="server" ID="CommitButton" OnClick="Commit_Click" Text="Commit" CssClass="btn btn-default myButton" />
            </div>
        </div>
        <div class="form-group" runat="server" id="PaymentInstruction" visible="false">
            <h3>Please use one of the following payment methods:</h3>
            <hr />
            - PayPal:
            <asp:Literal ID="paypalAmount" runat="server"></asp:Literal><br />
            <a runat="server" id="paypalLink" target="_blank">
                <img src="images/PayPal.png"></a><hr />
            Bitcoin: 
            <asp:Literal ID="btcAmount" runat="server"></asp:Literal><br />
            <img src="images/BitcoinT.png" width="143" height="120">&nbsp;&nbsp;<img width="256" height="256" src="images/BTC_1NEfXKHAkMpFQcL6EufxK5BrMK8f3upCJn.png">
            <h4>1NEfXKHAkMpFQcL6EufxK5BrMK8f3upCJn</h4>
            <hr />
            Bitcoin Cash: 
            <asp:Literal ID="bchAmount" runat="server"></asp:Literal><br />
            <img src="images/BCH.png">&nbsp;&nbsp;<img src="images/BCH_qr50qdxpyugzzuhmlxawezypfj4kgeedhygqwx8p9g.png">
            <h4>qr50qdxpyugzzuhmlxawezypfj4kgeedhygqwx8p9g</h4>
            <hr />
            We will process the upgrade request as soon as the funds arrive.
            <br />
            <div class="col-md-offset-0 col-md-10">
                <asp:Button runat="server" ID="CancelCommitButton" OnClick="CancelCommit_Click" Text="Cancel Commit" CssClass="btn btn-default myButton" />
            </div>

        </div>
    </div>
</asp:Content>
