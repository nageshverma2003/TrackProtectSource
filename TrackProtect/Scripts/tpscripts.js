// Scripts by Hans Grimm - www.grimm.nl

$(function() {
	
	//$('.faq .leftColumn h1').append('<span class="faqAll"><a href="javascript:;" class="openall">Klap alles open/dicht</a></span>');
	//$('.openall').click(function() {
		//$('.answer').slideToggle(500);
		//$(this).removeClass('openall').addClass('closeall');
	//});
	
	// ### Faqlist
	$('.faqlist .answer').hide();
	$('.faqlist .question').each(function() {
		var animSpeed = 500;
		$(this).wrap('<a href="javascript:;"></a>');
		$(this, 'a').click(function(){
			$(this).closest('ul').siblings().find('.answer').slideUp(animSpeed);
			$(this).closest('li').siblings().find('.answer').slideUp(animSpeed);
			$(this).closest('li').find('.answer').slideToggle(500);
		});
	});
	
	// ### validation upload tracks form
	// check file size
	$('.registerdocument .fileUpload').bind('change', function() {
		var fileSize = this.files[0].size;
			if (fileSize > 11000000) {
				alert('Bestand is te groot');
				$(this).val('');
			}
	});
	// check if mp3 file
	$('#ctl00_MainContent_FileUpload1').bind('change', function() {
		var ext = $(this).val().split('.').pop().toLowerCase();
		if($.inArray(ext, ['mp3']) == -1) {
    		alert('Geen MP3 bestand');
    		$(this).val('');
    	}
	});
	// check if correct text file 
	$('#ctl00_MainContent_FileUpload2').bind('change', function() {
		var ext = $(this).val().split('.').pop().toLowerCase();
		if($.inArray(ext, ['doc','docx','txt','rtf','pdf']) == -1) {
    		alert('Geldige bestanden: DOC, DOCX, TXT, RTF, PDF');
    		$(this).val('');
    	}
	});
	// check if correct text file 
	$('#ctl00_MainContent_FileUpload3').bind('change', function() {
		var ext = $(this).val().split('.').pop().toLowerCase();
		if($.inArray(ext, ['doc','docx','txt','rtf','pdf']) == -1) {
    		alert('Geldige bestanden: DOC, DOCX, TXT, RTF, PDF');
    		$(this).val('');
    	}
	});
	// on submit, check if mp3 upload field is not empty
	$('#ctl00_MainContent_RegisterDocumentButton').click(function() { 
		if ($('#ctl00_MainContent_FileUpload1').val() == '') {
			alert('Geen MP3 bestand opgegeven');
			return false;
		}
		//alert('x')
	});
	
	// ### Info
	
	$('span.info').each(function(){
		var infoText = $(this).attr('title');		
		//console.log(infoText);
		$(this).after('<span class="infoTextPopup">' + infoText + '</span>');
		$(this).attr('title','').css({'cursor':'pointer'});
		$('.infoTextPopup').hide();
		$(this).mouseenter(function(){
			$(this).next('.infoTextPopup').show();
		});
		$(this).mouseleave(function(){
			$(this).next('.infoTextPopup').hide();
		});
	});
	
});