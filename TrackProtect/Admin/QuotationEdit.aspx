﻿ <%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuotationEdit.aspx.cs" Inherits="TrackProtect.Admin.QuotationEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin_quotationedit">
        		    <div>
        		        <div><asp:Label runat="server" Text="Credits"/></div>
        		        <div><asp:TextBox runat="server" ID="Credits" ReadOnly="True" /></div>
        		    </div>
                        <div><asp:Label runat="server" Text="Amount"/></div>
                        <div><asp:TextBox runat="server" ID="Amount"/></div>
                    </div>
                        <asp:Button runat="server" ID="Store" OnCommand="StoreQuotation" Text="<%$ Resources: Resource, btnAccept %>"/>
                    </div>
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