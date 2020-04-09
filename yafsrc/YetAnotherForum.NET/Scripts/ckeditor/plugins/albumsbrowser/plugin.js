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

CKEDITOR.dialog.add("albumsbrowserDialog",
    function(editor) {
        return {
            minWidth: 500,
            minHeight: 400,
            title: editor.lang.albumsbrowser.title,
            onLoad: function() {
                this.getElement().removeClass('cke_reset_all');
            },
            onShow: function() {
                if (jQuery("#PostAlbumsListPlaceholder").length) {
                    var pageSize = 5;
                    var pageNumber = 0;
                    getAlbumImagesData(pageSize, pageNumber, false);
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
                            html: '<div id="AlbumsListPager"></div>' +
                                '<div id="PostAlbumsLoader"> ' +
                                '<div class="text-center mb-3">' +
                                editor.lang.albumsbrowser.loading +
                                '<div class="fa-3x text-center"><i class="fas fa-spinner fa-pulse"></i></div></div>' +
                                '</div>' +
                                '<div class="content">' +
                                '<div id="PostAlbumsListPlaceholder" data-url="' +
                                CKEDITOR.basePath.replace('Scripts/ckeditor/', '') +
                                '" data-userid="0" data-notext="' +
                                editor.lang.albumsbrowser.noAlbums +
                                '" style="clear: both;">' +
                                '<ul class="list-group-albums list-group list-group-horizontal">' +
                                '</ul>' +
                                '</div>'
                        }
                    ]
                }
            ]
        }
    }), CKEDITOR.plugins.add("albumsbrowser",
    {
        requires: 'dialog',
        lang: 'en',
        init: function (editor) {

            editor.addCommand("albumsbrowserStart",
                new CKEDITOR.dialogCommand("albumsbrowserDialog")), CKEDITOR.tools.insertAlbumImage = function(b) {
                    var c, d;
                    console.log(b), a = CKEDITOR.currentInstance, c = CKEDITOR.dialog.getCurrent(), d =
                            '[albumimg]' + b + '[/albumimg]',
                        a.config.allowedContent = !0, a.insertHtml(d.trim()), c.hide()
                }, editor.ui.addButton("albumsbrowser",
                {
                    label: editor.lang.albumsbrowser.title,
                    command: "albumsbrowserStart",
                    icon: this.path + "images/images-regular.svg"
                })
        }
    });