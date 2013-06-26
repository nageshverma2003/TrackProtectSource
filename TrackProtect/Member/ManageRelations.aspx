<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ManageRelations.aspx.cs" Inherits="TrackProtect.Member.ManageRelations" %>

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
    <style type="text/css">
        #ctl00_MainContent_dlMyRelations td, tr, th
        {
            border: 0px solid transparent;
            background: white;
            color: #E4510A;
            font-family: "Open Sans" , "Helvetica Neue" , "Helvetica" ,Helvetica,Arial,sans-serif;
            font-weight: 700;
            margin-left: 1em;
            text-transform: capitalize;
            padding: 0px !important;
            font-size: 1.0em;
        }
        #ctl00_MainContent_dlMyManagedRelations td, tr, th
        {
            border: 0px solid transparent;
            background: white;
            color: #E4510A;
            font-family: "Open Sans" , "Helvetica Neue" , "Helvetica" ,Helvetica,Arial,sans-serif;
            font-weight: 700;
            margin-left: 1em;
            text-transform: capitalize;
            padding: 0px !important;
            font-size: 1.0em;
        }
        #controlpanel label
        {
            font-size: 0.875em !important;
        }
        .divPending
        {
            width: 90%;
            padding: 1% 1% 2% 1%;
            border: 1px solid #E4510A;
        }
        #relationships .friends .button-group .button1
        {
            background-color: #E4510A !important;
            color: White !important;
            border: medium none;
            box-shadow: none;
            font-family: "League Gothic" , "Helvetica Neue" , "Helvetica" ,Helvetica,Arial,sans-serif;
            font-weight: normal;
            transition: none 1s ease 0s;
        }
        #relationships .friends .button-group .button
        {
            line-height: 1em !important;
        }
    </style>
    <script src="../js/vendor/jquery.js" type="text/javascript"></script>
    <script src="../js/vendor/custom.modernizr.js" type="text/javascript"></script>
    <script type="text/javascript">
        function confirmdelete() {

            var msg = '<%= Resources.Resource.ConfirmDeleteRelation %>';
            return confirm(msg);
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_artistrelation" style="display: none">
        <table width="100%">
            <tr valign="top">
                <td class="leftColumn" style="width: 80%;">
                    <div class="artistrelation">
                        <div class="relationContainer">
                            <div class="existingRelations">
                                <asp:GridView ID="ArtistsTable" runat="server" GridLines="None" AutoGenerateColumns="False"
                                    EmptyDataText="<%$ Resources:Resource, NoArtistsRegistered %>" HeaderStyle-HorizontalAlign="Left"
                                    RowStyle-HorizontalAlign="Left" CellPadding="4" Width="100%" BorderStyle="None"
                                    BorderWidth="0px" OnRowCommand="ArtistsTable_RowCommand" OnRowDataBound="ArtistsTable_RowDataBound">
                                    <AlternatingRowStyle BackColor="#F6F6F6" />
                                    <Columns>
                                        <asp:BoundField DataField="name" HeaderText="<%$ Resources: Resource, Name %>">
                                            <HeaderStyle Width="80%" CssClass="fieldName" Font-Size="0.9em" />
                                            <ItemStyle CssClass="accountData" />
                                        </asp:BoundField>
                                        <asp:ImageField DataImageUrlField="reltype" DataImageUrlFormatString="~\Images\reltype{0}.png">
                                        </asp:ImageField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:HiddenField ID="HiddenFieldUserId" runat="server" Value='<%# Bind("userid") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Image" CommandName="RelateUser" ImageUrl="~/Images/link_add.png"
                                            Text="Link">
                                            <HeaderStyle HorizontalAlign="Center" Width="32px" />
                                            <ItemStyle HorizontalAlign="Center" Width="32px" />
                                        </asp:ButtonField>
                                        <asp:ButtonField ButtonType="Image" CommandName="DeleteUser" ImageUrl="~/Images/remove-user.png"
                                            Text="Delete">
                                            <HeaderStyle HorizontalAlign="Right" Width="32px" />
                                            <ItemStyle HorizontalAlign="Right" Width="32px" />
                                        </asp:ButtonField>
                                    </Columns>
                                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    <RowStyle HorizontalAlign="Left"></RowStyle>
                                </asp:GridView>
                            </div>
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
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, ManageRelation %>" /></h1>
            </div>
            <div class="row">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                    <a class="button extra-large expand control_p_btn" href="memberhome.aspx"><i class="arrow-left"></i>
                        <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></a>
                    <div id="user-info">
                        <header>
                            <p>
                                <asp:Localize runat="server" ID="LoggedOnTitle" /></p>
                            <h2>
                                <a href="Profile.aspx">
                                    <asp:Literal runat="server" ID="LoggedOnUserName" /></a></h2>
                        </header>
                        <section class="row collapse">
                            <a href="FinancialOverview.aspx" class="box small-6 columns">
                                <h2>
                                    <asp:Literal runat="server" ID="CreditsLiteral" /></h2>
                                <span class="orange">CREDITS</span> </a><a href="MemberTracks.aspx" class="box small-6 columns">
                                    <h2>
                                        <asp:Literal runat="server" ID="ProtectedLiteral" /></h2>
                                    <span class="orange">PROTETED TRACKS</span> </a>
                            <!-- <div class="large-1 hide-for-small"></div> -->
                        </section>
                        <section class="social-network">
                            <div class="row collapse">
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Member/Profile.aspx#social"
                                    CssClass="box small-4 columns">
                                    <h2 id="FacebookHeading" runat="server" class="social facebook">
                                        F</h2>
                                </asp:HyperLink>
                                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Member/Profile.aspx#social"
                                    CssClass="box small-4 columns">
                                    <h2 class="social">
                                        <i runat="server" id="SoundcloudItag" class="soundcloud"></i>
                                    </h2>
                                </asp:HyperLink>
                                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Member/Profile.aspx#social"
                                    CssClass="box small-4 columns">
                                    <h2 id="TwitterHeading" runat="server" class="social twitter">
                                        L</h2>
                                </asp:HyperLink>
                            </div>
                        </section>
                        <section class="actions">
                            <div class="row collapse">
                                <a href="../Account/ChangePassword.aspx" class="box small-12 columns">
                                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a>
                            </div>
                            <div id="divAccPerCompleted" runat="server" class="row collapse border box small-12 columns">
                                <a href="Profile.aspx" class="">
                                    <asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral"
                                        Text="<%$ Resources: Resource, ClickToEdit %>" /></a>
                            </div>
                        </section>
                        <footer>
                            <a class="button extra-large expand border" href="Subscription.aspx?pid=4&country=NL&price=149,0000">
                                upgrade plan</a> <a class="button extra-large expand" href="SelectProduct.aspx">
                                    <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources : Resource, GetMore %>"></asp:Literal></a>
                        </footer>
                    </div>
                </div>
                <!-- End Right Column -->
                <!-- Main Content -->
                <div class="large-8 columns pull-4">
                    <div>
                        <%--class="row"--%>
                        <a href="Invitation.aspx" class="button">
                            <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources: Resource, InviteRelations %>" />
                        </a>
                    </div>
                    <!-- End Invite -->
                    <div>
                        <%-- class="row"--%>
                        <asp:Label runat="server" ID="pendingInvitationsHeader" Text="" />
                        <asp:Label runat="server" ID="ResultLabel" Text="" />
                        <asp:ListView runat="server" ID="pendingInvitations" OnItemCommand="pendingInvitations_ItemCommand"
                            OnLayoutCreated="OnLayoutCreated">
                            <layouttemplate>
                                <h2>
                                    <asp:Localize ID="InvitationType" runat="server" />
                                </h2>
                                <ul class="no-style">
                                    <li id="itemPlaceholder" runat="server" visible="false" style="display: none;" />
                                </ul>
                            </layouttemplate>
                            <itemtemplate>
                                <div class="large-11 columns">
                                    <div class="row friends">
                                        <div class="large-4 small-9 columns add-side-borders">
                                            <%# Eval("name") %>
                                        </div>
                                        <div class="large-6 small-9 columns add-side-borders">
                                            <%# Eval("email") %>
                                            <span>
                                                <asp:HiddenField runat="server" ID="invitationId" Value='<%# Bind("confirmation_id") %>' />
                                            </span>
                                        </div>
                               
                                                                        
                                        <asp:ImageButton runat="server" ID="processButton" Text="<%$ Resources: Resource, Accept%>"
                                                    CommandName="Accept" ImageUrl="~/image/Accept.png" Height="37px" Width="37px" style="margin-top:4px; margin-left:2px;"/>                                                  
                                        <asp:ImageButton runat="server" ID="processDeclineButton" Text="<%$ Resources: Resource, Accept%>"
                                                    CommandName="Decline" ImageUrl="~/image/Decline.png" Height="37px" Width="37px" style="margin-top:4px; margin-left:4px;"/>
                                                    
                                                                                                     
                                       <%-- <div class="large-1 columns">
                                            <div>
                                                <asp:Button runat="server" ID="processButton" Text="<%$ Resources: Resource, Accept%>"
                                                    CommandName="Accept" CssClass="button small" />
                                            </div>
                                        </div>--%>
                                    </div>
                                </div>
                                <div class="row">
                                </div>
                            </itemtemplate>
                        </asp:ListView>
                    </div>
                    <div style="clear: both;">
                    </div>
                    <div>
                        <%-- class="row"--%>
                        <asp:Label runat="server" ID="pendingManageInvitationsHeader" Text="" />
                        <asp:Label runat="server" ID="ManageResultLabel" Text="" />
                        <asp:ListView runat="server" ID="pendingManageInvitations" OnItemCommand="pendingManageInvitations_ItemCommand"
                            OnLayoutCreated="ManageOnLayoutCreated">
                            <layouttemplate>
                                <h2>
                                    <asp:Localize ID="ManageInvitationType" runat="server" />
                                </h2>
                                <ul class="no-style">
                                    <li id="itemPlaceholder" runat="server" visible="false" style="display: none;" />
                                </ul>
                            </layouttemplate>
                            <itemtemplate>
                                <div class="large-11 columns">
                                    <div class="row friends">
                                        <div class="large-4 small-9 columns add-side-borders">
                                            <%# Eval("name") %>
                                        </div>
                                        <div class="large-6 small-9 columns add-side-borders">
                                            <%# Eval("email") %>
                                            <span>
                                                <asp:HiddenField runat="server" ID="ManageinvitationId" Value='<%# Bind("confirmation_id") %>' />
                                            </span>
                                        </div>
                                          <asp:ImageButton runat="server" ID="ManageprocessButton" Text="<%$ Resources: Resource, Accept%>"
                                                    CommandName="Accept" ImageUrl="~/image/Accept.png" Height="37px" Width="37px" style="margin-top:4px; margin-left:2px;"/>                                                  
                                        <asp:ImageButton runat="server" ID="ManageprocessDeclineButton" Text="<%$ Resources: Resource, Accept%>"
                                                    CommandName="Decline" ImageUrl="~/image/Decline.png" Height="37px" Width="37px" style="margin-top:4px; margin-left:4px;"/>
                                        <%--<div class="large-2 columns">
                                            <div>
                                                <asp:Button runat="server" ID="ManageprocessButton" Text="<%$ Resources: Resource, Accept%>"
                                                    CommandName="Accept" CssClass="button small" />
                                            </div>
                                        </div>--%>
                                    </div>
                                </div>
                                <div class="row">
                                </div>
                            </itemtemplate>
                        </asp:ListView>
                    </div>
                    <!-- My Relationships -->
                    <h2>
                        <asp:Localize ID="Localize4" runat="server" Text='<%$ Resources : Resource, MyRelationships %>'></asp:Localize></h2>
                    <!-- First Friend -->
                    <div class="row">
                        <asp:DataList ID="dlMyRelations" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                            Width="100%" BorderStyle="None" OnItemCommand="dlMyRelations_ItemCommand" OnItemDataBound="dlMyRelations_ItemDataBound">
                            <ItemTemplate>
                                <div class="large-11 columns">
                                    <div class="row collapse friends">
                                        <div class="large-1 small-3 column">
                                            <span class="center-align"><i class="default-avatar"></span></i></div>
                                        <div class="large-6 small-9 columns add-side-borders">
                                            <h5>
                                                <a href="#">
                                                    <%#Eval("name") %></a></h5>
                                        </div>
                                        <div class="large-5 columns">
                                            <ul class="button-group even-3">
                                                <li style="width: 0% !important;">
                                                    <div style="margin-top: 10px;" runat="server" id="liRelation">
                                                        <a href="#" class="button"><i class="relations"></i></a>
                                                        <asp:HiddenField ID="hdnRelType" runat="server" Value='<%# Eval("RelType") %>' />
                                                        <asp:HiddenField ID="hdnmanaged" runat="server" Value='<%# Eval("manage") %>' />
                                                    </div>
                                                </li>
                                                <li style="width: 50% !important">
                                                    <div style="margin-top: 10px;">
                                                        <asp:LinkButton ID="lnkbtnRelate" CommandArgument='<%# Eval("userid") %>' runat="server"
                                                            CssClass="button" ForeColor="#E4510A" CommandName="RelateUser" Text="<%$ Resources : Resource, ManagementRequest %>"></asp:LinkButton></div>
                                                </li>
                                                <li style="width: 31% !important">
                                                    <div style="margin-top: 10px;">
                                                        <asp:LinkButton ID="lnkbtn" CommandArgument='<%# Eval("userid") %>' runat="server"
                                                            OnClientClick="return confirmdelete();" CssClass="button" CommandName="DeleteUser"
                                                            ForeColor="#E4510A" Text="<%$ Resources : Resource, DeleteText %>"></asp:LinkButton></div>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:DataList></div>
                    <asp:Label ID="lblMSg" runat="server" Text="<%$ Resources:Resource, NoArtistsRegistered %>"></asp:Label>
                    <div style="clear: both;">
                    </div>
                    <br />
                    <div id="divManagedArtists" runat="server">
                        <%--class="row"--%>
                        <h2>
                            <asp:Localize runat="server" Text='<%$ Resources : Resource, MyManagedArtists %>'></asp:Localize></h2>
                        <div class="row">
                            <asp:DataList ID="dlMyManagedRelations" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                                Width="100%" BorderStyle="None" OnItemCommand="dlMyManagedRelations_ItemCommand">
                                <ItemTemplate>
                                    <div class="large-11 columns">
                                        <div class="row collapse friends">
                                            <div class="large-1 small-3 column">
                                                <span class="center-align"><i class="default-avatar"></i></span>
                                            </div>
                                            <div class="large-6 small-9 columns add-side-borders">
                                                <h5>
                                                    <a href="#">
                                                        <%#Eval("name") %>
                                                    </a>
                                                </h5>
                                            </div>
                                            <div class="large-5 columns">
                                                <ul class="button-group even-3">
                                                    <li>
                                                        <div style="margin-top: 10px;">
                                                            <asp:Button ID="btnAddTrack" CommandName="AddTrack" runat="server" Text="<%$ Resources: Resource, AddTrack %>"
                                                                CommandArgument='<%# Eval("userid") %>' ForeColor="#E4510A" CssClass="button" />
                                                        </div>
                                                    </li>
                                                    <li>
                                                        <div style="margin-top: 10px;">
                                                            <asp:Button ID="btnViewTracks" CommandArgument='<%# Eval("userid") %>' runat="server"
                                                                Text="<%$ Resources: Resource, ViewTracks %>" CssClass="button" CommandName="ViewTracks"
                                                                ForeColor="#E4510A" /></div>
                                                    </li>
                                                    <li>
                                                        <div style="margin-top: 10px;">
                                                            <asp:LinkButton ID="lnkbtn" CommandArgument='<%# Eval("userid") %>' runat="server"
                                                                OnClientClick="return confirmdelete();" CssClass="button" CommandName="DeleteUser"
                                                                ForeColor="#E4510A" Text="<%$ Resources : Resource, DeleteText %>"></asp:LinkButton></div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                        </div>
                        <asp:Label ID="lblMSgMANACC" runat="server" Text="<%$ Resources:Resource, NoArtistsRegistered %>"></asp:Label>
                    </div>
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
