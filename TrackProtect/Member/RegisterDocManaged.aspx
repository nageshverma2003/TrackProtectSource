<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="RegisterDocManaged.aspx.cs" Inherits="TrackProtect.Member.RegisterDocManaged" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ MasterType VirtualPath="~/Site.master" %>
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
    <script type="text/javascript">
        function checklengthSoundCloud(sender, arguments) {
            var txtid = document.getElementById('<%=SoundCloudMsg.ClientID %>');

            if (txtid.value.length > 140) {
                arguments.IsValid = false;
            }
            else {
                arguments.IsValid = true;
            }
        }

        function checklengthFacebook(sender, arguments) {
            var txtid = document.getElementById('<%=FacebookMsg.ClientID %>');

            if (txtid.value.length > 140) {
                arguments.IsValid = false;
            }
            else {
                arguments.IsValid = true;
            }
        }

        function checklengthTwitter(sender, arguments) {
            var txtid = document.getElementById('<%=TwitterMsg.ClientID %>');

            if (txtid.value.length > 70) {
                arguments.IsValid = false;
            }
            else {
                arguments.IsValid = true;
            }
        }

        function Count(text, long) {
            var maxlength = new Number(long);
            var remChar = maxlength - document.getElementById('<%=TwitterMsg.ClientID%>').value.length;
            if (remChar > -1) {
                document.getElementById('<%=lblWordRemainingCount.ClientID%>').innerHTML = remChar;
            }
            if (document.getElementById('<%=TwitterMsg.ClientID%>').value.length > maxlength) {
                text.value = text.value.substring(0, maxlength);
            }
        }

        $(document).ready(function () {
            $('#<%=cbxShareToFriends.ClientID %>').change(function () {
                if (this.checked) {
                    $('#divFriendList').fadeIn('slow');
                }
                else {
                    $('#divFriendList').fadeOut('slow');
                }
            });

            $('#<%=cbxShareToPages.ClientID %>').change(function () {
                if (this.checked) {
                    $('#divPageList').fadeIn('slow');
                }
                else {
                    $('#divPageList').fadeOut('slow');
                }
            });

            $('#ctl00_MainContent_cbxShareToOwnFriends').change(function () {
                if (this.checked) {
                    $('#divOwnFriendList').fadeIn('slow');
                }
                else {
                    $('#divOwnFriendList').fadeOut('slow');
                }
            });

            $('#ctl00_MainContent_cbxShareToOwnPages').change(function () {
                if (this.checked) {
                    $('#divOwnPageList').fadeIn('slow');
                }
                else {
                    $('#divOwnPageList').fadeOut('slow');
                }
            });

            $('#ctl00_MainContent_cbxSendToFacebook').change(function () {
                if (this.checked) {
                    $('#<%=divFBSharing.ClientID %>').fadeIn('slow');
                    $('#divFriendList').fadeIn('slow');
                    $('#divPageList').fadeIn('slow');
                    $('#divOwnFriendList').fadeIn('slow');
                    $('#divOwnPageList').fadeIn('slow');

                    $('#<%=cbxShareToFriends.ClientID %>').each(function () { this.checked = true; });
                    $('#<%=cbxShareToPages.ClientID %>').each(function () { this.checked = true; });
                    $('#ctl00_MainContent_cbxShareToOwnFriends').each(function () { this.checked = true; });
                    $('#ctl00_MainContent_cbxShareToOwnPages').each(function () { this.checked = true; });
                }
                else {
                    $('#<%=divFBSharing.ClientID %>').fadeOut('slow');
                    $('#divFriendList').fadeOut('slow');
                    $('#divPageList').fadeOut('slow');
                    $('#divOwnFriendList').fadeOut('slow');
                    $('#divOwnPageList').fadeOut('slow');

                    $('#<%=cbxShareToFriends.ClientID %>').each(function () { this.checked = false; });
                    $('#<%=cbxShareToPages.ClientID %>').each(function () { this.checked = false; });
                    $('#ctl00_MainContent_cbxShareToOwnFriends').each(function () { this.checked = false; });
                    $('#ctl00_MainContent_cbxShareToOwnPages').each(function () { this.checked = false; });
                }
            });
        });

        function pageLoad() {
            var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
            if (isAsyncPostback) {
                $(document).foundation();
                $(document).ready();
                $('#ctl00_MainContent_cbxSendToFacebook').change(function () {
                    if (this.checked) {
                        $('#<%=divFBSharing.ClientID %>').fadeIn('slow');
                        $('#divFriendList').fadeIn('slow');
                        $('#divPageList').fadeIn('slow');
                        $('#divOwnFriendList').fadeIn('slow');
                        $('#divOwnPageList').fadeIn('slow');

                        $('#<%=cbxShareToFriends.ClientID %>').each(function () { this.checked = true; });
                        $('#<%=cbxShareToPages.ClientID %>').each(function () { this.checked = true; });
                        $('#<%=cbxShareToFriends.ClientID %>').each(function () { this.checked = true; });
                        $('#ctl00_MainContent_cbxShareToPages').each(function () { this.checked = true; });
                    }
                    else {
                        $('#<%=divFBSharing.ClientID %>').fadeOut('slow');
                        $('#divFriendList').fadeOut('slow');
                        $('#divPageList').fadeOut('slow');
                        $('#divOwnFriendList').fadeOut('slow');
                        $('#divOwnPageList').fadeOut('slow');

                        $('#<%=cbxShareToFriends.ClientID %>').each(function () { this.checked = false; });
                        $('#<%=cbxShareToPages.ClientID %>').each(function () { this.checked = false; });
                        $('#ctl00_MainContent_cbxShareToOwnFriends').each(function () { this.checked = false; });
                        $('#ctl00_MainContent_cbxShareToOwnPages').each(function () { this.checked = false; });
                    }
                });
                $('#<%=cbxShareToFriends.ClientID %>').change(function () {
                    if (this.checked) {
                        $('#divFriendList').fadeIn('slow');
                    }
                    else {
                        $('#divFriendList').fadeOut('slow');
                    }
                });

                $('#<%=cbxShareToPages.ClientID %>').change(function () {
                    if (this.checked) {
                        $('#divPageList').fadeIn('slow');
                    }
                    else {
                        $('#divPageList').fadeOut('slow');
                    }
                });

                $('#ctl00_MainContent_cbxShareToOwnFriends').change(function () {
                    if (this.checked) {
                        $('#divOwnFriendList').fadeIn('slow');
                    }
                    else {
                        $('#divOwnFriendList').fadeOut('slow');
                    }
                });

                $('#ctl00_MainContent_cbxShareToOwnPages').change(function () {
                    if (this.checked) {
                        $('#divOwnPageList').fadeIn('slow');
                    }
                    else {
                        $('#divOwnPageList').fadeOut('slow');
                    }
                });
            }
        }
    </script>
    <style type="text/css">
        .style4
        {
            width: 215px;
        }
        .pnloverlay_template
        {
            background-color: #000000;
            height: 100%;
            left: 0px;
            margin-top: 0px;
            position: fixed;
            top: 0px;
            width: 100%;
            z-index: 10010;
            filter: alpha(opacity=50);
            -moz-opacity: 0.50;
            opacity: 0.50;
        }
        
        .validator
        {
            width: 45%;
            float: right;
            margin: 1%;
        }
        .btnRightMargin
        {
            margin-right: 8px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--<div class="member_registerdocmanaged">
	<table width="100%">
		<tr valign="top">
			<td class="leftColumn">
                <div class="backtocontrolpanel">
                    <div class="backtocontrolpaneltext"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>"/></div>
                    <div class="backtocontrolpanelbutton">
                        <a href="/Member/MemberHome.aspx" class="backtocontrolpanellink">
                            <img src="/Images/cp_settings.png" class="backtocontrolpanelimage"/>
                        </a>
                    </div>
                </div>
        		<div class="registerdocmanaged">
        		    <h1 class="headerLine"><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, RegisterDocument %>"/></h1>
                    <div class="dialogText"><asp:Literal runat="server" ID="litDialogText" /></div>
				    <div class="accountInfo">
                    <asp:Panel ID="Panel1" runat="server" DefaultButton="RegisterDocumentButton">
                        <div>
                            <div>
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="fieldName"><asp:Localize runat="server" Text="<%$ Resources: Resource, ManagedUser %>"/></td>
                                        <td>
                                            <asp:Label ID="ManagedArtistLabel" runat="server" Text="Label"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
				        <fieldset class="register">
				            <legend><asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, DocumentInformation %>"/></legend>
                            <h2><asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, TrackInformation %>"></asp:Localize></h2>
                            <div class="formContainer">
                                <div class="formField">
                                    <div class="formFieldName"><asp:Label ID="Label4" runat="server" Text="<%$ Resources: Resource, TrackName %>" CssClass="fieldNameLabel"/></div>
                                    <div class="formFieldEntry">
                                        <asp:TextBox runat="server" ID="TrackNameText" CssClass="registerDocCtl"/>
                                        <span id="Span1" runat="server" class="info" title="<%$ Resources: Resource, InfoTitle %>">?</span>
                                    </div>
                                </div>
                                <div class="formField">
                                    <div class="formFieldName"><asp:Label ID="Label1" runat="server" Text="<%$ Resources: Resource, IsrcCode %>" CssClass="fieldNameLabel"/></div>
                                    <div class="formFieldEntry">
                                        <div class="fieldIsrcHandle"><asp:Label runat="server" ID="IsrcHandle" Text="ISRC - - -" CssClass="isrcHandle" Width="140px"/></div>
                                        <div class="fieldIsrcPostfix"><asp:TextBox runat="server" ID="IsrcPostfix" CssClass="textEntryExtraSmall"/></div>
                                    </div>
                                </div>
                            </div>
                            <h2><asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, CooperatingArtists %>"></asp:Localize></h2>
                            <div class="coArtistContainer">
                                <div class="coArtistName"> 
                                    <div class="coArtistNameHeader"><asp:Label ID="Label2" runat="server" Text="<%$ Resources: Resource, CoArtists %>" CssClass="fieldNameLabel" AssociatedControlID="CoArtistDropDown"/></div>
                                    <div class="coArtistNameControl"><asp:DropDownList ID="CoArtistDropDown" runat="server" CssClass="registerDocCtl"/></div>
                                </div>
                                <div class="coArtistRole">
                                    <div class="coArtistRoleHeader"><asp:Label ID="Label3" runat="server" Text="<%$ Resources: Resource, Role %>" CssClass="fieldNameLabel" AssociatedControlID="CoArtistRole"/></div>
                                    <div class="coArtistRoleControl"><asp:TextBox runat="server" ID="CoArtistRole" CssClass="registerDocCtl"></asp:TextBox></div>
                                </div>
                                <div class="coArtistSubmit">
                                    <div class="coArtistSubmitControl"><asp:Button runat="server" ID="AddCoArtistButton" Text="+" Height="24px" Width="24px" OnCommand="AddCoArtist"/></div>
                                </div>
                                <asp:ListView ID="CoArtistsList" runat="server" 
                                    onitemdeleting="CoArtistsList_ItemDeleting">
                                    <LayoutTemplate>
                                        <ul><asp:PlaceHolder runat="server" ID="itemPlaceHolder"/></ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <span class="name"><%# Eval("name") %></span>
                                            <span class="role"><%# Eval("role") %></span>
                                            <span class="clientid"><asp:HiddenField runat="server" ID="ArtistClientId" Value='<%# Bind("clientid") %>' ></asp:HiddenField></span>
                                            <span class="btndelete"><asp:ImageButton ID="DeleteButton" runat="server" AlternateText="Delete" ImageUrl="~/Images/remove-user.png" CommandName="Delete" /></span>
                                        </li>
                                    </ItemTemplate>
                                </asp:ListView>
                            </div>
                            <h2><asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, Files %>"></asp:Localize></h2>
                            <div class="formField">
                                <div class="formFieldName"><asp:Label ID="Document1Label" runat="server" AssociatedControlID="FileUpload1" Text="<%$ Resources: Resource, YourTrack %>" CssClass="fieldNameLabel"/></div>
                                <div class="formFieldEntry"><asp:FileUpload runat="server" ID="FileUpload1" CssClass="fileUpload"/></div>
                            </div>
                            <div class="formField">
                                <div class="formFieldName"><asp:Label ID="Document2Label" runat="server" AssociatedControlID="FileUpload2" Text="<%$ Resources: Resource, Lyrics %>" CssClass="fieldNameLabel"/></div>
                                <div class="formFieldEntry"><asp:FileUpload runat="server" ID="FileUpload2" CssClass="fileUpload"/></div>
                            </div>
                            <div class="formField">
                                <div class="formFieldName"><asp:Label ID="Document3Label" runat="server" AssociatedControlID="FileUpload3" Text="<%$ Resources: Resource, SheetMusic %>" CssClass="fieldNameLabel" /></div>
                                <div class="formFieldEntry"><asp:FileUpload runat="server" ID="FileUpload3" CssClass="fileUpload"/></div>
                            </div>
                            <div class="errorMessage"><asp:Label runat="server" ID="ErrorMessage" ForeColor="Red"></asp:Label></div>
				        </fieldset>
                        </div>
				        <!--<p class="submitButton">-->
                        <div style="margin-top: 24px; margin-left: 16px; margin-right: 16px; text-align: center; width: 100%;">
				            <asp:ImageButton ID="RegisterDocumentButton" runat="server" AlternateText="OK" 
									ImageUrl="<%$ Resources: Resource, imgUploadTracks %>"
				                    ValidationGroup="RegisterDocumentValidationGroup" oncommand="SubmitButton_Command" />
                        </div>
				        <!--</p>-->
				        <p>
				        <asp:Literal ID="StatusInfo1" runat="server" /><br />
				        <asp:Literal ID="StatusInfo2" runat="server" /><br />
				        <asp:Literal ID="StatusInfo3" runat="server" /><br />
				        <asp:Literal ID="StatusInfo4" runat="server" />
				        </p>
                    </asp:Panel>
				    </div>
				</div>
			</td>

			<td class="centerColumn">
				<div class="centerDivide">
					<asp:Image id="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />
				</div>
			</td>
			
			<td class="rightColumn">
				<div class="divRightColumn">
                    <div class="divRightContent">
                        <div class="statusPanel">
                            <div class="statusPanelTitle"><asp:Localize runat="server" ID="LoggedOnTitle"/></div>
                            <div class="statusPanelUserName"><asp:Literal runat="server" ID="LoggedOnUserName"/></div>
                            <div class="statusPanelCredits"><asp:Literal runat="server" ID="CreditsLiteral"/></div>
                            <div class="statusPanelProtected"><asp:Literal runat="server" ID="ProtectedLiteral"/></div>
                            <div class="statusPanelCompleted"><asp:Literal runat="server" ID="CompletedLiteral"/></div>
                            <div class="statusPanelLink"><asp:Literal ID="ClickToLinkLiteral" runat="server" Text="<%$ Resources: Resource, ClickToEdit %>"/></div>
                        </div>
                    </div>
					<br/><br/>
					<div class="divRightContent">
					    <asp:Literal runat="server" ID="RegisterDocumentInc"/>
					</div>
					<br/><br/><br/><br/>
				    <asp:Literal runat="server" ID="RhosMovementInc"/>
				</div>
			</td>
		</tr>
	</table>
    </div>--%>
    <div style="display: none">
        <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, RegisterDocument %>" />
        <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, DocumentInformation %>" />
        <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, TrackInformation %>"></asp:Localize>
        name <span id="Span1" runat="server" class="info" title="<%$ Resources: Resource, InfoTitle %>">
            ?</span>
        <asp:Label runat="server" ID="ErrorMessage" ForeColor="Red"></asp:Label>
        <asp:Literal runat="server" ID="litDialogText" />
        <asp:Label runat="server" ID="lblSoundCloudID" Text="[soundcloudid]" /><asp:HyperLink
            runat="server" ID="linkSoundCloud" ImageUrl="~/Images/SoundCloud-Button.png"
            NavigateUrl="javascript:soundCloudAuthorize();" Visible="False" />
        <span class="socialidentity">
            <asp:Label runat="server" ID="lblFacebookID" Text="[facebookid]" /></span> <span
                class="socialaccounts">
                <asp:DropDownList runat="server" ID="ddlFacebookAccounts" />
            </span><span class="socialconnect">
                <asp:HyperLink runat="server" ID="linkFacebook" ImageUrl="~/Images/Facebook-Button.png"
                    NavigateUrl="javascript:facebookAuthorize();" Visible="False" />
            </span><span class="socialidentity">
                <asp:Label runat="server" ID="lblTwitterID" Text="[twitterid]" /></span>
        <span class="socialaccounts">&nbsp;</span> <span class="socialconnect" />
        <asp:HyperLink runat="server" ID="linkTwitter" ImageUrl="~/Images/Twitter-Button.png"
            NavigateUrl="~/Social/twconnect.aspx" Visible="False" />
        <asp:Literal runat="server" ID="liMessage" />
        <asp:Literal ID="StatusInfo1" runat="server" /><br />
        <asp:Literal ID="StatusInfo2" runat="server" /><br />
        <asp:Literal ID="StatusInfo3" runat="server" /><br />
        <asp:Literal ID="StatusInfo4" runat="server" />
        <asp:Image ID="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />
        <asp:Literal runat="server" ID="RegisterDocumentInc" />
        <asp:Literal runat="server" ID="RhosMovementInc" />
    </div>
    <div id="contropanel" class="row">
        <div class="large-12 columns" style="padding-bottom: 50px;">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources : Resource, ProtectTrackText %>"></asp:Localize></h1>
            </div>
            <div class="row">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                    <a class="button extra-large expand control_p_btn" href="memberhome.aspx"><i class="arrow-left">
                    </i>
                        <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></a>
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
                                <a href="../Account/ChangePassword.aspx" class="box small-12 columns"><asp:Localize ID="Localize13" runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a>
                              </div>
                              <div id="divAccPerCompleted" runat="server" class="row collapse border box small-12 columns">
                                <a href="Profile.aspx" class=""><asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral" Text="<%$ Resources: Resource, ClickToEdit %>" /></a>
                              </div>
                            </section>
                        <footer>

                                <a class="button extra-large expand border" href="Subscription.aspx?pid=4&country=NL&price=149,0000">upgrade plan</a>
                                <a class="button extra-large expand" href="SelectProduct.aspx"><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources : Resource, GetMore %>"></asp:Literal></a>
                            </footer>
                    </div>
                </div>
                <!-- End Right Column -->
                <!-- Main Content -->
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            <asp:Localize runat="server" Text="<%$ Resources : Resource, ProtectTrackText %>"></asp:Localize></h1>
                    </div>
                    <div class="row">
                        <!-- Just for padding -->
                        <div class="large-11 columns">
                            <!-- Protect Track Form -->
                            <form id="protect-form" method="post" class="custom" enctype="multipart/form-data"
                            action="#">
                            <asp:ValidationSummary ID="ValidationSummary" ShowMessageBox="false" ShowSummary="true"
                                CssClass="failureNotification" EnableClientScript="true" ValidationGroup="TrackProtectValidation"
                                runat="server" DisplayMode="BulletList" BorderColor="#75B891" BorderWidth="1"
                                EnableTheming="true" />
                            <label>
                                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, ManagedUser %>" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Display="Dynamic" runat="server"
                                    ErrorMessage="<%$ Resources : Resource, ArtistSelectionValidation %>" Text="*"
                                    ControlToValidate="ddlManagedArtist" InitialValue="0" ValidationGroup="TrackProtectValidation"></asp:RequiredFieldValidator></label><asp:UpdatePanel
                                        runat="server" ID="updPnlManagedArtist">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="ddlManagedArtist" ValidationGroup="TrackProtectValidation"
                                                runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlManagedArtist_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <label>
                                                <asp:Label ID="Label6" runat="server" Text="<%$ Resources : Resource, StageName %>"></asp:Label><asp:RequiredFieldValidator
                                                    ID="RequiredFieldValidator4" runat="server" Text="*" ErrorMessage="<%$ Resources : Resource, StageNameRequired %>"
                                                    ValidationGroup="TrackProtectValidation" ControlToValidate="StageNameText" Display="Dynamic"></asp:RequiredFieldValidator></label><asp:TextBox
                                                        runat="server" ID="StageNameText"></asp:TextBox><label><asp:Label ID="Label4" runat="server"
                                                            Text="<%$ Resources: Resource, TrackName %>" />
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Text="*"
                                                                ErrorMessage="<%$ Resources : Resource, TrackNameIsRequired %>" ControlToValidate="TrackNameText"
                                                                Display="Dynamic" ValidationGroup="TrackProtectValidation"></asp:RequiredFieldValidator></label><asp:TextBox
                                                                    runat="server" ID="TrackNameText" />
                                            <label>
                                                <asp:Label runat="server" ID="IsrcHandle" Text="ISRC Handle" Width="140px" />
                                            </label>
                                            <asp:TextBox runat="server" ID="IsrcPostfix" />
                                            <br />
                                            <div>
                                                <label>
                                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, AssistantArtist %>"></asp:Localize></label></div>
                                            <br />
                                            <div class="row">
                                                <div class="large-6 columns">
                                                    <asp:Label CssClass="sub" ID="Label2" runat="server" Text="<%$ Resources: Resource, CoArtists %>"
                                                        AssociatedControlID="CoArtistDropDown" />
                                                    <asp:DropDownList ID="CoArtistDropDown" runat="server" CssClass="custom dropdown" />
                                                </div>
                                                <div class="large-6 columns">
                                                    <asp:Label ID="Label3" CssClass="sub" runat="server" Text="<%$ Resources: Resource, Role %>"
                                                        AssociatedControlID="CoArtistRole" />
                                                    <div class="row">
                                                        <div class="small-10 columns no-left-padding">
                                                            <asp:TextBox runat="server" ID="CoArtistRole" CssClass="registerDocCtl"></asp:TextBox></div>
                                                        <div class="small-2 columns">
                                                            <%--  <a href="#" class="button postfix expand add-button">--%>
                                                            <asp:Button runat="server" ID="AddCoArtistButton" CssClass="button postfix expand add-button"
                                                                Text="+" Height="42px" CausesValidation="false" Width="42px" OnCommand="AddCoArtist" /><%--</a>--%>
                                                        </div>
                                                    </div>
                                                    <asp:ListView ID="CoArtistsList" runat="server" OnItemDeleting="CoArtistsList_ItemDeleting">
                                                        <LayoutTemplate>
                                                            <ul>
                                                                <asp:PlaceHolder runat="server" ID="itemPlaceHolder" />
                                                            </ul>
                                                        </LayoutTemplate>
                                                        <ItemTemplate>
                                                            <li><span class="name">
                                                                <%# Eval("name") %></span> <span class="role">
                                                                    <%# Eval("role") %></span> <span class="clientid">
                                                                        <asp:HiddenField runat="server" ID="ArtistClientId" Value='<%# Bind("clientid") %>'>
                                                                        </asp:HiddenField>
                                                                    </span><span class="btndelete">
                                                                        <asp:ImageButton ID="DeleteButton" runat="server" AlternateText="Delete" ImageUrl=""
                                                                            CommandName="Delete" CssClass="button small" CausesValidation="false" /></span>
                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:ListView>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="RegisterDocumentButton" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="updPnlManagedArtist">
                                <ProgressTemplate>
                                    <div style="position: absolute; z-index: 5;" align="center" id="PB">
                                        <asp:Panel ID="Panel2" runat="server" CssClass="pnloverlay_template">
                                            <img alt="" src="../Images/ajax-loader.gif" style="margin-top: 270px;" />
                                        </asp:Panel>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <div class="row-title">
                                <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, Files %>"></asp:Localize></div>
                            <%--File upload 1--%><div class="row">
                                <div class="large-5 columns">
                                    <asp:Label ID="Document1Label" CssClass="sub" runat="server" AssociatedControlID="FileUpload1"
                                        Text="<%$ Resources: Resource, YourTrack %>" />
                                </div>
                                <div class="large-7 columns track-upload">
                                    <asp:FileUpload runat="server" ID="FileUpload1" Style="display: none;" />
                                    <div class="row collapse">
                                        <div class="large-5 small-6 columns">
                                            <span class="prefix file-upload">
                                                <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources : Resource, FILESELECT %>"></asp:Localize></span></div>
                                        <div class="large-7 small-6 columns">
                                            <span class="postfix">
                                                <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources : Resource, Nofilechosen %>"></asp:Localize></span></div>
                                    </div>
                                </div>
                            </div>
                            <%--File1--%><div class="validator">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" Display="Dynamic"
                                    runat="server" Text="*" ErrorMessage="<%$Resources : Resource, SelectMp3File %>"
                                    ValidationExpression="^.+(.mp3|.MP3)$" ControlToValidate="FileUpload1" ValidationGroup="TrackProtectValidation"></asp:RegularExpressionValidator><%--<asp:RequiredFieldValidator Text="*" runat="server" ErrorMessage="<%$Resources : Resource, SelectMp3File %>"
                                        ControlToValidate="FileUpload1" ValidationGroup="TrackProtectValidation">
                               </asp:RequiredFieldValidator>--%><%-- <asp:CustomValidator Text="*" EnableClientScript="true" ID="checkFileSize" runat="server" ControlToValidate="FileUpload1"
                                    ValidationGroup="TrackProtectValidation" ErrorMessage="<%$ Resources : Resource, FileSizeExceed %>"
                                    Display="Dynamic" OnServerValidate="checkFileSize_Validate"></asp:CustomValidator>--%></div>
                            <%--File upload 2--%><div class="row">
                                <div class="large-5 columns">
                                    <asp:Label ID="Document2Label" CssClass="sub" runat="server" AssociatedControlID="FileUpload2"
                                        Text="<%$ Resources: Resource, Lyrics %>"></asp:Label></div>
                                <div class="large-7 columns track-upload">
                                    <%--<input type="file" name="lyric" id="lyric-file" >--%>
                                    <asp:FileUpload runat="server" ID="FileUpload2" Style="display: none;" />
                                    <div class="row collapse">
                                        <div class="large-5 small-6 columns">
                                            <span class="prefix file-upload">
                                                <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources : Resource, FILESELECT %>"></asp:Localize></span></div>
                                        <div class="large-7 small-6 columns">
                                            <span class="postfix">
                                                <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources : Resource, Nofilechosen %>"></asp:Localize></span></div>
                                    </div>
                                </div>
                            </div>
                            <div class="validator">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" Display="Dynamic"
                                    Text="*" runat="server" ErrorMessage="<%$ Resources : Resource, FileNotAllowed %>"
                                    ValidationExpression="^.+(.doc|.docx|.DOC|.DOCX|.txt|.TXT|.PDF|.pdf)$" ControlToValidate="FileUpload2"></asp:RegularExpressionValidator></div>
                            <%--File upload 3--%><div class="row">
                                <div class="large-5 columns">
                                    <asp:Label ID="Document3Label" runat="server" CssClass="sub" AssociatedControlID="FileUpload3"
                                        Text="<%$ Resources: Resource, SheetMusic %>" />
                                </div>
                                <div class="large-7 columns track-upload">
                                    <%--<input type="file" name="lyric" id="lyric-file" style="display: none;">--%>
                                    <asp:FileUpload runat="server" ID="FileUpload3" Style="display: none" />
                                    <div class="row collapse">
                                        <div class="large-5 small-6 columns">
                                            <span class="prefix file-upload">
                                                <asp:Localize ID="Localize12" runat="server" Text="<%$ Resources : Resource, FILESELECT %>"></asp:Localize></span></div>
                                        <div class="large-7 small-6 columns">
                                            <span class="postfix">
                                                <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources : Resource, Nofilechosen %>"></asp:Localize></span></div>
                                    </div>
                                </div>
                            </div>
                            <div class="validator">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" Display="Dynamic"
                                    Text="*" runat="server" ErrorMessage="<%$ Resources : Resource, FileNotAllowed %>"
                                    ValidationExpression="^.+(.doc|.docx|.DOC|.DOCX|.txt|.TXT|.PDF|.pdf)$" ControlToValidate="FileUpload3"></asp:RegularExpressionValidator></div>
                            <br />
                            <asp:UpdatePanel runat="server" ID="updPnlDefault">
                                <ContentTemplate>
                                    <!-- Tagging Section -->
                                    <div>
                                        <label>
                                            <asp:Localize runat="server" Text="<%$ Resources : Resource, tagging %>"></asp:Localize></label></div>
                                    <br />
                                    <div class="row">
                                        <!-- Major Genre Selection Row -->
                                        <div class="large-3 columns">
                                            <label class="sub">
                                                <asp:Localize runat="server" Text="<% $ Resources : Resource, GenreMax3 %>"></asp:Localize></label></div>
                                        <div class="large-3 columns">
                                            <asp:DropDownList runat="server" AutoPostBack="true" ID="Genre1" CssClass="custom dropdown"
                                                OnSelectedIndexChanged="Genre1_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="large-3 columns">
                                            <asp:DropDownList runat="server" AutoPostBack="true" ID="Genre2" CssClass="custom dropdown"
                                                OnSelectedIndexChanged="Genre2_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="large-3 columns">
                                            <asp:DropDownList runat="server" AutoPostBack="true" ID="Genre3" CssClass="custom dropdown"
                                                OnSelectedIndexChanged="Genre3_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <div data-tooltip class="has-tip tip-right icon-help" title="Please select the major genres for the track.">
                                                ?</div>
                                        </div>
                                    </div>
                                    <!-- End Major Genre Selection Row-->
                                    <div class="row">
                                        <!-- Sub Genre Selection Row -->
                                        <div class="large-3 columns">
                                            <label class="sub">
                                                <asp:Localize runat="server" Text="<%$ Resources : Resource, SubGenreMax3  %>"></asp:Localize></label></div>
                                        <div class="large-3 columns">
                                            <!-- 
                                            This selection box should be dynamically filled after the user selected
                                            a major genre. -->
                                            <asp:DropDownList runat="server" AutoPostBack="true" ID="SubGenre1" CssClass="custom dropdown"
                                                OnSelectedIndexChanged="SubGenre1_Selectionchanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="large-3 columns">
                                            <!-- 
                                            This selection box should be dynamically filled after the user selected
                                            a major genre. -->
                                            <asp:DropDownList runat="server" AutoPostBack="true" ID="SubGenre2" CssClass="custom dropdown"
                                                OnSelectedIndexChanged="SubGenre2_Selectionchanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="large-3 columns">
                                            <!-- 
                                            This selection box should be dynamically filled after the user selected
                                            a major genre. -->
                                            <asp:DropDownList runat="server" AutoPostBack="true" ID="SubGenre3" CssClass="custom dropdown"
                                                OnSelectedIndexChanged="SubGenre3_Selectionchanged">
                                            </asp:DropDownList>
                                            <div data-tooltip class="has-tip tip-right icon-help" title="Please select the sub genres for the track.">
                                                ?</div>
                                        </div>
                                    </div>
                                    <!-- End Sub Genre Selection Row -->
                                    <!-- Tag Selection Rows -->
                                    <div class="row">
                                        <div class="large-6 columns">
                                            <label class="sub">
                                                <asp:Localize runat="server" Text="<%$ Resources : Resource, SoundsLikeTagsMax3 %>"></asp:Localize></label></div>
                                        <div class="large-6 columns">
                                            <!-- Selections will also be pulled from the database -->
                                            <asp:DropDownList ID="Tag1" runat="server" CssClass="custom dropdown" AutoPostBack="true"
                                                OnSelectedIndexChanged="Tag1_SelectionChanged">
                                                <asp:ListItem>--Select--</asp:ListItem>
                                                <asp:ListItem Value="1">La Roux</asp:ListItem>
                                                <asp:ListItem Value="2">Radiohead</asp:ListItem>
                                                <asp:ListItem Value="3">U2</asp:ListItem>
                                            </asp:DropDownList>
                                            <div data-tooltip class="has-tip tip-right icon-help" title="Please select similar musicians.">
                                                ?</div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-6 large-offset-6 columns">
                                            <!-- Selections will also be pulled from the database -->
                                            <asp:DropDownList ID="Tag2" runat="server" CssClass="custom dropdown" AutoPostBack="true"
                                                OnSelectedIndexChanged="Tag2_SelectionChanged">
                                                <asp:ListItem>--Select--</asp:ListItem>
                                                <asp:ListItem Value="1">La Roux</asp:ListItem>
                                                <asp:ListItem Value="2">Radiohead</asp:ListItem>
                                                <asp:ListItem Value="3">U2</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-6 large-offset-6 columns">
                                            <!-- Selections will also be pulled from the database -->
                                            <asp:DropDownList ID="Tag3" runat="server" CssClass="custom dropdown" AutoPostBack="true"
                                                OnSelectedIndexChanged="Tag3_SelectionChanged">
                                                <asp:ListItem>--Select--</asp:ListItem>
                                                <asp:ListItem Value="1">La Roux</asp:ListItem>
                                                <asp:ListItem Value="2">Radiohead</asp:ListItem>
                                                <asp:ListItem Value="3">U2</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <!-- End Tag Selection Rows -->
                                    <!-- End Tagging Section -->
                                    <!-- Social Sharing -->
                                    <div>
                                        <label>
                                            <asp:Localize runat="server" Text="<%$ Resources : Resource,Connections %>"></asp:Localize></label></div>
                                    <div class="row">
                                        <div class="large-4 columns">
                                            <label class="checkbox-label" for="ctl00_MainContent_cbxSendToSoundCloud">
                                                <asp:CheckBox runat="server" ID="cbxSendToSoundCloud" /><span class="custom checkbox"></span>
                                                Soundcloud
                                            </label>
                                        </div>
                                        <div class="large-8 columns">
                                            <asp:TextBox ID="SoundCloudMsg" MaxLength="140" runat="server" TextMode="MultiLine"></asp:TextBox><asp:CustomValidator
                                                ID="SoundCloudMaxLengthValidator" Text="*" ErrorMessage="<%$ Resources : Resource, SoundcloudMaxLengthValidation %>"
                                                ValidationGroup="TrackProtectValidation" ControlToValidate="SoundCloudMsg" runat="server"
                                                ClientValidationFunction="checklengthSoundCloud" EnableClientScript="true"></asp:CustomValidator></div>
                                    </div>
                                    <div class="row">
                                        <div class="large-4 columns">
                                            <label class="checkbox-label" for="ctl00_MainContent_cbxSendToFacebook">
                                                <asp:CheckBox runat="server" ID="cbxSendToFacebook" /><span class="custom checkbox"></span>
                                                Facebook
                                            </label>
                                            &nbsp;
                                            <label class="checkbox-label" for="ctl00_MainContent_cbxSendToGenreCommunityPage">
                                                <asp:CheckBox Visible="false" runat="server" ID="cbxSendToGenreCommunityPage" /><span
                                                    runat="server" id="cbxGenreCross" visible="false" class="custom checkbox"></span><asp:Label
                                                        runat="server" Font-Italic="true" ID="CommunityGenrePageLabel" Visible="false"></asp:Label></label>&nbsp;
                                            <label class="checkbox-label" for="ctl00_MainContent_cbxSendToSubGenreCommunityPage">
                                                <asp:CheckBox Visible="false" runat="server" ID="cbxSendToSubGenreCommunityPage" /><span
                                                    runat="server" id="cbxSubGenreCross" visible="false" class="custom checkbox"></span><asp:Label
                                                        runat="server" Font-Italic="true" ID="CommunitySubGenrePageLabel" Visible="false"></asp:Label></label></div>
                                        <div class="large-8 columns">
                                            <asp:TextBox ID="FacebookMsg" MaxLength="140" runat="server" TextMode="MultiLine"></asp:TextBox><asp:CustomValidator
                                                ID="FacebookMaxLengthValidator" runat="server" Text="*" ErrorMessage="<%$ Resources : Resource, FacebookMaxLengthValidation %>"
                                                ControlToValidate="FacebookMsg" ClientValidationFunction="checklengthFacebook"
                                                EnableClientScript="true" ValidationGroup="TrackProtectValidation"></asp:CustomValidator></div>
                                    </div>
                                    <div id="divFBSharing" runat="server" style="display: none;">
                                        <div id="divShareToUserWall" runat="server" class="row">
                                            <div class="large-4 columns">
                                            </div>
                                            <div class="large-8 columns">
                                                <label class="checkbox-label" for="<%=cbxShareToUserWall.ClientID %>">
                                                    <asp:CheckBox runat="server" ID="cbxShareToUserWall" />
                                                    <span class="custom checkbox"></span>&nbsp;<asp:Localize runat="server" ID="ShareToUserWall">
                                                    </asp:Localize></label><br />
                                            </div>
                                        </div>
                                        <div id="divShareToFriends" runat="server" class="row" style="display: none;">
                                            <div class="large-4 columns">
                                            </div>
                                            <div class="large-8 columns">
                                                <label class="checkbox-label" for="<%=cbxShareToFriends.ClientID %>">
                                                    <asp:CheckBox runat="server" ID="cbxShareToFriends" Checked="true" />
                                                    <span class="custom checkbox"></span>&nbsp;<asp:Localize runat="server" ID="ShareToFriends"></asp:Localize></label><br />
                                                <div id="divFriendList" style="height: 190px; width: 378px; overflow-y: scroll; border: 1px solid #cc3;">
                                                    <asp:DataList ID="dlFriendList" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                                                        BorderStyle="None" Width="100%">
                                                        <ItemTemplate>
                                                            <div>
                                                                <image height="20px" width="20px" src="<%# Eval("ProfileImg") %>" alt="img"></image>
                                                                &nbsp;
                                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Name") %>'></asp:Label>&nbsp;
                                                                <label class="checkbox-label" style="width: 40px; float: right;" for='<%=cbxSendToSoundCloud.ClientID %>'>
                                                                    <asp:CheckBox ID="cbxfriendID" runat="server" /><span class="custom checkbox"></span>
                                                                </label>
                                                                <asp:HiddenField ID="hfFriendId" runat="server" Value='<%# Eval("FacebookId") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:DataList>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="divShareToPages" runat="server" class="row">
                                            <div class="large-4 columns">
                                            </div>
                                            <div class="large-8 columns">
                                                <label class="checkbox-label" for="<%=cbxShareToPages.ClientID %>">
                                                    <asp:CheckBox runat="server" ID="cbxShareToPages" Checked="true" />
                                                    <span class="custom checkbox"></span>&nbsp;<asp:Localize runat="server" ID="ShareToPages"></asp:Localize></label><br />
                                                <div id="divPageList" style="height: 190px; width: 378px; overflow-y: scroll; border: 1px solid #cc3;">
                                                    <asp:DataList ID="dlPageList" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                                                        BorderStyle="None" Width="100%">
                                                        <ItemTemplate>
                                                            <div>
                                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("PageName") %>'></asp:Label>&nbsp;
                                                                <label class="checkbox-label" style="width: 40px; float: right;" for='<%=cbxSendToSoundCloud.ClientID %>'>
                                                                    <asp:CheckBox ID="cbxPageID" runat="server" /><span class="custom checkbox"></span>
                                                                </label>
                                                                <asp:HiddenField ID="hfPageId" runat="server" Value='<%# Eval("PageID") %>' />
                                                                <asp:HiddenField ID="hfPageAccessToken" runat="server" Value='<%# Eval("AccessToken") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:DataList>
                                                </div>
                                            </div>
                                        </div>
                                        <br />
                                        <div id="divShareToMyWall" runat="server" class="row">
                                            <div class="large-4 columns">
                                            </div>
                                            <div class="large-8 columns">
                                                <label class="checkbox-label" for="<%=cbxShareToMyWall.ClientID %>">
                                                    <asp:CheckBox runat="server" ID="cbxShareToMyWall" />
                                                    <span class="custom checkbox"></span>&nbsp;<asp:Localize runat="server" ID="ShareToMyWall"></asp:Localize></label><br />
                                            </div>
                                        </div>
                                        <div id="divShareToOwnFriends" runat="server" class="row" style="display: none;">
                                            <div class="large-4 columns">
                                            </div>
                                            <div class="large-8 columns">
                                                <label class="checkbox-label" for="<%=cbxShareToOwnFriends.ClientID %>">
                                                    <asp:CheckBox runat="server" ID="cbxShareToOwnFriends" Checked="true" />
                                                    <span class="custom checkbox"></span>&nbsp;<asp:Localize runat="server" ID="ShareToMyFriends"></asp:Localize></label><br />
                                                <div id="divOwnFriendList" style="height: 190px; width: 378px; overflow-y: scroll;
                                                    border: 1px solid #cc3;">
                                                    <asp:DataList ID="dlOwnFriendList" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                                                        BorderStyle="None" Width="100%">
                                                        <ItemTemplate>
                                                            <div>
                                                                <image height="20px" width="20px" src="<%# Eval("ProfileImg") %>" alt="img"></image>
                                                                &nbsp;
                                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Name") %>'></asp:Label>&nbsp;
                                                                <label class="checkbox-label" style="width: 40px; float: right;" for='<%=cbxSendToSoundCloud.ClientID %>'>
                                                                    <asp:CheckBox ID="cbxfriendID" runat="server" /><span class="custom checkbox"></span>
                                                                </label>
                                                                <asp:HiddenField ID="hfFriendId" runat="server" Value='<%# Eval("FacebookId") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:DataList>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="divShareToOwnPages" runat="server" class="row">
                                            <div class="large-4 columns">
                                            </div>
                                            <div class="large-8 columns">
                                                <label class="checkbox-label" for="<%=cbxShareToOwnPages.ClientID %>">
                                                    <asp:CheckBox runat="server" ID="cbxShareToOwnPages" Checked="true" />
                                                    <span class="custom checkbox"></span>&nbsp;<asp:Localize runat="server" ID="ShareToMyPages"></asp:Localize></label><br />
                                                <div id="divOwnPageList" style="height: 190px; width: 378px; overflow-y: scroll;
                                                    border: 1px solid #cc3;">
                                                    <asp:DataList ID="dlOwnPageList" runat="server" RepeatColumns="1" RepeatDirection="Horizontal"
                                                        BorderStyle="None" Width="100%">
                                                        <ItemTemplate>
                                                            <div>
                                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("PageName") %>'></asp:Label>&nbsp;
                                                                <label class="checkbox-label" style="width: 40px; float: right;" for='<%=cbxSendToSoundCloud.ClientID %>'>
                                                                    <asp:CheckBox ID="cbxPageID" runat="server" /><span class="custom checkbox"></span>
                                                                </label>
                                                                <asp:HiddenField ID="hfPageId" runat="server" Value='<%# Eval("PageID") %>' />
                                                                <asp:HiddenField ID="hfPageAccessToken" runat="server" Value='<%# Eval("AccessToken") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:DataList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlManagedArtist" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <br />
                            <asp:UpdatePanel runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="large-4 columns">
                                            <label class="checkbox-label" for="ctl00_MainContent_cbxSendToTwitter">
                                                <asp:CheckBox runat="server" ID="cbxSendToTwitter" /><span class="custom checkbox"></span>
                                                Twitter</label>
                                        </div>
                                        <div class="large-8 columns">
                                            <asp:TextBox ID="TwitterMsg" MaxLength="70" runat="server" TextMode="MultiLine" onKeyUp="Count(this,70);"
                                                onChange="Count(this,70);"></asp:TextBox><div class="wordcount">
                                                    <asp:Label ID="lblRemainingLabel" Style="float: right;" runat="server"> characters remaining</asp:Label><span
                                                        id="lblWordRemainingCount" runat="server" readonly="true">70</span>
                                                </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlManagedArtist" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <!-- End Social Sharing -->
                            <asp:Button ID="RegisterDocumentButton" runat="server" CssClass="button small border right"
                                OnCommand="SubmitButton_Command" ValidationGroup="TrackProtectValidation" BorderWidth="1"
                                Text="<%$Resources : Resource, ProtectTrackText %>" />
                            </form>
                            <!-- End Protect Track Form -->
                        </div>
                    </div>
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
    <script type="text/javascript">
        $(document).ready(function ($) {
            $(".file-upload").bind("click", function (event) {
                event.preventDefault();
                $(this).closest('.track-upload').find('input[type=file]').click();
            });
            $('input[type=file]').bind('change', function () {
                var filename = $(this).val();
                if (filename) {
                    console.log($(this).closest('div').find('.postfix'));
                    $(this).closest('div').find('.postfix').text(filename);
                }
            }).change();
        });
    </script>
    <script type="text/javascript">

        $(document).foundation();
       
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="updPnlDefault">
        <ProgressTemplate>
            <div style="position: absolute; z-index: 5;" align="center" id="PB">
                <asp:Panel ID="Panel1" runat="server" CssClass="pnloverlay_template">
                    <img alt="" src="../Images/ajax-loader.gif" style="margin-top: 270px;" />
                </asp:Panel>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    </span></asp:Content>
