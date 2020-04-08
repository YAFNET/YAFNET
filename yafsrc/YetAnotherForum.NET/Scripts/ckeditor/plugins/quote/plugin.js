CKEDITOR.plugins.add("quote",
    {
        lang: ["en"],
        init: function(editor) {
            editor.addCommand("quote",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function() {
                        var writer = new CKEDITOR.htmlWriter();

                        var selection = editor.getSelection();
                        if (!selection) {
                            CKEDITOR.htmlParser.fragment.fromBBCode("[quote] [/qoute]").writeHtml(writer);

                            editor.insertHtml(writer.getHtml());
                        } else {

                            var text = selection.getSelectedText();

                            CKEDITOR.htmlParser.fragment.fromBBCode("[quote]" + text + "[/qoute]").writeHtml(writer);

                            editor.insertHtml(writer.getHtml());
                        }
                    }
                });

            editor.ui.addButton("quote",
                {
                    label: editor.lang.quote.title,
                    command: "quote",
                    icon: this.path + "quote-left-solid.svg"
                });
        }
    });