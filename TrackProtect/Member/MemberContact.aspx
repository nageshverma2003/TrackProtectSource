<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MemberContact.aspx.cs" Inherits="TrackProtect.Member.MemberContact" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    CONTACT</h1>
            </div>
            <div class="row contact-info">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                    <a class="button extra-large expand control_p_btn" href="memberhome.aspx"><i class="arrow-left">
                    </i>
                        <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></a>
                    <div id="user-info">
                        <header>
                                <p style="color:Black;"><asp:Localize runat="server" ID="LoggedOnTitle" /></p>
                                <h2><a href="Profile.aspx"><asp:Literal runat="server" ID="LoggedOnUserName" /></a></h2>
                            </header>
                        <section class="row collapse">
                                <a href="FinancialOverview.aspx" class="box small-6 columns">
                                    <h2><asp:Literal runat="server" ID="CreditsLiteral" /></h2>

                                    <span class="orange">CREDITS</span>
                                </a>
                                <a href="MemberTracks.aspx" class="box small-6 columns">
                                    <h2><asp:Literal runat="server" ID="ProtectedLiteral" /></h2>
                                    <span class="orange">PROTETED TRACKS</span>
                                </a>
                                <!-- <div class="large-1 hide-for-small"></div> -->

                            </section>
                        <section class="social-network">
                              <div class="row collapse">
                              <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 id="FacebookHeading" runat="server" class="social facebook">F</h2>
                                </asp:HyperLink>
                                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 class="social"><i runat="server" id="SoundcloudItag" class="soundcloud"></i></h2>
                                </asp:HyperLink>
                                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 id="TwitterHeading" runat="server" class="social twitter">L</h2>                                
                                </asp:HyperLink>
                              </div>
                            </section>
                        <section class="actions">
                              <div class="row collapse">
                                <a href="../Account/ChangePassword.aspx" class="box small-12 columns"><asp:Localize ID="Localize4" runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a>
                              </div>
                              <div id="divAccPerCompleted" runat="server" class="row collapse border box small-12 columns">
                                <a href="Profile.aspx" class=""><asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral" Text="<%$ Resources: Resource, ClickToEdit %>" /></a>
                              </div>
                            </section>
                        <footer>

                                <a class="button extra-large expand border" href="Subscription.aspx?pid=4&country=NL&price=149,0000">upgrade plan</a>
                                <a class="button extra-large expand" href="SelectProduct.aspx"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources : Resource, GetMore %>"></asp:Literal></a>
                            </footer>
                    </div>
                </div>
                <!-- End Right Column -->
                <!-- Left Column -->
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            CONTACT</h1>
                    </div>
                    <div class="row contact-info">
                        <div class="large-11 columns">
                            <asp:Literal ID="ContactLiteral" runat="server"></asp:Literal>
                            <!-- Contact Form -->
                            <form id="contact" method="post" action="">
                            <label>
                                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources : Resource, Name %>"></asp:Localize></label>
                            <asp:TextBox runat="server" ID="Name"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Name"
                                Display="Dynamic" CssClass="failureNotification" ErrorMessage="Name is required."
                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            <label>
                                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, DefaultContactEmail %>" /></label>
                            <asp:TextBox runat="server" ID="Email"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                Display="Dynamic" CssClass="failureNotification" ErrorMessage="E-mail is required."
                                ToolTip="<%$ Resources: Resource, EmailRequired %>" ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="EmailValidate" Display="Dynamic" ControlToValidate="Email"
                                ValidationExpression="^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$"
                                ErrorMessage="Email is not valid." Text="*" ValidationGroup="RegisterUserValidationGroup"
                                runat="server">
                            </asp:RegularExpressionValidator>
                            <label>
                                <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, DefaultContactMesage %>" /></label>
                            <asp:TextBox runat="server" ID="Message" TextMode="MultiLine"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Message"
                                Display="Dynamic" CssClass="failureNotification" ErrorMessage="Message is required."
                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            <asp:Button ID="Send" runat="server" Text="<%$ Resources: Resource, DefaultContactSend %>"
                                ValidationGroup="RegisterUserValidationGroup" CssClass="button small border right"
                                OnClick="Send_Click" />
                            </form>
                        </div>
                    </div>
                    <!-- End Contact Form -->
                </div>
                <!-- End Left Column -->
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
