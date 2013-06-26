<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true"
    CodeBehind="ResetPassword.aspx.cs" Inherits="TrackProtect.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
    <style type="text/css">
        .fieldName
        {
            border: 1px solid #CC4809;
            background: white !important;
        }
        #controlpanel td, tr, th
        {
            border: 0px;
            background: white !important;
        }
        .failureNotification
        {
            padding-top: 3px;
            padding-left: 5px;
            font-size: .9em;
            background: #F0F0F0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row" style="min-height: 600px;">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, ResetPassword %>"></asp:Localize></h1>
            </div>
            <div class="row">
                <div class="large-8 columns">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, ResetPassword %>"></asp:Localize></h1>
                    </div>
                    <div>
                        <asp:ValidationSummary Style="width: 70%;" ID="ValidationSummary" ShowMessageBox="false"
                            ShowSummary="true" CssClass="failureNotification" EnableClientScript="true" ValidationGroup="TrackProtectValidation"
                            runat="server" DisplayMode="List" BorderColor="#75B891" BorderWidth="1" EnableTheming="true" />
                        <br />
                        <table width="70%" class="fieldName">
                            <tr>
                                <td style="width: 12%;">
                                    <asp:Label ID="EmailLabel" runat="server" Text="<%$ Resources: Resource, Email %>" />
                                </td>
                                <td>
                                    <asp:TextBox ID="Email" runat="server" CssClass="textEntry2" />
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                        ErrorMessage="<%$ Resources: Resource, EmailRequired %>" Text="*" ValidationGroup="TrackProtectValidation"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="EmailValidate" ControlToValidate="Email" ValidationExpression="^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$"
                                        ErrorMessage="<%$ Resources : Resource, EmailNotValid %>" Text="*" ValidationGroup="TrackProtectValidation"
                                        runat="server">
                                    </asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20%;">
                                </td>
                                <td style="text-align: left;">
                                    <asp:Button ID="ResetPasswordButton" runat="server" Text="Reset" OnCommand="ResetPasswordCommand"
                                        CssClass="button small border" ValidationGroup="TrackProtectValidation" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20%;">
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="ResultMessage" />
                                </td>
                            </tr>
                        </table>
                        <a class="button small border" href="Default.aspx">
                            <asp:Localize ID="Localize3" runat="server" Text='<%$ Resources : Resource, BackToHome %>'></asp:Localize></a>
                    </div>
                </div>
                <!-- End Miain Content -->
            </div>
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
