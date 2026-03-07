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
                        cwd: '../../YetAnotherForum.NET/wwwroot/assets',
                        dest: 'wwwroot/assets'
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
        }
    });

    // PLUGINS
    grunt.loadNpmTasks('grunt-contrib-copy');

    grunt.registerTask('default',
	    [
		    'copy'
	    ]);
};
