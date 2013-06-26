<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"    CodeBehind="TrackProtect.Member.About.aspx.cs" Inherits="TrackProtect.Member.About" %><asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content><asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="member_about">	<table width="100%">		<tr valign="top">			<td class="leftColumn">        		<div class="about">				    <asp:Literal runat="server" ID="AboutInc"/>				</div>			</td>			<td class="centerColumn">				<div class="centerDivide">					<asp:Image id="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />				</div>			</td>						<td class="rightColumn">				<div class="divRightColumn">                    <div class="divRightContent">
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
					<div class="divRightContent">					    <asp:Literal runat="server" ID="ProtectInc"/>					</div>				    <asp:Literal runat="server" ID="RhosMovementInc"/>				</div>			</td>		</tr>	</table>    </div></asp:Content>