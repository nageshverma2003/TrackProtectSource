<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="soundcloud1.aspx.cs" Inherits="TrackProtect.Social.soundcloud1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $.post(
                "http://localhost:4508/Social/soundcloud.aspx",
                { url: window.location.hash }, function () {
                    $(location).attr('href', 'https://test.trackprotect.com');
                });

        });
        window.onload = function () {
            window.opener.location.reload(true);
            window.close();
        } ();            
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
