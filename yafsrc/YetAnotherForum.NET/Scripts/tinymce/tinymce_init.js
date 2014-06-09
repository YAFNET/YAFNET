tinyMCE.init({
    // General options
    selector: "textarea",
    language: editorLanguage,
    menu: {
        edit: { title: 'Edit', items: 'undo redo | cut copy paste | selectall' },
        insert: { title: 'Insert', items: '|' },
        view: { title: 'View', items: 'visualaid' },
        format: { title: 'Format', items: 'bold italic underline strikethrough superscript subscript | formats | removeformat' }
    },
    convert_urls: false,

    // Replace values for the template plugin
    template_replace_values: {
        username: "Some User",
        staffid: "991234"
    }

});