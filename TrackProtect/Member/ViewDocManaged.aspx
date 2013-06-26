<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ViewDocManaged.aspx.cs" Inherits="TrackProtect.Member.ViewDocManaged" %>

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
    <link rel="stylesheet" type="text/css" href="../css/mp3-player-button.css" />
    <script type="text/javascript" src="../script/soundmanager2.js"></script>
    <script type="text/javascript" src="../script/mp3-player-button.js"></script>
    <script type="text/javascript">
        soundManager.setup({
            useFlashBlock: true, // optional - if used, required flashblock.css
            url: 'swf/' // required: path to directory containing SM2 SWF files
        });        
    </script>
    <style type="text/css">
        #ctl00_MainContent_dlMyTracks td, tr, th
        {
            border: 0px solid transparent;
            background: none;
            color: #E4510A;
            font-family: "Open Sans" , "Helvetica Neue" , "Helvetica" ,Helvetica,Arial,sans-serif;
            font-weight: 700;
            margin-left: 1em;
            text-transform: capitalize;
            padding: 0px !important;
            font-size: 1.0em;
        }
    </style>
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
    <asp:Literal runat="server" ID="RhosMovementInc" Visible="false" />
    <asp:Literal runat="server" ID="ProtectInc" Visible="false" />
    <div id="contropanel" class="row">
        <div class="large-12 columns" style="padding-bottom: 50px;">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, ManageArtists %>" /></h1>
            </div>
            <div class="row">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                    <a class="button extra-large expand control_p_btn" href="memberhome.aspx"><i class="arrow-left">
                    </i>
                        <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></a>
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
                                <a href="../Account/ChangePassword.aspx" class="box small-12 columns"><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a>
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
                <!-- Main Content -->
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            relaties</h1>
                    </div>
                    <!-- Invite -->
                    <asp:Label runat="server" ID="ResultLabel" Text="" />
                    <asp:DataList ID="dlMyTracks" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                        Width="100%" BorderStyle="None" OnItemDataBound="dlMyTracks_ItemDataBound" OnItemCommand="dlMyTracks_ItemCommand">
                        <ItemTemplate>
                            <div class="row collapse tracks">
                                <div class="large-1 small-2 columns add-right-border action-button">
                                    <asp:HyperLink ID="MusicLink" runat="server" title="Play &quot;Office Lobby&quot;"
                                        CssClass="sm2_button"></asp:HyperLink>
                                    <%-- <a href="#" class="button"><i class="icon-play"></i></a>--%>
                                </div>
                                <div class="large-5 small-10 columns add-right-border has-hidden-content">
                                    <h5>
                                        <%# Cutdesc(DataBinder.Eval(Container.DataItem, "name", "{0}"))%></h5>
                                </div>
                                <div class="large-4 small-12 columns add-right-border has-tip tip-bottom" data-tooltip
                                    title="Track validation date">
                                    <h5 class="meta-data">
                                        <%# DataBinder.Eval(Container.DataItem, "[registrationdate]", "{0:M/dd/yyyy}")%>
                                        -
                                        <%# DataBinder.Eval(Container.DataItem, "[expirationdate]", "{0:M/dd/yyyy}")%></h5>
                                </div>
                                <div class="large-1 small-6 columns add-right-border action-button">
                                    <asp:HyperLink NavigateUrl="javascript:void(0)" ID="downloadButton" runat="server"
                                        class="button"><i class="icon-cert"></i></asp:HyperLink>
                                </div>
                                <div class="large-1 small-6 columns action-button">
                                    <asp:HyperLink NavigateUrl="javascript:void(0)" runat="server" ID="downloadDocument"
                                        class="button"><i class="icon-pdf"></i></asp:HyperLink>
                                </div>
                                <%--  <div class="large-4 small-12 columns">
                                    <ul class="button-group even-3">
                                        <li><a onclick="this.firstChild.play()">
                                            <audio src='<%# Session["Path1"] %>'></audio>
                                            ▸</a>--%>
                                <asp:HiddenField ID="regIDHF" runat="server" Value='<%# Eval("register_id") %>' />
                                <%--<asp:LinkButton ID="PlayMusic" runat="server" class="button" CommandArgument='<%# Eval("register_id") %>'><i class="icon-play"></i></asp:LinkButton>--%>
                                <%--<asp:HyperLink ID="PlayMusic" runat="server" class="button" on><i class="icon-play"></i></asp:HyperLink>--%>
                                <%--  </li>
                                        <li>
                                            <asp:HyperLink ID="downloadButton" runat="server" class="button"><i class="icon-cert"></i></asp:HyperLink>
                                        </li>
                                        <li>
                                            <asp:HyperLink runat="server" ID="downloadDocument" class="button"><i class="icon-pdf"></i></asp:HyperLink>
                                        </li>
                                    </ul>
                                </div>--%>
                            </div>
                        </ItemTemplate>
                    </asp:DataList>
                    <%--<asp:GridView ID="RegistrationGrid" runat="server" GridLines="None" AutoGenerateColumns="False"
                            EmptyDataText="<%$ Resources:Resource, NoRegisteredDocuments %>" OnRowDataBound="RegistrationGrid_RowDataBound"
                            HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left" CellPadding="4"
                            Width="100%">
                            <AlternatingRowStyle BackColor="#F6F6F6" />
                            <Columns>
                                <asp:BoundField DataField="name" HeaderText="<%$ Resources: Resource, Name %>">
                                    <HeaderStyle Width="150px" CssClass="fieldName" Font-Size="0.9em"></HeaderStyle>
                                    <ItemStyle CssClass="accountData" />
                                </asp:BoundField>
                                <asp:BoundField DataField="registrationdate" HeaderText="<%$ Resources: Resource, RegisteredOn %>"
                                    DataFormatString="{0:d}">
                                    <HeaderStyle Width="65px" CssClass="fieldName" Font-Size="0.9em" />
                                    <ItemStyle CssClass="accountData" HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="expirationdate" HeaderText="<%$ Resources: Resource, ValidUntil %>"
                                    DataFormatString="{0:d}">
                                    <HeaderStyle Width="65px" CssClass="fieldName" Font-Size="0.9em" />
                                    <ItemStyle CssClass="accountData" HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="<%$ Resources: Resource, Certificate %>" ItemStyle-HorizontalAlign="Center"
                                    ItemStyle-Width="50px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="downloadButton" runat="server" Text="" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="24px" CssClass="fieldName" HorizontalAlign="Left" Font-Size="0.8em" />
                                    <ItemStyle HorizontalAlign="Left" CssClass="accountData"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<%$ Resources: Resource, CertPdf %>" ItemStyle-Width="50px">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="downloadDocument" Text=""></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle Width="24px" CssClass="fieldName" Font-Size="0.9em" />
                                    <ItemStyle CssClass="accountData" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle HorizontalAlign="Left" BackColor="#F6F6F6"></HeaderStyle>
                            <RowStyle HorizontalAlign="Left" BackColor="White"></RowStyle>
                        </asp:GridView>--%>
                    <!-- End First Friend -->
                </div>
                <!-- End Left-Side Main Content -->
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
