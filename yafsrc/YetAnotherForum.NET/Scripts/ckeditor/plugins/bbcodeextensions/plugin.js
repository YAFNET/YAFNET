/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
CKEDITOR.plugins.add("bbcodeextensions",
    {
        lang: ["en"],
        init: function(editor) {
            editor.addCommand("facebook",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function() {

                        insertBBCode(editor, "facebook");
                    }
                });

            editor.ui.addButton("facebook",
                {
                    label: editor.lang.bbcodeextensions.facebook,
                    command: "facebook",
                    icon: this.path + "facebook.svg"
                });

            editor.addCommand("instagram",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function () {

                        insertBBCode(editor, "instagram");
                    }
                });

            editor.ui.addButton("instagram",
                {
                    label: editor.lang.bbcodeextensions.instagram,
                    command: "instagram",
                    icon: this.path + "instagram.svg"
                });

            editor.addCommand("twitter",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function () {

                        insertBBCode(editor, "twitter");
                    }
                });

            editor.ui.addButton("twitter",
                {
                    label: editor.lang.bbcodeextensions.twitter,
                    command: "twitter",
                    icon: this.path + "twitter.svg"
                });

            editor.addCommand("vimeo",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function () {

                        insertBBCode(editor, "vimeo");
                    }
                });

            editor.ui.addButton("vimeo",
                {
                    label: editor.lang.bbcodeextensions.vimeo,
                    command: "vimeo",
                    icon: this.path + "vimeo.svg"
                });

            editor.addCommand("youtube",
                {
                    modes: { wysiwyg: 1, source: 1 },
                    exec: function () {

                        insertBBCode(editor, "youtube");
                    }
                });

            editor.ui.addButton("youtube",
                {
                    label: editor.lang.bbcodeextensions.youtube,
                    command: "youtube",
                    icon: this.path + "youtube.svg"
                });

            function insertBBCode(editor, command) {
                if (editor.mode === "source") {
                    var selection = window["codemirror_" + editor.id].getSelection();

                    if (selection.length > 0) {
                        window["codemirror_" + editor.id].replaceSelection("[" + command + "]" + selection + "[/" + command + "]");
                    } else {

                        var doc = window["codemirror_" + editor.id].getDoc();
                        var cursor = doc.getCursor();

                        var pos = {
                            line: cursor.line,
                            ch: cursor.ch
                        }

                        doc.replaceRange("[" + command + "][/" + command + "]", pos);

                        window["codemirror_" + editor.id].focus();
                        window["codemirror_" + editor.id].setCursor({ line: cursor.line, ch: cursor.ch + 7 });

                    }
                } else {
                    var writer = new CKEDITOR.htmlWriter();

                    var selection = editor.getSelection();
                    if (!selection) {
                        CKEDITOR.htmlParser.fragment.fromBBCode("[" + command + "] [/" + command + "]").writeHtml(writer);

                        editor.insertHtml(writer.getHtml());
                    } else {

                        var text = selection.getSelectedText();

                        CKEDITOR.htmlParser.fragment.fromBBCode("[" + command + "]" + text + "[/" + command + "]")
                            .writeHtml(writer);

                        editor.insertHtml(writer.getHtml());
                    }
                }
            }
        }
    });