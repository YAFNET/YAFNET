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
                    { expand: true, src: "**/*.scss", cwd: "node_modules/bootswatch/dist", dest: "Content/Themes/" }
                ],
            },
        },

        shell: {
            emailTemplates: {
                command: [
                    "@echo off",
                    "echo Build cerulean theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/cerulean/EmailTemplate.html  -c Content/Themes/cerulean/bootstrap_email.config",

                    "echo Build cosmo theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/cosmo/EmailTemplate.html -c Content/Themes/cosmo/bootstrap_email.config",

                    "echo Build cyborg theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/cyborg/EmailTemplate.html -c Content/Themes/cyborg/bootstrap_email.config",

                    "echo Build darkly theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/darkly/EmailTemplate.html -c Content/Themes/darkly/bootstrap_email.config",

                    "echo Build flatly theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/flatly/EmailTemplate.html -c Content/Themes/flatly/bootstrap_email.config",

                    "echo Build journal theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/journal/EmailTemplate.html -c Content/Themes/journal/bootstrap_email.config",

                    "echo Build litera theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/litera/EmailTemplate.html -c Content/Themes/litera/bootstrap_email.config",

                    "echo Build lumen theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/lumen/EmailTemplate.html -c Content/Themes/lumen/bootstrap_email.config",

                    "echo Build lux theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/lux/EmailTemplate.html -c Content/Themes/lux/bootstrap_email.config",

                    "echo Build materia theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/materia/EmailTemplate.html -c Content/Themes/materia/bootstrap_email.config",

                    "echo Build minty theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/minty/EmailTemplate.html -c Content/Themes/minty/bootstrap_email.config",

                    "echo Build morph theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/morph/EmailTemplate.html -c Content/Themes/morph/bootstrap_email.config",

                    "echo Build pulse theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/pulse/EmailTemplate.html -c Content/Themes/pulse/bootstrap_email.config",

                    "echo Build quartz theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/quartz/EmailTemplate.html -c Content/Themes/quartz/bootstrap_email.config",

                    "echo Build sandstone theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/sandstone/EmailTemplate.html -c Content/Themes/sandstone/bootstrap_email.config",

                    "echo Build simplex theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/simplex/EmailTemplate.html -c Content/Themes/simplex/bootstrap_email.config",

                    "echo Build sketchy theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/sketchy/EmailTemplate.html -c Content/Themes/sketchy/bootstrap_email.config",

                    "echo Build slate theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/slate/EmailTemplate.html -c Content/Themes/slate/bootstrap_email.config",

                    "echo Build solar theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/solar/EmailTemplate.html -c Content/Themes/solar/bootstrap_email.config",

                    "echo Build spacelab theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/spacelab/EmailTemplate.html -c Content/Themes/spacelab/bootstrap_email.config",

                    "echo Build superhero theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/superhero/EmailTemplate.html -c Content/Themes/superhero/bootstrap_email.config",

                    "echo Build united theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/united/EmailTemplate.html -c Content/Themes/united/bootstrap_email.config",

                    "echo Build vapor theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/vapor/EmailTemplate.html -c Content/Themes/vapor/bootstrap_email.config",

                    "echo Build yaf theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/yaf/EmailTemplate.html -c Content/Themes/yaf/bootstrap_email.config",

                    "echo Build yeti theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/yeti/EmailTemplate.html -c Content/Themes/yeti/bootstrap_email.config",

                    "echo Build zephyr theme email template",
                    "call bootstrap-email Resources/EmailTemplate.html > Content/Themes/zephyr/EmailTemplate.html -c Content/Themes/zephyr/bootstrap_email.config",

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
                    "Scripts/jquery-3.6.2.js",
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
                    "Scripts/bootstrap.bundle.js",
                    "Scripts/bootbox.js",
                    "Scripts/bootstrap-notify.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/blueimp-gallery-indicator.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
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
                    "Scripts/bootbox.js",
                    "Scripts/bootstrap-notify.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/blueimp-gallery-indicator.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
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
                    "Scripts/bootstrap.bundle.js",
                    "Scripts/bootbox.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
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
                    "Scripts/bootbox.js",
                    "Scripts/bootstrap-notify.js",
                    "Scripts/jquery.bootstrap-touchspin.js",
                    "Scripts/select2.js",
                    "Scripts/blueimp-gallery/blueimp-gallery.js",
                    "Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                    "Scripts/jquery.hovercard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
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
