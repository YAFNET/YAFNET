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