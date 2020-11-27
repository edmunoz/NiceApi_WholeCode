<%@ Page Title="Loopback" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Loopback.aspx.cs" Inherits="Loopback" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div class="row">
        <div class="col-md-8">
            <section id="loginForm">
                <div class="form-horizontal">
                    <h4>Loopback.</h4>
                    <hr />
                    <div id="StatusText" runat="server" style="font-family: Courier New; max-width: 1200px; width: 1200px">
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                            <asp:Button runat="server" OnClick="Click_AddScreenshot" Text="Add Screenshot" CssClass="btn btn-default" />
                        </div>
                        <div class="col-md-offset-2 col-md-10">
                            <asp:Button runat="server" OnClick="Click_Refresh" Text="Refresh" CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </div>
</asp:Content>

