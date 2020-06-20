/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
CKEDITOR.dialog.add("syntaxhighlightDialog", function(editor) {
	
	var getDefaultOptions = function(options) {
        var options = new Object();
        options.bbcodeName = null;
        options.code = "";
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
        onOk: function () {
            var editor = this.getParentEditor();

            var data = getDefaultOptions();
            this.commitContent(data);
            var optionsString = getStringForOptions(data);

            var insert = "[code=" + optionsString + "]" + data.code + "[/code]";

            if (editor.mode === "source") {
                var doc = window["codemirror_" + editor.id].getDoc();
                var cursor = doc.getCursor();

                var pos = {
                    line: cursor.line,
                    ch: cursor.ch
                }

                doc.replaceRange(insert, pos);

            } else {
                editor.insertHtml(insert);
            }

        },
        contents: [
            {
                id: "source",
                label: editor.lang.syntaxhighlight.sourceTab,
                accessKey: "S",
                elements:
                [
                    {
                        type: "vbox",
                        children: [
                            {
                                id: "cmbBBCode",
                                type: "select",
                                labelLayout: "horizontal",
                                label: editor.lang.syntaxhighlight.langLbl,
                                'default': "markup",
                                widths: ["25%", "75%"],
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
                                        contentBox = dialog.getContentElement("source", "CodeBox");
                                }
                            }
                        ]
                    },
                    {
                        type: "textarea",
						label: editor.lang.syntaxhighlight.sourceTab,
                        id: "CodeBox",
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
