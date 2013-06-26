<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true" CodeBehind="Promo.aspx.cs" Inherits="TrackProtect.Promo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
     <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    FAQ</h1>
            </div>
            <div class="row">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                    <div id="user-info">
                        <header>
                                <p><asp:Localize runat="server" ID="LoggedOnTitle" /></p><h2> <asp:Literal runat="server" ID="LoggedOnUserName" /></h2></header>
                        <section class="row"><div class="box large-3 large-offset-1 small-4 columns">
                                    <h2> <asp:Literal runat="server" ID="CreditsLiteral" /></h2><span class="orange">CREDITS</span> </div><div class="box large-6 large-offset-1 small-7 small-offset-1 columns">
                                    <h2><asp:Literal runat="server" ID="ProtectedLiteral" /></h2><span class="orange">PROTECTED TRACKS</span> </div><div class="large-1 hide-for-small"></div>
                            </section>
                        <footer>
                                <asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral" Text="<%$ Resources: Resource, ClickToEdit %>" /> </footer>
                    </div>
                </div>
                <!-- End Right Column -->
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            </h1>
                    </div>
                    <div class="row">
                         <asp:Literal runat="server" ID="PromoInc"/>
                    </div>
                </div>
                <!-- End Miain Content -->
            </div>
        </div>
    </div>

</asp:Content>
