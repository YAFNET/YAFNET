CKEDITOR.dialog.add('bbcodeselector', function(editor) {
    var parseHtml = function(htmlString) {
        htmlString = htmlString.replace(/<br>/g, '\n');
        htmlString = htmlString.replace(/&amp;/g, '&');
        htmlString = htmlString.replace(/&lt;/g, '<');
        htmlString = htmlString.replace(/&gt;/g, '>');
        htmlString = htmlString.replace(/&quot;/g, '"');
        return htmlString;
    }

    var getDefaultOptions = function(options) {
        var options = new Object();
        options.bbcodeName = null;
        options.code = '';
        return options;
    }

    var getOptionsForString = function(optionsString) {
        var options = getDefaultOptions();
        if (optionsString) {
            if (optionsString.indexOf("brush") > -1) {
                var match = /brush:[ ]{0,1}(\w*)/.exec(optionsString);
                if (match != null && match.length > 0) {
                    options.bbcodeName = match[1].replace(/^\s+|\s+$/g, "");
                }
            }
        }
        return options;
    }

    var getStringForOptions = function(optionsObject) {
        return optionsObject.bbcodeName;
    }

    return {
        title: editor.lang.bbcodeselector.title,
        minWidth: 500,
        minHeight: 400,
        onShow: function() {
            // Try to grab the selected Custom BB COde
            /*var editor = this.getParentEditor();
            var selection = editor.getSelection();
            var element = selection.getStartElement();
            var preElement = element && element.getAscendant('code', true);
            
            // Set the content for the textarea
            var text = '';
            var optionsObj = null;
            if (preElement) {
                code = parseHtml(preElement.getHtml());
                optionsObj = getOptionsForString(preElement.getAttribute('title'));
                optionsObj.code = code;
            } else {
                optionsObj = getDefaultOptions();
            }
            this.setupContent(optionsObj)*/


        },
        onOk: function() {
            var editor = this.getParentEditor();
            var selection = editor.getSelection();
            var element = selection.getStartElement();

            var data = getDefaultOptions();
            this.commitContent(data);
            var optionsString = getStringForOptions(data);
            /*
			var newElement = new CKEDITOR.dom.element(optionsString);
            newElement.setText("[" + optionsString + "]" + data.code + "[/"+ optionsString + "]");
            
			editor.insertElement(newElement);*/

            editor.insertHtml("[" + optionsString + "]" + data.code + "[/" + optionsString + "]");
        },
        contents: [
            {
                id: 'source',
                label: editor.lang.bbcodeselector.langLbl,
                accessKey: 'S',
                elements:
                [
                    {
                        type: 'vbox',
                        children: [
                            {
                                id: 'cmbBBCode',
                                type: 'select',
                                labelLayout: 'horizontal',
                                label: editor.lang.bbcodeselector.bbCodeLbl,
                                'default': 'youtube',
                                widths: ['25%', '75%'],
                                items:
                                    window["arrayTest"],
                                setup: function(data) {
                                    if (data.bbcodeName)
                                        this.setValue(data.bbcodeName);
                                },
                                commit: function(data) {
                                    data.bbcodeName = this.getValue();
                                },
                                onChange: function(data) {
                                    var dialog = this.getDialog(),
                                        bbCodeType = this.getValue(),
                                        contentBox = dialog.getContentElement('source', 'CodeBox');

                                    switch (bbCodeType) {
                                    case "youtube":
                                        {
                                            var text = editor.lang.bbcodeselector.youtube;
                                            contentBox.setValue(text);
                                        }
                                        break;
                                    case "vimeo":
                                        {
                                            var text = editor.lang.bbcodeselector.vimeo;
                                            contentBox.setValue(text);
                                        }
                                        break;
                                    case "googlewidget":
                                        {
                                            var text = editor.lang.bbcodeselector.googlewidget;
                                            contentBox.setValue(text);
                                        }
                                        break;
                                    case "spoiler":
                                        {
                                            var text = editor.lang.bbcodeselector.spoiler;
                                            contentBox.setValue(text);
                                        }
                                        break;
                                    case "userlink":
                                    {
                                        var text = editor.lang.bbcodeselector.userLink;
                                        contentBox.setValue(text);
                                    }
                                    }
                                }
                            }
                        ]
                    },
                    {
                        type: 'textarea',
                        id: 'CodeBox',
                        rows: 22,
                        style: "width: 100%",
                        setup: function(data) {
                            if (data.code)
                                this.setValue(data.code);
                        },
                        commit: function(data) {
                            data.code = this.getValue();
                        }
                    }
                ]
            }
        ]
    };
});
