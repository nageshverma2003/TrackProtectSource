﻿<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Management.aspx.cs" Inherits="TrackProtect.Account.Management" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="account_management">
	<table width="100%">
	                <div style="margin-top: 16px; margin-left: 16px; margin-right: 16px;">
	                <table width="100%">
		                <tr>
			                <td class="fieldName"><asp:Label ID="TpIdLabel" runat="server" Text="e-mail"/></td>
			                <td><asp:TextBox ID="TpIdText" runat="server" CssClass="textEntry2" />
			                <asp:Button ID="TpIdButton" runat="server" Text="Search" OnCommand="TpIdSearch"/></td>
		                </tr>
		                <tr>
			                <td class="fieldName"><asp:Label ID="NameLabel" runat="server" Text="Name"/></td>
			                <td ><asp:Label ID="ManagerNameLabel" runat="server"/></td>
		                </tr>
                        <tr>
                            <td class="fieldName"><asp:Label runat="server" Text="VCL"></asp:Label></td>
                            <td><asp:TextBox runat="server" ID="VclText"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td class="fieldName"><asp:Label ID="Label1" runat="server" Text="ECL"></asp:Label></td>
                            <td><asp:TextBox runat="server" ID="EclText"></asp:TextBox></td>
                        </tr>
		                <tr>
			                <td>&nbsp;</td>
			                <td style="text-align: left;"><asp:Button ID="AcceptButton" runat="server" Text="Accept" OnCommand="AcceptUser"/></td>
		                </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td><asp:Literal runat="server" ID="ResultMessage"/></td>
                        </tr>
	                </table>
	                </div>
	                <div style="margin-top: 16px; margin-left: 16px;">
	                <asp:GridView runat="server" ID="ManagersTable" AutoGenerateColumns="False" 
                            Width="100%" GridLines="None">
	                        <AlternatingRowStyle BackColor="#F6F6F6" />
                            <asp:BoundField DataField="name" HeaderText="<%$ Resources: Resource, Name %>">
                            <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="vcl" HeaderText="<%$ Resources: Resource, VCL %>">
                            <HeaderStyle Width="75px" HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ecl" HeaderText="<%$ Resources: Resource, ECL %>">
                            <HeaderStyle Width="75px" HorizontalAlign="Left" />
                            </asp:BoundField>
                        </Columns>
	                    
	                </asp:GridView>
	                </div>
				</div>