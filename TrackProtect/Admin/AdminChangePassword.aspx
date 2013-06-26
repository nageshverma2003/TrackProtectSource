<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true"
    CodeBehind="AdminChangePassword.aspx.cs" Inherits="TrackProtect.Admin.AdminChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
        .failureNotification
        {
            padding-top: 3px;
            padding-left: 5px;
            font-size: .9em;
            background: #F0F0F0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row" style="min-height: 600px;">
        <asp:Literal runat="server" ID="RhosMovementInc" Visible="false" />
        <asp:Literal runat="server" ID="ProtectInc" Visible="false" />
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small" style="margin-left: 1%;">
                <h1>
                    <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, ModifyPassword %>" /></h1>
                <br />
                <br />
                <label>
                    <asp:Label ID="Localize4" runat="server" Text="<%$ Resources: Resource, ModifyPasswordUseForm %>" /></label>
                <br />
                <br />
                <label>
                    <asp:Literal runat="server" ID="ChangePasswordInc" Visible="false" /></label>
                <label>
                    <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: Resource, NewPasswordReq1 %>" /></label>
                <label>
                    <%= Membership.MinRequiredPasswordLength %>
                    <asp:Localize ID="Localize6" Text="<%$ Resources: Resource, NewPasswordReq2 %>" runat="server" /></label>
            </div>
            <div>
                <div class="large-8 columns">
                    <div class="section-title small to-left show-for-small" style="margin-left: 1%;">
                        <h1>
                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, ModifyPassword %>"></asp:Localize></h1>
                        <br />                       
                        <label>
                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, ModifyPasswordUseForm %>" /></label>
                        <br />                       
                        <label>
                            <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources: Resource, NewPasswordReq1 %>" /></label>
                        <label>
                            <%= Membership.MinRequiredPasswordLength %>
                            <asp:Localize ID="Localize9" Text="<%$ Resources: Resource, NewPasswordReq2 %>" runat="server" /></label>
                    </div>
                    <div>
                        <asp:ChangePassword ID="ChangeUserPassword" runat="server" CancelDestinationPageUrl="~/Member/MemberHome.aspx"
                            EnableViewState="false" RenderOuterTable="false" SuccessPageUrl="ChangePasswordSuccess.aspx">
                            <ChangePasswordTemplate> 
                                <asp:ValidationSummary Style="width: 73%;" ID="ValidationSummary" ShowMessageBox="false"
                                    ShowSummary="true" CssClass="failureNotification" EnableClientScript="true" ValidationGroup="ChangeUserPasswordValidationGroup"
                                    runat="server" DisplayMode="List" BorderColor="#75B891" BorderWidth="1" EnableTheming="true" />                               
                                <br />
                                <div>
                                    <asp:Panel ID="Panel1" runat="server" DefaultButton="ChangePasswordPushButton">
                                        <fieldset style="border: 1px solid #E4510A;">
                                            <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword"
                                                Text="<%$ Resources: Resource, OldPassword %>" />
                                            <asp:TextBox ID="CurrentPassword" runat="server" CssClass="fieldName" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="CurrentPasswordRequired" Display="Dynamic" runat="server"
                                                ControlToValidate="CurrentPassword" ErrorMessage="Password is required." ToolTip="Old Password is required."
                                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                                            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"
                                                Text="<%$ Resources: Resource, NewPassword %>" />
                                            <asp:TextBox ID="NewPassword" runat="server" CssClass="fieldName" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="NewPasswordRequired" Display="Dynamic" runat="server"
                                                ControlToValidate="NewPassword" ErrorMessage="New Password is required." ToolTip="New Password is required."
                                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"
                                                Text="<%$ Resources: Resource, ConfirmPassword %>"></asp:Label>
                                            <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="fieldName" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" Display="Dynamic" runat="server"
                                                ControlToValidate="ConfirmNewPassword" ErrorMessage="Confirm New Password is required."
                                                ToolTip="Confirm New Password is required." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                                                ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry."
                                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:CompareValidator>
                                        </fieldset>
                                        <p class="">
                                            <!--<asp:Button ID="CancelPushButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="<%$ Resources: Resource, Cancel %>"/>-->
                                            <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
                                                Text="<%$ Resources: Resource, ChangePassword %>" ValidationGroup="ChangeUserPasswordValidationGroup"
                                                CssClass="button small border" />
                                        </p>
                                    </asp:Panel>
                                </div>
                            </ChangePasswordTemplate>
                        </asp:ChangePassword>
                    </div>
                </div>
                <!-- End Miain Content -->
            </div>
        </div>
    </div>
</asp:Content>
