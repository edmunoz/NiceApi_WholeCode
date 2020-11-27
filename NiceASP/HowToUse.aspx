<%@ Page Title="How to use the service" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="HowToUse.aspx.cs" Inherits="HowToUse" %>

<asp:Content ContentPlaceHolderID="HeaderAdditionPlaceHolder" runat="server">
    <link rel="canonical" href="https://niceapi.net/HowToUse" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <p>This page describs what you have to de to use our service.</p>
    <br />
    <p>
        1) You
        <asp:HyperLink runat="server" ID="HyperLnkink1" NavigateUrl="~/Register" ViewStateMode="Disabled">register</asp:HyperLink>
        with your email address and up to three mobile number to which you have access to. We allocate you an unique UserId.
    </p>
    <p>2) You change you application to send a http or hppts post request to our host. This post request contains your UserId, the phone number where you want to send your message to and the message text.</p>
    <p>3) We deliver your message for you.</p>
    <br />
    Here are some examples in various languages showing how to send the HTTP POST request:<br />
    - in <a href="#div_pythen">Pythen</a><br />
    - in <a href="#div_cs">C#</a><br />
    - in <a href="#div_vb">Visual Basic</a><br />
    - in <a href="#div_java">Java</a><br />
    - with <a href="#div_curl">curl</a><br />
    - if you have a working example in another language, <a href="mailto:support@NiceApi.net?Subject=Examples" target="_top">please let us know.</a><br />

    For a list of possible host responses, please see <a href="https://github.com/SupportAtNiceApiDotNet/WhatsAppAPI/wiki" target="_blank">our github wiki.</a><br />

    <p style="margin-bottom: 3cm" />

    <p />
    <div id="div_pythen">
        In Pythen 3<br />
        <style type="text/css">
            span.co {
                color: #a9a9a9;
            }

            span.im {
                color: #7f0074;
            }

            span.te {
                color: #006400;
            }
        </style>
        <div style="background-color: white; color: black; font-family: Courier New;">
            <pre>
<span class="co"># Python 3</span>
<span class="im">import</span> http.client

yourId = <span class="te">&quot;&lt;Your unique X-APIId&gt;&quot;</span>
yourMobile = <span class="te">&quot;&lt;Mobile number&gt;&quot;</span>
yourMessage = <span class="te">&quot;What a great day.&quot;</span>

c = http.client.HTTPSConnection(<span class="te">&quot;NiceApi.net&quot;</span>)
c.request(<span class="te">&quot;POST&quot;</span>, <span class="te">&quot;/API&quot;</span>, yourMessage, {<span class="te">&quot;X-APIId&quot;</span>: yourId, <span class="te">&quot;X-APIMobile&quot;</span>: yourMobile})
response = c.getresponse()
data = response.read()
print (data)

</pre>
        </div>
    </div>

    <p />
    <div id="div_cs">
        In C#:<br />
        <style type="text/css">
            span.bl {
                color: blue;
            }

            span.br {
                color: #2b91af;
            }

            span.te {
                color: #bf4f2c;
            }
        </style>
        <div style="background-color: white; color: black; font-family: Courier New;">
            <pre>
<span class="bl">using </span>System;
<span class="bl">using </span>System.Net;
<span class="bl">using </span>System.Net.Http;
<span class="bl">using </span>System.IO;

<span class="bl">namespace </span>CS_Example
{
    class <span class="br">Program</span>
    {
        <span class="bl">static void</span> Main(<span class="bl">string</span>[] args)
        {
            <span class="bl">string</span> yourId = <span class="te">&quot;&lt;Your unique X-APIId&gt;&quot;</span>;
            <span class="bl">string</span> yourMobile = <span class="te">&quot;&lt;Mobile number&gt;&quot;</span>;
            <span class="bl">string</span> yourMessage = <span class="te">&quot;What a great day.&quot;</span>;

            <span class="bl">try</span>
            {
                <span class="bl">string</span> url = <span class="bl">&quot;https://NiceApi.net/API&quot;</span>;
                <span class="br">HttpWebRequest</span> request = (<span class="br">HttpWebRequest</span>)<span class="br">WebRequest</span>.Create(url);
                request.Method = <span class="te">&quot;POST&quot;</span>
                request.ContentType = <span class="te">&quot;application/x-www-form-urlencoded&quot;</span>
                request.Headers.Add(<span class="te">&quot;X-APIId&quot;</span>, yourId)
                request.Headers.Add(<span class="te">&quot;X-APIMobile&quot;</span>, yourMobile)
                <span class="bl">using</span> (<span class="br">StreamWriter</span> streamOut = <span class="bl">new</span> <span class="br">StreamWriter</span>(request.GetRequestStream()))
                {
                    streamOut.Write(yourMessage);
                }
                <span class="bl">using</span> (<span class="br">StreamReader</span> streamIn = <span class="bl">new</span> <span class="br">StreamReader</span>(request.GetResponse().GetResponseStream()))
                {
                    <span class="br">Console</span>.WriteLine(streamIn.ReadToEnd());
                }
            }
            <span class="bl">catch</span> (<span class="br">SystemException</span> se)
            {
                <span class="br">Console</span>.WriteLine(se.Message);
            }
            <span class="br">Console</span>.ReadLine();
        }
    }
}
</pre>
        </div>
    </div>

    <p />
    <div id="div_vb">
        In Visual Basic:<br />
        <style type="text/css">
            span.bl {
                color: blue;
            }

            span.br {
                color: #2b91af;
            }

            span.te {
                color: #bf4f2c;
            }
        </style>
        <div style="background-color: white; color: black; font-family: Courier New;">
            <pre>
<span class="bl">Imports</span> System
<span class="bl">Imports</span> System.Net
<span class="bl">Imports</span> System.Net.Http
<span class="bl">Imports</span> System.IO

<span class="bl">Module</span> <span class="br">Module1</span>
    <span class="bl">Sub</span> Main()
        <span class="bl">Dim</span> yourId <span class="bl">As String</span> = <span class="te">&quot;&lt;Your unique X-APIId&gt;&quot;</span>
        <span class="bl">Dim</span> yourMobile <span class="bl">As String</span> = <span class="te">&quot;&lt;Mobile number&gt;&quot;</span>
        <span class="bl">Dim</span> yourMessage <span class="bl">As String</span> = <span class="te">&quot;What a great day.&quot;</span>

        <span class="bl">Try</span>
            <span class="bl">Dim</span> url <span class="bl">As String</span> = <span class="bl">&quot;https://NiceApi.net/API&quot;</span>
            <span class="bl">Dim</span> request <span class="bl">As</span> System.Net.<span class="br">HttpWebRequest</span> = <span class="bl">CType</span>(System.Net.<span class="br">WebRequest</span>.Create(url), System.Net.<span class="br">HttpWebRequest</span>)
            request.Method = <span class="te">&quot;POST&quot;</span>;
            request.ContentType = <span class="te">&quot;application/x-www-form-urlencoded&quot;</span>;
            request.Headers.Add(<span class="te">&quot;X-APIId&quot;</span>, yourId);
            request.Headers.Add(<span class="te">&quot;X-APIMobile&quot;</span>, yourMobile);
            <span class="bl">Using</span> streamOut <span class="bl">As New</span> <span class="br">StreamWriter</span>(request.GetRequestStream())
                streamOut.Write(yourMessage)
            <span class="bl">End Using</span>
            <span class="bl">Using</span> streamIn <span class="bl">As New</span> <span class="br">StreamReader</span>(request.GetResponse().GetResponseStream())
                <span class="br">Console</span>.WriteLine(streamIn.ReadToEnd())
            <span class="bl">End Using</span>
        <span class="bl">Catch</span> ex <span class="bl">As</span> <span class="br">Exception</span>
            <span class="br">Console</span>.WriteLine(ex.Message)
        <span class="bl">End Try</span>
    <span class="bl">End Sub</span>
<span class="bl">End Module</span>
</pre>
        </div>
    </div>

    <p />
    <div id="div_java">
        In Java:<br />
        <style type="text/css">
            span.co {
                color: #a9a9a9;
            }

            span.bl {
                color: blue;
            }

            span.br {
                color: #2b91af;
            }

            span.te {
                color: #bf4f2c;
            }
        </style>
        <div style="background-color: white; color: black; font-family: Courier New;">
            <pre>
<span class="co">// tested with OpenJDK version 1.8</span>       
<span class="bl">import</span> java.net.*;
<span class="bl">import</span> java.io.*;

<span class="bl">public class</span> NiceApiDemo <span class="bl">{</span>
    <span class="bl">public static void</span> main(<span class="br">String</span>[] args) { 
        <span class="br">String</span> yourId = <span class="te">&quot;&lt;Your unique X-APIId&gt;&quot;</span>;
        <span class="br">String</span> yourMobile = <span class="te">&quot;&lt;Mobile number&gt;&quot;</span>;
        <span class="br">String</span> yourMessage = <span class="te">&quot;What a great day.&quot;</span>;
        
        <span class="br">HttpURLConnection</span> connection = null;
        <span class="bl">try {</span>
            <span class="br">URL</span> url = new <span class="br">URL</span>(<span class="te">&quot;https://NiceApi.net/API&quot;</span>);
            connection = (<span class="br">HttpURLConnection</span>) url.openConnection();
            connection.setRequestMethod(<span class="te">&quot;POST&quot;</span>);
            connection.setRequestProperty(<span class="te">&quot;X-APIId&quot;</span>, yourId);
            connection.setRequestProperty(<span class="te">&quot;X-APIMobile&quot;</span>, yourMobile);
            connection.setRequestProperty(<span class="te">&quot;Content-Type&quot;</span>, <span class="te">&quot;application/x-www-form-urlencoded&quot;</span>);
            connection.setUseCaches(<span class="bl">false</span>);
            connection.setDoOutput(<span class="bl">true</span>);
           <span class="br"> DataOutputStream</span> streamOut = <span class="bl">new</span> <span class="br">DataOutputStream</span>(connection.getOutputStream());
            streamOut.writeBytes(yourMessage);
            streamOut.close();
            <span class="br">InputStream</span> streamIn = connection.getInputStream();
            <span class="br">BufferedReader</span> readerIn = <span class="bl">new</span> <span class="br">BufferedReader</span>(<span class="bl">new</span> <span class="br">InputStreamReader</span>(streamIn));
            System.out.println(readerIn.readLine());
            readerIn.close();
        <span class="bl">} catch</span> (<span class="br">Exception</span> e) <span class="bl">{</span>
            System.out.println(e.getMessage());
        <span class="bl">} finally {</span>
            if (connection != null) <span class="bl">{</span>
                connection.disconnect();
            <span class="bl">}</span>
        <span class="bl">}</span>
    <span class="bl">}</span>    
<span class="bl">}</span></pre>
        </div>
    </div>

    <p />
    <div id="div_curl">
        With curl:<br />
        <style type="text/css">
span.co { color: #a9a9a9; }
span.bl { color: blue; }
span.br { color: #2b91af; }
span.te { color: #bf4f2c; }
span.ch { color: #ff8010; }
        </style>
        <div style="background-color: white; color: black; font-family: Courier New;">
            <pre>
<span class="bl">curl</span> <span class="br">-d</span> <span class="te">&quot;What a great day.&quot;</span> <span class="br">--header</span> <span class="te">&quot;X-APIId: <span class="ch">&lt;Your unique X-APIId&gt;</span>&quot;</span> <span class="br">--header</span> <span class="te">&quot;X-APIMobile: <span class="ch">&lt;Mobile number&gt;</span>&quot;</span> <span class="bl">https://NiceApi.net/API</span>
            </pre>
        </div>


    </div>

</asp:Content>

