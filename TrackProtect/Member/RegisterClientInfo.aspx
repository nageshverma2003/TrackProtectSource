<%@ Page Title="Register Client info" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterClientInfo.aspx.cs" Inherits="TrackProtect.Member.RegisterClientInfo" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<table width="100%">
		<tr valign="top">
			<td class="leftColumn">
    <div class="accountInfo">
		<p class="pageHeader">Sign up to get rollin' with trackprotect</p>
        <span class="failureNotification">
            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
        </span>
        <fieldset class="register">
            <legend>Account information</legend>
            <table style="width:600px">
                <tr><td style="width:120px">First name</td><td colspan="3"><asp:TextBox ID="FirstName" runat="server" Width="100%" /></td></tr>
                <tr><td>Last name</td><td colspan="3"><asp:TextBox ID="LastName" runat="server" Width="100%" /></td></tr>
                <tr><td>Address line 1</td><td colspan="3"><asp:TextBox ID="AddressLine1" runat="server" Width="100%" /></td></tr>
                <tr><td>Address line 2</td><td colspan="3"><asp:TextBox ID="AddressLine2" runat="server" Width="100%" /></td></tr>
                <tr><td>Postal Code</td><td style="width:70px"><asp:TextBox ID="Zipcode" runat="server" Width="65px" /></td>
                    <td style="width:55px">City</td><td><asp:TextBox ID="City" runat="server" Width="100%" /></td></tr>
				<tr><td>State/province</td><td colspan="3"><asp:TextBox ID="State" runat="server" Width="100%" /></td></tr>
                <tr><td>Country</td><td colspan="3"><asp:DropDownList ID="Country" runat="server" Width="100%" /></td></tr>
                <tr><td>Telephone</td><td colspan="3"><asp:TextBox ID="Telephone" runat="server" Width="100%" /></td></tr>
                <tr><td>Mobile/cellular</td><td colspan="3"><asp:TextBox ID="Cellular" runat="server" Width="100%" /></td></tr>
                <tr><td>Account owner</td><td colspan="3"><asp:TextBox ID="AccountOwner" runat="server" Width="100%" /></td></tr>
				<tr><td>Twitter ID</td><td colspan="3"><asp:TextBox ID="TwitterID" runat="server" Width="100%" /></td></tr>
				<tr><td>Facebook ID</td><td colspan="3"><asp:TextBox ID="FacebookID" runat="server" Width="100%" /></td></tr>
				<tr><td>Soniall ID</td><td colspan="3"><asp:TextBox ID="SoniallID" runat="server" Width="100%" /></td></tr>
				<tr><td>I am an</td><td colspan="3">
					<asp:DropDownList ID="OwnerKind" runat="server" Width="20%">
						<asp:ListItem>Artist</asp:ListItem>
					</asp:DropDownList>
				</td></tr>
				
				
            </table>
        </fieldset>
		<fieldset>
			<legend>Billing information</legend>
			<table style="width:600px">
				<tr><td style="width:120px">Name</td><td colspan="3"><asp:TextBox id="BillingName" runat="server" Width="100%" /></td></tr>
				<tr><td>Sign up date</td><td colspan="3"><asp:TextBox id="SignUpDate" runat="server"/></td></tr>
				<tr>
					<td>Subscription plan</td>
					<td colspan="3">
						<asp:DropDownList id="SubscriptionPlah" runat="server">
							<asp:ListItem>Starter</asp:ListItem>
							<asp:ListItem>Medium</asp:ListItem>
							<asp:ListItem>Pro</asp:ListItem>
							<asp:ListItem>Bulk</asp:ListItem>
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>Credit Card nr.</td><td><asp:TextBox id="CreditCardNr" runat="server"/></td>
					<td>CVV</td><td><asp:TextBox id="CVVNr" runat="server"/></td>
				</tr>
				<tr><td>E-mail for receipt</td><td colspan="3"><asp:TextBox id="EmailForReceipt" runat="server"/></td></tr>
				<tr><td>How did you hear about trackprotect</td><td colspan="3"><asp:TextBox id="Referer" runat="server"/></td></tr>
				<tr><td colspan="4"><asp:CheckBox id="Agreement" runat="server"/>I agree with the <a href="">general conditions</a> and the <a href="">terms</a> for trackprotect.</td></tr>
				<tr><td colspan="4"><asp:CheckBox id="Owner" runat="server"/>Ik ben eigenaar van alle rechten op de muziek die ik registreer bij trackprotect.</td></tr>

				<tr>
					<td>&nbsp;</td><td>&nbsp;</td>
					<td colspan="2" align="right">
						<asp:ImageButton ID="SubmitButton" runat="server" 
							ImageUrl="~/Images/create_account.png" oncommand="SubmitButton_Command" />
					</td>
				</tr>
			</table>
		</fieldset>
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
						<!--#include virtual="../Content/registerclientinfo.inc" -->
					</div>
				</div>
			</td>
		</tr>
	</table>
</asp:Content>
