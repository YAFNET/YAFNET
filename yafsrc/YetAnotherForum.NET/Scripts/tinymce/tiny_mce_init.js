tinyMCE.init({
	// General options
	selector: "textarea",
	language: editorLanguage,
    menu: { 
        edit: {title: 'Edit', items: 'undo redo | cut copy paste | selectall'}, 
        insert: {title: 'Insert', items: '|'}, 
        view: {title: 'View', items: 'visualaid'}, 
        format: {title: 'Format', items: 'bold italic underline strikethrough superscript subscript | formats | removeformat'}
    },
	convert_urls : false,
	
	// turn on/off SCAYT autostartup  
	scayt_auto_startup: true,  
	scayt_slang: "en_US", 

	// Replace values for the template plugin
	template_replace_values : {
		username : "Some User",
		staffid : "991234"
	},
	spellchecker_rpc_url : "editors/tiny_mce/spell.asp",
    spellchecker_languages : "+English=en,Danish=da,Dutch=nl,Finnish=fi,French=fr,German=de,Italian=it,Polish=pl,Portuguese=pt,Spanish=es,Swedish=sv"

});