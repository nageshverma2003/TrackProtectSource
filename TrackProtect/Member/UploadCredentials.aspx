<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadCredentials.aspx.cs" Inherits="TrackProtect.Member.UploadCredentials" %><asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content><asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_uploadcredentials">	<table width="100%">		<tr valign="top">			<td class="leftColumn">                <div class="backtocontrolpanel">
                    <div class="backtocontrolpaneltext"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>"/></div>
                    <div class="backtocontrolpanelbutton">
                        <a href="/Member/MemberHome.aspx" class="backtocontrolpanellink">
                            <img src="/Images/cp_settings.png" class="backtocontrolpanelimage"/>
                        </a>
                    </div>
                </div>
        		<div class="uploadcredentials">        		    <h1 class="headerLine"><asp:Localize runat="server" Text="<%$ Resources: Resource, UploadCredentials %>" /></h1>                    <div style="margin-left: 12px;">                        <table width="100%">                            <tr>                                <td><asp:Localize runat="server" Text="<%$ Resources: Resource, UploadAccount %>" /></td>                                <td width="400px"><asp:FileUpload runat="server" ID="UploadFile1" Width="100%"/></td>                            </tr>                            <tr>                                <td><asp:Localize runat="server" Text="<%$ Resources: Resource, UploadIdentification %>"/></td>                                <td width="400px"><asp:FileUpload runat="server" ID="UploadFile2" Width="100%"/></td>                            </tr>                            <tr>                                <td>&nbsp;</td>                                <td><asp:Literal runat="server" ID="StatusInfo2"/></td>                            </tr>                            <tr>                                <td>&nbsp;</td>                                <td style="text-align: right;">                                    <asp:ImageButton runat="server"                                         ID="UploadCredentialsButton" ImageUrl="<%$ Resources: Resource, imgUpload %>"                                         oncommand="UploadCredentialsButton_Command"/>                                </td>                            </tr>                        </table>                    </div>				</div>			</td>			<td class="centerColumn">				<div class="centerDivide">					<asp:Image id="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />				</div>			</td>						<td class="rightColumn">				<div class="divRightColumn">                    <div class="divRightContent">
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
					<div class="divRightContent">					    <asp:Literal runat="server" ID="ProtectInc"/>					</div>                    <br/><br/>
				    <asp:Literal runat="server" ID="RhosMovementInc"/>				</div>			</td>		</tr>	</table>    </div></asp:Content>