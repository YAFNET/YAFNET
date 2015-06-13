jQuery(document).ready(function () {
    var yafCKEditor = jQuery('textarea.YafTextEditor').ckeditor({
        extraPlugins: 'autosave,bbcode,syntaxhighlight,bbcodeselector,codemirror',
        autosave_saveDetectionSelectors: "a[id*='_PostReply'],a[id*='Cancel']",
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

    jQuery("a[id*='_PostReply'],a[id*='_Save']").click(function () {
        yafCKEditor.editor.updateElement();
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