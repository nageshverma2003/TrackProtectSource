<%@ Page Title="<%$ Resources: Resource, ttlFinancialOverview %>" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="FinancialOverview.aspx.cs" Inherits="TrackProtect.Member.FinancialOverview" %>

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
    <script src="../js/vendor/jquery.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.min.js"></script>
    <script type="text/javascript">
        function InitializeRequest(path) {
            // call server side method 
            PageMethods.SetDownloadPath(path);

            // Create an IFRAME.
            var iframe = document.createElement("iframe");
            iframe.src = "Download.aspx";

            // This makes the IFRAME invisible to the user.
            iframe.style.display = "none";

            // Add the IFRAME to the page. This will trigger
            // a request to GenerateFile now.
            document.body.appendChild(iframe);
        } 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_financialoverview" style="display: none">
        <table width="100%">
            <tr valign="top">
                <td class="leftColumn">
                    <div class="backtocontrolpanel">
                        <div class="backtocontrolpaneltext">
                            <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></div>
                        <div class="backtocontrolpanelbutton">
                            <a href="/Member/MemberHome.aspx" class="backtocontrolpanellink">
                                <img alt="" src="/Images/cp_settings.png" class="backtocontrolpanelimage" />
                            </a>
                        </div>
                    </div>
                    <div class="financialoverview">
                        <h1 class="headerLine">
                        </h1>
                        <div style="margin-left: 16px; margin-top: 12px; width: 580px;">
                            <asp:Table runat="server" ID="FinancialOverviewTable" Width="580px" />
                        </div>
                    </div>
                </td>
                <td class="centerColumn">
                    <div class="centerDivide">
                        <asp:Image ID="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="contropanel" class="row">
        <asp:Literal runat="server" ID="RhosMovementInc" Visible="false" />
        <asp:Literal runat="server" ID="FinancialEditInc" Visible="false" />
        <div class="large-12 columns" style="padding-bottom: 50px;">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, FinancialOverview %>" /></h1>
            </div>
            <div class="row">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                    <a class="button extra-large expand control_p_btn" href="memberhome.aspx"><i class="arrow-left"></i>
                        <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></a>
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
                                <a class="button extra-large expand" href="SelectProduct.aspx"><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources : Resource, GetMore %>"></asp:Literal></a>
                            </footer>
                    </div>
                </div>
                <!-- End Right Column -->
                <!-- Main Content -->
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, FinancialOverview %>" /></h1>
                    </div>
                    <!--
                        <h2>credit overview</h2>
                        -->
                    <a href="SelectProduct.aspx" class="button small border right" style="margin-right: 10px;">
                        <asp:Localize ID="Localize2" runat="server" Text="<%$resources : Resource, GetMoreCredits %>"></asp:Localize></a>
                    <asp:DataList ID="dlMyTracks" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                        Width="100%" BorderStyle="None" OnItemDataBound="dlMyTracks_ItemDataBound">
                        <ItemTemplate>
                            <div class="row collapse credits">
                                <%--<div class="large-5 small-12 columns add-right-border">
                                    <h6>
                                        <%#Eval("Description")%>
                                    </h6>
                                </div>
                                <div class="large-2 small-2 columns add-right-border has-tip tip-bottom" data-tooltip
                                    title="Reminding Credits">
                                    <span>
                                        <%#Eval("Credits")%></span></div>
                                <div class="large-4 small-10 columns add-right-border has-tip tip-bottom" data-tooltip
                                    title="Validation date">
                                    <span>
                                        <%#Convert.ToDateTime(Eval("PurchaseDate").ToString()).ToString("MM/dd/yyyy")%>
                                        -
                                        <%#Convert.ToDateTime(Eval("PurchaseDate").ToString()).AddYears(1) .ToString("MM/dd/yyyy")%></span></div>
                                <div class="large-1 small-12 columns">--%>
                                <%--<a href="#" alt="Get Invoice PDF" class="has-tip tip-bottom" data-tooltip title="Get Your Invoice"></a>--%>
                                <%-- <asp:HyperLink NavigateUrl="~/Member/SelectProduct.aspx" ID="hlInvoice" runat="server"
                                        CssClass="has-tip tip-bottom" ToolTip="Get Your Invoice" Style="cursor: hand;">
                                        <i class="icon-pdf"></i></asp:HyperLink>
                                    <asp:HiddenField ID="hdnInvoice" runat="server" Value='<%#Eval("InvoiceFile")%>' />
                                </div>--%>
                                <div class="large-1 small-2 columns add-right-border ">
                                    <a href="#"><i class="icon-doc"></i></a>
                                </div>
                                <div class="large-5 small-10 columns add-right-border">
                                    <h6>
                                        <%#Eval("Description")%></h6>
                                </div>
                                <div class="large-1 small-2 columns add-right-border has-tip tip-bottom" data-tooltip
                                    title="Reminding Credits">
                                    <span class="meta-data">
                                        <%#Eval("Credits")%></span></div>
                                <div class="large-4 small-8 columns add-right-border has-tip tip-bottom" data-tooltip
                                    title="Validation date">
                                    <span class="meta-data">
                                        <%#Convert.ToDateTime(Eval("PurchaseDate").ToString()).ToString("MM/dd/yyyy")%>
                                        -
                                        <%#Convert.ToDateTime(Eval("PurchaseDate").ToString()).AddYears(1) .ToString("MM/dd/yyyy")%></span></div>
                                <div class="large-1 small-2 columns">
                                    <asp:HyperLink NavigateUrl="javascript:void(0)" ID="hlInvoice" runat="server" CssClass="has-tip tip-bottom"
                                        ToolTip="Get Your Invoice" Style="cursor: hand;">
                                        <i class="icon-pdf"></i></asp:HyperLink>
                                    <asp:HiddenField ID="hdnInvoice" runat="server" Value='<%#Eval("InvoiceFile")%>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:DataList>
                </div>
                <!-- End Main Content -->
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
