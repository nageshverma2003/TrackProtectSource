<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="TrackProtect.Message" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height: 200px; width: 300px">
        <asp:Table ID="Table1" runat="server" Height="100%" HorizontalAlign="Center" Width="100%">
            <asp:TableRow ID="TableRow1" runat="Server" Height="100%" Width="100%">
                <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="Center" VerticalAlign="Middle"
                    Width="54px">
                    &nbsp;
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" HorizontalAlign="Left" VerticalAlign="Middle">
                    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" Font-Names="Arial; Helvetica; Sans-serif" Height="50px" Text="Label"
                        Width="350px"></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server">
                <asp:TableCell ID="TableCell3" runat="server" HorizontalAlign="Center" ColumnSpan="2">
                    <input id="btnClose" type="button" value="Close" onclick="javascript:closePopup(false);" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    </form>
</body>
</html>
