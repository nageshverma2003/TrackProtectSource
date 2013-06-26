<%@ Page Title="" Language="C#" MasterPageFile="~/Profile.Master" AutoEventWireup="true"
    CodeBehind="ProfileInfo.aspx.cs" Inherits="TrackProtect.Member.ProfileInfo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript" src="../js/vendor/jquery.js"></script>
    <script type="text/javascript" src="../js/vendor/custom.modernizr.js"></script>
    <script type="text/javascript" src="http://connect.soundcloud.com/sdk.js"></script>
    <%-- <script type="text/javascript" src="../Scripts/soundcloud.js"></script>--%>
    <script type="text/javascript">
        function SoundCloudAuthorize() {
            SC.initialize({
                client_id: 'f6404360399e2900b176fd17aab771e3',
                redirect_uri: 'http://test.trackprotect.com/Social/soundcloud.aspx'
            });

            SC.connect(function () {
                SC.get('/me', function (me) {
                    alert('Hello, ' + me.username);
                });
            });
        }
    </script>
    <script type="text/javascript">
        function InitializeRequest(path) {
            // call server side method 
            PageMethods.SetDownloadPath(path);

            // Create an IFRAME.
            var iframe = document.createElement("iframe");
            iframe.src = "Download.aspx";

            // This makes the IFRAME invisible to the user.
            iframe.style.display = "none";

            // Add the IFRAME to the page. This will trigger
            // a request to GenerateFile now.
            document.body.appendChild(iframe);
        } 
    </script>
    <script type="text/javascript" src="../Scripts/facebook.js"></script>
    <script type="text/javascript" src="../Scripts/twitter.js"></script>
    <link rel="stylesheet" href="../css/jquery.ui.all.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/ui/jquery.ui.core.js"></script>
    <script type="text/javascript" src="../Scripts/ui/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../Scripts/ui/jquery.ui.datepicker.js"></script>
    <link rel="stylesheet" href="../css/demo.css" />
    <script language="javascript" type="text/javascript">
        function StageNameEnability() {
            if (document.getElementById('ctl00_BodyContent_StageNameChkBx').checked) {
                var firstName = document.getElementById('ctl00_BodyContent_FirstName').value;
                var lastName = document.getElementById('ctl00_BodyContent_LastName').value;
                document.getElementById('ctl00_BodyContent_StageName').value = firstName + ' ' + lastName;
            }
            else {
                document.getElementById('ctl00_BodyContent_StageName').value = '';
            }
        }

        $(document).ready(function () {
            $("#ctl00_BodyContent_DOB").datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        });

        function Count(text, long) {
            var maxlength = new Number(long);
            var remChar = maxlength - document.getElementById('<%=CompanyName.ClientID%>').value.length;
            if (remChar > -1) {
            }
            if (document.getElementById('<%=CompanyName.ClientID%>').value.length > maxlength) {
                text.value = text.value.substring(0, maxlength);
            }
        }
     
    </script>
    <style type="text/css">
        .active
        {
            border: 1px solid #E4510A;
            padding: 0 1em;
        }
        .CalendarCSS
        {
            background-color: #E4510A;
            color: Snow;
            font-size: medium;
        }
        .ajax__calendar_month
        {
            cursor: pointer;
            height: 47px;
            overflow: hidden;
            text-align: center;
            width: 31px !important;
            color: black;
            background-color: #E6E6E6;
        }
        .ajax__calendar_day, .ajax__calendar_dayname
        {
            cursor: pointer;
            height: 47px;
            overflow: hidden;
            text-align: center;
            color: black;
            background-color: #E6E6E6;
        }
        .ajax__calendar_year
        {
            cursor: pointer;
            height: 47px;
            overflow: hidden;
            text-align: center;
            width: 31px !important;
            color: black;
            background-color: #E6E6E6;
        }
        .btnSize
        {
            height: 25px;
            cursor: pointer;
            border: medium none;
            box-shadow: none;
            font-family: "League Gothic" , "Helvetica Neue" , "Helvetica" ,Helvetica,Arial,sans-serif;
            font-weight: normal;
            display: inline-block;
            transition: background-color 300ms ease-out 0s;
            background-color: #E4510A;
            color: white;
            position: relative;
            text-align: center;
            text-decoration: none;
        }
        #divRadio label
        {
            font-size: 14px;
            vertical-align: middle;
            float: right;
            text-align: left;
            width: 320px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources : Resource, Profile %>"></asp:Localize></h1>
            </div>
            <div class="row">
                <div class="large-4 columns push-8">
                    <asp:HyperLink runat="server" CssClass="button extra-large expand control_p_btn" NavigateUrl="~/Member/MemberHome.aspx">
                        <i class="arrow-left"></i>
                        <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></asp:HyperLink>
                    <div id="user-info">
                        <header>
                            <p>
                                <asp:Localize runat="server" ID="LoggedOnTitle" /></p>
                            <h2>
                                <a href="Profile.aspx">
                                    <asp:Literal runat="server" ID="LoggedOnUserName" /></a></h2>
                        </header>
                        <section class="row collapse">
                            <a href="FinancialOverview.aspx" class="box small-6 columns">
                                <h2>
                                    <asp:Literal runat="server" ID="CreditsLiteral" /></h2>
                                <span class="orange">CREDITS</span> </a><a href="MemberTracks.aspx" class="box small-6 columns">
                                    <h2>
                                        <asp:Literal runat="server" ID="ProtectedLiteral" /></h2>
                                    <span class="orange">PROTETED TRACKS</span> </a>
                            <!-- <div class="large-1 hide-for-small"></div> -->
                        </section><section class="social-network"><div class="row collapse">
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Member/Profile.aspx#social"
                                CssClass="box small-4 columns">
                                <h2 id="FacebookHeading" runat="server" class="social facebook">
                                    F</h2>
                            </asp:HyperLink><asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Member/Profile.aspx#social"
                                CssClass="box small-4 columns">
                                <h2 class="social">
                                    <i runat="server" id="SoundcloudItag" class="soundcloud"></i>
                                </h2>
                            </asp:HyperLink><asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Member/Profile.aspx#social"
                                CssClass="box small-4 columns">
                                <h2 id="TwitterHeading" runat="server" class="social twitter">
                                    L</h2>
                            </asp:HyperLink></div>
                        </section><section class="actions"><div class="row collapse">
                            <a href="../Account/ChangePassword.aspx" class="box small-12 columns">
                                <asp:Localize ID="Localize22" runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a></div>
                            <div id="divAccPerCompleted" runat="server" class="row collapse border box small-12 columns">
                                <a href="Profile.aspx" class="">
                                    <asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral"
                                        Text="<%$ Resources: Resource, ClickToEdit %>" /></a>
                            </div>
                        </section>
                        <footer>
                            <a class="button extra-large expand border" href="Subscription.aspx?pid=4&country=NL&price=149,0000">upgrade plan</a> <a class="button extra-large expand"
                                href="SelectProduct.aspx">
                                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources : Resource, GetMore %>"></asp:Literal></a></footer></div>
                </div>
                <div class="large-8 columns pull-4">
                    <!-- User Account Information Form -->
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, Profile %>" /></h1>
                    </div>
                    <div class="custom active">
                        <h2>
                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, PersonalInformation %>" /></h2>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, FirstName %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox MaxLength="15" ID="FirstName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ControlToValidate="FirstName" CssClass="failureNotification" Display="Dynamic"
                                    ErrorMessage="<%$ Resources : Resource, FirstNameRequired %>" ID="FirstNameRequired"
                                    ToolTip="<%$ Resources : Resource, FirstNameRequired %>" ValidationGroup="RegisterUserValidationGroup"
                                    runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, LastName %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox MaxLength="15" ID="LastName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ControlToValidate="LastName" CssClass="failureNotification" Display="Dynamic"
                                    ErrorMessage="<%$ Resources : Resource, LastNameRequired %>" ID="RequiredFieldValidator1"
                                    ToolTip="<%$ Resources : Resource, LastNameRequired %>" ValidationGroup="RegisterUserValidationGroup"
                                    runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                            </div>
                            <div id="divRadio" class="large-8 columns" style="width: 397px;">
                                <asp:CheckBox runat="server" onClick="javascript:StageNameEnability();" ID="StageNameChkBx"
                                    Text="<%$ Resources : Resource, StageNameChkBxText %>" Style="float: left;" />
                            </div>
                        </div>
                        &nbsp;
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources : Resource, StageName %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="StageName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ControlToValidate="StageName" CssClass="failureNotification" Display="Dynamic"
                                    ErrorMessage="<%$ Resources : Resource, LastNameRequired %>" ID="RequiredFieldValidator2"
                                    ToolTip="<%$ Resources : Resource, LastNameRequired %>" ValidationGroup="RegisterUserValidationGroup"
                                    runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources : Resource, CompanyName %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="CompanyName" runat="server" onKeyUp="Count(this,35);" onChange="Count(this,35);"></asp:TextBox></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources : Resource, Gender %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <label class="radio" for="ctl00_BodyContent_Gender1">
                                    <asp:RadioButton ID="Gender1" GroupName="Gender" runat="server" />
                                    <span class="custom radio"></span>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, ManText %>"></asp:Localize></label><label
                                        class="radio" for="ctl00_BodyContent_Gender2"><asp:RadioButton ID="Gender2" GroupName="Gender"
                                            runat="server" /><span class="custom radio"></span>
                                        <asp:Localize runat="server" Text="<%$ Resources : Resource, WomanText %>"></asp:Localize></label></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources : Resource, Birthday %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="DOB" runat="server"></asp:TextBox><%--<ajaxToolkit:ToolkitScriptManager
                                    ID="ToolkitScriptManager1" runat="server">
                                </ajaxToolkit:ToolkitScriptManager>--%><%-- <asp:CalendarExtender ID="CalendarExtender" runat="server" TargetControlID="DOB"
                                    PopupPosition="TopLeft" Animated="true" Format="dd-MM-yyyy" StartDate="1901-01-01"
                                    CssClass="CalendarCSS">
                                </asp:CalendarExtender>--%><asp:RequiredFieldValidator ControlToValidate="DOB" CssClass="failureNotification"
                                    Display="Dynamic" ErrorMessage="<%$ Resources : Resource, LastNameRequired %>"
                                    ID="RequiredFieldValidator3" ToolTip="<%$ Resources : Resource, LastNameRequired %>"
                                    ValidationGroup="RegisterUserValidationGroup" runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize17" runat="server" Text="<%$ Resources : Resource, Address %>"></asp:Localize>&nbsp;1</label>
                            </div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="Address1" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ControlToValidate="Address1" CssClass="failureNotification" Display="Dynamic"
                                    ErrorMessage="<%$ Resources : Resource, LastNameRequired %>" ID="RequiredFieldValidator4"
                                    ToolTip="<%$ Resources : Resource, LastNameRequired %>" ValidationGroup="RegisterUserValidationGroup"
                                    runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize18" runat="server" Text="<%$ Resources : Resource, Address %>"></asp:Localize>&nbsp;2</label>
                            </div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="Address2" runat="server"></asp:TextBox></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize16" runat="server" Text="<%$ Resources : Resource, Zipcode %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="Pincode" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ControlToValidate="Pincode" CssClass="failureNotification" Display="Dynamic"
                                    ErrorMessage="<%$ Resources : Resource, LastNameRequired %>" ID="RequiredFieldValidator5"
                                    ToolTip="<%$ Resources : Resource, LastNameRequired %>" ValidationGroup="RegisterUserValidationGroup"
                                    runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize12" runat="server" Text="<%$ Resources : Resource, City %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="City" runat="server"></asp:TextBox><asp:RequiredFieldValidator ControlToValidate="City"
                                    CssClass="failureNotification" Display="Dynamic" ErrorMessage="<%$ Resources : Resource, LastNameRequired %>"
                                    ID="RequiredFieldValidator6" ToolTip="<%$ Resources : Resource, LastNameRequired %>"
                                    ValidationGroup="RegisterUserValidationGroup" runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources : Resource, State %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="State" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ControlToValidate="State" CssClass="failureNotification" Display="Dynamic" ErrorMessage="<%$ Resources : Resource, LastNameRequired %>"
                                    ID="RequiredFieldValidator7" ToolTip="<%$ Resources : Resource, LastNameRequired %>"
                                    ValidationGroup="RegisterUserValidationGroup" runat="server">*</asp:RequiredFieldValidator></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, Country %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <p>
                                    <asp:DropDownList ID="Country" runat="server" CssClass="custom dropdown">
                                    </asp:DropDownList>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, Telephone %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:TextBox ID="Number" runat="server"></asp:TextBox></div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, Emailadress %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <asp:Label ID="Email" runat="server"></asp:Label></div>
                        </div>
                        <div class="row">
                            <br />
                            <div class="large-4 columns">
                                <label>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, Iam %>"></asp:Localize></label></div>
                            <div class="large-8 columns">
                                <label for="ctl00_BodyContent_usertype_1">
                                    <asp:CheckBox runat="server" ID="usertype_1" />
                                    <span class="custom checkbox"></span>
                                    <asp:Localize runat="server" Text="<%$ Resources : Resource, Musician %>"></asp:Localize></label><label
                                        for="ctl00_BodyContent_usertype_2"><asp:CheckBox runat="server" ID="usertype_2" />
                                        <span class="custom checkbox"></span>
                                        <asp:Localize ID="Localize19" runat="server" Text="<%$ Resources : Resource, Songwriter %>"></asp:Localize></label><label
                                            for="ctl00_BodyContent_usertype_3"><asp:CheckBox runat="server" ID="usertype_3" />
                                            <span class="custom checkbox"></span>
                                            <asp:Localize ID="Localize20" runat="server" Text="<%$ Resources : Resource, Producer %>"></asp:Localize></label><label
                                                for="ctl00_BodyContent_usertype_4"><asp:CheckBox runat="server" ID="usertype_4" />
                                                <span class="custom checkbox"></span>
                                                <asp:Localize ID="Localize21" runat="server" Text="<%$ Resources : Resource, ArtistManager %>"></asp:Localize></label></div>
                        </div>
                        <div class="button-row row">
                            <div class="large-12 columns">
                                <asp:Button ID="Accept" Text="<%$ Resources : Resource, Accept %>" class="button small border highlight right"
                                    runat="server" OnClick="Accept_Click" ValidationGroup="RegisterUserValidationGroup" />
                                <asp:Button ID="Reject" Text="<%$ Resources : Resource, Back %>" class="button small border right"
                                    runat="server" OnClick="Reject_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                    <!-- End User Account Information Form -->
                    <!-- Registratie Info -->
                    <div class="row">
                        <div class="large-6 small-8 columns">
                            <h2>
                                <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, Registration %>" /></h2>
                        </div>
                        <div class="large-6 small-4 columns">
                            <asp:HyperLink NavigateUrl="~/Member/ProfileReg.aspx" CssClass="button small border right edit"
                                Text="<%$ Resources : Resource, Edit %>" runat="server"></asp:HyperLink></div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                Buma/Stemra nr.</p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="BumaNo" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                SENA reg. nr.</p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="SenaNo" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                ISRC handle</p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="ISRC" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <!-- End Registration Info -->
                    <!-- Social Login -->
                    <%--<div class="social-connect row">
                        <div class="large-12 columns">
                            <h2>
                                koppelingen</h2>
                            <div>
                                <asp:HyperLink runat="server" ID="linkSoundCloud" class="button soundcloud" NavigateUrl="javascript:SoundCloudAuthorize();"><i class="icon-soundcloud"></i>CONNECT</asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Literal ID="SoundCloudLabel" runat="server" /></div>
                            <div>
                                <asp:HyperLink runat="server" ID="linkFacebook" NavigateUrl="javascript:facebookAuthorize();"
                                    class="button facebook"><span class="social">F</span>CONNECT</asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Literal
                                        ID="FacebookIdLabel" runat="server" /></div>
                            <div>
                                <asp:HyperLink runat="server" ID="linkTwitter" class="button twitter" NavigateUrl="javascript:twitterAuthorize();"><span class="social">L</span>CONNECTE</asp:HyperLink>&nbsp;&nbsp;<asp:Literal
                                    ID="TwitterIdLabel" runat="server" />
                            </div>
                        </div>
                    </div>--%><div class="social-connect row">
                        <div class="large-12 columns">
                            <h2>
                                <asp:Localize ID="Localize14" runat="server" Text="<%$Resources : Resource, Couplings %>"></asp:Localize></h2>
                            <div style="width: 100%">
                                <div style="float: left; width: 28%;">
                                    <asp:HyperLink runat="server" ID="linkSoundCloud" class="button soundcloud" NavigateUrl="javascript:soundCloudAuthorize();">
                                        <i class="icon-soundcloud"></i>
                                        <asp:Label ID="lblsoundcloud" runat="server"></asp:Label>
                                    </asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;
                                </div>
                                <div style="border: 1px solid #E4510A; float: left; min-width: 32%; padding: 1.5% 0 1.5%;"
                                    id="soundclouddiv" runat="server">
                                    &nbsp;
                                    <asp:Literal ID="SoundCloudLabel" runat="server" />
                                    &nbsp;<asp:Button runat="server" ID="RemoveSoundCloud" Text="<%$ Resources : Resource, Remove %>"
                                        CssClass="btnSize" OnCommand="RemoveSoundCloud_Submit" />&nbsp;</div>
                            </div>
                            <div style="clear: both;">
                            </div>
                            <div style="width: 100%">
                                <div style="float: left; width: 26%;">
                                    <asp:HyperLink runat="server" ID="linkFacebook" NavigateUrl="javascript:facebookAuthorize();"
                                        class="button facebook">
                                        <span class="social">F</span><asp:Label ID="lblFacebook" runat="server"></asp:Label></asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>
                                <div style="border: 1px solid #3B5B99; float: left; min-width: 34%; padding: 1.5% 0 1.5%;"
                                    id="facebookdiv" runat="server">
                                    &nbsp;
                                    <asp:Literal ID="FacebookIdLabel" runat="server" />
                                    &nbsp;
                                    <asp:Button runat="server" ID="RemoveFacebook" Text="<%$ Resources : Resource, Remove %>"
                                        CssClass="btnSize" OnCommand="RemoveFacebook_Submit" />&nbsp;</div>
                            </div>
                            <div style="clear: both;">
                            </div>
                            <div style="width: 100%">
                                <div style="float: left; width: 30%;">
                                    <asp:HyperLink runat="server" ID="linkTwitter" class="button twitter" NavigateUrl="javascript:twitterAuthorize();">
                                        <span class="social">L</span><asp:Label ID="lblTwitter" runat="server"></asp:Label></asp:HyperLink><%--</li>--%><%--<li><a class="button twitter border" href="#">@johndefault</a></li>--%>&nbsp;&nbsp;
                                </div>
                                <div style="border: 1px solid #00ACED; float: left; min-width: 34%; padding: 1.5% 0 1.5%;"
                                    id="twitterdiv" runat="server">
                                    <asp:Literal ID="TwitterIdLabel" runat="server" />&nbsp;<asp:Button runat="server"
                                        ID="RemoveTwitter" Text="<%$ Resources : Resource, Remove %>" CssClass="btnSize"
                                        OnCommand="RemoveTwitter_Submit" /><%--</ul>--%></div>
                            </div>
                        </div>
                    </div>
                    <!-- User Account Info Print -->
                    <div class="row">
                        <div class="large-6 small-8 columns">
                            <h2>
                                <asp:Localize ID="Localize15" runat="server" Text="<%$Resources : Resource,UserAccount %>"></asp:Localize></h2>
                        </div>
                        <div class="large-6 small-4 columns">
                            <asp:HyperLink ID="HyperLink2" NavigateUrl="~/Member/ProfilePrint.aspx" CssClass="button small border right edit"
                                Text="<%$ Resources : Resource, Edit %>" runat="server"></asp:HyperLink></div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources : Resource, MemberSince %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="MemberSince" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources : Resource, PersonalInformation %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <div>
                                <div style="min-width: 32%; float: left;">
                                    <asp:Literal runat="server" ID="IdentityCertificate" /></div>
                                <div style="float: left; margin-left: 20px;">
                                    <asp:HyperLink NavigateUrl="javascript:void(0)" runat="server" ID="DownloadIdent"
                                        Visible="false"><i
                                class="icon-cert"></i></asp:HyperLink></div>
                                <div style="float: left; margin-left: 20px;">
                                    <asp:LinkButton runat="server" ID="AccountOverview" OnClick="AccountOverview_Click"><i
                                class="icon-pdf"></i></asp:LinkButton></div>
                            </div>
                        </div>
                    </div>
                    <!-- End User Account Print -->
                </div>
            </div>
        </div>
    </div>
    <%--<script type="text/javascript">
        document.write('<script src=' +
    ('__proto__' in {} ? 'js/vendor/zepto' : 'js/vendor/jquery') +
    '.js><\/script>')
    </script>--%><script type="text/javascript" src="../js/foundation/foundation.js"></script><script
        type="text/javascript" src="../js/foundation/foundation.cookie.js"></script><script
            type="text/javascript" src="../js/foundation/foundation.magellan.js"></script><script
                type="text/javascript" src="../js/foundation/foundation.joyride.js"></script><script
                    type="text/javascript" src="../js/foundation/foundation.topbar.js"></script><script
                        type="text/javascript" src="../js/foundation/foundation.clearing.js"></script><script
                            type="text/javascript" src="../js/foundation/foundation.tooltips.js"></script><script
                                type="text/javascript" src="../js/foundation/foundation.alerts.js"></script><script
                                    type="text/javascript" src="../js/foundation/foundation.placeholder.js"></script><script
                                        type="text/javascript" src="../js/foundation/foundation.section.js"></script><script
                                            type="text/javascript" src="../js/foundation/foundation.reveal.js"></script><script
                                                type="text/javascript" src="../js/foundation/foundation.orbit.js"></script><script
                                                    type="text/javascript" src="../js/foundation/foundation.dropdown.js"></script><script
                                                        type="text/javascript" src="../js/foundation/foundation.forms.js"></script><script
                                                            type="text/javascript">
                                                                                                                                       $(document).foundation();
        
                                                        </script></asp:Content>
