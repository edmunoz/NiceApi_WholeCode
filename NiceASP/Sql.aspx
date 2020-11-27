<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sql.aspx.cs" Inherits="Sql" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a href="./">Home</a><br />
        <br />
        <asp:TextBox ID="TextBox_Cfg" runat="server" Width="100%" BackColor="lightyellow"  />
        <asp:TextBox ID="TextBox_Sql" runat="server" Height="200px" Width="100%" BackColor="lightsalmon" TextMode="MultiLine" />
        <asp:Button ID="Button1" runat="server" Text="Go" onclick="Button1_Click" />
        <br />
        <br />
        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>    

    </div>
    </form>
</body>
</html>