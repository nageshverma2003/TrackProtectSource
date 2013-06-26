<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ChangePasswordSuccess.aspx.cs" Inherits="TrackProtect.Account.ChangePasswordSuccess" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .fieldName
        {
            border: 1px solid #CC4809;
            background: white !important;
        }
        #controlpanel td, tr, th
        {
            border: 0px;
            background: white !important;
        }
        p
        {
            color: #E4510A !important;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div id="contropanel" class="row" style="min-height: 600px;">
        <asp:Literal runat="server" ID="RhosMovementInc" Visible="false" />
        <asp:Literal runat="server" ID="ProtectInc" Visible="false" />
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small" style="margin-left: 1%;">
                <h1>
                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, ModifyPassword %>" /></h1>
                <p style="margin-top: 2%;">
                    <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, ChangePasswordSuccessful %>" />.</p>
            </div>
            <div class="section-title small to-left show-for-small" style="margin-left: 1%;">
                <h1>
                    <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, ModifyPassword %>" /></h1>
                <p style="margin-top: 2%;">
                    <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, ChangePasswordSuccessful %>" />.</p>
            </div>
        </div>
    </div>
</asp:Content>
