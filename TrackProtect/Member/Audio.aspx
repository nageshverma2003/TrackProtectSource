<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Audio.aspx.cs" Inherits="TrackProtect.Member.Audio" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="QuickTimePlayer" Namespace="QuickTimePlayer" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cc1:QTPlayer runat="server" ID="QTPlayer1" Width="410" Height="125">
        </cc1:QTPlayer>
    </div>
    </form>
</body>
</html>
