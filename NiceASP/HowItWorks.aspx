<%@ Page Title="How it works" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="HowItWorks.aspx.cs" Inherits="HowItWorks" %>

<asp:Content ContentPlaceHolderID="HeaderAdditionPlaceHolder" runat="server">
    <link rel="canonical" href="https://niceapi.net/HowItWorks" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>So you have an application that sends you status and event messages via email. That's old standard.</h3>
    <br/><br />
    <div style="float:left">
    <img src="images/Hardware.jpg">
    </div>
Now you can receive your application's messages on your favourite communication tool, WhatsApp.<br />
Our custom made hardware design lets us interact directly with the WhatsApp network to simulate a real smart phone.
    <br/><br />
    <br/><br />


</asp:Content>
