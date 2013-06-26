<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="MemberHome.aspx.cs" Inherits="TrackProtect.Member.MemberHome"
    UICulture="auto" Culture="auto" %>

<%@ Import Namespace="TrackProtect" %>
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
    <style type="text/css">
        div.controlPanelItem
        {
            display: block;
        }
        div.controlPanelItemIcon
        {
            display: block;
        }
        div.controlPanelItemText
        {
            display: inline;
        }
        div.controlPanelItemInfo
        {
            display: inline;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, MemberHome %>" /></h1>
            </div>
            <div class="row">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                    <div id="user-info">
                        <header>
                                <p><asp:Localize runat="server" ID="LoggedOnTitle" /></p>
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
                              <asp:HyperLink runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 id="FacebookHeading" runat="server" class="social facebook">F</h2>
                                </asp:HyperLink>
                                <asp:HyperLink runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 class="social"><i runat="server" id="SoundcloudItag" class="soundcloud"></i></h2>
                                </asp:HyperLink>
                                <asp:HyperLink runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 id="TwitterHeading" runat="server" class="social twitter">L</h2>                                
                                </asp:HyperLink>
                              </div>
                            </section>
                        <section class="actions">
                              <div class="row collapse">
                                <a href="../Account/ChangePassword.aspx" class="box small-12 columns"><asp:Localize runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a>
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
                    <%--<div id="user-info">
                        <header>
                                <p><asp:Localize runat="server" ID="LoggedOnTitle" /></p><h2> <asp:Literal runat="server" ID="LoggedOnUserName" /></h2></header>
                        <section class="row">                        
                                    <asp:HyperLink runat="server" NavigateUrl="~/Member/FinancialOverview.aspx">
                                    <div class="box large-3 large-offset-1 small-4 columns">
                                    <h2> <asp:Literal runat="server" ID="CreditsLiteral" /></h2><span class="orange">CREDITS</span> </div>
                                    </asp:HyperLink>

                                    <asp:HyperLink runat="server" NavigateUrl="~/Member/MemberTracks.aspx">
                                    <div class="box large-6 large-offset-1 small-7 small-offset-1 columns">                                   
                                    <h2><asp:Literal runat="server" ID="ProtectedLiteral" /></h2><span class="orange">PROTECTED TRACKS</span> </div><div class="large-1 hide-for-small"></div>
                                    </asp:HyperLink>
                            </section>
                        <footer>
                                <asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral" Text="<%$ Resources: Resource, ClickToEdit %>" />
                                <br />
                                <br />
                                <asp:HyperLink runat="server" Text="<%$ Resources : Resource, ChangePassword %>" NavigateUrl="~/Account/ChangePassword.aspx"></asp:HyperLink>
                                <br />
                                <br />
                                <span style="color:#E4510A;">
                                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources : Resource, GetMore %>"></asp:Literal>
                                <asp:HyperLink Font-Underline="true" Font-Bold="true" Text="<%$ Resources : Resource, GetMoreText %>" ID="HyperLink3" runat="server" NavigateUrl="~/Member/SelectProduct.aspx"></asp:HyperLink></span>
                                <br />
                                <br />
                                <div><a href="Profile.aspx#social"><img alt="" height="55px" width="55px" src="../Images/socialmedia/soundcloud-icon.png" /><asp:Image ID="ConnectionToSoundCloud" Height="20" Width="20" runat="server"/>&nbsp;&nbsp;&nbsp;<img alt="" height="54px" width="54px" src="../Images/socialmedia/facebook-logo.png" /><asp:Image ID="ConnectionToFacebook" Height="20" Width="20" runat="server" />&nbsp;&nbsp;&nbsp;<img alt="" height="52px" width="52px" src="../Images/socialmedia/Twitter-Icon.png" /><asp:Image ID="ConnectionToTwitter" Height="20" Width="20" runat="server" /></a></div>
                        </footer>
                    </div>--%>
                </div>
                <!-- End Right Column -->
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources: Resource, MemberHome %>" /></h1>
                    </div>
                    <ul class="control-nav large-block-grid-3 small-block-grid-2">
                        <li><a href="MemberTracks.aspx" class="usercontrol-sprite">
                            <img class="spacer" src="../images/spacing.png" alt="image spacer" />
                            <img class="sprite" src="../images/user-controls.gif" alt="My Tracks" />
                        </a>
                            <h2>
                                <asp:HyperLink ID="MemberTracksLink" runat="server" NavigateUrl="~/Member/MemberTracks.aspx">
                                    <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: Resource, TrackOverview %>" /></asp:HyperLink></h2>
                            <p>
                                <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources: Resource, txtProtectedTracks %>" /></p>
                        </li>
                        <li><a href="Profile.aspx" class="usercontrol-sprite">
                            <img class="spacer" src="../images/spacing.png" alt="image spacer" />
                            <img class="sprite profile" src="../images/user-controls.gif" alt="My Profile" />
                        </a>
                            <h2>
                                <asp:HyperLink ID="UserPanelLink" runat="server" NavigateUrl="~/Member/Profile.aspx">
                                    <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, MyProfileText %>" />
                                </asp:HyperLink></h2>
                            <p>
                                <asp:Localize ID="Localize12" runat="server" Text="<%$ Resources: Resource, txtAccountSettings %>" />
                            </p>
                        </li>
                        <li><a href="ManageRelations.aspx" class="usercontrol-sprite">
                            <img class="spacer" src="../images/spacing.png" alt="image spacer" />
                            <img class="sprite relations" src="../images/user-controls.gif" alt="My Relationships" />
                        </a>
                            <h2>
                                <asp:HyperLink runat="server" ID="RelatedArtistsLink" NavigateUrl="~/Member/ManageRelations.aspx">
                                    <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources: Resource, RelatedArtists %>" />
                                </asp:HyperLink></h2>
                            <p>
                                <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: Resource, txtManagedRelations %>" /></p>
                        </li>
                        <li><a href="FinancialOverview.aspx" class="usercontrol-sprite">
                            <img class="spacer" src="../images/spacing.png" alt="image spacer" />
                            <img class="sprite credits" src="../images/user-controls.gif" alt="My Credits" />
                        </a>
                            <h2>
                                <asp:HyperLink ID="CreditOverviewLink" runat="server" NavigateUrl="~/Member/FinancialOverview.aspx">
                                    <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, CreditOverview %>" />
                                </asp:HyperLink></h2>
                            <p>
                                <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, txtPurchases %>" /></p>
                        </li>
                        <li><a href="Promo.aspx" class="usercontrol-sprite">
                            <img class="spacer" src="../images/spacing.png" alt="image spacer" />
                            <img class="sprite rhos" src="../images/user-controls.gif" alt="RHOS" />
                        </a>
                            <h2>
                                <asp:HyperLink runat="server" ID="HyperLink1" NavigateUrl="~/Member/Promo.aspx">
                                    <asp:Localize ID="Localize14" runat="server" Text="<%$ Resources: Resource, txtPromoPage %>" />
                                </asp:HyperLink></h2>
                            <p>
                                <asp:Localize ID="Localize15" runat="server" Text="<%$ Resources: Resource, txtPromoPageInfo %>" />
                            </p>
                        </li>
                        <li><a href="couponcode.aspx" class="usercontrol-sprite">
                            <img class="spacer" src="../images/spacing.png" alt="image spacer" />
                            <img class="sprite coupon" src="../images/user-controls.gif" alt="Coupon Code" />
                        </a>
                            <h2>
                                <asp:HyperLink runat="server" ID="HyperLink2" NavigateUrl="~/Member/CouponCode.aspx">
                                    <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources: Resource, CouponCodes %>" />
                                </asp:HyperLink></h2>
                            <p>
                                <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources: Resource, CouponCodesInfo %>" /></p>
                        </li>
                    </ul>
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
