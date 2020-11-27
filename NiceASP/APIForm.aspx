<%@ Page Title="Send a test message" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="APIForm.aspx.cs" Inherits="APIForm" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div class="row">
        <div class="col-md-8">
            <section id="loginForm">
                <div class="form-horizontal" runat="server" id="StateError" visible="false">
                    <asp:PlaceHolder runat="server" ID="PlaceHolder1" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="Literal1" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Status" CssClass="col-md-2 control-label">Status</asp:Label>
                        <div class="col-md-10">
                            <asp:Label runat="server" ID="Status" CssClass="form-control" Text="cool" />
                        </div>
                    </div>
                    <div runat="server" id="AdditionalText" />
                </div>
                <div class="form-horizontal" runat="server" id="State1">
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    Select the number to use:
                    <div class="form-group">
                        <asp:RadioButton ID="rbTel1" runat="server" GroupName="telNumerSelection"></asp:RadioButton>
                        <asp:Label runat="server" ID="labTel1"></asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:RadioButton ID="rbTel2" runat="server" GroupName="telNumerSelection"></asp:RadioButton>
                        <asp:Label runat="server" ID="labTel2"></asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:RadioButton ID="rbTel3" runat="server" GroupName="telNumerSelection"></asp:RadioButton>
                        <asp:Label runat="server" ID="labTel3"></asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:RadioButton ID="rbTel4" runat="server" GroupName="telNumerSelection"></asp:RadioButton>
                        <asp:Label runat="server" ID="labTel4"></asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:RadioButton ID="rbTel5" runat="server" GroupName="telNumerSelection"></asp:RadioButton>
                        <asp:Label runat="server" ID="labTel5"></asp:Label>
                    </div>
                    Enter the WhatsApp message text
                    <div class="form-group">
                        <asp:TextBox runat="server" ID="MessageText" Style="resize: none" Width="330px" Height="200px" Wrap="true" CssClass="form-control" MaxLength="500" TextMode="MultiLine" />
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-0 col-md-10">
                            <asp:Button runat="server" OnClick="SendMessage_Click" Text="Send" CssClass="btn btn-default myButton" />
                        </div>
                    </div>

                </div>
                <div class="form-horizontal" runat="server" id="State2" visible="false">
    You have just sent the following HTTP message:
    <div id="sent2" runat="server" style="background-color: black; color: white; font-family: Courier New; max-width: 600px;">
    </div>
    <p></p>
    And you have received the following response:
    <div id="received2" runat="server" style="background-color: black; color: white; font-family: Courier New; max-width: 600px;">
    </div>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
