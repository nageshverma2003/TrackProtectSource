<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TrackProtect.Account.Register" %>

<!DOCTYPE html>
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
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.min.js"></script>
    <!-- Fonts -->
    <script type="text/javascript">
        function AcceptTermsCheckBoxValidation(oSource, args) {
            var myCheckBox = document.getElementById('<%= (RegisterUser.WizardSteps[0].Controls[0].FindControl("Agreement") as CheckBox).ClientID %>');
            args.IsValid = myCheckBox.checked;
        }
        function OwnerCheckBoxValidation(oSource, args) {
            var myCheckBox = document.getElementById('<%= (RegisterUser.WizardSteps[0].Controls[0].FindControl("Owner")).ClientID %>');
            args.IsValid = myCheckBox.checked;
        }


        function HighLightLangBtn(id) {
            var ele = document.getElementById(id);
            ele.className = "lang active";
        }
        function UnHighLightLangBtn(id) {
            var ele = document.getElementById(id);
            ele.className = "lang";
        }


        //        $(document).ready(function () {
        //            $('#linkFBSignUp').click(function () {
        //                var terms = $('#fbAgreement').is(':checked');
        //                var owner = $('#fbOwner').is(':checked');

        //                if (terms == true && owner == true) {
        //                    document.location.href = 'http://test.trackprotect.com/Social/fbsignup.aspx';
        //                }
        //                else {
        //                    if ($('#divTerms').css("display") == 'none') {
        //                        $('#divTerms').fadeIn('fast');
        //                    }
        //                    else {
        //                        $('#divTerms').fadeOut('slow');
        //                        $('#fbAgreement').removeAttr('checked');
        //                        $('#fbOwner').removeAttr('checked');
        //                    }
        //                }
        //            });

        //            $('#linkFBSignUpMob').click(function () {
        //                var terms = $('#fbAgreement').is(':checked');
        //                var owner = $('#fbOwner').is(':checked');

        //                if (terms == true && owner == true) {
        //                    document.location.href = 'http://test.trackprotect.com/Social/fbsignup.aspx';
        //                }
        //                else {
        //                    if ($('#divTerms').css("display") == 'none') {
        //                        $('#divTerms').fadeIn('fast');
        //                    }
        //                    else {
        //                        $('#divTerms').fadeOut('slow');
        //                        $('#fbAgreement').removeAttr('checked');
        //                        $('#fbOwner').removeAttr('checked');
        //                    }
        //                }
        //            });
        //        });

        //                function showDiv(id) {
        //                    var terms = document.getElementById("fbAgreement").checked;
        //                    var owner = document.getElementById("fbOwner").checked;
        //                    if (terms == true && owner == true) {
        //                        window.location.href = "Social/fbsignup.aspx";
        //                    }
        //                    else {
        //                        var div = document.getElementById(id);
        //                        div.style.display = 'block';
        //                    }
        //                }

    </script>
    <style type="text/css">
        #RegisterUser td, table, tr, tbody, th
        {
            border: 0px solid transparent;
            background: none repeat scroll 0 0 transparent !important;
        }
        #RegisterUser td
        {
            color: #E4510A;
            cursor: pointer;
            display: block;
            font-size: 0.98em;
            font-weight: 500;
            margin-bottom: 0.1875em;
            background: white;
        }
        .padleft
        {
            margin-left: 2%;
        }
        .row .row
        {
            margin: 0 -0.9375em !important;
            max-width: none;
            width: auto;
        }
        .inputleft
        {
            margin-left: 3% !important;
            width: 95% !important;
        }
        .inputRight
        {
            margin-left: 3% !important;
            width: 95% !important;
        }
        .failureNotification
        {
            padding-top: 3px;
            padding-left: 5px;
            font-size: .9em;
            background: #F0F0F0;
        }
        
        #divRadio1 label
        {
            font-size: 13px;
            vertical-align: middle;
            float: right;
            text-align: left;
            width: 510px;
        }
        
        #divRadio2 label
        {
            font-size: 13px;
            vertical-align: middle;
            float: right;
            text-align: left;
            width: 510px;
        }
        
        #divFBRadio1 label
        {
            font-size: 13px;
            vertical-align: middle;
            float: right;
            text-align: left;
            width: 300px;
        }
        
        #divFBRadio2 label
        {
            font-size: 13px;
            vertical-align: middle;
            float: right;
            text-align: left;
            width: 300px;
        }
    </style>
</head>
<body id="signup">
    <form class="custom" method="post" enctype="multipart/form-data" runat="server">
    <div id="headerwrap">
        <div id="header" class="row">
            <div class="large-6 columns">
                <a href="../Default.aspx">
                    <img class="logo" src="../Images/logo.png" alt="TrackProctect Logo" /></a>
            </div>
            <div class="large-5 large-offset-1 columns hide-for-small" id="login-form">
                <asp:LoginView ID="HeadLoginView" runat="server" EnableViewState="false">
                    <AnonymousTemplate>
                        <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/Member/MemberHome.aspx"
                            BorderPadding="0" BorderStyle="None" BorderWidth="0" BackColor="Transparent"
                            CreateUserText="Register" CreateUserUrl="~/Account/Register.aspx" FailureText="<%$ Resources: Resource, LoginFailed %>"
                            OnAuthenticate="Login1_Authenticate" FailureAction="RedirectToLoginPage">
                            <LayoutTemplate>
                                <div class="row collapse">
                                    <div class="large-5 columns">
                                        <asp:TextBox ID="UserName" runat="server" Font-Size="1.0em" TabIndex="1" CssClass="loginText"
                                            ToolTip="Gebruikersnaam">
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserName"
                                            ErrorMessage="<%$ Resources: Resource, UsernameRequired %>" ToolTip="<%$ Resources: Resource, UsernameRequired %>"
                                            ValidationGroup="ctl00$Login1">*</asp:RequiredFieldValidator>
                                    </div>
                                    <div class="large-5 columns">
                                        <asp:TextBox ID="Password" runat="server" Font-Size="1.0em" TextMode="Password" CssClass="loginText"
                                            TabIndex="2" ToolTip="Password">
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Password"
                                            ErrorMessage="<%$ Resources: Resource, PasswordRequired %>" ToolTip="<%$ Resources: Resource, PasswordRequired %>"
                                            ValidationGroup="ctl00$Login1">*</asp:RequiredFieldValidator>
                                    </div>
                                    <div class="large-2 columns">
                                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" ValidationGroup="ctl00$Login1"
                                            AlternateText="LogIn" TabIndex="3" Text="LOGIN" CssClass="button" />
                                    </div>
                                </div>
                            </LayoutTemplate>
                            <LoginButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
                                Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" />
                            <TextBoxStyle Font-Size="0.8em" />
                            <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="0.9em" ForeColor="White" />
                        </asp:Login>
                    </AnonymousTemplate>
                    <LoggedInTemplate>
                        <div id="header-user-info" class="row">
                            <div class="large-6 columns">
                                <h2 class="name">
                                    <asp:Literal ID="HeadLoginName" runat="server" /></h2>
                                <p class="actions">
                                    <a style="color: rgba(230, 230, 230, 0.8);" href="#">
                                        <asp:Localize runat="server" Text="<%$ Resources : Resource, Profile %>"></asp:Localize></a>
                                    | <a>
                                        <asp:LoginStatus ID="HeadLoginStatus" Style="color: rgba(230, 230, 230, 0.8);" runat="server"
                                            LogoutAction="Redirect" LogoutText="Log Out" LogoutPageUrl="~/Default.aspx" />
                                    </a>
                                </p>
                            </div>
                            <div class="large-6 columns">
                                <img alt="" class="avatar" src="../Images/icon-avatar.png">
                            </div>
                        </div>
                    </LoggedInTemplate>
                </asp:LoginView>
                <div class="alert">
                    <asp:HyperLink runat="server" NavigateUrl="~/Account/ChangePassword.aspx">
                        <asp:Localize runat="server" Text="<%$ Resources: Resource, ForgotPassword %>" /></asp:HyperLink>
                    <div class="lang-row right" style="margin-right: 65px;">
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
    <div id="contentwrap">
        <div class="show-for-small fixed">
            <nav class="top-bar">
                <ul class="title-area">
                    <li class="name">
                        <h1>
                        <asp:HyperLink ID="HyperLink6" NavigateUrl="~/Default.aspx" runat="server">Home</asp:HyperLink>
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
                        <asp:Localize runat="server" Text="<%$ Resources : Resource, SignUp %>"></asp:Localize></h1>
                </div>
                <div class="row">
                    <div class="large-4 columns">
                        <div class="push-down hide-for-small">
                        </div>
                        <a id="linkFBSignUp" class="button fb-button expand hide-for-small" href="../Social/fbsignup.aspx">
                            <span class="fb-button-left social">F</span> <span class="fb-button-center">
                                <asp:Localize runat="server" Text="<%$ Resources : Resource, ConnectWithFacebook %>"></asp:Localize></span>
                        </a><a id="linkFBSignUpMob" class="button fb-button expand show-for-small" href="../Social/fbsignup.aspx">
                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources : Resource, ConnectWithFacebook %>"></asp:Localize></a>
                        <div class="fb-terms">
                            <asp:Localize runat="server" Text="<%$ Resources : Resource, FBTerms %>"></asp:Localize></div>
                    </div>
                    <div class="large-1 column">
                        <div class="push-down-text hide-for-small">
                        </div>
                        <div>
                            <h2 class="center-align orange">
                                <asp:Localize runat="server" Text="<%$ Resources : Resource, or %>"></asp:Localize></h2>
                        </div>
                    </div>
                    <div class="large-7 columns" id="signup-form">
                        <asp:HiddenField runat="server" ID="LanguageIndex" Value="-1" />
                        <asp:CreateUserWizard ID="RegisterUser" runat="server" EnableViewState="false" OnCreatedUser="RegisterUser_CreatedUser"
                            OnCreatingUser="RegisterUser_CreatingUser" BorderStyle="None" EnableTheming="True"
                            Width="100%">
                            <WizardSteps>
                                <asp:CreateUserWizardStep ID="RegisterUserWizardStep" runat="server">
                                    <ContentTemplate>
                                        <asp:ValidationSummary ID="ValidationSummary" ShowMessageBox="false" ShowSummary="true"
                                            CssClass="failureNotification" EnableClientScript="true" ValidationGroup="TrackProtectValidation"
                                            runat="server" DisplayMode="List" BorderColor="#75B891" BorderWidth="1" EnableTheming="true" />
                                        <br />
                                        <div style="display: none;">
                                            <asp:TextBox ID="UserName" runat="server" />
                                        </div>
                                        <div class="row">
                                            <div class="large-6 columns" style="padding-left: 15px; padding-right: 15px;">
                                                <asp:Label ID="FirstNameLabel" runat="server" AssociatedControlID="FirstName" Text="<%$ Resources: Resource, FirstName %>" />
                                                <asp:TextBox MaxLength="15" ID="FirstName" runat="server" />
                                                <asp:RequiredFieldValidator ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="<%$ Resources : Resource, FirstNameRequired %>"
                                                    Text="*" ID="FirstNameRequired" ValidationGroup="TrackProtectValidation" runat="server"></asp:RequiredFieldValidator>
                                            </div>
                                            <div class="large-6 columns" style="padding-right: 15px; padding-left: 15px">
                                                <asp:Label ID="LastNameLabel" runat="server" AssociatedControlID="LastName" Text="<%$ Resources: Resource, LastName %>" />
                                                <asp:TextBox MaxLength="15" ID="LastName" runat="server" />
                                                <asp:RequiredFieldValidator ControlToValidate="LastName" Display="Dynamic" ErrorMessage="<%$ Resources: Resource, LastNameRequired %>"
                                                    ID="LastNameRequired" Text="*" ValidationGroup="TrackProtectValidation" runat="server"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <%-- <asp:Label ID="FirstNameLabel" runat="server" AssociatedControlID="FirstName" Text="<%$ Resources: Resource, FirstName %>" />
                                        <asp:TextBox MaxLength="15" ID="FirstName" runat="server" />
                                        <asp:RequiredFieldValidator ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="<%$ Resources : Resource, FirstNameRequired %>"
                                            Text="*" ID="FirstNameRequired" ValidationGroup="TrackProtectValidation" runat="server"></asp:RequiredFieldValidator>
                                        <asp:Label ID="LastNameLabel" runat="server" AssociatedControlID="LastName" Text="<%$ Resources: Resource, LastName %>" />
                                        <asp:TextBox MaxLength="15" ID="LastName" runat="server" />
                                        <asp:RequiredFieldValidator ControlToValidate="LastName" Display="Dynamic" ErrorMessage="<%$ Resources: Resource, LastNameRequired %>"
                                            ID="LastNameRequired" Text="*" ValidationGroup="TrackProtectValidation" runat="server"></asp:RequiredFieldValidator>--%>
                                        <asp:Label ID="Label1" runat="server" AssociatedControlID="Email" Text="<%$ Resources: Resource, Email %>" />
                                        <asp:TextBox ID="Email" runat="server" />
                                        <asp:RequiredFieldValidator ID="EmailRequired" runat="server" Display="Dynamic" ControlToValidate="Email"
                                            ErrorMessage="<%$ Resources: Resource, EmailRequired %>" Text="*" ValidationGroup="TrackProtectValidation"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="EmailValidate" ControlToValidate="Email" Display="Dynamic"
                                            ValidationExpression="^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$"
                                            ErrorMessage="<%$ Resources : Resource, EmailNotValid %>" Text="*" ValidationGroup="TrackProtectValidation"
                                            runat="server">
                                        </asp:RegularExpressionValidator>
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="<%$ Resources:Resource, Password %>" />
                                        <asp:TextBox ID="Password" runat="server" TextMode="Password" />
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                            Display="Dynamic" ErrorMessage="<%$ Resources:Resource, PasswordRequired %>"
                                            Text="*" ValidationGroup="TrackProtectValidation"></asp:RequiredFieldValidator><asp:Label
                                                ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword"
                                                Text="<%$ Resources: Resource, ConfirmPassword %>" />
                                        <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" />
                                        <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" Display="Dynamic"
                                            ErrorMessage="<%$ Resources: Resource, ConfirmPasswordRequired %>" Text="*" ID="ConfirmPasswordRequired"
                                            runat="server" ValidationGroup="TrackProtectValidation"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                            ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="<%$ Resources:Resource, PasswordMustMatch %>"
                                            ValidationGroup="TrackProtectValidation" Text="*"></asp:CompareValidator>
                                        <%-- <div id="divRadio1">
                                            <asp:CheckBox runat="server" ID="Agreement" Text="<%$ Resources: Resource, IAgree %>" />
                                        </div>--%>
                                        <label class="terms" for="RegisterUser_CreateUserStepContainer_Owner">
                                            <asp:CheckBox runat="server" ID="Owner" /><span class="custom checkbox"></span>
                                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, IamOwner %>"></asp:Localize>
                                        </label>
                                        <asp:CustomValidator ID="ValOwner" ClientValidationFunction="OwnerCheckBoxValidation"
                                            runat="server" ErrorMessage="<%$ Resources: Resource, AcceptOwner %>" ValidationGroup="TrackProtectValidation"
                                            Text="*"></asp:CustomValidator>
                                        <label class="terms" for="RegisterUser_CreateUserStepContainer_Agreement">
                                            <asp:CheckBox runat="server" ID="Agreement" />
                                            <span class="custom checkbox"></span>
                                            <asp:Localize runat="server" Text="<%$ Resources: Resource, IAgree %>"></asp:Localize></label>
                                        <asp:CustomValidator ID="ValTerms" ClientValidationFunction="AcceptTermsCheckBoxValidation"
                                            runat="server" ErrorMessage="<%$ Resources: Resource, AcceptTerms %>" ValidationGroup="TrackProtectValidation"
                                            Text="*"></asp:CustomValidator>
                                        <%--<div style="clear: both;">
                                        </div>--%>
                                        <div class="button-row-center">
                                            <asp:Button ID="CreateUserButton" class="button small border" Text="<%$ Resources: Resource, DefaultContactSend %>"
                                                CommandName="MoveNext" runat="server" ValidationGroup="TrackProtectValidation" />
                                        </div>
                                    </ContentTemplate>
                                    <CustomNavigationTemplate>
                                    </CustomNavigationTemplate>
                                </asp:CreateUserWizardStep>
                                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                                </asp:CompleteWizardStep>
                            </WizardSteps>
                        </asp:CreateUserWizard>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="footer">
        <asp:Literal ID="FooterLiteral" runat="server"></asp:Literal>
    </div>
    </form>
    <!--End of content -->
    <!-- Start of footer -->
    <!-- end of footer -->
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
</body>
</html>
