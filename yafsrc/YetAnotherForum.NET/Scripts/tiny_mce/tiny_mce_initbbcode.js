tinyMCE.init({
	selector: "textarea",
	language: editorLanguage,
	plugins : "bbcode,spellchecker",	
	toolbar : "bold,italic,underline,undo,redo,link,unlink,image,forecolor,removeformat,cleanup,code,scayt,spellchecker",
    menubar : false,
	//theme_advanced_styles : "Code=codeStyle;Quote=quoteStyle",
	entity_encoding : "raw",
	convert_fonts_to_spans: false,
	convert_urls : false,
	language: editorLanguage,
	spellchecker_rpc_url : "editors/tiny_mce/spell.asp",
    spellchecker_languages : "+English=en,Danish=da,Dutch=nl,Finnish=fi,French=fr,German=de,Italian=it,Polish=pl,Portuguese=pt,Spanish=es,Swedish=sv"
});