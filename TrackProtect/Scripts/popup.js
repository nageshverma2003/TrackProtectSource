/*
javascript for content-centric info display
*/
var infoWin;
var infoDiv;
function contentPopup(url, winEl, left, top, width, height) {
    if (infoDiv != null)
        resetPopup();

    infoDiv = document.createElement("div");
    infoDiv.className = "popUp";
    infoDiv.style.left = left;
    infoDiv.style.top = top;
    document.body.appendChild(infoDiv);

    this.iframe = document.createElement("iframe");
    this.iframe.className = "popUpIframe";
    this.iframe.src = url;
    this.iframe.width = width;
    this.iframe.height = height;
    this.iframe.frameborder = 0;
    this.iframe.scrolling = "no";
    infoDiv.appendChild(this.iframe);
    infoDiv.style.display = "block";
}

function showPopup(url, left, top, width, height) {
    infoWin = new contentPopup(url, this.id, left, top, width, height);
}

function resetPopup() {
    if (infoDiv != null)
        infoDiv.style.display = "none";
}

function closePopup(referingPage, reloadDoc) {
    var parentWin = document.parentWindow;
    if (parentWin != null) {
        parentWin.frameElement.parentElement.style.display = "none";
        if (reloadDoc) {
            var srcLoc = parentWin.frameElement.parentElement.parentElement.parentElement.document.location;
            var dest = srcLoc.protocol + "//" + srcLoc.host + srcLoc.pathname;
            var pos = dest.lastIndexOf('/');
            if (pos > -1)
                dest = dest.substring(0, pos);
            referingPage = referingPage.replace('~', dest);
            parentWin.frameElement.parentElement.parentElement.document.location.href = referingPage;
            //parentWin.frameElement.parentElement.parentElement.document.location.reload();
        }
    }
}

function reloadDocument() {
    document.location.reload();
}

function showMessageIfPending() {
    var infoEl = document.getElementById('ctl00_hfdMessage');
    if (infoEl != null && infoEl.value != null) {
        var data = infoEl.value;

        var pos = data.indexOf("|");
        if (pos > -1) {
            var type = data.substring(0, pos);
            var msg = data.substring(pos + 1);

            if (type == "Warnings" || type == "Errors")
                showPopup("/" + type + ".aspx", 150, 100, 500, 500);
            else
                showPopup("/" + type + ".aspx", 150, 100, 300, 200);
        }
    }
}

function openDialog() {
    $("#dialogDiv").dialog({
        modal: true,
        buttons: {
            Ok: function() {
                $(this).dialog("close");
            }
        }
    });
}

function showDialogIfPending() {
    var infoEl = document.getElementById('ctl00_hfdMessage');
    if (infoEl != null && infoEl.value != null) {
        var data = infoEl.value;
        if (data != null)
            openDialog();
    }
}

function hideWatermark(theID) {
    var element = document.getElementById(theID);
    element.style.backgroundImage = 'none';
    element.style.backgroundColor = 'white';
}

function showUsername(theID) {
    var element = document.getElementById(theID);
    if (element == null)
        return;

    if (element.value.length == 0)
        element.style.backgroundImage = 'url("../Images/mask_username.png")';
    else
        element.style.backgroundColor = 'white';
}

function showPassword(theID) {
    var element = document.getElementById(theID);
    if (element == null)
        return;

    if (element.value.length == 0)
        element.style.backgroundImage = 'url("../Images/mask_password.png")';
    else
        element.style.backgroundColor = 'white';
}

function showWatermark(theID, imageUrl) {
    var element = document.getElementById(theID);
    if (element == null)
        return;

    if (element.value.length == 0)
        element.style.backgroundImage = 'url(imageUrl)';
    else
        element.style.backgroundColor = 'white';
}

function startDownload(url) {
    window.open(url, 'Download');
}

function initPage() {
    //showMessageIfPending();
    showDialogIfPending();
    showUsername('ctl00_HeadLoginView_Login1_UserName');
    showPassword('ctl00_HeadLoginView_Login1_Password');
}
