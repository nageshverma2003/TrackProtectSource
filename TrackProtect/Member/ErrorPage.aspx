﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="TrackProtect.Member.ErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="member_errorpage">
                    <asp:Label runat="server" ID="ErrorTitle" CssClass="errortitle"/>
                    <br/>
                    <asp:Label runat="server" ID="ErrorMessage" CssClass="errormessage"/>
                    <br/>
                    <asp:HyperLink runat="server" ID="ReturnLink" Text="<%$ Resources: Resource, Return %>" />
                </div>
			</td>