﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorDialog.aspx.cs" Inherits="TrackProtect.Member.ErrorDialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TrackProtect</title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="errordialog">
        <asp:Label runat="server" ID="ErrorTitle" CssClass="errortitle"/>
        <br/>
        <asp:Label runat="server" ID="ErrorMessage" CssClass="errormessage"/>
    </div>
    </form>
</body>
</html>
