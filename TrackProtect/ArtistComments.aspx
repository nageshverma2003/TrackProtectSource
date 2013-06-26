<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true"
    CodeBehind="ArtistComments.aspx.cs" Inherits="TrackProtect.ArtistComments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="artistcomments">
        <table width="100%">
            <tr valign="top">
                <td class="leftColumn">
                    <div>
                        <asp:Literal runat="server" ID="ArtistCommentsInc" />
                    </div>
                </td>
                <td class="centerColumn">
                    <div class="centerDivide">
                        <asp:Image ID="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />
                    </div>
                </td>
                <td class="rightColumn">
                    <div class="divRightColumn">
                        <div class="divRightContent">
                            <asp:Literal runat="server" ID="ProtectInc" />
                        </div>
                        <br />
                        <br />
                        <div class="divRightContent">
                            <asp:HyperLink ID="SignupButton" ImageUrl="<%$ Resources: resource, imgSignup %>"
                                NavigateUrl="~/Account/Register.aspx" runat="server" />
                        </div>
                        <br />
                        <br />
                        <br />
                        <br />
                        <asp:Literal runat="server" ID="RhosMovementInc" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
