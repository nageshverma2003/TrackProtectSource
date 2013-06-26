<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true" CodeBehind="Activation.aspx.cs" Inherits="TrackProtect.Activation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="contact">
	<table width="100%">
		<tr valign="top">
			<td class="leftColumn">
        		<div>
				    <asp:Literal runat="server" ID="ActivateInc"/>
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
					    <asp:Literal runat="server" ID="ProtectInc"/>
					</div>
					<br/><br/>
					<div class="divRightContent">
						<asp:HyperLink ID="SignupButton" ImageUrl="<%$ Resources: resource, imgSignup %>" NavigateUrl="~/Account/Register.aspx" runat="server"/>
					</div>
					<br/><br/><br/><br/>
				    <asp:Literal runat="server" ID="RhosMovementInc"/>
				</div>
			</td>
		</tr>
	</table>
    </div>
</asp:Content>
