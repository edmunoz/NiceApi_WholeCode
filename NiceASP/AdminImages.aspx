<%@ Page Title="AdminImages" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AdminImages.aspx.cs" Inherits="AdminImages" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div class="row">
        <div class="col-md-8">
            <section id="loginForm">
                <div class="form-horizontal">
                    <hr />
                    <asp:Literal runat="server" ID="LiteralText" />
                </div>
                <div class="form-horizontal">
                    <hr />
                     <img src="ItemX.aspx?id=Screen" width="960" height="540" border="2" />
                </div>
            </section>
        </div>
    </div>
</asp:Content>

