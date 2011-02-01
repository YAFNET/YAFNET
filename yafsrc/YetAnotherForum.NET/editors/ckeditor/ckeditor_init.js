function CKEditor_Load() {
       if (arguments.callee.done) return;

       arguments.callee.done = true;
	   
	   
	   CKEDITOR.replaceAll(function( textarea, config ){
		  
		  config.toolbar_Full = [ 
		                          ['Cut','Copy','Paste'],['Undo','Redo','-','Find','Replace','-','SelectAll','RemoveFormat'],['Link','Unlink','Image'],['About'],'/', 
		                          ['Bold','Italic','Underline','Strike'],['Font','FontSize'],['TextColor','BGColor']
								] ;
		  });
};
   
   if (document.addEventListener) {
       document.addEventListener("DOMContentLoaded", CKEditor_Load, false);
   }

window.onload = CKEditor_Load;