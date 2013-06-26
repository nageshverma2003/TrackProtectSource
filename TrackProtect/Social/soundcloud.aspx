<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="soundcloud.aspx.cs" Inherits="TrackProtect.Social.soundcloud" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
    <link type="text/css" rel="stylesheet" href="../css/app.css" media="screen, projector, print" />
    <script type="text/javascript">
        function replaceUrl() {
            var location = window.location.href;
            alert(location);
            if (location.indexOf('#') >= 0) {
                var redirectUrl = location.replace('#', '&');
                window.location.href = redirectUrl;
            }
            else {
                alert('onClose');
                closePage();
            }
        }
        function closePage() {
            window.opener.location.href = 'http://test.trackprotect.com/Member/Profile.aspx';
            window.close();
        }        
    </script>
</head>
<body onload="replaceUrl();">
    <form id="form1" runat="server">
    <div>
        Please wait...
    </div>
    </form>
</body>
</html>
