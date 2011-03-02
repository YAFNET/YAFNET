CKEDITOR.dialog.add('syntaxhighlight', function(editor)
{    
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
        options.hideGutter = false;
        options.hideControls = false;
        options.collapse = false;
        options.showColumns = false;
        options.noWrap = false;
        options.firstLineChecked = false;
        options.firstLine = 0;
        options.highlightChecked = false;
        options.highlight = null;
        options.lang = null;
        options.code = '';
        return options;
    }
    
    var getOptionsForString = function(optionsString) {
        var options = getDefaultOptions();
        if (optionsString) {
            if (optionsString.indexOf("brush") > -1) {
                var match = /brush:[ ]{0,1}(\w*)/.exec(optionsString);
                if (match != null && match.length > 0) {
                    options.lang = match[1].replace(/^\s+|\s+$/g, "");
                }
            }
            
            if (optionsString.indexOf("gutter") > -1)
                options.hideGutter = true;

            if (optionsString.indexOf("toolbar") > -1)
                options.hideControls = true;

            if (optionsString.indexOf("collapse") > -1)
                options.collapse = true;

            if (optionsString.indexOf("first-line") > -1) {
                var match = /first-line:[ ]{0,1}([0-9]{1,4})/.exec(optionsString);
                if (match != null && match.length > 0 && match[1] > 1) {
                    options.firstLineChecked = true;
                    options.firstLine = match[1];
                }
            }
            
            if (optionsString.indexOf("highlight") > -1) {
                // make sure we have a comma-seperated list
                if (optionsString.match(/highlight:[ ]{0,1}\[[0-9]+(,[0-9]+)*\]/)) {
                    // now grab the list
                    var match_hl = /highlight:[ ]{0,1}\[(.*)\]/.exec(optionsString);
                    if (match_hl != null && match_hl.length > 0) {
                        options.highlightChecked = true;
                        options.highlight = match_hl[1];
                    }
                }
            }

            if (optionsString.indexOf("ruler") > -1)
                options.showColumns = true;
            
            if (optionsString.indexOf("wrap-lines") > -1)
                options.noWrap = true;
        }
        return options;
    }
    
    var getStringForOptions = function(optionsObject) {
        return optionsObject.lang;
    }
    
    return {
        title: editor.lang.syntaxhighlight.title,
        minWidth: 500,
        minHeight: 400,
        onShow: function() {
            // Try to grab the selected pre tag if any
            var editor = this.getParentEditor();
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
            this.setupContent(optionsObj);
        },
        onOk: function() {
            var editor = this.getParentEditor();
            var selection = editor.getSelection();
            var element = selection.getStartElement();
            var preElement = element && element.getAscendant('code', true);
            var data = getDefaultOptions();
            this.commitContent(data);
            var optionsString = getStringForOptions(data);
            
            if (preElement) {
                preElement.setAttribute('title', optionsString);
                preElement.setText(data.code);
            } else {
                /*var newElement = new CKEDITOR.dom.element('code');
                newElement.setAttribute('title', optionsString);
                newElement.setText(data.code);
                editor.insertElement(newElement);*/
				
				editor.insertHtml("[code=" + optionsString + "]" + data.code + "[/code]");
            }
        },
        contents : [
            {
                id : 'source',
                label : editor.lang.syntaxhighlight.sourceTab,
                accessKey : 'S',
                elements :
                [
                    {
                        type : 'vbox',
                        children: [
                          {
                              id: 'cmbLang',
                              type: 'select',
                              labelLayout: 'horizontal',
                              label: editor.lang.syntaxhighlight.langLbl,
                              'default': 'plain',
                              widths : [ '25%','75%' ],
                              items: [
                                      ['Plain (Text)', 'plain'],
									  ['ActionScript3', 'as3'],
									  ['Bash(shell)', 'bash'],
									  ['ColdFusion', 'coldfusion'], 
									  ['C#', 'csharp'],
									  ['C++', 'cpp'], 
									  ['CSS', 'css'],
									  ['Delphi', 'delphi'], 
									  ['Diff', 'diff'], 
									  ['Erlang', 'erlang'], 
									  ['Groovy', 'groovy'], 
									  ['JavaScript', 'jscript'],
									  ['Java', 'java'],
									  ['JavaFX', 'javafx'], 
									  ['Perl', 'perl'], 
									  ['PHP', 'php'],
									  ['PowerShell', 'powershell'], 
									  ['Pyton', 'python'], 
									  ['Ruby', 'ruby'],
									  ['Scala', 'scala'],
									  ['SQL', 'sql'], 
									  ['Visual Basic', 'vb'], 
									  ['XML/XHTML', 'xml'],
                              ],
                              setup: function(data) {
                                  if (data.lang)
                                      this.setValue(data.lang);
                              },
                              commit: function(data) {
                                  data.lang = this.getValue();
                              }
                          }
                        ]
                    },
                    {
                        type: 'textarea',
                        id: 'hl_code',
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
