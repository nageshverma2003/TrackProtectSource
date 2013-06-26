<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true"
    CodeBehind="AdminHome.aspx.cs" Inherits="TrackProtect.Admin.AdminHome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .login
        {
            border: 1px solid #CC4809;
        }
        .SignUpBtn
        {
            padding: 0.6em 0.5em 0.4em;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row" style="min-height: 600px;">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    Admin LOGIN</h1>
            </div>
            <div class="row">
                <div class="large-8 columns">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            Admin LOGIN</h1>
                    </div>
                    <div class="row" style="min-height: 80%">
                        <p>
                            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, EnterUsernameAndPassword %>" />
                        </p>
                        <p>
                            <asp:Literal runat="server" ID="LogonMessage" /></p>
                        <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                            ValidationGroup="LoginUserValidationGroup" />
                        <div class="accountInfo">
                            <asp:Panel ID="Panel1" runat="server" DefaultButton="LoginButton">
                                <fieldset class="login">
                                    <p>
                                        <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" Text="<%$ Resources: Resource, Username %>" />
                                        <asp:TextBox ID="Email" runat="server" CssClass="textEntry" />
                                        <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                            CssClass="failureNotification" ErrorMessage="<%$ Resources: Resource, UsernameRequired %>"
                                            ToolTip="<%$ Resources: Resource, UsernameRequired %>" Display="Dynamic" ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator></p>
                                    <br />
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="<%$ Resources: Resource, Password %>" />
                                    <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                        CssClass="failureNotification" ErrorMessage="<%$ Resources: Resource, PasswordRequired %>"
                                        ToolTip="<%$ Resources: Resource, PasswordRequired %>" Display="Dynamic" ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                    <br />
                                    <br />
                                    <asp:Button ID="LoginButton" runat="server" CssClass="button" OnClick="Login_Click"
                                        Text="Log In" ValidationGroup="LoginUserValidationGroup" />
                                </fieldset>
                                <div class="row">
                                    <div style="float: right;">
                                    </div>
                                    <div style="float: right; width: 15%">
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                        <div style="clear: both;">
                        </div>
                    </div>
                </div>
                <!-- End Main Content -->
            </div>
        </div>
    </div>
</asp:Content>
