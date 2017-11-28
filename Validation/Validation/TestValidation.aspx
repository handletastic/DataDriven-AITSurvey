<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestValidation.aspx.cs" Inherits="Validation.TestValidation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" Text="Name"></asp:Label>
&nbsp;<asp:TextBox ID="NameTextbox" runat="server"></asp:TextBox>
&nbsp;<asp:RequiredFieldValidator ID="NameRequiredFieldValidator" runat="server" 
            ControlToValidate="NameTextbox" Display="Dynamic" 
            ErrorMessage="Name is required!"></asp:RequiredFieldValidator>
    
    </div>
    <asp:Label ID="Label2" runat="server" Text="DoB"></asp:Label>
&nbsp;<asp:TextBox ID="DobTextbox" runat="server"></asp:TextBox>
&nbsp;<asp:RangeValidator ID="DobRangeValidator" runat="server" 
        ControlToValidate="DobTextbox" Display="Dynamic" 
        ErrorMessage="Date should be &gt; 1869!" MaximumValue="2018/03/23" 
        MinimumValue="1869/01/01" Type="Date"></asp:RangeValidator>
    <br />
    <asp:Label ID="Label3" runat="server" Text="Age"></asp:Label>
&nbsp;<asp:TextBox ID="AgeTextbox" runat="server"></asp:TextBox>
&nbsp;<asp:CompareValidator ID="AgeCompareValidator" runat="server" 
        ControlToValidate="AgeTextbox" Display="Dynamic" 
        ErrorMessage="You must be at least 18 years old or older!" 
        Operator="GreaterThanEqual" Type="Integer" ValueToCompare="18"></asp:CompareValidator>
    <br />
    <asp:Label ID="TextBox" runat="server" Text="Email"></asp:Label>
&nbsp;<asp:TextBox ID="EmailTextbox" runat="server"></asp:TextBox>
&nbsp;<asp:RegularExpressionValidator ID="EmailRegularExpressionValidator" 
        runat="server" ControlToValidate="EmailTextbox" Display="Dynamic" 
        ErrorMessage="Invalid email address!" 
        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
    <p>
        <asp:Button ID="SubmitButton" runat="server" Text="Submit" />
    </p>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        HeaderText="&lt;strong&gt;Please review errors&lt;/strong&gt;" />
    </form>
</body>
</html>
