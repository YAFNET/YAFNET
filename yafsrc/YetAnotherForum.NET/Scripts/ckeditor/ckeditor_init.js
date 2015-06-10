jQuery(document).ready(function() {
    jQuery('textarea.YafTextEditor').ckeditor({
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
});