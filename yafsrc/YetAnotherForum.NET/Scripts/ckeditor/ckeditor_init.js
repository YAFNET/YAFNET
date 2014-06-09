function CKEditor_Load() {
       if (arguments.callee.done) return;

       arguments.callee.done = true;
	   
	   
	   CKEDITOR.replaceAll(function( textarea, config ){

		  config.disableNativeSpellChecker = false;
		  config.scayt_autoStartup = true; 
		  
	      config.extraPlugins = 'bbcodehtml,syntaxhighlight,bbcodeselector';
	      config.toolbar = [
                                 ['Source'],
		                         ['Cut', 'Copy', 'Paste'], ['Undo', 'Redo', '-', 'Find', 'Replace', '-', 'SelectAll', 'RemoveFormat'],
								 ['-', 'NumberedList', 'BulletedList'],
								 ['-', 'Link', 'Unlink', 'Image'],
		                         ['Blockquote', 'syntaxhighlight', 'bbcodeselector'],
		                         ['SelectAll', 'RemoveFormat'],
								 ['About'],
								 '/',
								 ['Bold', 'Italic', 'Underline', '-', 'TextColor', 'Font', 'FontSize'],
								 ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'PasteText','PasteFromWord'],
		                         ['Outdent', 'Indent'],
								 ['Scayt']
								] ;
								
			config.entities_greek = false;
			config.entities_latin = false;
			config.language = editorLanguage;
			
			config.contentsCss = 'editors/ckeditor/contents.css';
		  });
};
   
   if (document.addEventListener) {
       document.addEventListener("DOMContentLoaded", CKEditor_Load, false);
   }

window.onload = CKEditor_Load;