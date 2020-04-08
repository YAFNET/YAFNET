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
                if (jQuery("#PostAttachmentListPlaceholder").length) {
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

            editor.addCommand("attachmentsStart",
                new CKEDITOR.dialogCommand("attachmentsDialog")), CKEDITOR.tools.insertAttachment = function(b) {
                var c, d;
                console.log(b), a = CKEDITOR.currentInstance, c = CKEDITOR.dialog.getCurrent(), d =
                        '[attach]' + b + '[/attach]',
                    a.config.allowedContent = !0, a.insertHtml(d.trim()), c.hide()
            }, editor.ui.addButton("attachments",
                {
                    label: editor.lang.attachments.title,
                    command: "attachmentsStart",
                    icon: this.path + "images/paperclip-solid.svg"
                })
        }
        });
