function Focus(objname, waterMarkText) {
    obj = document.getElementById(objname);
    if (obj) {
    	if (obj.value == waterMarkText)
	    	obj.value = "";
	    obj.className = "NormalTextBox";
	    if (obj.value == "Gebruikersnaam" || 
	    	obj.value == "Username" || 
	    	obj.value == "" || 
	    	obj.value == null) {
	        obj.style.color = "black";
	    }
    }
}

function Blur(objname, waterMarkText) {
    obj = document.getElementById(objname);
    if (obj.value == "") {
        obj.value = waterMarkText;
        if (objname != "txtPwd" && 
        	objname != "Password" && 
        	objname != "ctl00_HeadLoginView_Login1_Password") {
            obj.className = "WaterMarkedTextBox";
        } else {
            obj.className = "WaterMarkedTextBoxPSW";
        }
    } else {
        obj.className = "NormalTextBox";
    }

    if (obj.value == "Gebruikersnaam" || 
    	obj.value == "Username" || 
    	obj.value == "" || 
    	obj.value == null) {
        obj.style.color = "gray";
    }
}
