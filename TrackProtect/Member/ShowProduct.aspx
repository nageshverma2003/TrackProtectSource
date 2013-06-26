<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ShowProduct.aspx.cs" Inherits="TrackProtect.ShowProduct" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../js/vendor/jquery.js" type="text/javascript"></script>
    <link type="text/css" rel="stylesheet" href="../css/general_foundicons.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="../css/normalize.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="../css/app.css" media="screen, projector, print" />
    <script type="text/javascript" src="../js/vendor/custom.modernizr.js"></script>
    <!-- Fonts -->
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,600,800,800italic,700italic,600italic,400italic'
        rel='stylesheet' type='text/css' />
    <meta name="robots" content="noindex" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_showproduct">
        <table width="100%">
            <tr valign="top">
                <td class="leftColumn">
                    <div style="padding: 24px 0px 0px 0px; margin-left: 2px;">
                        <table>
                            <tr>
                                <td>
                                    <asp:Image ID="DescriptionImage" runat="server"></asp:Image>
                                </td>
                                <td>
                                    <div style="margin-left: 16px; margin-right: 16px;">
                                        <asp:Literal ID="TitleLiteral" runat="server" /></div>
                                </td>
                            </tr>
                        </table>
                        <div class="textLiteral" style="margin-left: 24px; margin-top: 32px;">
                            <asp:Literal ID="DescriptionLiteral" runat="server"></asp:Literal>
                        </div>
                    </div>
                    <div style="width: 100%; text-align: center; margin-top: 40px;">
                        <asp:ImageButton ID="BuyProductButton" runat="server" ImageUrl="<%$ Resources: Resource, imgGoForIt %>"
                            CommandName="-1" OnCommand="BuyProductButton_Command" />
                    </div>
                </td>
                <td class="centerColumn">
                    <div style="padding: 20px 0px 0px 0px">
                        <asp:Image ID="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />
                    </div>
                </td>
                <td class="rightColumn">
                    <div class="divRightColumn">
                        <div class="divRightContent">
                            <div class="statusPanel">
                                <div class="statusPanelTitle">
                                    <asp:Localize runat="server" ID="LoggedOnTitle" /></div>
                                <div class="statusPanelUserName">
                                    <asp:Literal runat="server" ID="LoggedOnUserName" /></div>
                                <div class="statusPanelCredits">
                                    <asp:Literal runat="server" ID="CreditsLiteral" /></div>
                                <div class="statusPanelProtected">
                                    <asp:Literal runat="server" ID="ProtectedLiteral" /></div>
                                <div class="statusPanelCompleted">
                                    <asp:Literal runat="server" ID="CompletedLiteral" /></div>
                                <div class="statusPanelLink">
                                    <asp:Literal ID="ClickToLinkLiteral" runat="server" Text="<%$ Resources: Resource, ClickToEdit %>" /></div>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="divRightContent">
                            <asp:Literal runat="server" ID="ShowProductInc" />
                        </div>
                        <br />
                        <br />
                        <br />
                        <br />
                        <asp:Literal runat="server" ID="RhosMovementInc" />
                    </div>
                </td>
            </tr>
        </table>
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
