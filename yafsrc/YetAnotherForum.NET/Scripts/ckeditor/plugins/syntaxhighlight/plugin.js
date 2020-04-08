CKEDITOR.plugins.add( 'syntaxhighlight', {
	requires : 'dialog',
	lang : 'en,de,fr,zh-cn', // %REMOVE_LINE_CORE%
	init : function( editor ) {
		editor.addCommand( 'syntaxhighlightDialog', new CKEDITOR.dialogCommand('syntaxhighlightDialog') );
		editor.ui.addButton && editor.ui.addButton( 'Syntaxhighlight',
		{
			label : editor.lang.syntaxhighlight.title,
			command : 'syntaxhighlightDialog',
			icon: this.path + 'icons/code-solid.svg'
		} );

		if ( editor.contextMenu ) {
			editor.addMenuGroup( 'syntaxhighlightGroup' );
			editor.addMenuItem( 'syntaxhighlightItem', {
				label: editor.lang.syntaxhighlight.contextTitle,
				icon: this.path + 'icons/code-solid.svg',
				command: 'syntaxhighlightDialog',
				group: 'syntaxhighlightGroup'
			});
		
		}

		CKEDITOR.dialog.add( 'syntaxhighlightDialog', this.path + 'dialogs/syntaxhighlight.js' );
	
	}
});
