function CKEditor_Load() {
       if (arguments.callee.done) return;

       arguments.callee.done = true;
	   
	   CKEDITOR.replaceAll(function( textarea, config ){
		  
		  config.extraPlugins = 'bbcode,syntaxhighlight,bbcodeselector';
		  config.toolbar_Full = [
		                         ['Source'],
		                         ['Undo','Redo'],
								 ['-','NumberedList','BulletedList'], 
								 ['-','Link', 'Unlink', 'Image'], 
		                         ['Blockquote', 'syntaxhighlight','bbcodeselector'],
		                         ['SelectAll', 'RemoveFormat'],
								 ['About'],
								 '/',
								 ['Bold','Italic','Underline','-', 'TextColor', 'Font', 'FontSize'],
								 ['JustifyLeft','JustifyCenter','JustifyRight'],
		                        ] ;
								
			config.entities_greek = false;
			config.entities_latin = false;
			
			config.contentsCss = 'editors/ckeditor/contents.css';
		  });
};
   
   if (document.addEventListener) {
       document.addEventListener("DOMContentLoaded", CKEditor_Load, false);
   }

window.onload = CKEditor_Load;