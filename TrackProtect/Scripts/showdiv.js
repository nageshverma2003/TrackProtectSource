function showDiv(module) {
	try {
	    var arrayOfDivs = document.getElementsByTagName('div');
		for(i = 0; i < arrayOfDivs.length; i++) {
	        var name = arrayOfDivs[i].id;
	        if (name.substr(0, 7) == 'module_') {
				document.getElementById(name).style.display = 'none';
			}
	    }
	    document.getElementById(module).style.display = 'block';
    } catch (err) {
    	alert(err)
    }
}
