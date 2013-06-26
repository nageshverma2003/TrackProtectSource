<%@ Page Title="<%$ Resources: Resource, ttlHome %>" Language="C#" MasterPageFile="~/Logon.Master"
    AutoEventWireup="true" CodeBehind="FAQ.aspx.cs" Inherits="TrackProtect.FAQ" %>

<%@ Import Namespace="Resources" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../js/vendor/jquery.js" type="text/javascript"></script>
    <link type="text/css" rel="stylesheet" href="../css/general_foundicons.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="../css/normalize.css" media="screen, projector, print" />
    <link type="text/css" rel="stylesheet" href="../css/app.css" media="screen, projector, print" />
    <script type="text/javascript" src="../js/vendor/custom.modernizr.js"></script>
    <!-- Fonts -->
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,600,800,800italic,700italic,600italic,400italic'
        rel='stylesheet' type='text/css' />
    <meta name="robots" content="noindex" />
    <script type="text/javascript" src="Scripts/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.9.0.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/tpscripts.js"></script>
    <script type="text/javascript" src="Scripts/utility.js"></script>
    <script type="text/javascript" src="Scripts/popup.js"></script>
    <style type="text/css">
        .faq h2, .member_faq h2
        {
            font-size: 1.2em;
            margin: 5px 0;
            float: left;
            clear: both;
        }
        .faq .leftColumn h1, .faq .leftColumn h2, .member_faq .leftColumn h1, .member_faq .leftColumn h2
        {
            color: green;
        }
        .faqlist
        {
            list-style: none;
            padding: 0;
            margin: 0;
            float: left;
            clear: both;
            margin: 5px 0;
            width: 100%;
        }
        .faqlist li
        {
            margin: 2px 0;
        }
        .faqlist li span
        {
            display: block;
        }
        .faqlist .question
        {
            font-weight: bold;
            padding: 2px 0;
        }
        .faqlist .answer
        {
            padding: 10px;
            background: #fff;
            margin: 10px;
            border: 1px dashed #ff9900;
        }
        .faqAll
        {
            float: right;
            font-size: 0.8em;
        }
        .member_faq h1
        {
            margin-top: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contropanel" class="row">
        <div class="large-12 columns">
            <div class="section-title to-left hide-for-small">
                <h1>
                    FAQ</h1>
            </div>
            <div class="row" style="padding-bottom: 40px;">
                <!-- Right Column -->
                <div class="large-4 columns push-8">
                </div>
                <!-- End Right Column -->
                <div class="large-8 columns pull-4">
                    <div class="section-title small to-left show-for-small">
                        <h1>
                            FAQ
                        </h1>
                    </div>
                    <div class="row  collapse tracks">
                        <asp:Literal runat="server" ID="FAQInc" />
                    </div>
                </div>
                <!-- End Miain Content -->
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
</asp:Content>
