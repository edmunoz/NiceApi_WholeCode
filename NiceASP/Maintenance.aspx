<%@ Page Title="Maintenance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Maintenance.aspx.cs" Inherits="Maintenance" %>

<asp:Content ContentPlaceHolderID="HeaderAdditionPlaceHolder" runat="server">
    <link rel="canonical" href="https://niceapi.net/Maintenance" />
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><asp:Literal runat="server" ID="MaintenanceTitle" /></h2>
    <p><asp:Literal runat="server" ID="MaintenanceText" /></p>
    <p style="display: none"><asp:Literal runat="server" ID="MaintenanceDebug" /></p>
</asp:Content>
