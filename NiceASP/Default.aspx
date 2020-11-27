<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ContentPlaceHolderID="HeaderAdditionPlaceHolder" runat="server">
    <link rel="canonical" href="https://niceapi.net/" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <H1>Send API Whatsapp Message</H1>
    <h2>Let your application send you WhatsApp messages.</h2>
    <h3>This service is NOT designed and NOT suitable for sending spam messages.</h3>
    <p>&nbsp;</p>
    <h4><asp:HyperLink runat="server" ID="HyperLink1" NavigateUrl="~/HowToUse" ViewStateMode="Disabled">How to use the service.</asp:HyperLink></h4>
    <h4><asp:HyperLink runat="server" ID="HowItWorks" NavigateUrl="~/HowItWorks" ViewStateMode="Disabled">How it works.</asp:HyperLink></h4>
    <h4><asp:HyperLink runat="server" ID="Register" NavigateUrl="~/Register" ViewStateMode="Disabled">Registration</asp:HyperLink> is free.</h4>
    <br />
    <p>
Our exciting API provides you a simple means to send a Whatsapp message to your own smartphone. <br />
Whether you have an IPhone, a Windows phones or an Android, our service supports any smartphone.<br />
<br />
The API can be used by any application written in any language as long as your application has access to the Internet. <br />
Most of our users have applications written in PHP, C#, ASP.NET, Java, Go,C++ or any REST API client.<br />
Many of these applications track financial values. <br />
When a certain condition is met, for example the exchange rate between US Dollar and Euro drops, the application might triggers an event and uses our service to alert the owner.<br />
<br />
To do so, your application just needs to send a simple HTTP(S) POST request containing your unique APIId, your mobile phone number and the alert text.<br />
Our host than forwards the alert text as Whatsapp message to your registered smartphone.<br />
</p>
    <br />
    <span style="color:blue">REST API</span> Example:<br />
    <span style="color:blue">POST</span> request to <span style="color:blue">https://niceapi.net/apiv1/message/</span> with the following <span style="color:blue">JSON</span>:<br />
    <div style="background-color:rgba(255,230,230,0.2); color: white; font-family: Courier New; max-width: 600px;">
        <span style="color:rgba(148,180,177,1)">{</span><br />
        <span style="color:rgba(214,166,83,1)">&nbsp;&nbsp;"APIId":&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span style="color:rgba(148,180,177,1)">"&lt;Your unique X-APIId&gt;",</span><br />
        <span style="color:rgba(214,166,83,1)">&nbsp;&nbsp;"APIMobile":&nbsp;</span><span style="color:rgba(148,180,177,1)">"&lt;Mobile number(s)&gt;",</span><br />
        <span style="color:rgba(214,166,83,1)">&nbsp;&nbsp;"Message":&nbsp;&nbsp;&nbsp;</span><span style="color:rgba(148,180,177,1)">"What a great day"</span><br />
        <span style="color:rgba(148,180,177,1)">}</span><br />
    </div>
    <p></p>
    JSON response from our server:<br />
    <div style="background-color:rgba(255,230,230,0.2); color: white; font-family: Courier New; max-width: 600px;">
        <span style="color:rgba(148,180,177,1)">{</span><br />
        <span style="color:rgba(214,166,83,1)">&nbsp;&nbsp;"Result":&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span style="color:rgba(148,180,177,1)">"queued ",</span><br />
        <span style="color:rgba(148,180,177,1)">}</span><br />
    </div>
    <br />
    HTTP Example:<br />
    Your application sends:<br />
    <div id="ExOut" runat="server" style="background-color: black; color: white; font-family: Courier New; max-width: 600px;"/>
    <p></p>
    Our server responses:<br />
    <div id="ExRes" runat="server" style="background-color: black; color: white; font-family: Courier New; max-width: 600px;"/>
    <br />
</asp:Content>
