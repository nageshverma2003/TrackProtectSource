﻿<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuotationSuccess.aspx.cs" Inherits="TrackProtect.Member.QuotationSuccess" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_quotationsuccess">
	<table width="100%">
                    <div class="backtocontrolpanel">
                        <div class="backtocontrolpaneltext"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>"/></div>
                        <div class="backtocontrolpanelbutton">
                            <a href="/Member/MemberHome.aspx" class="backtocontrolpanellink">
                                <img src="/Images/cp_settings.png" class="backtocontrolpanelimage"/>
                            </a>
                        </div>
                    </div>
                    <div class="quotationsuccess">
        		        <div class="quotationsuccessline">
        		            <asp:Literal runat="server" ID="QuotationSuccessText" Text="<%$ Resources: Resource, QuotationSuccess %>"/>
        		        </div>
                    </div>
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
    </div>
</asp:Content>