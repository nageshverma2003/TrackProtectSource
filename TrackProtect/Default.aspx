<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TrackProtect.Default" %>

<!DOCTYPE html>
<html class="no-js" lang="en">
<head>
    <meta charset="utf-8" />
    <!-- Set the viewport width to device width for mobile -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>TrackProtect</title>
    <!-- Included CSS Files -->
    <link type="text/css" rel="stylesheet" href="css/general_foundicons.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="css/normalize.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="css/app.css" media="screen, projector, print" />
    <script type="text/javascript" src="js/vendor/custom.modernizr.js"></script>
    <!-- Fonts -->
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,600,800,800italic,700italic,600italic,400italic'
        rel='stylesheet' type='text/css' />
    <style type="text/css">
        #ProductList1 tr
        {
            background: none transparent;
        }
        #ProductList1
        {
            border: 0px;
        }
    </style>
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
<body id="home">
    <form id="form1" runat="server">
    <div id="headerwrap">
        <!-- Begin Logo & Intro Section -->
        <div id="header" class="row">
            <div class="large-6 small-12 columns">
                <a href="#">
                    <img class="logo" src="../Images/logo.png" alt="TrackProctect Logo" /></a>
            </div>
            <div class="large-5 large-offset-1 columns hide-for-small" id="login-form">
                <asp:LoginView ID="HeadLoginView" runat="server" EnableViewState="true">
                    <AnonymousTemplate>
                        <asp:Login ID="Login1" runat="server" DestinationPageUrl="FirstLogon.aspx" BorderPadding="0"
                            BorderStyle="None" BorderWidth="0" BackColor="Transparent" CreateUserText="Register"
                            CreateUserUrl="~/Account/Register.aspx" FailureText="<%$ Resources: Resource, LoginFailed %>"
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
                        <asp:Login ID="Login1" runat="server" DestinationPageUrl="FirstLogon.aspx" BorderPadding="0"
                            BorderStyle="None" BorderWidth="0" BackColor="Transparent" CreateUserText="Register"
                            CreateUserUrl="~/Account/Register.aspx" FailureText="<%$ Resources: Resource, LoginFailed %>"
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
                    </LoggedInTemplate>
                </asp:LoginView>
                <div class="alert">
                    <a style="margin-left: 9px;" href="ResetPassword.aspx">
                        <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources: Resource, ForgotPassword %>" /></a>
                    <div class="lang-row right" style="margin-right: 68px;">
                        <asp:Button ID="LanguageUS" Text="EN" runat="server" OnCommand="SelectLanguage" CommandArgument="en-US"
                            BorderStyle="None" />
                        <asp:Button ID="LanguageNL" Text="NL" runat="server" OnCommand="SelectLanguage" CommandArgument="nl-NL"
                            BorderStyle="None" />
                    </div>
                </div>
            </div>
        </div>
        <div id="intro" class="row">
            <asp:Literal ID="IntroLiteral" runat="server"></asp:Literal>
        </div>
    </div>
    <!-- End Logo & Intro Section -->
    <!-- Begin content -->
    <div id="contentwrap">
        <div id="menu" class="row hide-for-small collapse">
            <div class="large-12 large-centered columns">
                <nav class="big-screen top-bar top-bar-section">
          <div data-magellan-expedition='fixed'>
            <ul>
                <li data-magellan-arrival='infographic' class="active">
                <a class="scroll" href="#infographic">
                <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: Resource, DefaultMenuHowItWorks %>" />
                </a>
                </li>
                <li data-magellan-arrival='usps'>
                <a class="scroll" href="#usps">
                <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources: Resource, DefaultMenuBenefits %>" />
                </a>
                </li>
                <li data-magellan-arrival='products'>
                <a class="scroll" href="#products">
                <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources: Resource, DefaultMenuProducts %>" />
                </a>
                </li>
                <li data-magellan-arrival='about'>
                <a class="scroll" href="#about">
                <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: Resource, DefaultMenuAboutUs %>" />
                </a>
                </li>
                <li data-magellan-arrival='news'>
                <a class="scroll" href="#news">
                <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources: Resource, DefaultNews %>" />
                </a>
                </li>
                <li data-magellan-arrival='faq'>
                <a class="scroll" href="#faq">FAQ</a>
               </li>
            </ul>
          </div>
        </nav>
            </div>
        </div>
        <div class="show-for-small fixed">
            <nav class="top-bar">
            <ul class="title-area">
                <li class="name">
                    <h1>
                        <a href="Account/Login.aspx">login</a>
                        <div class="lang-row right">
                           <asp:Button ID="btnENSmall" Text="EN" runat="server" OnCommand="SelectLanguage" CommandArgument="en-US"
                                    BorderStyle="None" />
                                <asp:Button ID="btnNLSmall" Text="NL" runat="server" OnCommand="SelectLanguage" CommandArgument="nl-NL"
                                    BorderStyle="None" />
                        </div>
                    </h1>
                </li>
                <li class="toggle-topbar menu-icon"><a href="#"><span></span></a></li>
            </ul>
            <section class="top-bar-section">
                <ul>
                    <li><a class="scroll" href="#infographic">
                            <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources: Resource, DefaultMenuHowItWorks %>" /></a></li>
                        <li><a class="scroll" href="#usps">
                            <asp:Localize ID="Localize14" runat="server" Text="<%$ Resources: Resource, DefaultMenuBenefits %>" /></a></li>
                        <li><a class="scroll" href="#products">
                            <asp:Localize ID="Localize15" runat="server" Text="<%$ Resources: Resource, DefaultMenuProducts %>" /></a></li>
                        <li><a class="scroll" href="#about">
                            <asp:Localize ID="Localize17" runat="server" Text="<%$ Resources: Resource, DefaultMenuAboutUs %>" /></a></li>
                        <li><a class="scroll" href="#news">
                            <asp:Localize ID="Localize16" runat="server" Text="<%$ Resources: Resource, DefaultNews %>" /></a></li>
                        <li><a class="scroll" href="#faq">FAQ</a></li>
                </ul>
            </section>
        </nav>
        </div>
        <div id="infographic" class="row" data-magellan-destination="infographic">
            <asp:Literal ID="InfoGraphicLiteral" runat="server"></asp:Literal>
        </div>
        <div id="usps" class="row" data-magellan-destination="usps">
            <div class="large-12 columns">
                <asp:Literal ID="UpsLiteral" runat="server"></asp:Literal>
            </div>
        </div>
        <div id="products" class="row" data-magellan-destination="products">
            <div class="large-12 columns">
                <div class="section-title center">
                    <h1>
                        <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, DefaultProductHeading %>" />
                    </h1>
                </div>
                <ul class="large-block-grid-4 small-block-grid-1">
                    <asp:Literal ID="ltrProducts" runat="server"></asp:Literal>
                </ul>
            </div>
        </div>
        <div id="about" class="row" data-magellan-destination="about">
            <div class="large-12 columns">
                <div class="section-title center">
                    <h1>
                        <asp:Localize runat="server" Text="<%$ Resources : Resource, DefaultMenuAboutUs %>"></asp:Localize>
                    </h1>
                </div>
                <div class="row">
                    <div class="large-8 columns">
                        <asp:Literal ID="AboutLiteral" runat="server"></asp:Literal>
                        <h3>
                            Contact Us</h3>
                        <form id="contact" method="post" action="">
                        <label>
                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, DefaultContactName %>" />
                        </label>
                        <asp:TextBox ID="ContactText" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ContactText"
                            Display="Dynamic" CssClass="failureNotification" ErrorMessage="Name is required."
                            ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                        <label>
                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, DefaultContactEmail %>" />
                        </label>
                        <asp:TextBox ID="EmailText" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="EmailText"
                            Display="Dynamic" CssClass="failureNotification" ErrorMessage="Email is required."
                            ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                        <label>
                            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, DefaultContactMesage %>" />
                        </label>
                        <asp:TextBox ID="MsgText" runat="server" TextMode="MultiLine"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="MsgText"
                            Display="Dynamic" CssClass="failureNotification" ErrorMessage="Message is required."
                            ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                        <asp:Button ID="Send" class="button small border" ValidationGroup="RegisterUserValidationGroup"
                            OnClick="Send_Click" Text="<%$ Resources: Resource, DefaultContactSend %>" runat="server" />
                        </form>
                    </div>
                </div>
            </div>
        </div>
        <div id="news" class="row" data-magellan-destination="news">
            <div class="large-12 columns">
                <div class="section-title center">
                    <h1>
                        <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, DefaultNews %>" />
                    </h1>
                </div>
                <div class="row">
                    <div class="large-8 columns">
                        <asp:Literal ID="newsLiteral" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
        <!-- FAQ -->
        <div id="faq" class="row" data-magellan-destination="faq">
            <div class="large-12 columns">
                <asp:Literal ID="FAQLiteral" runat="server"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="small-12 columns">
                <a class="right border small button" href="#top">
                    <asp:Localize ID="Localize12" runat="server" Text="<%$ Resources: Resource, BackToTop %>" /></a>
            </div>
        </div>
    </div>
    <!-- End Contact -->
    <!-- Start of footer -->
    <div id="footer">
        <asp:Literal ID="FooterLiteral" runat="server"></asp:Literal>
    </div>
    <!-- end footer -->
    <!-- scrollTop animation won't work without this -->
    <%--<script type="text/javascript" src="http://code.jquery.com/jquery-1.9.1.min.js"></script>--%>
    <script type="text/javascript">
        document.write('<script src=' +
    ('__proto__' in {} ? 'js/vendor/zepto' : 'js/vendor/jquery') +
    '.js><\/script>')
    </script>
    <script type="text/javascript" src="js/foundation/foundation.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.cookie.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.magellan.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.topbar.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.tooltips.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.alerts.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.placeholder.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.dropdown.js"></script>
    <script type="text/javascript" src="js/foundation/foundation.forms.js"></script>
    <script type="text/javascript" src="js/zepto.scroll.js"></script>
    <%--<script type="text/javascript" src="Scripts/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.9.0.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/tpscripts.js"></script>--%>
    <!-- scripts that we are not likely to use
    <script src="js/foundation/foundation.joyride.js"></script>
    <script src="js/foundation/foundation.clearing.js"></script>
    <script src="js/foundation/foundation.section.js"></script>
    <script src="js/foundation/foundation.reveal.js"></script>
    <script src="js/foundation/foundation.orbit.js"></script>
    -->
    <script type="text/javascript">
        $(document).foundation();

        $(document).ready(function ($) {
            // We will dynamically set the min-width of our navbar
            var resize_navbar = function () {
                var magellan = $("[data-magellan-expedition]");
                var arrow = $(".arrow-down");
                var proper_width = $("#infographic").width();
                var offset = 131;
                // When the width is less than 1000, we need to make the top nav shorter
                // We also need to move the green arrow to make it some how between products
                // and About menu item.
                if (proper_width < 1000) {
                    proper_width = proper_width - 15;
                    var center = proper_width / 2;
                    offset = center - $(arrow).parent().offset().left + 21;
                }

                magellan.css("min-width", proper_width + "px");
                arrow.css("left", offset + "px");
            };

            resize_navbar();

            $(window).on('resize', function (e) {
                resize_navbar();
            });

            // Smooth Scroll
            $(".scroll").on('click', function (e) {
                //console.log($(this.hash).offset().top);
                e.preventDefault();
                offset = $(this.hash).offset().top;
                $.scrollTo(offset, 1000);
            });
        });
    </script>
    </form>
</body>
</html>
