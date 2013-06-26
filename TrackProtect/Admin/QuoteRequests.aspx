<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuoteRequests.aspx.cs" Inherits="TrackProtect.Admin.QuoteRequests" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin_quoterequests">
	<table width="100%">
		<tr valign="top">
			<td class="leftColumn">
        		<div class="quoterequests">
                    <asp:ListView id="quotationsList" runat="server" 
                        onitemediting="quotationsList_ItemEditing">
	                    <LayoutTemplate>
		                    <ul>
			                    <li id="itemPlaceholder" runat="server"></li>
		                    </ul>
	                    </LayoutTemplate>
	
	                    <ItemTemplate>
		                    <li id="quotationsListItem" runat="server" class="">
			                    <span class="date"><%# Eval("date", "{0:d}") %></span>
			                    <span class="name"><%# Eval("email") %></span>
			                    <span class="amount"><%# Eval("amount", "{0:C}") %></span>
			                    <span class="credits"><%# Eval("credits") %></span>
			                    <span class="transid">
			                        <asp:HiddenField 
			                            runat="server" 
			                            ID="transactionId" 
			                            Value='<%# Bind("transaction_id") %>'/>
			                    </span>
			                    <span class="button">
			                        <asp:ImageButton 
                                        runat="server" 
                                        ID="processButton" 
                                        AlternateText="Edit" 
                                        ImageUrl="~/Images/edit-icon.png" 
                                        CommandName="Edit" />
			                    </span>
		                    </li>
	                    </ItemTemplate>
	
	                    <SelectedItemTemplate>
		                    <li id="quotationsListItem" runat="server" class="">
		                    </li>
	                    </SelectedItemTemplate>
	
	                    <EmptyItemTemplate>
		                    <li id="quotationsListItem" runat="server">
			                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, NoQuotationsUnprocessed %>"/>
		                    </li>
	                    </EmptyItemTemplate>
                    </asp:ListView>
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
				    <asp:Literal runat="server" ID="RhosMovementInc"/>
				</div>
			</td>
		</tr>
	</table>
    </div>
</asp:Content>
