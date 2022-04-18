/// <binding />
module.exports = function(grunt) {

    // CONFIGURATION
    grunt.initConfig({
        pkg: grunt.file.readJSON("package.json"),

        // Minimize JS
        uglify: {
            installWizard: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "Scripts/jquery-3.6.0.js",
                    "Scripts/select2.js",
                    "Scripts/bootstrap.bundle.js",
                    "Scripts/forum/InstallWizard.js"
                ],
                dest: "Scripts/InstallWizard.comb.js"
            },

            fileUpload: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "Scripts/jquery.FileUpload/vendor/jquery.ui.widget.js",
                    "Scripts/jquery.FileUpload/tmpl.js",
                    "Scripts/jquery.FileUpload/load-image.all.min.js",
                    "Scripts/jquery.FileUpload/canvas-to-blob.js",
                    "Scripts/jquery.FileUpload/jquery.iframe-transport.js",
                    "Scripts/jquery.FileUpload/jquery.fileupload.js",
                    "Scripts/jquery.FileUpload/jquery.fileupload-process.js",
                    "Scripts/jquery.FileUpload/jquery.fileupload-image.js",
                    "Scripts/jquery.FileUpload/jquery.fileupload-validate.js",
                    "Scripts/jquery.FileUpload/jquery.fileupload-ui.js"
                ],
                dest: "Scripts/jquery.fileupload.comb.js"
            },
            forumExtensions: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "node_modules/node_modules/moment/min/moment-with-locales.js",
                    "Scripts/bootstrap.bundle.js",
                    "Scripts/bootbox.js",
                    "Scripts/bootstrap-notify.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/bootstrap-tagsinput.js",
                    "Scripts/bootstrap-typeahead.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/blueimp-gallery-indicator.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "Scripts/hammer.min.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Attachments.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.SearchResults.js",
                    "Scripts/forum/yaf.SimilarTitles.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/jquery.ForumExtensions.js"
            },
            forumExtensionsDnn: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "Scripts/moment-with-locales.js",
                    "Scripts/bootbox.js",
                    "Scripts/bootstrap-notify.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/bootstrap-tagsinput.js",
                    "Scripts/bootstrap-typeahead.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/blueimp-gallery-indicator.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "Scripts/hammer.min.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Attachments.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.SearchResults.js",
                    "Scripts/forum/yaf.SimilarTitles.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/jquery.ForumExtensionsDnn.js"
            },
            forumAdminExtensions: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "Scripts/moment-with-locales.js",
                    "Scripts/bootstrap.bundle.js",
                    "Scripts/bootbox.js",
                    "Scripts/bootstrap-datetimepicker.js",
                    "Scripts/bootstrap-notify.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "Scripts/hammer.min.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/jquery.ForumAdminExtensions.js"
            },
            forumAdminExtensionsDnn: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "node_modules/node_modules/moment/min/moment-with-locales.js",
                    "Scripts/bootbox.js",
                    "Scripts/bootstrap-datetimepicker.js",
                    "Scripts/bootstrap-notify.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "Scripts/hammer.min.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/jquery.ForumAdminExtensionsDnn.js"
            },
            minify: {
                files: {
                    "Scripts/InstallWizard.comb.min.js": "Scripts/InstallWizard.comb.js",
                    "Scripts/jquery.fileupload.comb.min.js": "Scripts/jquery.fileupload.comb.js",
                    "Scripts/jquery.ForumExtensions.min.js": "Scripts/jquery.ForumExtensions.js",
                    "Scripts/jquery.ForumExtensionsDnn.min.js": "Scripts/jquery.ForumExtensionsDnn.js",
                    "Scripts/jquery.ForumAdminExtensions.min.js": "Scripts/jquery.ForumAdminExtensions.js",
                    "Scripts/jquery.ForumAdminExtensionsDnn.min.js": "Scripts/jquery.ForumAdminExtensionsDnn.js"

                }
            }
        },

        sass: {
            installWizard: {
                files: {
                    "Content/InstallWizard.css": "Content/InstallWizard.scss"
                }
            },
            forum: {
                files: {
                    "Content/forum.css": "Content/forum.scss"
                }
            },
            forumAdmin: {
                files: {
                    "Content/forum-admin.css": "Content/forum-admin.scss"
                }
            },
            bootstrap: {
                files: {
                    "Content/bootstrap/bootstrap.css": "Content/bootstrap/bootstrap.scss"
                }
            },
            themes: {
                files: {
                    "Content/Themes/zephyr/bootstrap-forum.css": "Content/Themes/zephyr/bootstrap-forum.scss",
                    "Content/Themes/yaf/bootstrap-forum.css": "Content/Themes/yaf/bootstrap-forum.scss",
                    "Content/Themes/yeti/bootstrap-forum.css": "Content/Themes/yeti/bootstrap-forum.scss",
                    "Content/Themes/vapor/bootstrap-forum.css": "Content/Themes/vapor/bootstrap-forum.scss",
                    "Content/Themes/united/bootstrap-forum.css": "Content/Themes/united/bootstrap-forum.scss",
                    "Content/Themes/superhero/bootstrap-forum.css": "Content/Themes/superhero/bootstrap-forum.scss",
                    "Content/Themes/spacelab/bootstrap-forum.css": "Content/Themes/spacelab/bootstrap-forum.scss",
                    "Content/Themes/solar/bootstrap-forum.css": "Content/Themes/solar/bootstrap-forum.scss",
                    "Content/Themes/slate/bootstrap-forum.css": "Content/Themes/slate/bootstrap-forum.scss",
                    "Content/Themes/sketchy/bootstrap-forum.css": "Content/Themes/sketchy/bootstrap-forum.scss",
                    "Content/Themes/simplex/bootstrap-forum.css": "Content/Themes/simplex/bootstrap-forum.scss",
                    "Content/Themes/sandstone/bootstrap-forum.css": "Content/Themes/sandstone/bootstrap-forum.scss",
                    "Content/Themes/quartz/bootstrap-forum.css": "Content/Themes/quartz/bootstrap-forum.scss",
                    "Content/Themes/pulse/bootstrap-forum.css": "Content/Themes/pulse/bootstrap-forum.scss",
                    "Content/Themes/morph/bootstrap-forum.css": "Content/Themes/morph/bootstrap-forum.scss",
                    "Content/Themes/minty/bootstrap-forum.css": "Content/Themes/minty/bootstrap-forum.scss",
                    "Content/Themes/materia/bootstrap-forum.css": "Content/Themes/materia/bootstrap-forum.scss",
                    "Content/Themes/lux/bootstrap-forum.css": "Content/Themes/lux/bootstrap-forum.scss",
                    "Content/Themes/lumen/bootstrap-forum.css": "Content/Themes/lumen/bootstrap-forum.scss",
                    "Content/Themes/litera/bootstrap-forum.css": "Content/Themes/litera/bootstrap-forum.scss",
                    "Content/Themes/journal/bootstrap-forum.css": "Content/Themes/journal/bootstrap-forum.scss",
                    "Content/Themes/flatly/bootstrap-forum.css": "Content/Themes/flatly/bootstrap-forum.scss",
                    "Content/Themes/darkly/bootstrap-forum.css": "Content/Themes/darkly/bootstrap-forum.scss",
                    "Content/Themes/cyborg/bootstrap-forum.css": "Content/Themes/cyborg/bootstrap-forum.scss",
                    "Content/Themes/cosmo/bootstrap-forum.css": "Content/Themes/cosmo/bootstrap-forum.scss",
                    "Content/Themes/cerulean/bootstrap-forum.css": "Content/Themes/cerulean/bootstrap-forum.scss"
                }
            }
        },

        postcss: {
            options: {
                map: false,
                processors: [
                    require("autoprefixer")({ overrideBrowserslist: "last 2 versions" })
                ]
            },
            installWizard: {
                src: "Content/InstallWizard.css"
            },
            forum: {
                src: "Content/forum.css"
            },
            forumAdmin: {
                src: "Content/forum-admin.css"
            },
            themes: {
                src: [
                    "Content/Themes/zephyr/bootstrap-forum.css",
                    "Content/Themes/yaf/bootstrap-forum.css",
                    "Content/Themes/yeti/bootstrap-forum.css",
                    "Content/Themes/vapor/bootstrap-forum.css",
                    "Content/Themes/united/bootstrap-forum.css",
                    "Content/Themes/superhero/bootstrap-forum.css",
                    "Content/Themes/spacelab/bootstrap-forum.css",
                    "Content/Themes/solar/bootstrap-forum.css",
                    "Content/Themes/slate/bootstrap-forum.css",
                    "Content/Themes/sketchy/bootstrap-forum.css",
                    "Content/Themes/simplex/bootstrap-forum.css",
                    "Content/Themes/sandstone/bootstrap-forum.css",
                    "Content/Themes/quartz/bootstrap-forum.css",
                    "Content/Themes/pulse/bootstrap-forum.css",
                    "Content/Themes/morph/bootstrap-forum.css",
                    "Content/Themes/minty/bootstrap-forum.css",
                    "Content/Themes/materia/bootstrap-forum.css",
                    "Content/Themes/lux/bootstrap-forum.css",
                    "Content/Themes/lumen/bootstrap-forum.css",
                    "Content/Themes/litera/bootstrap-forum.css",
                    "Content/Themes/journal/bootstrap-forum.css",
                    "Content/Themes/flatly/bootstrap-forum.css",
                    "Content/Themes/darkly/bootstrap-forum.css",
                    "Content/Themes/cyborg/bootstrap-forum.css",
                    "Content/Themes/cosmo/bootstrap-forum.css",
                    "Content/Themes/cerulean/bootstrap-forum.css"
                ]
            }
        },

        // CSS Minify
        cssmin: {
            fileUpload: {
                files: {
                    "Content/jquery.fileupload.comb.min.css": [
                        "Content/jQuery.FileUpload/jquery.fileupload-ui.css",
                        "Content/jQuery.FileUpload/jquery.fileupload.css"
                    ]
                }
            },
            other: {
                files: {
                    "Content/InstallWizard.min.css": "Content/InstallWizard.css",
                    "Content/forum.min.css": "Content/forum.css",
                    "Content/forum-admin.min.css": "Content/forum-admin.css"
                }
            },
            themes: {
                files: {
                    "Content/Themes/zephyr/bootstrap-forum.min.css": "Content/Themes/zephyr/bootstrap-forum.css",
                    "Content/Themes/yaf/bootstrap-forum.min.css": "Content/Themes/yaf/bootstrap-forum.css",
                    "Content/Themes/yeti/bootstrap-forum.min.css": "Content/Themes/yeti/bootstrap-forum.css",
                    "Content/Themes/vapor/bootstrap-forum.min.css": "Content/Themes/vapor/bootstrap-forum.css",
                    "Content/Themes/united/bootstrap-forum.min.css": "Content/Themes/united/bootstrap-forum.css",
                    "Content/Themes/superhero/bootstrap-forum.min.css": "Content/Themes/superhero/bootstrap-forum.css",
                    "Content/Themes/spacelab/bootstrap-forum.min.css": "Content/Themes/spacelab/bootstrap-forum.css",
                    "Content/Themes/solar/bootstrap-forum.min.css": "Content/Themes/solar/bootstrap-forum.css",
                    "Content/Themes/slate/bootstrap-forum.min.css": "Content/Themes/slate/bootstrap-forum.css",
                    "Content/Themes/sketchy/bootstrap-forum.min.css": "Content/Themes/sketchy/bootstrap-forum.css",
                    "Content/Themes/simplex/bootstrap-forum.min.css": "Content/Themes/simplex/bootstrap-forum.css",
                    "Content/Themes/sandstone/bootstrap-forum.min.css": "Content/Themes/sandstone/bootstrap-forum.css",
                    "Content/Themes/quartz/bootstrap-forum.min.css": "Content/Themes/quartz/bootstrap-forum.css",
                    "Content/Themes/pulse/bootstrap-forum.min.css": "Content/Themes/pulse/bootstrap-forum.css",
                    "Content/Themes/morph/bootstrap-forum.min.css": "Content/Themes/morph/bootstrap-forum.css",
                    "Content/Themes/minty/bootstrap-forum.min.css": "Content/Themes/minty/bootstrap-forum.css",
                    "Content/Themes/materia/bootstrap-forum.min.css": "Content/Themes/materia/bootstrap-forum.css",
                    "Content/Themes/lux/bootstrap-forum.min.css": "Content/Themes/lux/bootstrap-forum.css",
                    "Content/Themes/lumen/bootstrap-forum.min.css": "Content/Themes/lumen/bootstrap-forum.css",
                    "Content/Themes/litera/bootstrap-forum.min.css": "Content/Themes/litera/bootstrap-forum.css",
                    "Content/Themes/journal/bootstrap-forum.min.css": "Content/Themes/journal/bootstrap-forum.css",
                    "Content/Themes/flatly/bootstrap-forum.min.css": "Content/Themes/flatly/bootstrap-forum.css",
                    "Content/Themes/darkly/bootstrap-forum.min.css": "Content/Themes/darkly/bootstrap-forum.css",
                    "Content/Themes/cyborg/bootstrap-forum.min.css": "Content/Themes/cyborg/bootstrap-forum.css",
                    "Content/Themes/cosmo/bootstrap-forum.min.css": "Content/Themes/cosmo/bootstrap-forum.css",
                    "Content/Themes/cerulean/bootstrap-forum.min.css": "Content/Themes/cerulean/bootstrap-forum.css"
                }
            }
        }
    });

    // PLUGINS
    grunt.loadNpmTasks("grunt-contrib-sass");
    grunt.loadNpmTasks("@lodder/grunt-postcss");
    grunt.loadNpmTasks("grunt-contrib-uglify");
    grunt.loadNpmTasks("grunt-contrib-cssmin");

    grunt.registerTask("default",
        [
            "uglify", "sass", "postcss", "cssmin"
        ]);

    grunt.registerTask("js",
        [
            "uglify"
        ]);

    grunt.registerTask("css",
        [
            "sass", "postcss", "cssmin"
        ]);
};
