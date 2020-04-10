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
CKEDITOR.dialog.add("bbcodeselector",
    function(editor) {
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
            title: editor.lang.bbcodeselector.title,
            minWidth: 500,
            minHeight: 400,
            onOk: function() {
                var editor = this.getParentEditor();

                var data = getDefaultOptions();
                this.commitContent(data);
                var optionsString = getStringForOptions(data);

                var insert = "[" + optionsString + "]" + data.code + "[/" + optionsString + "]";

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
                    label: editor.lang.bbcodeselector.langLbl,
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
                                    label: editor.lang.bbcodeselector.bbCodeLbl,
                                    'default': "youtube",
                                    widths: ["25%", "75%"],
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
                                            contentBox = dialog.getContentElement("source", "CodeBox");

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
                                        case "facebook":
                                            {
                                                var text = editor.lang.bbcodeselector.facebook;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "spoiler":
                                            {
                                                var text = editor.lang.bbcodeselector.spoiler;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "twitter":
                                            {
                                                var text = editor.lang.bbcodeselector.twitter;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "instagram":
                                            {
                                                var text = editor.lang.bbcodeselector.instagram;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "hide":
                                            {
                                                var text = editor.lang.bbcodeselector.hide;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "group-hide":
                                            {
                                                var text = editor.lang.bbcodeselector.grouphide;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "hide-thanks":
                                            {
                                                var text = editor.lang.bbcodeselector.hidethanks;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "hide-reply-thanks":
                                            {
                                                var text = editor.lang.bbcodeselector.hidereplythanks;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "hide-reply":
                                            {
                                                var text = editor.lang.bbcodeselector.hidereply;
                                                contentBox.setValue(text);
                                            }
                                            break;
                                        case "hide-posts":
                                            {
                                                var text = editor.lang.bbcodeselector.hideposts;
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
                            type: "textarea",
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