function CKEditor_Load() {
       if (arguments.callee.done) return;

       arguments.callee.done = true;
	   
	   CKEDITOR.replaceAll(function( textarea, config ){
		  
		   config.extraPlugins = 'bbcode,syntaxhighlight';
		  config.toolbar_Full = [
		                         ['Source'],
		                         ['Undo','Redo'],
								 ['-','NumberedList','BulletedList'], 
								 ['-','Link', 'Unlink', 'Image'], 
		                         ['Blockquote', 'syntaxhighlight'],
		                         ['SelectAll', 'RemoveFormat'],
								 ['About'],
								 '/',
								 ['Bold','Italic','Underline','-', 'TextColor', 'Font', 'FontSize'],
								 ['JustifyLeft','JustifyCenter','JustifyRight'],
		                        ] ;

// Add the BBCode plugin.
CKEDITOR.config.extraPlugins = 'bbcode';
		  });
};
   
   if (document.addEventListener) {
       document.addEventListener("DOMContentLoaded", CKEditor_Load, false);
   }

window.onload = CKEditor_Load;