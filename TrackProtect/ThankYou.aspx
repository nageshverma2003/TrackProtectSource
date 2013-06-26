<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThankYou.aspx.cs" Inherits="TrackProtect.ThankYou" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <!-- Set the viewport width to device width for mobile -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>TrackProtect</title>
    <!-- Included CSS Files -->
    <script src="../js/vendor/jquery.js" type="text/javascript"></script>
    <link type="text/css" rel="stylesheet" href="../css/general_foundicons.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="../css/normalize.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="../css/app.css" media="screen, projector, print" />
    <script type="text/javascript" src="../js/vendor/custom.modernizr.js"></script>
    <!-- Fonts -->
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,600,800,800italic,700italic,600italic,400italic'
        rel='stylesheet' type='text/css' />
    <meta name="robots" content="noindex" />
    <script type="text/javascript">
        function HighLightLangBtn(id) {
            var ele = document.getElementById(id);
            ele.className = "lang active";
        }
        function UnHighLightLangBtn(id) {
            var ele = document.getElementById(id);
            ele.className = "lang";
        }   
    </script>
</head>
<body id="signup">
    <form id="Form1" class="custom" method="post" enctype="multipart/form-data" runat="server">
    <div id="headerwrap">
        <div id="header" class="row">
            <div class="large-6 columns">
                <a href="../Default.aspx">
                    <img class="logo" src="../Images/logo.png" alt="TrackProctect Logo" /></a>
            </div>
            <div class="large-5 large-offset-1 columns hide-for-small" id="login-form">
                <div class="alert" style="float: right; margin-top: 8px;">
                    <div class="lang-row right">
                        <asp:Button ID="LanguageUS" Text="EN" runat="server" OnCommand="SelectLanguage" CommandArgument="en-US"
                            BorderStyle="None" />
                        <asp:Button ID="LanguageNL" Text="NL" runat="server" OnCommand="SelectLanguage" CommandArgument="nl-NL"
                            BorderStyle="None" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Begin content -->
    <div id="contentwrap" style="height: 550px;">
        <div class="show-for-small fixed">
            <nav class="top-bar">
                <ul class="title-area">
                    <li class="name">
                        <h1>
                        <asp:HyperLink ID="HyperLink6" NavigateUrl="~/Member/MemberHome.aspx" runat="server">Home</asp:HyperLink>
                        <div class="lang-row right">
                        <asp:Button ID="Button1" Text="EN" runat="server" OnCommand="SelectLanguage" CommandArgument="en-US"
                                        BorderStyle="None"/>
                                    <asp:Button ID="Button2" Text="NL" runat="server" OnCommand="SelectLanguage" CommandArgument="nl-NL"
                                        BorderStyle="None"/>
                                        </div>

                        </h1>
                    </li>
                    <li class="toggle-topbar menu-icon"><a href="#"><span></span></a></li>
                </ul>                
            </nav>
        </div>
        <div id="contropanel" class="row">
            <div class="large-12 columns">
                <div class="section-title to-left">
                    <h1>
                        <asp:Localize runat="server" Text="<%$ Resources : Resource, ThankyouTitle %>"></asp:Localize></h1>
                </div>
                <div>
                    <asp:Literal runat="server" ID="ThankyouMsg"></asp:Literal>
                </div>
                <div style="clear: both">
                </div>
                <div style="margin-top: 10px">
                    <a href="Default.aspx" class="small button border">Home</a>
                </div>
            </div>
        </div>
    </div>
    <!--End of content -->
    <!-- Start of footer -->
    <div id="footer">
        <asp:Literal ID="FooterLiteral" runat="server"></asp:Literal>
    </div>
    <!-- end of footer -->
    <script type="text/ecmascript">
        document.write('<script src=' +
    ('__proto__' in {} ? 'js/vendor/zepto' : 'js/vendor/jquery') +
    '.js><\/script>')
    </script>
    <script type="text/ecmascript" src="../js/foundation/foundation.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.cookie.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.magellan.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.joyride.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.topbar.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.clearing.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.tooltips.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.alerts.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.placeholder.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.section.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.reveal.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.orbit.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.dropdown.js"></script>
    <script type="text/ecmascript" src="../js/foundation/foundation.forms.js"></script>
    <script type="text/ecmascript">
        $(document).foundation();
    </script>
    </form>
</body>
</html>
