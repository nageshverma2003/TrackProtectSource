//function SoundCloudAuthorize() {
//    alert('going to soundcloud');
//    SC.initialize({
//        client_id: 'e71491a4c5b1572be1f6389f533f4ef6',
//        redirect_uri: 'http://localhost:4508/Social/soundcloud.aspx'
//    });

//    SC.connect(function () {
//        SC.get('/me', function (me) {
//            alert('Hello, ' + me.username);
//        });
//    });
//    //window.open("/Social/soundcloud.aspx", "Sound Cloud", "menubar=0,resizable=1,width=900,height=475");
//}
function SoundCloudAuthorize() {
    alert('going to soundcloud');
    SC.initialize({
        client_id: 'e26a1f8c7ee5728378d12909075fa6bf',
        redirect_uri: 'http://localhost:4508/Social/soundcloud.aspx'
    });

    SC.connect(function () {
        SC.get('/me', function (me) {
            alert('Hello, ' + me.username);
        });
    });
    //window.open("/Social/soundcloud.aspx", "Sound Cloud", "menubar=0,resizable=1,width=900,height=475");
}