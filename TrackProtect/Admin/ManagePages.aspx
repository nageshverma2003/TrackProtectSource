<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true"
    CodeBehind="ManagePages.aspx.cs" Inherits="TrackProtect.Admin.ManagePages" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- Fonts -->
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,600,800,800italic,700italic,600italic,400italic'
        rel='stylesheet' type='text/css' />
    <link type="text/css" rel="stylesheet" href="../css/normalize.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="../css/app.css" media="screen, projector, print" />
    <script src="../js/vendor/jquery.js" type="text/javascript"></script>
    <script src="../js/vendor/custom.modernizr.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.min.js"></script>
    <script type="text/javascript" src="../Scripts/facebookForAdmin.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#ctl00_MainContent_GenreList').change(function () {
                var value = $('#ctl00_MainContent_GenreList').val();
                if (value == 'Add') {
                    $('#divAddGenre').show();
                    $('#divDeleteGenre').hide();
                    $('#EditGenre').hide();
                    $('#divSubGenre').hide();
                }
                else if (value == 'Select') {
                    $('#EditGenre').hide();
                    $('#divDeleteGenre').hide();
                    $('#divAddGenre').hide();
                    $('#divSubGenre').hide();
                }
                else {
                    $('#divDeleteGenre').hide();
                    $('#divAddGenre').hide();
                    $('#EditGenre').show();
                    $('#divSubGenre').show();
                }
            });

            $('#ctl00_MainContent_SubGenreList').change(function () {
                var value = $('#ctl00_MainContent_SubGenreList').val();
                if (value == 'Add') {
                    $('#divAddSubGenre').show();
                    $('#divDeleteSubGenre').hide();
                    $('#EditSubGenre').hide();
                }
                else if (value == 'Select') {
                    $('#EditSubGenre').hide();
                    $('#divDeleteSubGenre').hide();
                    $('#divAddSubGenre').hide();
                }
                else {
                    $('#divDeleteSubGenre').hide();
                    $('#divAddSubGenre').hide();
                    $('#EditSubGenre').show();
                }
            });


            $('#showDivMapping').click(function () {
                $('#divMapping').css("display", "block");
            });

            $('#closeMappingdiv').click(function () {
                $('#divMapping').css("display", "none");
            });


            // Change the dropdown value using jquery
            $('#CanceladdingGenre').click(function () {
                $('#divAddGenre').hide();
                $("#ctl00_MainContent_GenreList option[value='Select']").attr("selected", "selected");
            });

            $('#CanceladdingSubGenre').click(function () {
                $('#divAddSubGenre').hide();
                $("#ctl00_MainContent_SubGenreList option[value='Select']").attr("selected", "selected");
            });

            $('#CanceldeletingGenre').click(function () {
                $('#divDeleteGenre').hide();
            });

            $('#CanceldeletingSubGenre').click(function () {
                $('#divDeleteSubGenre').hide();
            });

            $('#EditGenre').click(function () {
                $('#divDeleteGenre').show();
                $('#ctl00_MainContent_deleteGenreText').val($('#ctl00_MainContent_GenreList').find('option:selected').text());
            });

            $('#EditSubGenre').click(function () {
                $('#divDeleteSubGenre').show();
                $('#ctl00_MainContent_DeleteSubGenreText').val($('#ctl00_MainContent_SubGenreList').find('option:selected').text());
            });
        });

        //        function radioBtnGenreClick(Ref) {
        //            document.getElementById('divGenre').style.display = 'block';
        //            document.getElementById('divSubGenre').style.display = 'none';
        //            document.getElementById('divAddSubGenre').style.display = 'none';
        //            document.getElementById('divDeleteSubGenre').style.display = 'none';
        //            document.getElementById('EditSubGenre').style.display = 'none';
        //            var select = document.getElementById('<%=GenreList.ClientID %>');
        //            select.options[0].selected = true;
        //        }

        //        function radionBtnSubgenreClick(Ref) {
        //            document.getElementById('divSubGenre').style.display = 'block';
        //            document.getElementById('divGenre').style.display = 'none';
        //            document.getElementById('divAddGenre').style.display = 'none';
        //            document.getElementById('divDeleteGenre').style.display = 'none';
        //            document.getElementById('EditGenre').style.display = 'none';
        //            var select = document.getElementById('<%=SubGenreList.ClientID %>');
        //            select.options[0].selected = true;
        //        }

        window.onload = function () {
            document.getElementById('divGenre').style.display = 'block';
            document.getElementById('divSubGenre').style.display = 'none';
        }


        function pageLoad() {
            var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
            if (isAsyncPostback) {
                $(document).foundation();
                $(document).ready();

                $('#ctl00_MainContent_GenreList').change(function () {
                    var value = $('#ctl00_MainContent_GenreList').val();
                    if (value == 'Add') {
                        $('#divAddGenre').show();
                        $('#divDeleteGenre').hide();
                        $('#EditGenre').hide();
                        $('#divSubGenre').hide();
                    }
                    else if (value == 'Select') {
                        $('#EditGenre').hide();
                        $('#divDeleteGenre').hide();
                        $('#divAddGenre').hide();
                        $('#divSubGenre').hide();
                    }
                    else {
                        $('#divDeleteGenre').hide();
                        $('#divAddGenre').hide();
                        $('#EditGenre').show();
                        $('#divSubGenre').show();
                    }
                });

                $('#ctl00_MainContent_SubGenreList').change(function () {
                    var value = $('#ctl00_MainContent_SubGenreList').val();
                    if (value == 'Add') {
                        $('#divAddSubGenre').show();
                        $('#divDeleteSubGenre').hide();
                        $('#EditSubGenre').hide();
                    }
                    else if (value == 'Select') {
                        $('#EditSubGenre').hide();
                        $('#divDeleteSubGenre').hide();
                        $('#divAddSubGenre').hide();
                    }
                    else {
                        $('#divDeleteSubGenre').hide();
                        $('#divAddSubGenre').hide();
                        $('#EditSubGenre').show();
                    }
                });
            }
        }
    </script>
    <style type="text/css">
        #divRadioGroup1 label
        {
            font-size: 16px;
            vertical-align: middle;
            float: right;
            width: 290px;
        }
        
        #divRadioGroup2 label
        {
            font-size: 16px;
            vertical-align: middle;
            float: right;
            text-align: left;
            width: 290px;
        }
        
        .BtnMargin
        {
            margin-left: 5px;
        }
        
        .pnloverlay_template
        {
            background-color: #000000;
            height: 100%;
            left: 0px;
            margin-top: 0px;
            position: fixed;
            top: 0px;
            width: 100%;
            z-index: 10010;
            filter: alpha(opacity=50);
            -moz-opacity: 0.50;
            opacity: 0.50;
        }
        .pnlMapping
        {
            background-color: #000000;
            height: 100%;
            left: 0px;
            margin-top: 0px;
            position: fixed;
            top: 0px;
            width: 100%;
            z-index: 10010;
            filter: alpha(opacity=75);
            -moz-opacity: 0.75;
            opacity: 0.75;
        }
        .pnlLogin
        {
            background-color: #000000;
            height: 100%;
            left: 0px;
            margin-top: 0px;
            position: fixed;
            top: 0px;
            width: 100%;
            z-index: 10010;
            filter: alpha(opacity=87);
            -moz-opacity: 0.87;
            opacity: 0.87;
        }
        .floatLeft
        {
            float: left;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row">
        <div id="custom">
            <div class="large-12 columns">
                <div class="section-title to-left hide-for-small">
                    <h1>
                        <asp:Localize runat="server" Text="<%$ Resources : Resource, ManagePages %>"></asp:Localize></h1>
                </div>
                <div class="row">
                    <!-- Right Column -->
                    <div class="large-4 columns push-8">
                        <%--<div id="user-info">
                        <header>
                                <p><asp:Localize runat="server" ID="LoggedOnTitle" /></p><h2> <asp:Literal runat="server" ID="LoggedOnUserName" /></h2></header>
                        <section class="row"><div class="box large-3 large-offset-1 small-4 columns">
                                    <h2> <asp:Literal runat="server" ID="CreditsLiteral" /></h2><span class="orange">CREDITS</span> </div><div class="box large-6 large-offset-1 small-7 small-offset-1 columns">
                                    <h2><asp:Literal runat="server" ID="ProtectedLiteral" /></h2><span class="orange">PROTECTED TRACKS</span> </div><div class="large-1 hide-for-small"></div>
                            </section>
                        <footer>
                                <asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral" Text="<%$ Resources: Resource, ClickToEdit %>" /> </footer>
                    </div>--%>
                    </div>
                    <!-- End Right Column -->
                    <div class="large-8 columns pull-4">
                        <div class="section-title small to-left show-for-small">
                            <h1>
                                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources : Resource, ManagePages %>"></asp:Localize>
                            </h1>
                        </div>
                        <div class="row">
                            <div runat="server" id="divFBAuthenticate" style="float: left;">
                                <asp:HyperLink ID="HLfbconnect1" runat="server" class="button fb-button expand hide-for-small"
                                    NavigateUrl="javascript:facebookAuthorize();">
                                    <span class="fb-button-left social">F</span> <span style="text-align: center;" class="fb-button-center">
                                        <asp:Localize ID="HLfbLocalize1" runat="server"></asp:Localize></span></asp:HyperLink>
                                <asp:HyperLink ID="HLfbconnect2" NavigateUrl="javascript:facebookAuthorize();" runat="server"
                                    CssClass="button fb-button expand show-for-small">
                                    <asp:Localize ID="HLfbLocalize2" runat="server"></asp:Localize></asp:HyperLink>
                            </div>
                            <div style="width: 100px; float: left; margin-top: 22px;">
                                <asp:Image ID="ConnectionToFacebook" Height="30" Width="30" runat="server" />
                            </div>
                            <asp:Button runat="server" CssClass="button" ID="RemoveBtn" Text="<%$ Resources : Resource, RemoveAuthentication %>"
                                OnClick="RemoveBtn_Click" />
                            <div style="clear: both;">
                            </div>
                            <br />
                            <div id="divPageMapping" runat="server">
                                <br />
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <contenttemplate>
                                        <a id="showDivMapping" style="cursor: pointer; text-decoration: underline;">
                                            <asp:Localize runat="server" Text="<%$ Resources: Resource, GenrePageMappingTable %>"></asp:Localize></a>
                                        <br />
                                        <br />
                                        <br />
                                        <div id="divGenre" style="width: 400px;">
                                            <label>
                                                <asp:Label runat="server" Text="Genre"></asp:Label>
                                            </label>
                                            <asp:DropDownList ID="GenreList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="GenreList_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:Button ID="EditGenre" Visible="false" runat="server" Text="Edit Genre" CssClass="button  small border right BtnMargin"
                                                OnClick="EditGenre_Click" />
                                            <div id="divAddGenre" runat="server" visible="false">
                                                <br />
                                                <asp:TextBox runat="server" ID="addGenreText"></asp:TextBox>
                                                <asp:Button ID="AddGenre" Text="Add Genre" runat="server" CssClass="button small border right BtnMargin"
                                                    OnClick="AddGenre_Click" />
                                            </div>
                                            <div id="divDeleteGenre" runat="server" visible="false">
                                                <br />
                                                <asp:TextBox runat="server" ID="deleteGenreText"></asp:TextBox>
                                                <asp:Button ID="DeleteGenre" Text="Delete Genre" runat="server" CssClass="button small border right BtnMargin"
                                                    OnClick="DeleteGenre_Click" />
                                                <asp:Button ID="CanceldeletingGenre" runat="server" CssClass="button small border right BtnMargin"
                                                    Text="Cancel" OnClick="CanceldeletingGenre_Click" />
                                            </div>
                                        </div>
                                        <br />
                                        <br />
                                        <div id="divSubGenre" runat="server" visible="false" style="width: 400px;">
                                            <label>
                                                <asp:Label runat="server" Text="SubGenre"></asp:Label>
                                            </label>
                                            <asp:DropDownList ID="SubGenreList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SubGenreList_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:Button ID="EditSubGenre" Visible="false" runat="server" Text="Edit SubGenre"
                                                CssClass="button small border right BtnMargin" OnClick="EditSubGenre_Click" />
                                            <div id="divAddSubGenre" runat="server" visible="false">
                                                <br />
                                                <asp:TextBox runat="server" ID="addSubGenreText"></asp:TextBox>
                                                <asp:Button ID="AddSubGenre" Text="Add Sub Genre" ValidationGroup="Required" runat="server"
                                                    CssClass="button small border right BtnMargin" OnClick="AddSubGenre_Click" />
                                            </div>
                                            <div id="divDeleteSubGenre" runat="server" visible="false">
                                                <br />
                                                <asp:TextBox runat="server" ID="DeleteSubGenreText"></asp:TextBox>
                                                <asp:Button ID="DeleteSubgenre" Text="Delete Sub Genre" runat="server" CssClass="button small border right BtnMargin"
                                                    OnClick="DeleteSubgenre_Click" />
                                                <asp:Button ID="CanceldeletingSubGenre" runat="server" CssClass="button small border right BtnMargin"
                                                    Text="Cancel" OnClick="CanceldeletingSubGenre_click" />
                                            </div>
                                        </div>
                                    </contenttemplate>
                                </asp:UpdatePanel>
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                    <progresstemplate>
                                        <div style="position: absolute; z-index: 5;" align="center">
                                            <asp:Panel ID="Panel1" runat="server" CssClass="pnloverlay_template">
                                                <img alt="" src="../Images/ajax-loader.gif" style="margin-top: 370px;" />
                                            </asp:Panel>
                                        </div>
                                    </progresstemplate>
                                </asp:UpdateProgress>
                                <br />
                                <br />
                                <br />
                                <label>
                                    <asp:Label ID="Label1" runat="server" Text="<%$ Resources: Resource, FacebookPages %>"></asp:Label>
                                </label>
                                <asp:DropDownList ID="FBPagesList" Width="275px" runat="server" CssClass="custom dropdown"
                                    AutoPostBack="true" OnSelectedIndexChanged="FBPagesList_Selectionchanged">
                                </asp:DropDownList>
                                <br />
                                <br />
                                <br />
                                <asp:Button runat="server" CssClass="button" ID="BtnMap" Text="<%$ Resources : Resource, SaveMapping %>"
                                    OnClick="BtnMap_Click" />
                            </div>
                            <br />
                        </div>
                    </div>
                    <!-- End Miain Content -->
                </div>
            </div>
        </div>
    </div>
    <div id="divMapping" style="position: absolute; z-index: 7; display: none;" align="center">
        <div style="position: absolute; z-index: 5; overflow: scroll;" align="center">
            <asp:Panel ID="pnlMapping" runat="server" CssClass="pnlMapping">
                <div style="margin-top: 300px; margin-left: 490px;" align="center">
                    <img id="closeMappingdiv" alt="" src="../Images/window-close-icon.png" style="height: 40px;
                        margin-left: 426px; z-index: 8;" />
                    <asp:GridView ID="MappingGrid" runat="server" Width="400px" Height="300px" AutoGenerateColumns="false">
                        <HeaderStyle BackColor="#006699" BorderStyle="Groove" ForeColor="White" Height="30px"
                            Font-Size="16px" />
                        <RowStyle ForeColor="Black" Font-Size="14px" Font-Bold="false" Height="28px" />
                        <Columns>
                            <asp:TemplateField HeaderText="RHOS Genre Page" Visible="true">
                                <ItemTemplate>
                                    <asp:Label ID="lblpageName" runat="server" Text='<%# Bind("pageName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Genre" Visible="true">
                                <ItemTemplate>
                                    <asp:Label ID="lblgenreName" runat="server" Text='<%# Bind("genreName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sub Genre" Visible="true">
                                <ItemTemplate>
                                    <asp:Label ID="lblsubGenreName" runat="server" Text='<%# Bind("subGenreName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
        </div>
    </div>
    <div id="divLogin" runat="server" align="center" style="z-index: 9; position: absolute;">
        <div style="position: absolute; z-index: 5;" align="center">
            <asp:Panel ID="loginPanel1" runat="server" CssClass="pnlLogin">
                <div id="Div1" class="row" style="min-height: 600px; margin-top: 20px;">
                    <div class="large-12 columns">
                        <div class="section-title to-left hide-for-small">
                        </div>
                        <div class="row">
                            <div class="large-8 columns">
                                <div class="section-title small to-left show-for-small">
                                </div>
                                <div class="row" style="min-height: 80%">
                                    <p>
                                        <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, EnterUsernameAndPassword %>" />
                                    </p>
                                    <p>
                                        <asp:Literal runat="server" ID="LogonMessage" /></p>
                                    <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                                        ValidationGroup="LoginUserValidationGroup" />
                                    <div class="accountInfo">
                                        <asp:Panel ID="Panel176" runat="server" DefaultButton="LoginButton">
                                            <fieldset class="login">
                                                <p>
                                                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" Text="<%$ Resources: Resource, Username %>" />
                                                    <asp:TextBox ID="Email" runat="server" CssClass="textEntry" />
                                                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                                        CssClass="failureNotification" ErrorMessage="<%$ Resources: Resource, UsernameRequired %>"
                                                        ToolTip="<%$ Resources: Resource, UsernameRequired %>" Display="Dynamic" ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="EmailValidate" ControlToValidate="Email" ValidationExpression="^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$"
                                                        ErrorMessage="<%$ Resources : Resource, EmailNotValid %>" ValidationGroup="LoginUserValidationGroup"
                                                        runat="server">
                                                    </asp:RegularExpressionValidator>
                                                </p>
                                                <br />
                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="<%$ Resources: Resource, Password %>" />
                                                <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password" />
                                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                                    CssClass="failureNotification" ErrorMessage="<%$ Resources: Resource, PasswordRequired %>"
                                                    ToolTip="<%$ Resources: Resource, PasswordRequired %>" Display="Dynamic" ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                                <br />
                                                <br />
                                                <asp:Button ID="LoginButton" runat="server" CssClass="button" OnClick="Login_Click"
                                                    Text="<%$ Resources: Resource, Login %>" ValidationGroup="LoginUserValidationGroup" />
                                                <br />
                                                <br />
                                                <a href="AdminChangePassword.aspx">Change Password</a>
                                            </fieldset>
                                            <div class="row">
                                                <div style="float: right;">
                                                </div>
                                                <div style="float: right; width: 15%">
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <div style="clear: both;">
                                    </div>
                                </div>
                            </div>
                            <!-- End Main Content -->
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <script type="text/javascript">
        document.write('<script src=' +
    ('__proto__' in {} ? 'js/vendor/zepto' : 'js/vendor/jquery') +
    '.js><\/script>')
    </script>
    <script type="text/javascript" src="../js/foundation/foundation.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.cookie.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.magellan.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.joyride.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.topbar.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.clearing.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.tooltips.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.alerts.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.placeholder.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.section.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.reveal.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.orbit.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.dropdown.js"></script>
    <script type="text/javascript" src="../js/foundation/foundation.forms.js"></script>
    <script type="text/javascript">
        $(document).foundation();
    </script>
    <script type="text/javascript">
        $(document).ready(function ($) {
            $(".file-upload").bind("click", function (event) {
                event.preventDefault();
                $(this).closest('.track-upload').find('input[type=file]').click();
            });
            $('input[type=file]').bind('change', function () {
                var filename = $(this).val();
                if (filename) {
                    console.log($(this).closest('div').find('.postfix'));
                    $(this).closest('div').find('.postfix').text(filename);
                }
            }).change();
        });
    </script>
</asp:Content>
