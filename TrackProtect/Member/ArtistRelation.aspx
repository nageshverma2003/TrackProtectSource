<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ArtistRelation.aspx.cs" Inherits="TrackProtect.Member.ArtistRelation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
        #controlpanel label
        {
            font-size: 0.875em !important;
        }
        .row
        {
            margin: 0 auto !important;
            max-width: 69.5em !important;
            width: 100% !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_artistrelation" style="display: none;">
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
                                <a href="../Account/ChangePassword.aspx" class="box small-12 columns"><asp:Localize ID="Localize3" runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a>
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
                    <h2>
                        <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, Invitation %>" /></h2>
                    <div class="row">
                        <div class="large-7 columns add-right-border">
                            <p class="help-text">
                                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, InvitationExplain %>" /></p>
                            <form action="">
                            <div class="row">
                                <div class="large-3 columns">
                                    <label>
                                        <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: Resource, ArtistEmail %>" /></label><asp:Label
                                            ID="TpIdLabel" runat="server" Text="" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:TextBox ID="TpIdText" runat="server" /></div>
                            </div>
                            <div class="row">
                                <div class="large-3 columns">
                                    <label>
                                        <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources: Resource, FirstName %>" /></label></div>
                                <div class="large-9 columns">
                                    <asp:TextBox runat="server" ID="FirstName" /></div>
                            </div>
                            <div class="row">
                                <div class="large-3 columns">
                                    <label>
                                        <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources: Resource, LastName %>" /></label></div>
                                <div class="large-9 columns">
                                    <asp:TextBox runat="server" ID="LastName" /></div>
                            </div>
                            </form>
                        </div>
                        <div class="social-connect add-left-padding large-5 columns">
                            <p class="help-text">
                                ...of connect met je vrienden via Facebook of Twitter</p>
                            <div>
                                <a class="button facebook" href="#"><span class="social">F</span>CONNECT</a></div>
                            <div>
                                <a class="button twitter" href="#"><span class="social">L</span>CONNECT</a></div>
                        </div>
                    </div>
                    <div style="clear: both;">
                    </div>
                    <div style="text-align: center;">
                        <asp:Button ID="TpIdButton" runat="server" Text="<%$ Resources: Resource, Accept %>"
                            OnCommand="TpIdSearch" Width="85px" CssClass="button" />
                    </div>
                    <!-- End Invite -->
                    <!-- My Relationships -->
                    <h2>
                        <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: Resource, Invitations %>" /></h2>
                    <!-- First Friend -->
                    <div class="row">
                        <asp:Label runat="server" ID="pendingInvitationsHeader" Text="" />
                        <asp:ListView runat="server" ID="pendingInvitations" OnItemCommand="pendingInvitations_ItemCommand">
                            <LayoutTemplate>
                                <ul>
                                    <li id="itemPlaceholder" runat="server" />
                                </ul>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <div class="large-11 columns">
                                    <div class="row collapse friends">
                                        <div class="large-7 small-9 columns add-side-borders">
                                            <%# Eval("name") %>
                                            <%# Eval("email") %>
                                            <asp:HiddenField runat="server" ID="invitationId" Value='<%# Bind("confirmation_id") %>' />
                                        </div>
                                        <div class="large-4 columns">
                                            <ul class="button-group even-3">
                                                <li><span class="button">
                                                    <asp:ImageButton runat="server" ID="processButton" AlternateText="<%$ Resources: Resource, Accept%>"
                                                        ImageUrl="" CommandName="Accept" CssClass="button" /></li>
                                                </span>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:ListView>
                        <asp:Label runat="server" ID="ResultLabel" Text="" />
                        <asp:DataList ID="dlMyRelations" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                            Width="100%" BorderStyle="None">
                            <ItemTemplate>
                                <div class="large-11 columns">
                                    <div class="row collapse friends">
                                        <div class="large-1 small-3 column">
                                            <span class="center-align"><i class="default-avatar">
                                                <asp:ImageField DataImageUrlField="reltype" DataImageUrlFormatString="~\Images\reltype{0}.png">
                                                </asp:ImageField></span></i></div>
                                        <div class="large-7 small-9 columns add-side-borders">
                                            <h5>
                                                <a href="#">
                                                    <%#Eval("name") %></a></h5>
                                        </div>
                                        <div class="large-4 columns">
                                            <ul class="button-group even-3">
                                                <li><a href="#" class="button"><i class="relations"></i></a></li>
                                                <li><a href="#" class="button"><i class="link"></i></a></li>
                                                <li>
                                                    <%--<asp:ImageButton ID="btnDelete" CssClass="button" runat="server" CommandName="DeleteUser"
                                                        AlternateText=" "></asp:ImageButton>--%>
                                                    <asp:LinkButton CommandArgument='<%# Eval("userid") %>' ID="lnkbtn" runat="server"
                                                        CssClass="button" CommandName="DeleteUser"><i class="remove"></i></asp:LinkButton>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>
                    <!-- End First Friend -->
                </div>
                <!-- End Left-Side Main Content -->
            </div>
        </div>
    </div>
</asp:Content>
