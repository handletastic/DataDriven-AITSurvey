<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Question.aspx.cs" Inherits="SQL_Connection.Question" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="usernameLabel" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <br />
    
    </div>
    <asp:PlaceHolder ID="questionPlaceHolder" runat="server"></asp:PlaceHolder>
    <br />
    <asp:Button ID="nextButton" runat="server" onclick="nextButton_Click" 
        Text="Next" />
    <br />
    <br />
    <asp:BulletedList ID="selectedAnswerBulletedList" runat="server">
    </asp:BulletedList>
    </form>
</body>
</html>
