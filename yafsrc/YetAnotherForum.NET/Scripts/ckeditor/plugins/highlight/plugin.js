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
CKEDITOR.plugins.add("highlight",
    {
        lang: ["en"],
        init: function(editor) {
            editor.addCommand("highlight",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function() {
                        if (editor.mode === "source") {
                            var selection = window["codemirror_" + editor.id].getSelection();

                            if (selection.length > 0) {
                                window["codemirror_" + editor.id].replaceSelection("[h]" + selection + "[/h]");
                            }
                            else {

                                var doc = window["codemirror_" + editor.id].getDoc();
                                var cursor = doc.getCursor();

                                var pos = {
                                    line: cursor.line,
                                    ch: cursor.ch
                                }

                                doc.replaceRange("[h][/h]", pos);

                                window["codemirror_" + editor.id].focus();
                                window["codemirror_" + editor.id].setCursor({ line: cursor.line, ch: cursor.ch + 3 });

                            }
                        } else {

                            var writer = new CKEDITOR.htmlWriter();

                            var selection = editor.getSelection();
                            if (!selection) {
                                CKEDITOR.htmlParser.fragment.fromBBCode("[h] [/h]").writeHtml(writer);

                                editor.insertHtml(writer.getHtml());
                            } else {

                                var text = selection.getSelectedText();

                                CKEDITOR.htmlParser.fragment.fromBBCode("[h]" + text + "[/h]").writeHtml(writer);

                                editor.insertHtml(writer.getHtml());
                            }
                        }
                    }
                });

            editor.ui.addButton("highlight",
                {
                    label: editor.lang.highlight.title,
                    command: "highlight",
                    icon: this.path + "pen-square-solid.svg"
                });
        }
    });