jQuery(document).ready(function() {
    var yafCKEditor = jQuery('textarea.YafTextEditor').ckeditor({
        extraPlugins: 'autosave,bbcode,syntaxhighlight,bbcodeselector,codemirror',
        autosave_saveDetectionSelectors: "a[id*='_PostReply'],a[id*='Cancel'],a[id*='_Preview']",
        toolbar: [
            ['Source'],
            ['Undo', 'Redo'],
            ['-', 'NumberedList', 'BulletedList'],
            ['-', 'Link', 'Unlink', 'Image'],
            ['Blockquote', 'syntaxhighlight', 'bbcodeselector'],
            ['SelectAll', 'RemoveFormat'],
            ['About'],
            '/',
            ['Bold', 'Italic', 'Underline', '-', 'TextColor', 'Font', 'FontSize'],
            ['JustifyLeft', 'JustifyCenter', 'JustifyRight'],
            ['Outdent', 'Indent']
        ],
        entities_greek: false,
        entities_latin: false,
        language: editorLanguage,
        disableObjectResizing: true,
        forcePasteAsPlainText: true,
        contentsCss: 'Scripts/ckeditor/yaf_contents.css',
        codemirror:
        {
            mode: 'bbcode'
        }
    });

    jQuery("a[id*='_PostReply'],a[id*='_Save'],a[id*='_Preview']").click(function () {
        yafCKEditor.editor.updateElement();
    });

    yafCKEditor.editor.addCommand('highlight', {
        modes: { wysiwyg: 1, source: 1 },
        exec: function(editor) {
            var selection = editor.getSelection();
            if (!selection) {
                editor.insertHtml('[h]' + '[/h]');
            }
            var text = selection.getSelectedText();

            editor.insertHtml('[h]' + text + '[/h]');
        }
    });

    yafCKEditor.editor.addCommand('postmessage', {
        modes: { wysiwyg: 1, source: 1 },
        exec: function () {
            yafCKEditor.editor.updateElement();
            if (jQuery("a[id*='_PostReply']").length) {
                __doPostBack(jQuery("a[id*='_PostReply']").attr('id').replace('_', '$').replace('_', '$'), '');
            } else if (jQuery("a[id*='_Save']").length) {
                __doPostBack(jQuery("a[id*='_Save']").attr('id').replace('_', '$').replace('_', '$'), '');
            }
        }
    });
});

CKEDITOR.on( 'dialogDefinition', function(ev) {
    var tab,
        name = ev.data.name,
        definition = ev.data.definition;

    if (name == 'link') {
        definition.removeContents('target');
        definition.removeContents('upload');
        definition.removeContents('advanced');
        tab = definition.getContents('info');
        tab.remove('emailSubject');
        tab.remove('emailBody');
    } else if (name == 'image') {
        definition.removeContents('advanced');
        tab = definition.getContents('Link');
        tab.remove('cmbTarget');
        tab = definition.getContents('info');
        tab.remove('txtAlt');
        tab.remove('basic');
    }
});