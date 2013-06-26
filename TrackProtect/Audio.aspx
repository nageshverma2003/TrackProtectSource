<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Audio.aspx.cs" Inherits="TrackProtect.Audio" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Assembly="QuickTimePlayer" Namespace="QuickTimePlayer" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <%--<script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" src="JWPlayer/jwplayer.js"></script>
    <script type="text/javascript" src="JWPlayer/swfobject.js"></script>--%>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ID="lblSound"></asp:Label>
        <br />
        <%--<div id="AudioPlayer" style="cursor: progress;">
            Loading...</div>
        <script type="text/javascript">
            jwplayer("AudioPlayer").setup({
                flashplayer: "JWPlayer/player.swf",
                file: "<%= audioFilePath %>",
                height: 150,
                width: 380
            });
        </script>--%>
        <cc1:QTPlayer runat="server" ID="QTPlayer1" Width="410" Height="125">
        </cc1:QTPlayer>
    </div>
    </form>
</body>
</html>
