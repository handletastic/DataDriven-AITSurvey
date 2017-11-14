<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SQL_Connection.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Label ID="userNameLabel" runat="server" Text="Username"></asp:Label>
&nbsp;
    <asp:TextBox ID="userNameTextBox" runat="server" Width="521px"></asp:TextBox>
    <br />
    <br />
    <asp:Label ID="passwordLabel" runat="server" Text="Password"></asp:Label>
&nbsp;<asp:TextBox ID="passwordTextBox" runat="server" style="margin-left: 12px" 
        Width="519px"></asp:TextBox>
    <br />
    <br />
    <asp:Button ID="loginButton" runat="server" onclick="loginButton_Click" 
        Text="Login" />
    </form>
</body>
</html>
