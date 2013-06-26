<%@ Page Title="" Language="C#" MasterPageFile="~/Logon.Master" AutoEventWireup="true"
    CodeBehind="idlookup.aspx.cs" Inherits="TrackProtect.idlookup" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.9.0.custom.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#Contact').click(function () {
                if ($('#divContact').css("display") == 'none') {
                    $('#divContact').fadeIn('slow');
                }
                else {
                    $('#divContact').fadeOut('slow');
                }
            });
        });        
    </script>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row">
        <div id="custom">
            <div class="large-12 columns">
                <div class="section-title to-left hide-for-small">
                    <h1>
                        <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources : Resource, YourTrackInformation %>"></asp:Localize></h1>
                </div>
                <div class="row">
                    <!-- Right Column -->
                    <div class="large-4 columns push-8">
                    </div>
                    <!-- End Right Column -->
                    <div class="large-8 columns pull-4">
                        <div class="section-title small to-left show-for-small">
                            <h1>
                                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources : Resource, YourTrackInformation %>"></asp:Localize>
                            </h1>
                        </div>
                        <div class="row">
                            <div class="row">
                                <div class="large-4 columns">
                                    <p class="title">
                                        <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources : Resource, StageName %>"></asp:Localize></p>
                                </div>
                                <div class="large-8 columns">
                                    <p>
                                        <asp:Label ID="StageName" runat="server"></asp:Label></p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-4 columns">
                                    <p class="title">
                                        <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources : Resource, TrackName %>"></asp:Localize></p>
                                </div>
                                <div class="large-8 columns">
                                    <p>
                                        <asp:Label ID="TrackName" runat="server"></asp:Label></p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-4 columns">
                                    <p class="title">
                                        <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources : Resource, ISRCCode %>"></asp:Localize></p>
                                </div>
                                <div class="large-8 columns">
                                    <p>
                                        <asp:Label ID="ISRCCode" runat="server"></asp:Label></p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-4 columns">
                                    <p class="title">
                                        <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources : Resource, AddedTags %>"></asp:Localize></p>
                                </div>
                                <div class="large-8 columns">
                                    <p>
                                        <asp:Label ID="AddedTags" runat="server"></asp:Label></p>
                                </div>
                            </div>
                            <br />
                            <br />
                            <div class="row">
                                <div class="large-4 columns">
                                    <p class="title">
                                        <asp:Localize ID="Localize7" runat="server" Text="Contact"></asp:Localize></p>
                                </div>
                                <div class="large-8 columns">
                                    <p>
                                        <a id="Contact" style="cursor: pointer; text-decoration: underline;">support@trackprotect.com</a></p>
                                </div>
                            </div>
                            <br />
                            <br />
                            <div id="divContact" style="display: none; border: 1px solid #E4510A;">
                                <div style="margin: 11px 11px 60px 16px;">
                                    <!-- Contact Form -->
                                    <form id="Form1" method="post" action="">
                                    <label>
                                        <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources : Resource, Subject %>"></asp:Localize></label>
                                    <asp:TextBox runat="server" ID="Subject"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="Subject"
                                        CssClass="failureNotification" ErrorMessage="<%$ Resources : Resource, SubjectRequired %>"
                                        ValidationGroup="RegisterUserValidationGroup"></asp:RequiredFieldValidator>
                                    <br />
                                    <label>
                                        <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources : Resource, Name %>"></asp:Localize></label>
                                    <asp:TextBox runat="server" ID="Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Name"
                                        CssClass="failureNotification" ErrorMessage="<%$ Resources : Resource, NameRequired %>"
                                        ValidationGroup="RegisterUserValidationGroup"></asp:RequiredFieldValidator>
                                    <br />
                                    <label>
                                        <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: Resource, DefaultContactEmail %>" /></label>
                                    <asp:TextBox runat="server" ID="Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                        CssClass="failureNotification" ErrorMessage="<%$ Resources: Resource, EmailRequired %>"
                                        ValidationGroup="RegisterUserValidationGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="EmailValidate" ControlToValidate="Email" ValidationExpression="^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$"
                                        ErrorMessage="<%$ Resources : Resource, EmailNotValid %>" ValidationGroup="RegisterUserValidationGroup"
                                        runat="server">
                                    </asp:RegularExpressionValidator>
                                    <br />
                                    <label>
                                        <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources: Resource, DefaultContactMesage %>" /></label>
                                    <asp:TextBox runat="server" ID="Message" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Message"
                                        CssClass="failureNotification" ErrorMessage="<%$ Resources : Resource, MessageRequired %>"
                                        ValidationGroup="RegisterUserValidationGroup"></asp:RequiredFieldValidator>
                                    <br />
                                    <asp:Button ID="Send" runat="server" Text="<%$ Resources: Resource, DefaultContactSend %>"
                                        ValidationGroup="RegisterUserValidationGroup" CssClass="button small border right"
                                        OnClick="Send_Click" />
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- End Miain Content -->
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
</asp:Content>
