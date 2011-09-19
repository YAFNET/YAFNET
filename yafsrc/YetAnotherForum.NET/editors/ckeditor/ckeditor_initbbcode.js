function CKEditor_Load() {
       if (arguments.callee.done) return;

       arguments.callee.done = true;
	   
	     CKEDITOR.replaceAll(function( textarea, config ){
	      
		  config.extraPlugins = 'bbcode,syntaxhighlight,bbcodeselector';
		  config.disableNativeSpellChecker = false;
		  config.scayt_autoStartup = true;  
		  
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
								 ['Scayt']
		                        ] ;
								
			config.entities_greek = false;
			config.entities_latin = false;
			config.language = editorLanguage;
			
			config.contentsCss = 'editors/ckeditor/contents.css';
		  });
};

CKEDITOR.on( 'dialogDefinition', function( ev )
	{
		var tab, name = ev.data.name,
			definition = ev.data.definition;

		if ( name == 'link' )
		{
			definition.removeContents( 'target' );
			definition.removeContents( 'upload' );
			definition.removeContents( 'advanced' );
			tab = definition.getContents( 'info' );
			tab.remove( 'emailSubject' );
			tab.remove( 'emailBody' );
		}
		else if ( name == 'image' )
		{
			definition.removeContents( 'advanced' );
			tab = definition.getContents( 'Link' );
			tab.remove( 'cmbTarget' );
			tab = definition.getContents( 'info' );
			tab.remove( 'txtAlt' );
			tab.remove( 'basic' );
		}
	});
   
   if (document.addEventListener) {
       document.addEventListener("DOMContentLoaded", CKEditor_Load, false);
   }

window.onload = CKEditor_Load;