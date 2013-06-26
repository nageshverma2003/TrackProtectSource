<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="scconnect.aspx.cs" Inherits="TrackProtect.Social.scconnect" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="http://connect.soundcloud.com/sdk.js"></script>
    <script type="text/javascript" src="/Scripts/soundcloud.js"></script>
    <%--<script type="text/javascript">
        window.onload = function () {
            soundCloudAuthorize();
        }();
    </script>--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
