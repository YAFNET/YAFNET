CKEDITOR.plugins.add("highlight",
    {
        lang: ["en"],
        init: function(editor) {
            editor.addCommand("highlight",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function() {
                        var writer = new CKEDITOR.htmlWriter();

                        var selection = editor.getSelection();
                        if (!selection) {
                            CKEDITOR.htmlParser.fragment.fromBBCode("[h] [/h]").writeHtml(writer);

                            editor.insertHtml(writer.getHtml());
                        } else {

                            var text = selection.getSelectedText();

                            CKEDITOR.htmlParser.fragment.fromBBCode("[h]" + text + "[/h]").writeHtml(writer);

                            editor.insertHtml(writer.getHtml());
                        }
                    }
                });

            editor.ui.addButton("highlight",
                {
                    label: editor.lang.highlight.title,
                    command: "highlight",
                    icon: this.path + "pen-square-solid.svg"
                });
        }
    });