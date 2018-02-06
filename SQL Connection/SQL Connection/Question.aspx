<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Question.aspx.cs" Inherits="SQL_Connection.Question" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:PlaceHolder ID="questionPlaceHolder" runat="server"></asp:PlaceHolder>
    <br />
        <div class="row">
                        
            <!-- next button -->
            <asp:Button CssClass="col s12 waves=effect waves-light btn-large" ID="nextButton" Text="Next" runat="server" onclick="nextButton_Click"/>
            <br />
            <br />
            <br />

        </div>
    <asp:BulletedList ID="selectedAnswerBulletedList" runat="server">
    </asp:BulletedList>
</asp:Content>