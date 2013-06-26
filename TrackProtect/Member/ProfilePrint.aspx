<%@ Page Title="" Language="C#" MasterPageFile="~/Profile.Master" AutoEventWireup="true"
    CodeBehind="ProfilePrint.aspx.cs" Inherits="TrackProtect.Member.ProfilePrint" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="../js/vendor/jquery.js" type="text/javascript"></script>
    <script src="../js/vendor/custom.modernizr.js" type="text/javascript"></script>
    <script type="text/javascript" src="http://connect.soundcloud.com/sdk.js"></script>
    <%--<script type="text/javascript" src="../Scripts/soundcloud.js"></script>--%>
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
    <script type="text/javascript" src="../Scripts/facebook.js"></script>
    <script type="text/javascript" src="../Scripts/twitter.js"></script>
    <style type="text/css">
        .active
        {
            border: 1px solid #E4510A;
            padding: 0 1em;
        }
        .custom
        {
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    <asp:Localize ID="Localize14" runat="server" Text="<%$ Resources : Resource, Profile %>"></asp:Localize></h1>
            </div>
            <div class="row">
                <div class="large-4 columns push-8">
                    <asp:HyperLink ID="HyperLink2" runat="server" CssClass="button extra-large expand control_p_btn" NavigateUrl="~/Member/MemberHome.aspx">
                        <i class="arrow-left"></i>
                        <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, ControlPanel %>" /></asp:HyperLink>
                    <div id="user-info">
                        <header>
                                <p><asp:Localize runat="server" ID="LoggedOnTitle" /></p>
                                <h2><a href="Profile.aspx"><asp:Literal runat="server" ID="LoggedOnUserName" /></a></h2>
                            </header>
                        <section class="row collapse">
                                <a href="FinancialOverview.aspx" class="box small-6 columns">
                                    <h2><asp:Literal runat="server" ID="CreditsLiteral" /></h2>

                                    <span class="orange">CREDITS</span>
                                </a>
                                <a href="MemberTracks.aspx" class="box small-6 columns">
                                    <h2><asp:Literal runat="server" ID="ProtectedLiteral" /></h2>
                                    <span class="orange">PROTETED TRACKS</span>
                                </a>
                                <!-- <div class="large-1 hide-for-small"></div> -->

                            </section>
                        <section class="social-network">
                              <div class="row collapse">
                              <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 id="FacebookHeading" runat="server" class="social facebook">F</h2>
                                </asp:HyperLink>
                                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 class="social"><i runat="server" id="SoundcloudItag" class="soundcloud"></i></h2>
                                </asp:HyperLink>
                                <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/Member/Profile.aspx#social" CssClass="box small-4 columns">
                                    <h2 id="TwitterHeading" runat="server" class="social twitter">L</h2>                                
                                </asp:HyperLink>
                              </div>
                            </section>
                        <section class="actions">
                              <div class="row collapse">
                                <a href="../Account/ChangePassword.aspx" class="box small-12 columns"><asp:Localize ID="Localize25" runat="server" Text="<%$ Resources : Resource, ChangePassword %>"></asp:Localize></a>
                              </div>
                              <div id="divAccPerCompleted" runat="server" class="row collapse border box small-12 columns">
                                <a href="Profile.aspx" class=""><asp:Literal runat="server" ID="CompletedLiteral" /><asp:Literal runat="server" ID="ClickToLinkLiteral" Text="<%$ Resources: Resource, ClickToEdit %>" /></a>
                              </div>
                            </section>
                        <footer>

                                <a class="button extra-large expand border" href="Subscription.aspx?pid=4&country=NL&price=149,0000">upgrade plan</a>
                                <a class="button extra-large expand" href="SelectProduct.aspx"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources : Resource, GetMore %>"></asp:Literal></a>
                            </footer>
                    </div>
                </div>
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Resource, Profile %>" /></h1>
                    </div>
                    <!-- User Account Information -->
                    <div class="row">
                        <div class="large-6 small-8 columns">
                            <h2>
                                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, PersonalInformation %>" /></h2>
                        </div>
                        <div class="large-6 small-4 columns">
                            <asp:HyperLink ID="HyperLink1" NavigateUrl="~/Member/ProfileInfo.aspx" CssClass="button small border right edit"
                                Text="<%$ Resources : Resource, Edit %>" runat="server"></asp:HyperLink></div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources : Resource, Name %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="Name" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources : Resource, StageName %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="StageName" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources : Resource, CompanyName %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="CompanyName" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources : Resource, Gender %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="Gender" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources : Resource, Birthday %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="DOB" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources : Resource, Address %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="Address" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources : Resource, Zipcode %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="Pincode" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize19" runat="server" Text="<%$ Resources : Resource, City %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="City" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize20" runat="server" Text="<%$ Resources : Resource, State %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="State" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize15" runat="server" Text="<%$ Resources : Resource, Country %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="Country" runat="server" /></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize16" runat="server" Text="<%$ Resources : Resource, Telephone %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="Number" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize18" runat="server" Text="<%$ Resources : Resource, Emailadress %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="Email" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                <asp:Localize ID="Localize17" runat="server" Text="<%$ Resources : Resource, Iam %>"></asp:Localize></p>
                        </div>
                        <div class="large-8 columns">
                            <p>
                                <asp:Label ID="OwnerKind" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <!-- End User Account Information -->
                    <!-- Registratie Info -->
                    <div class="row">
                        <div class="large-6 small-8 columns">
                            <h2>
                                <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: Resource, Registration %>" /></h2>
                        </div>
                        <div class="large-6 small-4 columns">
                            <asp:HyperLink runat="server" NavigateUrl="~/Member/ProfileReg.aspx" Text="<%$ Resources : Resource, Edit %>"
                                CssClass="button small border right edit"></asp:HyperLink></div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                Buma/Stemra nr.</p>
                        </div>
                        <div class="large-8 columns">
                            <asp:Label ID="BumaNo" runat="server"></asp:Label></div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                SENA reg. nr.</p>
                        </div>
                        <div class="large-8 columns">
                            <asp:Label ID="SenaNo" runat="server"></asp:Label></div>
                    </div>
                    <div class="row">
                        <div class="large-4 columns">
                            <p class="title">
                                ISRC handle</p>
                        </div>
                        <div class="large-8 columns">
                            <asp:Label ID="ISRC" runat="server"></asp:Label></div>
                    </div>
                    <!-- End Registration Info -->
                    <!-- Social Login -->
                    <%--<div class="social-connect row">
                        <div class="large-12 columns">
                            <h2>
                                koppelingen</h2>
                            <div>
                                <asp:HyperLink runat="server" ID="linkSoundCloud" class="button soundcloud" NavigateUrl="javascript:soundCloudAuthorize();"><i class="icon-soundcloud"></i>CONNECT</asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;
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
                                <asp:Localize ID="Localize21" runat="server" Text="<%$Resources : Resource, Couplings %>"></asp:Localize></h2>
                            <div style="width: 100%">
                                <div style="float: left; width: 28%;">
                                    <asp:HyperLink runat="server" ID="linkSoundCloud" CssClass="button soundcloud" NavigateUrl="javascript:SoundCloudAuthorize();">
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
                                    <asp:HyperLink runat="server" ID="linkFacebook" NavigateUrl="javascript:facebookAuthorize();" CssClass="button facebook">
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
                                    <asp:HyperLink runat="server" ID="linkTwitter" CssClass="button twitter" NavigateUrl="javascript:twitterAuthorize();">
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
                    <!-- User Account Info Print Form -->
                    <div class="custom active">
                        <h2>
                            <asp:Localize ID="Localize22" runat="server" Text="<%$Resources : Resource,UserAccount %>"></asp:Localize></h2>
                        <div class="row">
                            <div class="large-4 columns">
                                <p class="title">
                                    <asp:Localize ID="Localize12" runat="server" Text="<%$ Resources : Resource, MemberSince %>"></asp:Localize></p>
                            </div>
                            <div class="large-8 columns">
                                <p>
                                    <asp:Label ID="MemberSince" runat="server"></asp:Label></p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns">
                                <p class="title">
                                    <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources : Resource, PersonalInformation %>"></asp:Localize></p>
                            </div>
                            <div class="large-8 columns">
                                <%--<asp:FileUpload ID="UploadFile1" runat="server" Style="display: none;" />
                                <input type="file" name="pdf_name" id="file-input" style="display: none;">
                                <div class="row collapse">
                                    <div class="large-9 columns">
                                        <input id="filename" class="file-upload" type="text" placehold="upload signed doc">
                                    </div>
                                    <div class="large-3 columns">
                                        <span class="postfix file-upload">UPLOAD</span>
                                    </div>
                                </div>--%>
                                <div style="float: left;" class="large-7 track-upload">
                                    <asp:FileUpload ID="UploadFile1" runat="server" Style="display: none;" />
                                    <div class="row collapse">
                                        <div class="large-5 small-6 columns">
                                            <span class="prefix file-upload">
                                                <asp:Localize ID="Localize23" runat="server" Text="<%$ Resources : Resource, FILESELECT %>"></asp:Localize></span></div>
                            <div class="large-7 small-6 columns">
                                <span class="postfix">
                                    <asp:Localize ID="Localize24" runat="server" Text="<%$ Resources : Resource, Nofilechosen %>"></asp:Localize></span></div>
                            </div> </div>
                            <div style="float: left;">
                                <asp:RequiredFieldValidator runat="server" Text="*" ErrorMessage="*" ControlToValidate="UploadFile1"
                                    ValidationGroup="FileUploadValidate"></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                        ID="RegularExpressionValidator2" Display="Dynamic" ValidationGroup="FileUploadValidate"
                                        runat="server" ErrorMessage="file not allowed!" ValidationExpression="^.+(.doc|.docx|.DOC|.DOCX|.txt|.TXT|.PDF|.pdf|.jpg|.JPG|.gif|.GIF|.png|.PNG|.bmp|.BMP|.jpeg|.JPEG)$"
                                        ControlToValidate="UploadFile1"></asp:RegularExpressionValidator>
                            </div>
                            </div>
                        </div>
                        <div class="button-row row">
                            <div class="large-12 columns">
                                <asp:Button ID="Accept" ValidationGroup="FileUploadValidate" Text="<%$ Resources : Resource, Accept %>" CssClass="button small border highlight right" runat="server" OnClick="Accept_Click" />
                                <asp:Button ID="Reject" Text="<%$ Resources : Resource, Back %>" CssClass="button small border right"
                                    runat="server" OnClick="Reject_Click" />
                            </div>
                        </div>
                    </div>
                    <!-- End End User Account Print Form -->
                </div>
            </div>
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
    <script type="text/javascript">
        $(document).foundation();       
    </script>
</asp:Content>
