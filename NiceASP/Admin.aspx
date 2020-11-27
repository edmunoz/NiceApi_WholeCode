<%@ Page Title="Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>
    <div class="row">
        <div class="col-md-8">
            <div class="form-horizontal" runat="server" id="theLinks">
            </div>
        </div>
    </div>
</asp:Content>


