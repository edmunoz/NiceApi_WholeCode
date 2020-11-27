<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" %>

<asp:Content ContentPlaceHolderID="HeaderAdditionPlaceHolder" runat="server">
    <link rel="canonical" href="https://niceapi.net/Register" />
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2>Create a new account.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <div class="form-horizontal">
        <asp:Label runat="server" CssClass="text-danger" ID="errorSummary" />
        <asp:CustomValidator runat="server" ID="myAllCheck" 
            OnServerValidate="allFieldsValidation" CssClass="text-danger" />

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-2 control-label">User name</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorUserName" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
You will be able to send WhatsApp messages to the mobile numbers below.<br />
By clicking the box below you confirm that you have access to all the mobile numbers below and that you will not send spam using this service.
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="NoSpam" CssClass="col-md-2 control-label">No spam</asp:Label>
            <div class="col-md-10">
                <asp:CheckBox runat="server" ID="NoSpam" CssClass="form-control" AutoPostBack="True" 
                    OnCheckedChanged="NoSpam_CheckedChanged"
                    Text="&emsp;I will not use this service to send unsolicited messages." />
                <asp:CustomValidator runat="server" ID="myNoSpam" 
                    OnServerValidate="myNoSpam_ServerValidate" CssClass="text-danger" ErrorMessage="You must select the No spam box to proceed." />
            </div>
        </div>
        <div class="form-group">
Please enter YOUR phone numbers where you have WhatsApp installed.<br />
The number must be in the format +&lt;CountryCode&gt;&lt;AreaCode&gt;&lt;PhoneNumber&gt;
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Tel1" CssClass="col-md-2 control-label">Mobile # 1</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="Tel1" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorTel1" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Tel2" CssClass="col-md-2 control-label">Mobile # 2</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="Tel2" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorTel2" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Tel3" CssClass="col-md-2 control-label">Mobile # 3</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="Tel3" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorTel3" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Tel4" CssClass="col-md-2 control-label">Mobile # 4</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="Tel4" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorTel4" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Tel5" CssClass="col-md-2 control-label">Mobile # 5</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="Tel5" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorTel5" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">User email</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="Email" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorEmail" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="Password" TextMode="Password" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorPassword" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="ConfirmPassword" CssClass="col-md-2 control-label">Confirm password</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
                <asp:Label runat="server" Visible="false" ID="errorConfirmPassword" CssClass="test-danger" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="WhereHeard" CssClass="col-md-2 control-label">Where did you hear about us</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" Enabled="false" ID="WhereHeard" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button runat="server" Enabled="false" ID="CreateButton" OnClick="CreateUser_Click" Text="Register" CssClass="btn btn-default" />
            </div>  
        </div>
    </div>
</asp:Content>

