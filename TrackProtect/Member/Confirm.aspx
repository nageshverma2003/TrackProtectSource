<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Logon.Master"
    AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="TrackProtect.Member.Confirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
        function HighLightMenu(id) {
            if (id == 'Menu1') {
                var ele = document.getElementById(id);
                ele.className = "active home";
            }
            else {
                var ele = document.getElementById(id);
                ele.className = "active";
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize runat="server" Text="<%$ Resources : Resource, Confirmation %>"></asp:Localize></h1>
            </div>
            <div class="row">
                <!-- Main Content -->
                <div class="large-8 columns" style="height: 275px;">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources : Resource, Confirmation %>"></asp:Localize></h1>
                    </div>
                    <!-- What are coupon codes? -->
                    <div>
                        <!-- Just to add padding on the right -->
                        <div class="member_confirm">
                            <asp:Label runat="server" ID="ResultLabel" Text="" />
                        </div>
                        <br />
                        <a href="../Default.aspx">
                            <input id="Button1" type="button" class="button small border edit" runat="server"
                                value="Home" />
                        </a>
                    </div>
                </div>
                <!-- End Miain Content -->
            </div>
            <%--<div style="clear: both">
                </div>
                <div style="margin-top: 10px">
                    <a href="Default.aspx" class="small button border">Home</a>
                </div>--%>
        </div>
    </div>
    <script type="text/javascript">
        document.write('<script src=' +
    ('__proto__' in {} ? 'js/vendor/zepto' : 'js/vendor/jquery') +
    '.js><\/script>')
    </script>
    <script type="text/javascript" src="../js/foundation/foundation.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.cookie.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.magellan.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.joyride.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.topbar.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.clearing.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.tooltips.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.alerts.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.placeholder.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.section.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.reveal.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.orbit.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.dropdown.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.forms.js"></script>
    <script type="text/javascript">
        $(document).foundation();
    </script>
</asp:Content>
