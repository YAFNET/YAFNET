/**
 * Build process for YetAnotherForum.NET
 *
 * Don't know where to start?
 * Try: http://24ways.org/2013/grunt-is-not-weird-and-hard/
 */
module.exports = function(grunt) {

    // CONFIGURATION
    grunt.initConfig({
        pkg: grunt.file.readJSON("package.json"),

        copy: {
            bootswatchThemes: {
                files: [
                    // includes files within path
                    { expand: true, src: "**/*.scss", cwd: "node_modules/bootswatch/dist", dest: "wwwroot/lib/themes/" }
                ],
            },
            jQuery: {
                files: [
                    // includes files within path
                    { expand: true, src: "**/jquery.min.js", cwd: "node_modules/jquery/dist", dest: "wwwroot/js/" }
                ],
            }
        },

        shell: {
            emailTemplates: {
                command: [
                    "@echo off",
                    "echo Build cerulean theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/cerulean/EmailTemplate.html  -c  wwwroot/lib/themes/cerulean/bootstrap_email.config",

                    "echo Build cosmo theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/cosmo/EmailTemplate.html -c  wwwroot/lib/themes/cosmo/bootstrap_email.config",

                    "echo Build cyborg theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/cyborg/EmailTemplate.html -c  wwwroot/lib/themes/cyborg/bootstrap_email.config",

                    "echo Build darkly theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/darkly/EmailTemplate.html -c  wwwroot/lib/themes/darkly/bootstrap_email.config",

                    "echo Build flatly theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/flatly/EmailTemplate.html -c  wwwroot/lib/themes/flatly/bootstrap_email.config",

                    "echo Build journal theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/journal/EmailTemplate.html -c  wwwroot/lib/themes/journal/bootstrap_email.config",

                    "echo Build litera theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/litera/EmailTemplate.html -c  wwwroot/lib/themes/litera/bootstrap_email.config",

                    "echo Build lumen theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/lumen/EmailTemplate.html -c  wwwroot/lib/themes/lumen/bootstrap_email.config",

                    "echo Build lux theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/lux/EmailTemplate.html -c  wwwroot/lib/themes/lux/bootstrap_email.config",

                    "echo Build materia theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/materia/EmailTemplate.html -c  wwwroot/lib/themes/materia/bootstrap_email.config",

                    "echo Build minty theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/minty/EmailTemplate.html -c  wwwroot/lib/themes/minty/bootstrap_email.config",

                    "echo Build morph theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/morph/EmailTemplate.html -c  wwwroot/lib/themes/morph/bootstrap_email.config",

                    "echo Build pulse theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/pulse/EmailTemplate.html -c  wwwroot/lib/themes/pulse/bootstrap_email.config",

                    "echo Build quartz theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/quartz/EmailTemplate.html -c  wwwroot/lib/themes/quartz/bootstrap_email.config",

                    "echo Build sandstone theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/sandstone/EmailTemplate.html -c  wwwroot/lib/themes/sandstone/bootstrap_email.config",

                    "echo Build simplex theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/simplex/EmailTemplate.html -c  wwwroot/lib/themes/simplex/bootstrap_email.config",

                    "echo Build sketchy theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/sketchy/EmailTemplate.html -c  wwwroot/lib/themes/sketchy/bootstrap_email.config",

                    "echo Build slate theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/slate/EmailTemplate.html -c  wwwroot/lib/themes/slate/bootstrap_email.config",

                    "echo Build solar theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/solar/EmailTemplate.html -c  wwwroot/lib/themes/solar/bootstrap_email.config",

                    "echo Build spacelab theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/spacelab/EmailTemplate.html -c  wwwroot/lib/themes/spacelab/bootstrap_email.config",

                    "echo Build superhero theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/superhero/EmailTemplate.html -c  wwwroot/lib/themes/superhero/bootstrap_email.config",

                    "echo Build united theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/united/EmailTemplate.html -c  wwwroot/lib/themes/united/bootstrap_email.config",

                    "echo Build vapor theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/vapor/EmailTemplate.html -c  wwwroot/lib/themes/vapor/bootstrap_email.config",

                    "echo Build yaf theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/yaf/EmailTemplate.html -c  wwwroot/lib/themes/yaf/bootstrap_email.config",

                    "echo Build yeti theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/yeti/EmailTemplate.html -c  wwwroot/lib/themes/yeti/bootstrap_email.config",

                    "echo Build zephyr theme email template",
                    "call bootstrap-email wwwroot/Resources/EmailTemplate.html > wwwroot/css/themes/zephyr/EmailTemplate.html -c  wwwroot/lib/themes/zephyr/bootstrap_email.config",

                    "rmdir .sass-cache /s /q"
                ].join('&&')

            }
        },

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
                    "node_modules/jquery/dist/jquery.js",
                    "wwwroot/lib/select2.js",
                    "wwwroot/lib/bootstrap.bundle.js",
                    "wwwroot/lib/forum/InstallWizard.js"
                ],
                dest: "wwwroot/js/InstallWizard.comb.js"
            },

            fileUpload: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "wwwroot/lib/jquery.FileUpload/vendor/jquery.ui.widget.js",
                    "wwwroot/lib/jquery.FileUpload/tmpl.js",
                    "wwwroot/lib/jquery.FileUpload/load-image.all.min.js",
                    "wwwroot/lib/jquery.FileUpload/canvas-to-blob.js",
                    "wwwroot/lib/jquery.FileUpload/jquery.iframe-transport.js",
                    "wwwroot/lib/jquery.FileUpload/jquery.fileupload.js",
                    "wwwroot/lib/jquery.FileUpload/jquery.fileupload-process.js",
                    "wwwroot/lib/jquery.FileUpload/jquery.fileupload-image.js",
                    "wwwroot/lib/jquery.FileUpload/jquery.fileupload-validate.js",
                    "wwwroot/lib/jquery.FileUpload/jquery.fileupload-ui.js"
                ],
                dest: "wwwroot/js/jquery.fileupload.comb.js"
            },
            forumExtensions: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "wwwroot/lib/bootstrap.bundle.js",
                    "wwwroot/lib/bootbox.js",
                    "wwwroot/lib/bootstrap-notify.js",
                    "wwwroot/lib/jquery.bootstrap-touchspin.js",
                    "wwwroot/lib/select2.js",
                    "wwwroot/lib/blueimp-gallery/blueimp-gallery.js",
                    "wwwroot/lib/blueimp-gallery/blueimp-gallery-indicator.js",
                    "wwwroot/lib/blueimp-gallery/jquery.blueimp-gallery.js",
                    "wwwroot/lib/jquery.hovercard.js",
                    "wwwroot/lib/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
                    "wwwroot/lib/forum/yaf.Utilities.js",
                    "wwwroot/lib/forum/yaf.Albums.js",
                    "wwwroot/lib/forum/yaf.Attachments.js",
                    "wwwroot/lib/forum/yaf.Notify.js",
                    "wwwroot/lib/forum/yaf.SearchResults.js",
                    "wwwroot/lib/forum/yaf.SimilarTitles.js",
                    "wwwroot/lib/forum/yaf.Main.js",
                    "wwwroot/lib/forum/yaf.contextMenu.js",
                    "node_modules/jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax.js",
                    "node_modules/jquery-validation/dist/jquery.validate.js",
                    "wwwroot/lib/jquery.serializejson.js"
                ],
                dest: "wwwroot/js/jquery.ForumExtensions.js"
            },
            forumAdminExtensions: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "wwwroot/lib/bootstrap.bundle.js",
                    "wwwroot/lib/bootbox.js",
                    "wwwroot/lib/bootstrap-notify.js",
                    "wwwroot/lib/jquery.bootstrap-touchspin.js",
                    "wwwroot/lib/select2.js",
                    "wwwroot/lib/blueimp-gallery/blueimp-gallery.js",
                    "wwwroot/lib/blueimp-gallery/jquery.blueimp-gallery.js",
                    "wwwroot/lib/jquery.hovercard.js",
                    "wwwroot/lib/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
                    "wwwroot/lib/forum/yaf.Utilities.js",
                    "wwwroot/lib/forum/yaf.Albums.js",
                    "wwwroot/lib/forum/yaf.Notify.js",
                    "wwwroot/lib/forum/yaf.Main.js",
                    "wwwroot/lib/forum/yaf.contextMenu.js",
                    "node_modules/jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax.js",
                    "node_modules/jquery-validation/dist/jquery.validate.js",
                    "wwwroot/lib/jquery.serializejson.js"
                ],
                dest: "wwwroot/js/jquery.ForumAdminExtensions.js"
            },
            minify: {
                files: {
                    "wwwroot/js/InstallWizard.comb.min.js": "wwwroot/js/InstallWizard.comb.js",
                    "wwwroot/js/jquery.fileupload.comb.min.js": "wwwroot/js/jquery.fileupload.comb.js",
                    "wwwroot/js/jquery.ForumExtensions.min.js": "wwwroot/js/jquery.ForumExtensions.js",
                    "wwwroot/js/jquery.ForumAdminExtensions.min.js": "wwwroot/js/jquery.ForumAdminExtensions.js",

                }
            }
        },

        sass: {
            installWizard: {
                files: {
                    "wwwroot/css/InstallWizard.css": "wwwroot/lib/InstallWizard.scss"
                }
            },
            forum: {
                files: {
                    "wwwroot/css/forum.css": "wwwroot/lib/forum.scss"
                }
            },
            forumAdmin: {
                files: {
                    "wwwroot/css/forum-admin.css": "wwwroot/lib/forum-admin.scss"
                }
            },
            bootstrap: {
                files: {
                    "wwwroot/lib/bootstrap/bootstrap.css": "wwwroot/lib/bootstrap/bootstrap.scss"
                }
            },
            themes: {
                files: {
                    "wwwroot/css/themes/zephyr/bootstrap-forum.css": "wwwroot/lib/themes/zephyr/bootstrap-forum.scss",
                    "wwwroot/css/themes/yaf/bootstrap-forum.css": "wwwroot/lib/themes/yaf/bootstrap-forum.scss",
                    "wwwroot/css/themes/yeti/bootstrap-forum.css": "wwwroot/lib/themes/yeti/bootstrap-forum.scss",
                    "wwwroot/css/themes/vapor/bootstrap-forum.css": "wwwroot/lib/themes/vapor/bootstrap-forum.scss",
                    "wwwroot/css/themes/united/bootstrap-forum.css": "wwwroot/lib/themes/united/bootstrap-forum.scss",
                    "wwwroot/css/themes/superhero/bootstrap-forum.css": "wwwroot/lib/themes/superhero/bootstrap-forum.scss",
                    "wwwroot/css/themes/spacelab/bootstrap-forum.css": "wwwroot/lib/themes/spacelab/bootstrap-forum.scss",
                    "wwwroot/css/themes/solar/bootstrap-forum.css": "wwwroot/lib/themes/solar/bootstrap-forum.scss",
                    "wwwroot/css/themes/slate/bootstrap-forum.css": "wwwroot/lib/themes/slate/bootstrap-forum.scss",
                    "wwwroot/css/themes/sketchy/bootstrap-forum.css": "wwwroot/lib/themes/sketchy/bootstrap-forum.scss",
                    "wwwroot/css/themes/simplex/bootstrap-forum.css": "wwwroot/lib/themes/simplex/bootstrap-forum.scss",
                    "wwwroot/css/themes/sandstone/bootstrap-forum.css": "wwwroot/lib/themes/sandstone/bootstrap-forum.scss",
                    "wwwroot/css/themes/quartz/bootstrap-forum.css": "wwwroot/lib/themes/quartz/bootstrap-forum.scss",
                    "wwwroot/css/themes/pulse/bootstrap-forum.css": "wwwroot/lib/themes/pulse/bootstrap-forum.scss",
                    "wwwroot/css/themes/morph/bootstrap-forum.css": "wwwroot/lib/themes/morph/bootstrap-forum.scss",
                    "wwwroot/css/themes/minty/bootstrap-forum.css": "wwwroot/lib/themes/minty/bootstrap-forum.scss",
                    "wwwroot/css/themes/materia/bootstrap-forum.css": "wwwroot/lib/themes/materia/bootstrap-forum.scss",
                    "wwwroot/css/themes/lux/bootstrap-forum.css": "wwwroot/lib/themes/lux/bootstrap-forum.scss",
                    "wwwroot/css/themes/lumen/bootstrap-forum.css": "wwwroot/lib/themes/lumen/bootstrap-forum.scss",
                    "wwwroot/css/themes/litera/bootstrap-forum.css": "wwwroot/lib/themes/litera/bootstrap-forum.scss",
                    "wwwroot/css/themes/journal/bootstrap-forum.css": "wwwroot/lib/themes/journal/bootstrap-forum.scss",
                    "wwwroot/css/themes/flatly/bootstrap-forum.css": "wwwroot/lib/themes/flatly/bootstrap-forum.scss",
                    "wwwroot/css/themes/darkly/bootstrap-forum.css": "wwwroot/lib/themes/darkly/bootstrap-forum.scss",
                    "wwwroot/css/themes/cyborg/bootstrap-forum.css": "wwwroot/lib/themes/cyborg/bootstrap-forum.scss",
                    "wwwroot/css/themes/cosmo/bootstrap-forum.css": "wwwroot/lib/themes/cosmo/bootstrap-forum.scss",
                    "wwwroot/css/themes/cerulean/bootstrap-forum.css": "wwwroot/lib/themes/cerulean/bootstrap-forum.scss"
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
                src: "wwwroot/css/InstallWizard.css"
            },
            forum: {
                src: "wwwroot/css/forum.css"
            },
            forumAdmin: {
                src: "wwwroot/css/forum-admin.css"
            },
            themes: {
                src: [
                    "wwwroot/lib/themes/zephyr/bootstrap-forum.css",
                    "wwwroot/lib/themes/yaf/bootstrap-forum.css",
                    "wwwroot/lib/themes/yeti/bootstrap-forum.css",
                    "wwwroot/lib/themes/vapor/bootstrap-forum.css",
                    "wwwroot/lib/themes/united/bootstrap-forum.css",
                    "wwwroot/lib/themes/superhero/bootstrap-forum.css",
                    "wwwroot/lib/themes/spacelab/bootstrap-forum.css",
                    "wwwroot/lib/themes/solar/bootstrap-forum.css",
                    "wwwroot/lib/themes/slate/bootstrap-forum.css",
                    "wwwroot/lib/themes/sketchy/bootstrap-forum.css",
                    "wwwroot/lib/themes/simplex/bootstrap-forum.css",
                    "wwwroot/lib/themes/sandstone/bootstrap-forum.css",
                    "wwwroot/lib/themes/quartz/bootstrap-forum.css",
                    "wwwroot/lib/themes/pulse/bootstrap-forum.css",
                    "wwwroot/lib/themes/morph/bootstrap-forum.css",
                    "wwwroot/lib/themes/minty/bootstrap-forum.css",
                    "wwwroot/lib/themes/materia/bootstrap-forum.css",
                    "wwwroot/lib/themes/lux/bootstrap-forum.css",
                    "wwwroot/lib/themes/lumen/bootstrap-forum.css",
                    "wwwroot/lib/themes/litera/bootstrap-forum.css",
                    "wwwroot/lib/themes/journal/bootstrap-forum.css",
                    "wwwroot/lib/themes/flatly/bootstrap-forum.css",
                    "wwwroot/lib/themes/darkly/bootstrap-forum.css",
                    "wwwroot/lib/themes/cyborg/bootstrap-forum.css",
                    "wwwroot/lib/themes/cosmo/bootstrap-forum.css",
                    "wwwroot/lib/themes/cerulean/bootstrap-forum.css"
                ]
            }
        },

        // CSS Minify
        cssmin: {
            fileUpload: {
                files: {
                    "wwwroot/css/jquery.fileupload.comb.min.css": [
                        "wwwroot/lib/jQuery.FileUpload/jquery.fileupload-ui.css",
                        "wwwroot/lib/jQuery.FileUpload/jquery.fileupload.css"
                    ]
                }
            },
            other: {
                files: {
                    "wwwroot/css/InstallWizard.min.css": "wwwroot/css/InstallWizard.css",
                    "wwwroot/css/forum.min.css": "wwwroot/css/forum.css",
                    "wwwroot/css/forum-admin.min.css": "wwwroot/css/forum-admin.css"
                }
            },
            themes: {
                files: {
                    "wwwroot/css/themes/zephyr/bootstrap-forum.min.css": "wwwroot/css/themes/zephyr/bootstrap-forum.css",
                    "wwwroot/css/themes/yaf/bootstrap-forum.min.css": "wwwroot/css/themes/yaf/bootstrap-forum.css",
                    "wwwroot/css/themes/yeti/bootstrap-forum.min.css": "wwwroot/css/themes/yeti/bootstrap-forum.css",
                    "wwwroot/css/themes/vapor/bootstrap-forum.min.css": "wwwroot/css/themes/vapor/bootstrap-forum.css",
                    "wwwroot/css/themes/united/bootstrap-forum.min.css": "wwwroot/css/themes/united/bootstrap-forum.css",
                    "wwwroot/css/themes/superhero/bootstrap-forum.min.css": "wwwroot/css/themes/superhero/bootstrap-forum.css",
                    "wwwroot/css/themes/spacelab/bootstrap-forum.min.css": "wwwroot/css/themes/spacelab/bootstrap-forum.css",
                    "wwwroot/css/themes/solar/bootstrap-forum.min.css": "wwwroot/css/themes/solar/bootstrap-forum.css",
                    "wwwroot/css/themes/slate/bootstrap-forum.min.css": "wwwroot/css/themes/slate/bootstrap-forum.css",
                    "wwwroot/css/themes/sketchy/bootstrap-forum.min.css": "wwwroot/css/themes/sketchy/bootstrap-forum.css",
                    "wwwroot/css/themes/simplex/bootstrap-forum.min.css": "wwwroot/css/themes/simplex/bootstrap-forum.css",
                    "wwwroot/css/themes/sandstone/bootstrap-forum.min.css": "wwwroot/css/themes/sandstone/bootstrap-forum.css",
                    "wwwroot/css/themes/quartz/bootstrap-forum.min.css": "wwwroot/css/themes/quartz/bootstrap-forum.css",
                    "wwwroot/css/themes/pulse/bootstrap-forum.min.css": "wwwroot/css/themes/pulse/bootstrap-forum.css",
                    "wwwroot/css/themes/morph/bootstrap-forum.min.css": "wwwroot/css/themes/morph/bootstrap-forum.css",
                    "wwwroot/css/themes/minty/bootstrap-forum.min.css": "wwwroot/css/themes/minty/bootstrap-forum.css",
                    "wwwroot/css/themes/materia/bootstrap-forum.min.css": "wwwroot/css/themes/materia/bootstrap-forum.css",
                    "wwwroot/css/themes/lux/bootstrap-forum.min.css": "wwwroot/css/themes/lux/bootstrap-forum.css",
                    "wwwroot/css/themes/lumen/bootstrap-forum.min.css": "wwwroot/css/themes/lumen/bootstrap-forum.css",
                    "wwwroot/css/themes/litera/bootstrap-forum.min.css": "wwwroot/css/themes/litera/bootstrap-forum.css",
                    "wwwroot/css/themes/journal/bootstrap-forum.min.css": "wwwroot/css/themes/journal/bootstrap-forum.css",
                    "wwwroot/css/themes/flatly/bootstrap-forum.min.css": "wwwroot/css/themes/flatly/bootstrap-forum.css",
                    "wwwroot/css/themes/darkly/bootstrap-forum.min.css": "wwwroot/css/themes/darkly/bootstrap-forum.css",
                    "wwwroot/css/themes/cyborg/bootstrap-forum.min.css": "wwwroot/css/themes/cyborg/bootstrap-forum.css",
                    "wwwroot/css/themes/cosmo/bootstrap-forum.min.css": "wwwroot/css/themes/cosmo/bootstrap-forum.css",
                    "wwwroot/css/themes/cerulean/bootstrap-forum.min.css": "wwwroot/css/themes/cerulean/bootstrap-forum.css"
                }
            }
        },
        devUpdate: {
            main: {
                options: {
                    reportUpdated: true,
                    updateType: "force",
                    semver: false,
                    packages: {
                        devDependencies: true, //only check for devDependencies
                        dependencies: true
                    }
                }
            }
        }
    });

    // PLUGINS
    grunt.loadNpmTasks("grunt-contrib-sass");
    grunt.loadNpmTasks("@lodder/grunt-postcss");
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks("grunt-contrib-uglify");
    grunt.loadNpmTasks("grunt-contrib-cssmin");
    grunt.loadNpmTasks("grunt-dev-update");
    grunt.loadNpmTasks("grunt-shell");

    grunt.registerTask("default",
        [
           "devUpdate", "uglify", "sass", "postcss", "cssmin"
        ]);

    grunt.registerTask("updateBootswatchThemes",
        [
            "copy"
        ]);

    grunt.registerTask("emailTemplates",
        [
            "shell"
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
