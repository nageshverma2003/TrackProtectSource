<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Quotation.aspx.cs" Inherits="TrackProtect.Member.Quotation" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_quotation">
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
        		<div class="quotation">
        		    <h1 class="headerLine"><asp:Localize runat="server" Text="<%$ Resources: Resource, Quotation %>"/></h1>
                    <div style="margin-top: 16px; margin-left: 16px; margin-right: 16px;">
                        <asp:Panel runat="server" DefaultButton="SubmitButton">
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="fieldName" style="width: 250px;"><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, QuotationHowMany %>"></asp:Localize></td>
                                    <td><asp:TextBox runat="server" ID="QuotationAmount"/></td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td><asp:Button runat="server" ID="SubmitButton" Text="<%$ Resources: Resource, RequestQuotation %>" OnCommand="SendQuotation"/></td>
                                </tr>
                                <tr>
                                    <td colspan="2"><asp:Label runat="server" ID="ErrorMessage" ForeColor="Red" /></td>
                                </tr>
                            </table>
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
					    <asp:Literal runat="server" ID="ProtectInc"/>
					</div>
					<br/><br/><br/><br/>
				    <asp:Literal runat="server" ID="RhosMovementInc"/>
				</div>
			</td>
		</tr>
	</table>
    </div>
</asp:Content>
