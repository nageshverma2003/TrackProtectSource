 <%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuotationEdit.aspx.cs" Inherits="TrackProtect.Admin.QuotationEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin_quotationedit">	<table width="100%">		<tr valign="top">			<td class="leftColumn">        		<div class="quotationedit">
        		    <div>
        		        <div><asp:Label runat="server" Text="Credits"/></div>
        		        <div><asp:TextBox runat="server" ID="Credits" ReadOnly="True" /></div>
        		    </div>                    <div>
                        <div><asp:Label runat="server" Text="Amount"/></div>
                        <div><asp:TextBox runat="server" ID="Amount"/></div>
                    </div>                    <div>
                        <asp:Button runat="server" ID="Store" OnCommand="StoreQuotation" Text="<%$ Resources: Resource, btnAccept %>"/>
                    </div>                </div>			</td>			<td class="centerColumn">				<div class="centerDivide">					<asp:Image id="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />				</div>			</td>						<td class="rightColumn">				<div class="divRightColumn">                    <div class="divRightContent">
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
