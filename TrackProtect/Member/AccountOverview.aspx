<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountOverview.aspx.cs" Inherits="TrackProtect.Member.AccountOverview" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="http://connect.soundcloud.com/sdk.js"></script>
    <script type="text/javascript" src="/Scripts/soundcloud.js"></script>
    <script type="text/javascript" src="/Scripts/facebook.js"></script>
    <style type="text/css">
        .style4
        {
            width: 197px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_accountoverview">
    <table width="100%">
		<tr valign="top">
			<td class="leftColumn">
                <div class="backtocontrolpanel">
                    <div class="backtocontrolpaneltext"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>"/></div>
                    <div class="backtocontrolpanelbutton">
                        <a href="/Member/MemberHome.aspx" class="backtocontrolpanellink">
                            <img src="/Images/cp_settings.png" class="backtocontrolpanelimage" alt="Back"/>
                        </a>
                    </div>
                </div>
				<div class="accountoverview">
	                <h1 class="headerLine"><asp:Localize ID="AccountOverviewLocal" runat="server" Text="<%$ Resources: Resource, AccountOverview %>"/></h1>
				    
			        <div style="margin-left: 16px; margin-top: 20px;">
    				    <table border="0" cellpadding="0" cellspacing="0">
					        <tr class="itemRow">
						        <td class="fieldName">
						            <span class="pageSubTitle" style="margin-left: 4px;"><asp:Localize ID="AccountInformationLocal" runat="server" Text="<%$ Resources: Resource, AccountInformation %>"/></span>
						            <asp:ImageButton runat="server" ID="PrintPage" ImageUrl="<%$ Resources: Resource, imgPrint %>" AlternateText="print" OnClientClick="window.print();" />
						        </td>
						        <td style="text-align: right;">
							        <asp:HyperLink id="ButtonEditAccount" runat="server" ImageUrl="<%$ Resources: Resource, imgEdit %>" NavigateUrl="../Member/MemberEdit.aspx?mode=edit"/>
						        </td>
					        </tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, Name %>"/></div></td><td><asp:Literal id="AccountNameLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
    				        <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize runat="server" Text="<%$ Resources: Resource, Gender %>"/></div></td><td><asp:Literal ID="Gender" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
    				        <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize runat="server" Text="<%$ Resources: Resource, Birthday %>"/></div></td><td><asp:Literal ID="Birthdate" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, Country %>"/></div></td><td><asp:Literal id="Address" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, Telephone %>"/></div></td><td><asp:Literal id="Telephone" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <!--<tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, Cellular %>"/></div></td><td><asp:Literal id="Cellular" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>-->

				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: Resource, Email %>"/></div></td><td><asp:Literal id="EmailLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
    				        <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize7" runat="server" Text="<%$ Resources: Resource, Iam %>"/></div></td><td><asp:Literal id="IamLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="locBumaRegNo" runat="server" Text="<%$ Resources: Resource, BumaRegNo %>"/></div></td><td><asp:Literal id="BumaCodeLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize11" runat="server" Text="<%$ Resources: Resource, SENARegNo %>"/></div></td><td><asp:Literal id="SenaCodeLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize12" runat="server" Text="<%$ Resources: Resource, ISRCCode %>"/></div></td><td><asp:Literal id="IsrcCodeLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: Resource, Twitter %>"/></div></td>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td><asp:Literal id="TwitterIdLabel" runat="server"/></td>
                                            <td width="120"><asp:HyperLink runat="server" ID="linkTwitter" ImageUrl="~/Images/Twitter-Button.png" NavigateUrl="~/Social/twconnect.aspx"/></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize10" runat="server" Text="<%$ Resources: Resource, Facebook %>"/></div></td>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td><asp:Literal id="FacebookIdLabel" runat="server"/><div id="fb-root"></div></td>
                                            <td width="120"><asp:HyperLink runat="server" ID="linkFacebook" ImageUrl="~/Images/Facebook-Button.png" NavigateUrl="javascript:facebookAuthorize();"/></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="locSoundCloudId" runat="server" Text="<%$ Resources: Resource, SoundCloud %>"/></div></td>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td><asp:Literal id="SoundCloudLabel" runat="server"/></td>
                                            <td width="120"><asp:HyperLink runat="server" ID="linkSoundCloud" ImageUrl="~/Images/SoundCloud-Button.png" NavigateUrl="javascript:soundCloudAuthorize();"/></td>
                                        </tr>
                                    </table>
                                </td>
				            </tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
				            <!--<tr class="itemRow"><td class="fieldName"><asp:Localize runat="server" Text="<%$ Resources: Resource, Soniall %>"/></td><td><asp:Literal id="SoniallIdLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>-->
				            <tr class="itemRow"><td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize13" runat="server" Text="<%$ Resources: Resource, MemberSince %>"/></div></td><td><asp:Literal id="MemberSinceLabel" runat="server"/></td></tr>
					        <tr><td colspan="2"><img alt="hs" src="../Images/hor_sep.png"/></td></tr>
                            <tr class="itemRow">
                                <td class="fieldName"><div class="fieldName"><asp:Localize ID="Localize15" runat="server" Text="<%$ Resources: Resource, Identification %>"/></div></td>
                                <td>
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td><asp:Literal runat="server" ID="IdentityCertificate"/></td>
                                            <td width="20px"><asp:HyperLink runat="server" ID="DownloadIdent" ImageUrl="<%$ Resources: Resource, imgDownload %>" Visible="false"/></td>
                                            <td align="right" width="1px"><asp:HyperLink runat="server" ID="UploadCredentialsButton" ImageUrl="<%$ Resources: Resource, imgUploadIdent %>" NavigateUrl="~/Member/UploadCredentials.aspx"></asp:HyperLink></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <a href="/Account/ChangePassword.aspx" class="changePassword">
                                        <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, ChangePassword %>"/>
                                    </a>
                                </td>
                            </tr>
				        </table>
				
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
					    <asp:Literal runat="server" ID="AccountOverviewInc"/>
					    <br/><br/><br/><br/>
					</div>
                    <br/><br/>
					<div class="divRightContent">
						<asp:HyperLink ID="SignupButton" ImageUrl="<%$ Resources: Resource, imgUploadTracks %>" NavigateUrl="~/Member/RegisterDocument.aspx" runat="server"/>
					</div>
                    <br/>
                    <div class="divRightContent">
                        <asp:HyperLink ID="BuyMoreButton" ImageUrl="<%$ Resources: Resource, imgBuyMore %>" NavigateUrl="~/Member/SelectProduct.aspx" runat="server" />
                    </div>
					<br/><br/><br/><br/>
				    <asp:Literal runat="server" ID="RhosMovementInc"/>
				</div>
			</td>
		</tr>
	</table>
    </div>
</asp:Content>
