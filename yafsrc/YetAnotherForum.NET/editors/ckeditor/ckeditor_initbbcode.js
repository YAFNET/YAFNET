function CKEditor_Load() {
       if (arguments.callee.done) return;

       arguments.callee.done = true;
	   
	   CKEDITOR.replaceAll(function( textarea, config ){
		  
		   config.extraPlugins = 'bbcode,syntaxhighlight';
		  config.toolbar_Full = [
		                         ['Source'],
		                         ['Undo','Redo'],
		                         ['Bold','Italic','Underline','-','Link', 'Unlink'], 
		                         ['Blockquote', 'syntaxhighlight', 'TextColor', 'Image'],
		                         ['SelectAll', 'RemoveFormat'],
								 ['About']
		                        ] ;

// Add the BBCode plugin.
CKEDITOR.config.extraPlugins = 'bbcode';
		  });
};
   
   if (document.addEventListener) {
       document.addEventListener("DOMContentLoaded", CKEditor_Load, false);
   }

window.onload = CKEditor_Load;