CKEDITOR.plugins.add('bbcodeselector',   
  {    
    requires: ['dialog'],
	lang : ['en'], 
    init:function(a) { 
	var b="bbcodeselector";
	var c=a.addCommand(b,new CKEDITOR.dialogCommand(b));
		c.modes={wysiwyg:1,source:0};
		c.canUndo=false;
	a.ui.addButton("bbcodeselector",{
					label:a.lang.bbcodeselector.title,
					command:b,
					icon:this.path+"images/yafbbcode.gif"
	});
	CKEDITOR.dialog.add(b,this.path+"dialogs/bbcodeselector.js")}
});