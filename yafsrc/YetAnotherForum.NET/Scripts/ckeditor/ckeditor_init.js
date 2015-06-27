jQuery(document).ready(function() {
    var yafCKEditor = jQuery('textarea.YafTextEditor').ckeditor({
        extraPlugins: 'autosave,bbcodehtml,syntaxhighlight,bbcodeselector,codemirror,textselection,wordcount',
        autosave_saveDetectionSelectors: "a[id*='_PostReply'],a[id*='Cancel']",
        toolbar: [
            ['Source'],
            ['Cut', 'Copy', 'Paste'], ['Undo', 'Redo', '-', 'Find', 'Replace', '-', 'SelectAll', 'RemoveFormat'],
            ['-', 'NumberedList', 'BulletedList'],
            ['-', 'Link', 'Unlink', 'Image'],
            ['Blockquote', 'syntaxhighlight', 'bbcodeselector'],
            ['SelectAll', 'RemoveFormat'],
            ['About'],
            '/',
            ['Bold', 'Italic', 'Underline', '-', 'TextColor', 'Font', 'FontSize'],
            ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'PasteText', 'PasteFromWord'],
            ['Outdent', 'Indent']
        ],
        entities_greek: false,
        entities_latin: false,
        language: editorLanguage,
        contentsCss: 'Scripts/ckeditor/yaf_contents.css',
        codemirror:
        {
            mode: 'bbcodemixed'
        }
    });

    jQuery("a[id*='_PostReply'],a[id*='_Save']").click(function () {
        yafCKEditor.editor.updateElement();
    });

    yafCKEditor.editor.addCommand('highlight', {
        modes: { wysiwyg: 1, source: 1 },
        exec: function (editor) {
            var selection = editor.getSelection();
            if (!selection) {
                editor.insertHtml('[h]' + '[/h]');
            }
            var text = selection.getSelectedText();

            editor.insertHtml('[h]' + text + '[/h]');
        }
    });

    yafCKEditor.editor.addCommand('codeblock', {
        modes: { wysiwyg: 1, source: 1 },
        exec: function (editor) {
            var selection = editor.getSelection();
            if (!selection) {
                editor.insertHtml('[code]' + '[/code]');
            }
            var text = selection.getSelectedText();

            editor.insertHtml('[code]' + text + '[/code]');
        }
    });

    yafCKEditor.editor.addCommand('postmessage', {
        modes: { wysiwyg: 1, source: 1 },
        exec: function () {
            if (jQuery("a[id*='_PostReply']").length) {
                __doPostBack(jQuery("a[id*='_PostReply']").attr('id').replace('_', '$').replace('_', '$'), '');
            } else if (jQuery("a[id*='_Save']").length) {
                __doPostBack(jQuery("a[id*='_Save']").attr('id').replace('_', '$').replace('_', '$'), '');
            }
        }
    });
});