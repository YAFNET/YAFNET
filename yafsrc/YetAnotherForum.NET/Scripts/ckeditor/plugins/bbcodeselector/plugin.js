CKEDITOR.plugins.add('bbcodeselector',
{
    requires: ['dialog'],
    lang: ['en'],
    init: function(editor) {
        window["arrayTest"] = [];
        $.getJSON(CKEDITOR.basePath.replace('Scripts/ckeditor/', '') + "resource.ashx?bbcodelist=json", function (json) {
            $.each(json, function(idx, obj) {
                window["arrayTest"].push([obj.Name, obj.Name.toLowerCase()]);
            });
        });
        var b = "bbcodeselector";
        var c = editor.addCommand(b, new CKEDITOR.dialogCommand(b));
        c.modes = { wysiwyg: 1, source: 0 };
        c.canUndo = false;
        editor.ui.addButton("bbcodeselector", {
            label: editor.lang.bbcodeselector.title,
            command: b,
            icon: this.path + "images/yafbbcode.gif"
        });
        CKEDITOR.dialog.add(b, this.path + "dialogs/bbcodeselector.js");
    }
});