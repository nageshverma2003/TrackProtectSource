<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MemberEdit.aspx.cs" Inherits="TrackProtect.Member.MemberEdit" %>

<%@ Register assembly="Infragistics2.WebUI.WebDateChooser.v8.1, Version=8.1.20081.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" namespace="Infragistics.WebUI.WebSchedule" tagprefix="igsch" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
	<script language="javascript" type="text/javascript">
	    $(function () { $("#_ctl00_MainContent_Birthday").datepicker({ autoSize: true, showButtonPanel: false, changeMonth: true, changeYear: true, yearRange: "1900:2500" }); });
	</script>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_memberedit">
	<table width="100%">
		<tr valign="top">
			<td class="leftColumn">
			    <asp:HiddenField runat="server" ID="LanguageIndex" Value="-1"/>
                <div class="backtocontrolpanel">
                    <div class="backtocontrolpaneltext"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>"/></div>
                    <div class="backtocontrolpanelbutton">
                        <a href="/Member/MemberHome.aspx" class="backtocontrolpanellink">
                            <img alt="" src="/Images/cp_settings.png" class="backtocontrolpanelimage"/>
                        </a>
                    </div>
                </div>
				<div class="memberedit">
				    <h1 class="headerLine"><asp:Localize runat="server" Text="<%$ Resources: Resource, ModifyAccount %>"/></h1>
					<div style="margin-left: 16px;">
			                
					    <div class="failureNotification">
					        <span class="failureNotification">
					            <asp:Literal ID="ErrorMessage" runat="server"/>
					        </span>
					    </div>		
											
					    <div class="accountInfo">
					        <fieldset class="register">
					        <legend><asp:Localize runat="server" Text="<%$ Resources: Resource, AccountInformation %>" /></legend>
					            <table>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="FirstNameLabel" runat="server" CssClass="textLabel" AssociatedControlID="FirstName" Text="<%$ Resources: Resource, FirstName %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="FirstName" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="LastNameLabel" runat="server" CssClass="textLabel" AssociatedControlID="LastName" Text="<%$ Resources: Resource, LastName %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="LastName" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
                                    <tr>
                                        <td class="labelCell">
                                            <asp:Label runat="server" ID="GenderLabel" CssClass="textLabel" AssociatedControlID="Gender" Text="<%$ Resources: Resource, Gender %>"/>
                                        </td>
                                        <td colspan="3">
                                            <asp:RadioButtonList runat="server" ID="Gender" RepeatDirection="Horizontal">
                                                <asp:ListItem runat="server" Text="<%$ Resources: Resource, Male %>" Value="M"/>
                                                <asp:ListItem runat="server" Text="<%$ Resources: Resource, Female %>" Value="F"/>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="labelCell">
                                            <asp:Label runat="server" ID="BirthdayLabel" CssClass="textLabel" AssociatedControlID="Birthday" Text="<%$ Resources: Resource, Birthday %>"/>
                                        </td>
                                        <td colspan="3">
                                            <igsch:WebDateChooser ID="Birthday" runat="server" Height="16px" 
                                                MinDate="1850-01-01" Value="" NullDateLabel="0001-01-01">
                                                <CalendarLayout DropDownYearsNumber="150">
                                                    <CalendarStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                        Font-Strikeout="False" Font-Underline="False">
                                                    </CalendarStyle>
                                                </CalendarLayout>
                                            </igsch:WebDateChooser>
                                        </td>
                                    </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="AddressLine1Label" runat="server" CssClass="textLabel" AssociatedControlID="AddressLine1" Text="<%$ Resources: Resource, AddressLine1 %>"/>
					                    </td>
					                    <td colspan="3"><asp:TextBox ID="AddressLine1" runat="server" CssClass="textEntry"/></td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="AddressLine2Label" runat="server" CssClass="textLabel" AssociatedControlID="AddressLine2" Text="<%$ Resources: Resource, AddressLine2 %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="AddressLine2" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="ZipcodeLabel" runat="server" CssClass="textLabel" AssociatedControlID="Zipcode" Text="<%$ Resources: Resource, PostalCode %>"/>
					                    </td>
					                    <td style="width:70px">
					                        <asp:TextBox ID="Zipcode" runat="server" CssClass="textEntry" Width="65px" />
					                    </td>
					                    <td style="width:67px">
					                        <asp:Label id="CityLabel" runat="server" CssClass="textLabel" AssociatedControlID="City" Text="<%$ Resources: Resource, Residence %>"/>
					                    </td>
					                    <td>
					                        <asp:TextBox ID="City" runat="server" CssClass="textEntry2"/>
					                    </td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="StateLabel" runat="server" CssClass="textLabel" AssociatedControlID="State" Text="<%$ Resources: Resource, ProvinceState %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="State" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="CountryLabel" runat="server" CssClass="textLabel" AssociatedControlID="Country" Text="<%$ Resources: Resource, Country %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:DropDownList ID="Country" runat="server" ViewStateMode="Enabled" CssClass="ddlEntry"/>
					                    </td>
					                </tr>
                                    <!--
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="LanguageLabel" runat="server" CssClass="textLabel" AssociatedControlID="Language" Text="<%$ Resources: Resource, Language %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:DropDownList ID="Language" runat="server" ViewStateMode="Enabled" CssClass="ddlEntry" />
					                    </td>
					                </tr>
                                    -->
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="TelephoneLabel" runat="server" CssClass="textLabel" AssociatedControlID="Telephone" Text="<%$ Resources: Resource, Telephone %>" />
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="Telephone" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
                                    <!--
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="CellularLabel" runat="server" CssClass="textLabel" AssociatedControlID="Cellular" Text="<%$ Resources: Resource, Cellular %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="Cellular" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
                                    -->
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="OwnerKindLabel" runat="server" CssClass="textLabel" AssociatedControlID="OwnerKind" Text="<%$ Resources: Resource, Iam %>" />
					                    </td>
					                    <td colspan="3">
					                        <asp:DropDownList ID="OwnerKind" runat="server" ViewStateMode="Enabled" CssClass="ddlEntry">
					                            <asp:ListItem>Artist</asp:ListItem>
					                            <asp:ListItem>Agent</asp:ListItem>
					                            <asp:ListItem>Producer</asp:ListItem>
					                        </asp:DropDownList>
					                    </td>
					                </tr>
                                    <!--
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="AccountOwnerLabel" runat="server" CssClass="textLabel" AssociatedControlID="AccountOwner" Text="<%$ Resources: Resource, AccountOwner %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="AccountOwner" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
                                    -->
                                    <tr>
                                        <td class="labelCell">
                                            <asp:Label runat="server" ID="BumaIdLabel" CssClass="textLabel" AssociatedControlID="BumaID" Text="<%$ Resources: Resource, BumaRegNo %>"/>
                                        </td>
                                        <td colspan="3">
                                            <asp:TextBox runat="server" ID="BumaID" CssClass="textEntry"/>
                                        </td>
                                    </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="SenaCodeLabel" runat="server" CssClass="textLabel" AssociatedControlID="SenaCode" Text="<%$ Resources: Resource, SENARegNo %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="SenaCode" runat="server" CssClass="textEntry"/>
                                            <div class="linkSena1">
    											<asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" NavigateUrl="http://www.sena.nl/Makers"><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, SenaReadWhy %>"/></asp:HyperLink></div><div class="linkSena2">
                                                <asp:HyperLink ID="HyperLink2" runat="server" Target="_blank" NavigateUrl="http://www.sena.nl/Makers/Aanmelden"><asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, SenaRegister %>"/></asp:HyperLink></div></td></tr><tr>
					                    <td class="labelCell">
					                        <asp:Label id="IsrcCodeLabel" runat="server" CssClass="textLabel" AssociatedControlID="IsrcCode" Text="<%$ Resources: Resource, ISRCCode %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="IsrcCode" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="TwitterIDLabel" runat="server" CssClass="textLabel" AssociatedControlID="TwitterID" Text="<%$ Resources: Resource, TwitterID %>" />
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="TwitterID" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="FacebookIDLabel" runat="server" CssClass="textLabel" AssociatedControlID="FacebookID" Text="<%$ Resources: Resource, FacebookID %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="FacebookID" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
					                <tr>
					                    <td class="labelCell">
					                        <asp:Label id="SoundCloudIDLabel" runat="server" CssClass="textLabel" AssociatedControlID="SoundCloudID" Text="<%$ Resources: Resource, SoundCloud %>"/>
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="SoundCloudID" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>
					                <!--<tr>
					                    <td class="labelCell">
					                        <asp:Label id="SoniallIDLabel" runat="server" CssClass="textLabel" AssociatedControlID="SoniallID" Text="<%$ Resources: Resource, SoniallID %>" />
					                    </td>
					                    <td colspan="3">
					                        <asp:TextBox ID="SoniallID" runat="server" CssClass="textEntry"/>
					                    </td>
					                </tr>-->
					            </table>
					        </fieldset>
					                  <p class="submitButton">
					            <asp:ImageButton ID="ModifyUserButton" runat="server" OnClick="ModifyUserButtonClick"
					                    ImageUrl="<%$ Resources: Resource, imgAccept %>" ValidationGroup="RegisterUserValidationGroup"/>
					        </p>
					    </div>
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
					    <asp:Literal runat="server" ID="MemberHomeInc"/>
					</div>
					<br/><br/>
					<div class="divRightContent">
						<asp:HyperLink ID="UploadTracksButton" ImageUrl="<%$ Resources: Resource, imgUploadTracks %>" NavigateUrl="~/Member/RegisterDocument.aspx" runat="server"/>
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