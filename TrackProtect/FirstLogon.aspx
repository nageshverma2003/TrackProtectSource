<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="FirstLogon.aspx.cs" Inherits="TrackProtect.Explanation" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%--<script type="text/javascript">
        window.onload = function hideMenu() {
            document.getElementById('menu').style.visibility = "hidden";
        }
    </script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Literal runat="server" ID="LogonMode"></asp:Literal></h1>
            </div>
            <%--<div class="section-title small to-left show-for-small">
                <h1>
                    <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, Profile %>" /></h1>
            </div>--%>
            <asp:Literal ID="ltrFirstLogon" runat="server"></asp:Literal>
            <%--<div class="row">
                <!-- Full Width -->
                <!-- Main Content -->
                <div class="small-12 columns">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            First Login</h1>
                    </div>
                    <div class="row">
                        <div class="small-12 large-10 large-centered columns">
                            <h1 class="orange">
                                dear
                                <asp:Literal runat="server" ID="FirstNameLiteral"></asp:Literal></h1>
                            <p>
                                Thanks for registration. Yes, this is an explanation why we need you to do this.
                                We know it's a lot work, but it's in your best interest.</p>
                            <p>
                                To make it as easy as possible, we've divided things into 4 sections:</p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="small-12 large-6 large-offset-1">
                            <h2 class="step">
                                <a href="Member/ProfileInfo.aspx"><span class="number-circle">1</span>INFO<span class="explain">COMPLETE
                                    YOUR<br>
                                    personal information</span></a></h2>
                            <div id="dotted-arrow-1" class="hide-for-small">
                                <i class="dotted-arrow-1"></i>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="small-12 large-6 large-offset-6">
                            <h2 class="step">
                                <a href="Member/ProfileReg.aspx"><span class="number-circle">2</span>Registration<span
                                    class="explain">enter your<br>
                                    exsting details</span></a></h2>
                            <div id="dotted-arrow-2" class="hide-for-small">
                                <i class="dotted-arrow-2"></i>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="small-12 large-6 large-offset-2">
                            <h2 class="step">
                                <a href="Member/Profile.aspx"><span class="number-circle">3</span>connect<span class="explain">your
                                    social<br>
                                    media networks</span></a></h2>
                            <div id="dotted-arrow-3" class="hide-for-small">
                                <i class="dotted-arrow-3"></i>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="small-12 large-6 large-offset-5">
                            <h2 class="step">
                                <a href="Member/ProfilePrint.aspx"><span class="number-circle">4</span>verify<span
                                    class="explain">complete account<br>
                                    verification</span></a></h2>
                        </div>
                    </div>
                    <div class="profile-actions row">
                        <div class="small-12 large-6 columns">
                            <a class="button large border right" href="Member/MemberHome.aspx">NO THANKS, REMIND
                                ME LATER</a>
                        </div>
                        <div class="small-12 large-6 columns">
                            <a class="button large border highlight" href="Member/ProfileInfo.aspx">COMPLETE YOUR
                                PROFILE NOW</a>
                        </div>
                    </div>
                </div>
                <!-- End Main Content -->
            </div>--%>
        </div>
    </div>
</asp:Content>
