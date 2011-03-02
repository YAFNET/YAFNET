function CKEditor_Load() {
       if (arguments.callee.done) return;

       arguments.callee.done = true;
	   
	   
	   CKEDITOR.replaceAll(function( textarea, config ){

	      config.extraPlugins = 'syntaxhighlight,bbcodeselector';
	      config.toolbar_Full = [
                                 ['Source'],
		                         ['Cut', 'Copy', 'Paste'], ['Undo', 'Redo', '-', 'Find', 'Replace', '-', 'SelectAll', 'RemoveFormat'],
								 ['-', 'NumberedList', 'BulletedList'],
								 ['-', 'Link', 'Unlink', 'Image'],
		                         ['Blockquote', 'syntaxhighlight', 'bbcodeselector'],
		                         ['SelectAll', 'RemoveFormat'],
								 ['About'],
								 '/',
								 ['Bold', 'Italic', 'Underline', '-', 'TextColor', 'Font', 'FontSize'],
								 ['JustifyLeft', 'JustifyCenter', 'JustifyRight']
								] ;
		  });
};
   
   if (document.addEventListener) {
       document.addEventListener("DOMContentLoaded", CKEditor_Load, false);
   }

window.onload = CKEditor_Load;