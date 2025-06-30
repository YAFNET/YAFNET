/**
 * Build process for YetAnotherForum.NET
 *
 * Don't know where to start?
 * Try: http://24ways.org/2013/grunt-is-not-weird-and-hard/
 */
module.exports = function(grunt) {
	require('@lodder/time-grunt')(grunt);
    // CONFIGURATION
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        copy: {
            updateRazorPages: {
                files: [
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/Pages',
                        dest: 'Areas/Forums/Pages'
                    }, {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/ViewComponents',
                        dest: 'ViewComponents'
                    },
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/wwwroot/css',
                        dest: 'wwwroot/css'
                    },
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/wwwroot/favicons',
                        dest: 'wwwroot/favicons'
                    },
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/wwwroot/images',
                        dest: 'wwwroot/images'
                    },
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/wwwroot/js',
                        dest: 'wwwroot/js'
                    },
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/wwwroot/languages',
                        dest: 'wwwroot/languages'
                    },
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/wwwroot/resources',
                        dest: 'wwwroot/resources'
                    },
                    {
                        expand: true,
                        src: '**/*.*',
                        cwd: '../../YetAnotherForum.NET/wwwroot/webfonts',
                        dest: 'wwwroot/webfonts'
                    }
                ]
            }
        },

        devUpdate: {
            main: {
                options: {
                    reportUpdated: true,
                    updateType: 'force',
                    semver: true,
                    packages: {
                        devDependencies: true,
                        dependencies: true
                    }
                }
            }
        }
    });

    // PLUGINS
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('@w8tcha/grunt-dev-update');

    grunt.registerTask('default',
	    [
		    'copy'
	    ]);
};
