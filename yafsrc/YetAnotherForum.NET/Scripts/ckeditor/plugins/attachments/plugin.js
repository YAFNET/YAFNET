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
CKEDITOR.dialog.add("attachmentsDialog",
    function(editor) {
        return {
            maxWidth: 400,
            minWidth: 200,
            title: editor.lang.attachments.title,
            onLoad: function() {
                this.getElement().removeClass('cke_reset_all');
            },
            onShow: function() {
                if ($("#PostAttachmentListPlaceholder").length) {
                    var pageSize = 5;
                    var pageNumber = 0;
                    getPaginationData(pageSize, pageNumber, false);
                }
            },
            contents: [
                {
                    elements: [
                        {
                            type: "html",
                            align: "left",
                            id: "msg",
                            style: "",
                            html: '<div id="AttachmentsListPager"></div>' +
                                '<div id="PostAttachmentLoader"> ' +
                                '<div class="text-center mb-3">' +
                                editor.lang.attachments.loading +
                                '<div class="fa-3x text-center"><i class="fas fa-spinner fa-pulse"></i></div></div>' +
                                '</div>' +
                                '<div class="content">' +
                                '<div id="PostAttachmentListPlaceholder" data-url="' +
                                CKEDITOR.basePath.replace('Scripts/ckeditor/', '') +
                                '" data-userid="0" data-notext="' +
                                editor.lang.attachments.noAlbums +
                                '" style="clear: both;">' +
                                '<ul class="AttachmentList list-group-albums list-group">' +
                                '</ul>' +
                                '</div>' +
                                '<div class="OpenUploadDialog">' +
                                '<button type="button" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#UploadDialog">' +
                                editor.lang.attachments.upload +
                                '</button></div>'
                        }
                    ]
                }
            ]
        }
    }), CKEDITOR.plugins.add("attachments",
    {
        requires: 'dialog',
        lang: 'en',
        init: function(editor) {

            var command = editor.addCommand("attachmentsStart", new CKEDITOR.dialogCommand("attachmentsDialog"));
            command.modes = { wysiwyg: 1, source: 1 };

            CKEDITOR.tools.insertAttachment = function(id) {
                var dialog = CKEDITOR.dialog.getCurrent()
                var currentEditor = CKEDITOR.currentInstance;

                var insert = '[attach]' + id + '[/attach]';

                if (editor.mode === "source") {
                    var doc = window["codemirror_" + editor.id].getDoc();
                    var cursor = doc.getCursor();

                    var pos = {
                        line: cursor.line,
                        ch: cursor.ch
                    }

                    doc.replaceRange(insert, pos);
                } else {
                    currentEditor.insertHtml(insert);
                }

                dialog.hide();
            };

            editor.ui.addButton("attachments",
                {
                    label: editor.lang.attachments.title,
                    command: "attachmentsStart",
                    icon: this.path + "images/paperclip-solid.svg"
                });
        }
    });
