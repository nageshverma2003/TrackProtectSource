<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="TrackProtect.Member.ErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_errorpage">	<table width="100%">		<tr valign="top">			<td class="leftColumn">                <div class="errordialog">
                    <asp:Label runat="server" ID="ErrorTitle" CssClass="errortitle"/>
                    <br/>
                    <asp:Label runat="server" ID="ErrorMessage" CssClass="errormessage"/>
                    <br/>
                    <asp:HyperLink runat="server" ID="ReturnLink" Text="<%$ Resources: Resource, Return %>" />
                </div>
			</td>			<td class="centerColumn">				<div class="centerDivide">					<asp:Image id="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />				</div>			</td>						<td class="rightColumn">				<div class="divRightColumn">					<div class="divRightContent">					    <asp:Literal runat="server" ID="ProtectInc"/>					</div>					<br/><br/><br/><br/>				    <asp:Literal runat="server" ID="RhosMovementInc"/>				</div>			</td>		</tr>	</table>    </div></asp:Content>
