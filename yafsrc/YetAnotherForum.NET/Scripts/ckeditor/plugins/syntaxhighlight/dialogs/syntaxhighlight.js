CKEDITOR.dialog.add('syntaxhighlightDialog', function(editor) {
	
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
        title: editor.lang.syntaxhighlight.title,
        minWidth: 500,
        minHeight: 400,
        onShow: function() {
            
        },
        onOk: function() {
            var editor = this.getParentEditor();
            var selection = editor.getSelection();
            var element = selection.getStartElement();

            var data = getDefaultOptions();
            this.commitContent(data);
            var optionsString = getStringForOptions(data);
           
            editor.insertHtml("[code=" + optionsString + "]" + data.code + "[/code]");
        },
        contents: [
            {
                id: 'source',
                label: editor.lang.syntaxhighlight.sourceTab,
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
                                label: editor.lang.syntaxhighlight.langLbl,
                                'default': 'markup',
                                widths: ['25%', '75%'],
                                items:
								[
								["Plain Text", "markup"],
								[   "Bash(shell)","bash" ],
                                        [   "C","c" ],
                                        [   "C++","cpp" ],
                                        [   "C#","csharp" ],
                                        [   "CSS","css" ],
										
                                        [   "SCSS","scss" ],
                                        [   "Git","git" ],
                                        [   "HTML","markup" ],
                                        [   "Java","java" ],
                                        [   "JavaScript","javascript" ],
                                        [   "Python","python" ],
                                        [   "SQL","sql" ],
                                        [   "XML","markup" ],
                                        [  "Visual Basic","vb" ]
								],
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
                                }
                            }
                        ]
                    },
                    {
                        type: 'textarea',
						label: editor.lang.syntaxhighlight.sourceTab,
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
