/**
 * Build process for YetAnotherForum.NET
 *
 * Don't know where to start?
 * Try: http://24ways.org/2013/grunt-is-not-weird-and-hard/
 */

const lightBoxWebpackConfig = require("./Scripts/bs5-lightbox/webpack.cdn.js");
module.exports = function(grunt) {
    // CONFIGURATION
    grunt.initConfig({
        pkg: grunt.file.readJSON("package.json"),


        webpack: {
            lightBox: lightBoxWebpackConfig
        },

        copy: {
            bootstrap: {
                files: [
                    { expand: true, src: "**/*.scss", cwd: "node_modules/bootstrap/scss", dest: "Content/bootstrap/" },
                    {
                        expand: true,
                        src: "**/bootstrap.bundle.js",
                        cwd: "node_modules/bootstrap/dist/js/",
                        dest: "Scripts/"
                    },
                    {
                        expand: true,
                        src: "**/bootstrap.bundle.min.js",
                        cwd: "node_modules/bootstrap/dist/js/",
                        dest: "Scripts/"
                    }
                ]
            },
            bootswatchThemes: {
                files: [
                    { expand: true, src: "**/*.scss", cwd: "node_modules/bootswatch/dist", dest: "Content/Themes/" }
                ]
            },
            fontAwesome: {
                files: [
                    {
                        expand: true,
                        src: "**/*.scss",
                        cwd: "node_modules/@fortawesome/fontawesome-free/scss",
                        dest: "Content/fontawesome/"
                    },
                    {
                        expand: true,
                        src: "**/*.*",
                        cwd: "node_modules/@fortawesome/fontawesome-free/webfonts",
                        dest: "Content/webfonts/"
                    }
                ]
            },
            flagIcons: {
                files: [
                    {
                        expand: true,
                        src: "**/*.scss",
                        cwd: "node_modules/flag-icons/sass",
                        dest: "Content/flag-icons/"
                    },
                    {
                        expand: true,
                        src: "**/*.svg",
                        cwd: "node_modules/flag-icons/flags",
                        dest: "Content/flags/"
                    }
                ]
            }
        },

        replace: {
            bootswatch: {
                options: {
                    usePrefix: false,
                    patterns: [
                        {
                            match:
                                "box-shadow: 0 0 2px rgba($color, .9), 0 0 4px rgba($color, .4), 0 0 1rem rgba($color, .3), 0 0 4rem rgba($color, .1);",
                            replacement:
                                "box-shadow: 0 0 2px RGBA($color, .9), 0 0 4px RGBA($color, .4), 0 0 1rem RGBA($color, .3), 0 0 4rem RGBA($color, .1);"
                        }
                    ]
                },
                files: [
                    {
                        expand: true,
                        flatten: true,
                        src: ["Content/Themes/vapor/_bootswatch.scss"],
                        dest: "Content/Themes/vapor/"
                    }
                ]
            },
            fontAwesome: {
                options: {
                    usePrefix: false,
                    patterns: [
                        {
                            match: "../webfonts",
                            replacement: "../Content/webfonts"
                        }
                    ]
                },
                files: [
                    {
                        expand: true,
                        flatten: true,
                        src: ["Content/fontawesome/_variables.scss"],
                        dest: "Content/fontawesome/"
                    }
                ]
            },
            flagIcons: {
                options: {
                    usePrefix: false,
                    patterns: [
                        {
                            match: "../flags",
                            replacement: "flags"
                        }
                    ]
                },
                files: [
                    {
                        expand: true,
                        flatten: true,
                        src: ["Content/flag-icons/_variables.scss"],
                        dest: "Content/flag-icons/"
                    }
                ]
            }
        },

        shell: {
            emailTemplates: {
                command: [
                    "@echo off",
                    "echo Build cerulean theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/cerulean/EmailTemplate.html  -c Content/Themes/cerulean/bootstrap_email.json",
                    "echo Build cosmo theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/cosmo/EmailTemplate.html -c Content/Themes/cosmo/bootstrap_email.json",
                    "echo Build cyborg theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/cyborg/EmailTemplate.html -c Content/Themes/cyborg/bootstrap_email.json",
                    "echo Build darkly theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/darkly/EmailTemplate.html -c Content/Themes/darkly/bootstrap_email.json",
                    "echo Build flatly theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/flatly/EmailTemplate.html -c Content/Themes/flatly/bootstrap_email.json",
                    "echo Build journal theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/journal/EmailTemplate.html -c Content/Themes/journal/bootstrap_email.json",
                    "echo Build litera theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/litera/EmailTemplate.html -c Content/Themes/litera/bootstrap_email.json",
                    "echo Build lumen theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/lumen/EmailTemplate.html -c Content/Themes/lumen/bootstrap_email.json",
                    "echo Build lux theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/lux/EmailTemplate.html -c Content/Themes/lux/bootstrap_email.json",
                    "echo Build materia theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/materia/EmailTemplate.html -c Content/Themes/materia/bootstrap_email.json",
                    "echo Build minty theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/minty/EmailTemplate.html -c Content/Themes/minty/bootstrap_email.json",
                    "echo Build morph theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/morph/EmailTemplate.html -c Content/Themes/morph/bootstrap_email.json",
                    "echo Build pulse theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/pulse/EmailTemplate.html -c Content/Themes/pulse/bootstrap_email.json",
                    "echo Build quartz theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/quartz/EmailTemplate.html -c Content/Themes/quartz/bootstrap_email.json",
                    "echo Build sandstone theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/sandstone/EmailTemplate.html -c Content/Themes/sandstone/bootstrap_email.json",
                    "echo Build simplex theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/simplex/EmailTemplate.html -c Content/Themes/simplex/bootstrap_email.json",
                    "echo Build sketchy theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/sketchy/EmailTemplate.html -c Content/Themes/sketchy/bootstrap_email.json",
                    "echo Build slate theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/slate/EmailTemplate.html -c Content/Themes/slate/bootstrap_email.json",
                    "echo Build solar theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/solar/EmailTemplate.html -c Content/Themes/solar/bootstrap_email.json",
                    "echo Build spacelab theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/spacelab/EmailTemplate.html -c Content/Themes/spacelab/bootstrap_email.json",
                    "echo Build superhero theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/superhero/EmailTemplate.html -c Content/Themes/superhero/bootstrap_email.json",
                    "echo Build united theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/united/EmailTemplate.html -c Content/Themes/united/bootstrap_email.json",
                    "echo Build vapor theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/vapor/EmailTemplate.html -c Content/Themes/vapor/bootstrap_email.json",
                    "echo Build yaf theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/yaf/EmailTemplate.html -c Content/Themes/yaf/bootstrap_email.json",
                    "echo Build yeti theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/yeti/EmailTemplate.html -c Content/Themes/yeti/bootstrap_email.json",
                    "echo Build zephyr theme email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/EmailTemplate.html -d Content/Themes/zephyr/EmailTemplate.html -c Content/Themes/zephyr/bootstrap_email.json",
                    "rmdir .sass-cache /s /q"
                ].join("&&")
            },
            emailDigestTemplates: {
                command: [
                    "@echo off",
                    "echo Build cerulean theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/cerulean/DigestTopicTemplate.html  -c Content/Themes/cerulean/bootstrap_email.json",
                    "echo Build cosmo theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/cosmo/DigestTopicTemplate.html -c Content/Themes/cosmo/bootstrap_email.json",
                    "echo Build cyborg theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/cyborg/DigestTopicTemplate.html -c Content/Themes/cyborg/bootstrap_email.json",
                    "echo Build darkly theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/darkly/DigestTopicTemplate.html -c Content/Themes/darkly/bootstrap_email.json",
                    "echo Build flatly theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/flatly/DigestTopicTemplate.html -c Content/Themes/flatly/bootstrap_email.json",
                    "echo Build journal theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/journal/DigestTopicTemplate.html -c Content/Themes/journal/bootstrap_email.json",
                    "echo Build litera theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/litera/DigestTopicTemplate.html -c Content/Themes/litera/bootstrap_email.json",
                    "echo Build lumen theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/lumen/DigestTopicTemplate.html -c Content/Themes/lumen/bootstrap_email.json",
                    "echo Build lux theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/lux/DigestTopicTemplate.html -c Content/Themes/lux/bootstrap_email.json",
                    "echo Build materia theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/materia/DigestTopicTemplate.html -c Content/Themes/materia/bootstrap_email.json",
                    "echo Build minty theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/minty/DigestTopicTemplate.html -c Content/Themes/minty/bootstrap_email.json",
                    "echo Build morph theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/morph/DigestTopicTemplate.html -c Content/Themes/morph/bootstrap_email.json",
                    "echo Build pulse theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/pulse/DigestTopicTemplate.html -c Content/Themes/pulse/bootstrap_email.json",
                    "echo Build quartz theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/quartz/DigestTopicTemplate.html -c Content/Themes/quartz/bootstrap_email.json",
                    "echo Build sandstone theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/sandstone/DigestTopicTemplate.html -c Content/Themes/sandstone/bootstrap_email.json",
                    "echo Build simplex theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/simplex/DigestTopicTemplate.html -c Content/Themes/simplex/bootstrap_email.json",
                    "echo Build sketchy theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/sketchy/DigestTopicTemplate.html -c Content/Themes/sketchy/bootstrap_email.json",
                    "echo Build slate theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/slate/DigestTopicTemplate.html -c Content/Themes/slate/bootstrap_email.json",
                    "echo Build solar theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/solar/DigestTopicTemplate.html -c Content/Themes/solar/bootstrap_email.json",
                    "echo Build spacelab theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/spacelab/DigestTopicTemplate.html -c Content/Themes/spacelab/bootstrap_email.json",
                    "echo Build superhero theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/superhero/DigestTopicTemplate.html -c Content/Themes/superhero/bootstrap_email.json",
                    "echo Build united theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/united/DigestTopicTemplate.html -c Content/Themes/united/bootstrap_email.json",
                    "echo Build vapor theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/vapor/DigestTopicTemplate.html -c Content/Themes/vapor/bootstrap_email.json",
                    "echo Build yaf theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/yaf/DigestTopicTemplate.html -c Content/Themes/yaf/bootstrap_email.json",
                    "echo Build yeti theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/yeti/DigestTopicTemplate.html -c Content/Themes/yeti/bootstrap_email.json",
                    "echo Build zephyr theme digest email template",
                    "..\\Tools\\BootstrapEmail\\BootstrapEmail.Cli -f Resources/DigestTopicTemplate.html -d Content/Themes/zephyr/DigestTopicTemplate.html -c Content/Themes/zephyr/bootstrap_email.json",
                    "rmdir .sass-cache /s /q"
                ].join("&&")
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
                    "Scripts/bootstrap.bundle.js",
                    "Scripts/forum/InstallWizard.js"
                ],
                dest: "Scripts/InstallWizard.comb.js"
            },

            codeMirror: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "node_modules/codemirror/lib/codemirror.js",
                    "node_modules/codemirror/mode/sql/sql.js",
                    "node_modules/codemirror/addon/edit/matchbrackets.js",
                    "node_modules/codemirror/addon/hint/show-hint.js",
                    "node_modules/codemirror/addon/hint/sql-hint.js"
                ],
                dest: "Scripts/codemirror.min.js"
            },

            yafEditor: {
                options: {
                    sourceMap: false,
                    output: { beautify: true },
                    mangle: false,
                    compress: false
                },
                src: [
                    "Scripts/editor/editor.js",
                    "Scripts/editor/undoManager.js",
                    "Scripts/editor/autoCloseTags.js",
                    "Scripts/editor/mentions.js"
                ],
                dest: "Scripts/editor/editor.comb.js"
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
                    "Scripts/forum/bootstrap-touchspin.js",
                    "Scripts/choices/assets/scripts/choices.js",
                    "Scripts/bs5-lightbox/dist/index.bundle.min.js",
                    "Scripts/forum/yaf.hoverCard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Attachments.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.SearchResults.js",
                    "Scripts/forum/yaf.SimilarTitles.js",
                    "Scripts/forum/yaf.Paging.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/forumExtensions.js"
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
                    "Scripts/forum/bootstrap-touchspin.js",
                    "Scripts/choices/assets/scripts/choices.js",
                    "Scripts/bs5-lightbox/dist/index.bundle.min.js",
                    "Scripts/forum/yaf.hoverCard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Attachments.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.SearchResults.js",
                    "Scripts/forum/yaf.SimilarTitles.js",
                    "Scripts/forum/yaf.Paging.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/forumExtensionsDnn.js"
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
                    "Scripts/bootstrap-notify.js",
                    "Scripts/forum/bootstrap-touchspin.js",
                    "Scripts/choices/assets/scripts/choices.js",
                    "Scripts/bs5-lightbox/dist/index.bundle.min.js",
                    "Scripts/forum/yaf.hoverCard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.Paging.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/ForumAdminExtensions.js"
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
                    "Scripts/forum/bootstrap-touchspin.js",
                    "Scripts/choices/assets/scripts/choices.js",
                    "Scripts/bs5-lightbox/dist/index.bundle.min.js",
                    "Scripts/forum/yaf.hoverCard.js",
                    "Scripts/prism.js",
                    "node_modules/long-press-event/src/long-press-event.js",
                    "Scripts/forum/yaf.Utilities.js",
                    "Scripts/forum/yaf.Albums.js",
                    "Scripts/forum/yaf.Notify.js",
                    "Scripts/forum/yaf.Paging.js",
                    "Scripts/forum/yaf.Main.js",
                    "Scripts/forum/yaf.contextMenu.js"
                ],
                dest: "Scripts/ForumAdminExtensionsDnn.js"
            },
            minify: {
                files: {
                    "Scripts/editor/editor.min.js": "Scripts/editor/editor.comb.js",
                    "Scripts/InstallWizard.comb.min.js": "Scripts/InstallWizard.comb.js",
                    "Scripts/codemirror.min.js": "Scripts/codemirror.min.js",
                    "Scripts/fileUploader.min.js": "Scripts/fileUploader.js",
                    "Scripts/forumExtensions.min.js": "Scripts/forumExtensions.js",
                    "Scripts/forumExtensionsDnn.min.js": "Scripts/forumExtensionsDnn.js",
                    "Scripts/forumAdminExtensions.min.js": "Scripts/forumAdminExtensions.js",
                    "Scripts/forumAdminExtensionsDnn.min.js": "Scripts/forumAdminExtensionsDnn.js"

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
                src: "Content/Themes/**/*.css"
            }
        },

        // CSS Minify
        cssmin: {
            codeMirror: {
                files: {
                    "Content/codemirror.min.css": [
                        "node_modules/codemirror/lib/codemirror.css",
                        "node_modules/codemirror/theme/monokai.css",
                        "node_modules/codemirror/addon/hint/show-hint.css"
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
                    semver: true,
                    packages: {
                        devDependencies: true, //only check for devDependencies
                        dependencies: true
                    }
                }
            }
        },
        file_append: {
            bootstrap: {
                files: [
                    {
                        append:
                            "\n\n.form-check-custom input {\n    @extend .form-check-input;\n}\n\n.form-check-custom label {\n    @extend .form-check-label\n}\n\n.aspNetDisabled input {\n    @extend .form-check-input;\n}\n\n.aspNetDisabled label {\n    @extend .form-check-label\n}\n\n.form-check-inline li {\n    @extend .form-check-inline;\n    margin-right: 2rem;\n}",
                        input: "Content/bootstrap/bootstrap.scss",
                        output: "Content/bootstrap/bootstrap.scss"
                    }
                ]
            }
        }
    });

    // PLUGINS
    grunt.loadNpmTasks("grunt-contrib-sass");
    grunt.loadNpmTasks("@lodder/grunt-postcss");
    grunt.loadNpmTasks("grunt-contrib-copy");
    grunt.loadNpmTasks("grunt-contrib-uglify");
    grunt.loadNpmTasks("grunt-contrib-cssmin");
    grunt.loadNpmTasks("@w8tcha/grunt-dev-update");
    grunt.loadNpmTasks("grunt-shell");
    grunt.loadNpmTasks("grunt-replace");
    grunt.loadNpmTasks("grunt-file-append");
    grunt.loadNpmTasks("grunt-webpack");

    grunt.registerTask("default",
        [
            "devUpdate", "webpack:lightBox", "uglify", "sass", "postcss", "cssmin"
        ]);

    grunt.registerTask("updateBootstrap",
        [
            "copy:bootstrap", "file_append:bootstrap"
        ]);

    grunt.registerTask("updateFontAwesome",
        [
            "copy:fontAwesome","replace:fontAwesome"
        ]);

    grunt.registerTask("updateBootswatchThemes",
        [
            "copy:bootswatchThemes", "replace:bootswatch"
        ]);

    grunt.registerTask("updateFlagIcons",
        [
            "copy:flagIcons", "replace:flagIcons"
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
