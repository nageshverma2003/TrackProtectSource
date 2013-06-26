<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadCredentialResult.aspx.cs" Inherits="TrackProtect.Member.UploadCredentialResult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_uploadcredentialresult">	<table width="100%">		<tr valign="top">			<td class="leftColumn">                <div class="backtocontrolpanel">
                    <div class="backtocontrolpaneltext"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>"/></div>
                    <div class="backtocontrolpanelbutton">
                        <a href="/Member/MemberHome.aspx" class="backtocontrolpanellink">
                            <img src="/Images/cp_settings.png" class="backtocontrolpanelimage"/>
                        </a>
                    </div>
                </div>
        		<div class="uploadcredentialsresult">        		    <h1 class="headerLine"><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, UploadCredentialsResult %>" /></h1>                    <div style="margin-left: 12px;">
                        <asp:Literal runat="server" ID="result0"/>
                        <br/>
                        <asp:Literal runat="server" ID="result1"/>                        <br/>                        <asp:Literal runat="server" ID="result2"/>                    </div>				</div>			</td>			<td class="centerColumn">				<div class="centerDivide">					<asp:Image id="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />				</div>			</td>						<td class="rightColumn">				<div class="divRightColumn">                    <div class="divRightContent">
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
					<div class="divRightContent">					    <asp:Literal runat="server" ID="ProtectInc"/>					</div>					<br/><br/><br/><br/>				    <asp:Literal runat="server" ID="RhosMovementInc"/>				</div>			</td>		</tr>	</table>    </div></asp:Content>
