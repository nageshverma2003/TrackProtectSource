<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true" CodeBehind="SelectProduct.aspx.cs" Inherits="TrackProtect.SelectProduct" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="selectproduct">
	<table width="100%">
		<tr valign="top">
			<td class="leftColumn">
        		<div id="divProductList" style="padding-top: 16px">
					<asp:Table ID="ProductTable" runat="server">
					</asp:Table>
				</div>	
			</td>
			<td class="centerColumn">
				<div style="padding: 20px 0px 0px 0px">
					<asp:Image id="OnScreenSep" runat="server" ImageUrl="~/Images/screen_sep.png" />
				</div>
			</td>
			
			<td class="rightColumn">
				<div class="divRightColumn">
					<div class="divRightContent">
					    <asp:Literal runat="server" ID="SelectProductInc"/>
					</div>
                    <br/><br/>
					<div class="divRightContent" runat="server" id="divSignUp">
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
