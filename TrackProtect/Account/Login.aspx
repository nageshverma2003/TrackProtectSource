<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Logon.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TrackProtect.Account.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
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
        .login
        {
            border: 1px solid #CC4809;
        }
        .SignUpBtn
        {
            padding: 0.6em 0.5em 0.4em;
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
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    LOG IN</h1>
            </div>
            <div class="row">
                <div class="large-8 columns">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            LOG IN</h1>
                    </div>
                    <div style="min-height: 80%">
                        <p>
                            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, EnterUsernameAndPassword %>" />
                            or
                            <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false" Font-Bold="true"
                                NavigateUrl="~/Account/Register.aspx">
                                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, Register %>" />
                            </asp:HyperLink>
                        </p>
                        <p>
                            <asp:Literal runat="server" ID="LogonMessage" /></p>
                        <div>
                            <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false"
                                DestinationPageUrl="~/Member/MemberHome.aspx" FailureText="<%$ Resources: Resource, LoginFailed %>"
                                OnAuthenticate="LoginUser_Authenticate" OnLoggedIn="LoginUser_LoggedIn">
                                <LayoutTemplate>
                                    <asp:ValidationSummary Style="width: 73%;" ID="ValidationSummary" ShowMessageBox="false"
                                        ShowSummary="true" CssClass="failureNotification" EnableClientScript="true" ValidationGroup="LoginUserValidationGroup"
                                        runat="server" DisplayMode="List" BorderColor="#75B891" BorderWidth="1" EnableTheming="true" />
                                    <br />
                                    <div class="accountInfo">
                                        <asp:Panel ID="Panel1" runat="server" DefaultButton="LoginButton">
                                            <table class="login" style="width: 72%; border-collapse: separate;">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="<%$ Resources: Resource, Username %>" />
                                                    </td>
                                                </tr>
                                                <tr style="background: none;">
                                                    <td style="width: 95%;">
                                                        <asp:TextBox ID="UserName" runat="server" CssClass="textEntry" />
                                                    </td>
                                                    <td>
                                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                                            ErrorMessage="<%$ Resources: Resource, UsernameRequired %>" ToolTip="<%$ Resources: Resource, UsernameRequired %>"
                                                            Display="Dynamic" ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="<%$ Resources: Resource, Password %>" />
                                                    </td>
                                                </tr>
                                                <tr style="background: none;">
                                                    <td style="width: 95%;">
                                                        <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password" />
                                                    </td>
                                                    <td>
                                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                                            ErrorMessage="<%$ Resources: Resource, PasswordRequired %>" ToolTip="<%$ Resources: Resource, PasswordRequired %>"
                                                            Display="Dynamic" ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:HyperLink ID="HyperLink1" runat="server" CssClass="" Text="<%$ Resources: Resource, ForgotPassword %>"
                                                            BackColor="Transparent" NavigateUrl="~/ResetPassword.aspx" />
                                                    </td>
                                                </tr>
                                                <tr style="background: none;">
                                                    <td>
                                                        <asp:Button ID="LoginButton" runat="server" CssClass="button small border right"
                                                            CommandName="Login" Text="Log In" ValidationGroup="LoginUserValidationGroup" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <div class="row" style="width: 73.3%;">
                                                <a class="button small border right" href="../Default.aspx">
                                                    <asp:Localize ID="Localize2" runat="server" Text='<%$ Resources : Resource, BackToHome %>'></asp:Localize></a>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </LayoutTemplate>
                            </asp:Login>
                        </div>
                        <div style="clear: both;">
                        </div>
                    </div>
                </div>
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
