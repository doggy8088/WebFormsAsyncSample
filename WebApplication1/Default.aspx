<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" Async="true" %>
<form id="form1" runat="server">
    <asp:TextBox ID="TextBox1" runat="server" Rows="20" TextMode="MultiLine" Width="400px"></asp:TextBox>
    <br />
    <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" />
</form>

