<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="facebook.aspx.cs" Inherits="TrackProtect.Social.facebook" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <asp:Literal ID="liMessage" runat="server"></asp:Literal><br />
    <asp:DropDownList ID="ddlAccounts" runat="server"></asp:DropDownList><br />
    <asp:TextBox TextMode="MultiLine" ID="tbMessage" runat="server" Height="100px" /><br />
    <asp:Button ID="btPost" runat="server" Text="Button" onclick="btPost_Click" />
</asp:Content>
