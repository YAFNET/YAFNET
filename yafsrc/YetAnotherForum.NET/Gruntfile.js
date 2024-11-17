/**
 * Build process for YetAnotherForum.NET
 *
 * Don't know where to start?
 * Try: http://24ways.org/2013/grunt-is-not-weird-and-hard/
 */

const lightBoxWebpackConfig = require('./wwwroot/lib/bs5-lightbox/webpack.cdn.js');
const sass = require('sass');

module.exports = function(grunt) {
	// CONFIGURATION
	grunt.initConfig({
		secret: grunt.file.readJSON('../secret.json'),
		pkg: grunt.file.readJSON('package.json'),

		webpack: {
			lightBox: lightBoxWebpackConfig
		},

		copy: {
			bootstrap: {
				files: [
					{
						expand: true,
						src: '**/*.scss',
						cwd: 'node_modules/bootstrap/scss',
						dest: 'wwwroot/lib/bootstrap/'
					},
					{
						expand: true,
						src: '**/bootstrap.bundle.js',
						cwd: 'node_modules/bootstrap/dist/js/',
						dest: 'wwwroot/lib/'
					},
					{
						expand: true,
						src: '**/bootstrap.bundle.min.js',
						cwd: 'node_modules/bootstrap/dist/js/',
						dest: 'wwwroot/lib/'
					}
				]
			},
			fontAwesome: {
				files: [
					// includes files within path
					{
						expand: true,
						src: '**/*.scss',
						cwd: 'node_modules/@fortawesome/fontawesome-free/scss',
						dest: 'wwwroot/lib/fontawesome/'
					},
					{
						expand: true,
						src: '**/*.*',
						cwd: 'node_modules/@fortawesome/fontawesome-free/webfonts',
						dest: 'wwwroot/webfonts/'
					}
				]
			},
			bootswatchThemes: {
				files: [
					// includes files within path
					{ expand: true, src: '**/*.scss', cwd: 'node_modules/bootswatch/dist', dest: 'wwwroot/lib/themes/' }
				]
			},
			mdsDateTimePicker: {
				files: [
					// includes files within path
					{
						expand: true,
						src: 'mds.bs.datetimepicker.style.css',
						cwd: 'node_modules/md.bootstrappersiandatetimepicker/dist',
						dest: 'wwwroot/css/',
						rename: function(path) {
							return path + 'mds.datetimepicker.min.css';
						}
					},
					{
						expand: true,
						src: 'mds.bs.datetimepicker.js',
						cwd: 'node_modules/md.bootstrappersiandatetimepicker/dist',
						dest: 'wwwroot/js/',
						rename: function(path) {
							return path + 'mds.datetimepicker.min.js';
						}
					}
				]
			},
			flagIcons: {
				files: [
					{
						expand: true,
						src: '**/*.scss',
						cwd: 'node_modules/flag-icons/sass',
						dest: 'wwwroot/lib/flag-icons/'
					},
					{
						expand: true,
						src: '**/*.svg',
						cwd: 'node_modules/flag-icons/flags',
						dest: 'wwwroot/css/flags/'
					}
				]
			},
			SCEditor: {
				files: [
					{
						expand: true,
						src: '**/*.css',
						cwd: 'node_modules/@yafnet/sceditor/minified/themes',
						dest: 'wwwroot/css/'
					}, {
						expand: true,
						src: '**/*.js',
						cwd: 'node_modules/@yafnet/sceditor/languages',
						dest: 'wwwroot/js/sceditor/languages/'
					}
				]
			},
			appSettingsMySql: {
				files: [
					{
						expand: true,
						src: 'appsettings-MySql.json',
						cwd: '',
						dest: 'bin/Release/net9.0/publish/',
						rename: function(path) {
							return path + 'appsettings.json';
						}
					}
				]
			},
			appSettingsPostgreSQL: {
				files: [
					{
						expand: true,
						src: 'appsettings-PostgreSQL.json',
						cwd: '',
						dest: 'bin/Release/net9.0/publish/',
						rename: function(path) {
							return path + 'appsettings.json';
						}
					}
				]
			},
			appSettingsSqlite: {
				files: [
					{
						expand: true,
						src: 'appsettings-Sqlite.json',
						cwd: '',
						dest: 'bin/Release/net9.0/publish/',
						rename: function(path) {
							return path + 'appsettings.json';
						}
					}
				]
			},
			appSettingsSqlServer: {
				files: [
					{
						expand: true,
						src: 'appsettings-SqlServer.json',
						cwd: '',
						dest: 'bin/Release/net9.0/publish/',
						rename: function(path) {
							return path + 'appsettings.json';
						}
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
								'box-shadow: 0 0 2px rgba($color, .9), 0 0 4px rgba($color, .4), 0 0 1rem rgba($color, .3), 0 0 4rem rgba($color, .1);',
							replacement:
								'box-shadow: 0 0 2px RGBA($color, .9), 0 0 4px RGBA($color, .4), 0 0 1rem RGBA($color, .3), 0 0 4rem RGBA($color, .1);'
						}
					]
				},
				files: [
					{
						expand: true,
						flatten: true,
						src: ['wwwroot/lib/themes/vapor/_bootswatch.scss'],
						dest: 'wwwroot/lib/themes/vapor/'
					}
				]
			},

			flagIcons: {
				options: {
					usePrefix: false,
					patterns: [
						{
							match: '../flags',
							replacement: 'flags'
						}
					]
				},
				files: [
					{
						expand: true,
						flatten: true,
						src: ['wwwroot/lib/flag-icons/_variables.scss'],
						dest: 'wwwroot/lib/flag-icons/'
					}
				]
			}
		},

		shell: {
			syncLanguages: {
				command: [
					'@echo off',
					'cd ..\\Tools\\LanguageManager\\',
					'echo update languages',
					'SyncLangtoEnglish'
				].join('&&')
			},
			compileLanguages: {
				command: [
					'@echo off',
					'..\\Tools\\LanguageManager\\YAFNET.LanguageManager %CD%\\bin\\Release\\net9.0\\publish\\wwwroot\\languages\\ -minify'
				].join('&&')
			},
			deleteOldNuGetPackages: {
				command: [
					'@echo off',
					'cd ..\\deploy\\ ',
					'if exist *.nupkg (del *.nupkg) ',
					'cd ..\\ ',
					'del YAFNET.Types\\bin\\Release\\*.nupkg ',
					'del YAFNET.Configuration\\bin\\Release\\*.nupkg ',
					'del YAFNET.Core\\bin\\Release\\*.nupkg ',
					'del YAFNET.Web\\bin\\Release\\*.nupkg ',
					'del YAFNET.Data\\YAFNET.Data.SqlServer\\bin\\Release\\*.nupkg ',
					'del YAFNET.Data\\YAFNET.Data.MySql\\bin\\Release\\*.nupkg ',
					'del YAFNET.Data\\YAFNET.Data.PostgreSQL\\bin\\Release\\*.nupkg ',
					'del YAFNET.Data\\YAFNET.Data.Sqlite\\bin\\Release\\*.nupkg ',
					'del YAFNET.UI\\YAFNET.RazorPages\\bin\\Release\\*.nupkg ',
					'del YAFNET.UI\\YAFNET.UI.Chat\\bin\\Release\\*.nupkg '
				].join('&&')
			},
			createNuGetPackages: {
				command: [
					'cd ..\\ ',
					'dotnet pack YAFNET.Types/YAFNET.Types.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.Configuration/YAFNET.Configuration.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.Core/YAFNET.Core.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.Web/YAFNET.Web.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.Data/YAFNET.Data.SqlServer/YAFNET.Data.SqlServer.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.Data/YAFNET.Data.MySql/YAFNET.Data.MySql.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.Data/YAFNET.Data.PostgreSQL/YAFNET.Data.PostgreSQL.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.Data/YAFNET.Data.Sqlite/YAFNET.Data.Sqlite.csproj /p:Configuration=Release '
				].join('&&')
			},
			createNuGetUIPackages: {
				command: [
					'cd ..\\ ',
					'dotnet pack YAFNET.UI/YAFNET.RazorPages/YAFNET.RazorPages.csproj /p:Configuration=Release ',
					'dotnet pack YAFNET.UI/YAFNET.UI.Chat/YAFNET.UI.Chat.csproj /p:Configuration=Release '
				].join('&&')
			},
			copyNuGetPackages: {
				command: [
					'cd ..\\ ',
					'COPY YAFNET.Types\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.Configuration\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.Core\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.Web\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.Data\\YAFNET.Data.SqlServer\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.Data\\YAFNET.Data.MySql\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.Data\\YAFNET.Data.PostgreSQL\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.Data\\YAFNET.Data.Sqlite\\bin\\Release\\*.nupkg deploy\\ '
				].join('&&')
			},
			copyNuGetUIPackages: {
				command: [
					'cd ..\\ ',
					'COPY YAFNET.UI\\YAFNET.RazorPages\\bin\\Release\\*.nupkg deploy\\ ',
					'COPY YAFNET.UI\\YAFNET.UI.Chat\\bin\\Release\\*.nupkg deploy\\ '
				].join('&&')
			},
			deletePublish: {
				command: [
					'@echo off',
					'if exist bin\\Release\\net9.0\\publish\\ (rmdir bin\\Release\\net8.0\\publish\\ /s /q)'
				].join('&&')
			},
			deleteAppSettings: {
				command: [
					'@echo off',
					'del bin\\Release\\net9.0\\publish\\appsettings-MySql.json ',
					'del bin\\Release\\net9.0\\publish\\appsettings-PostgreSQL.json ',
					'del bin\\Release\\net9.0\\publish\\appsettings-Sqlite.json ',
					'del bin\\Release\\net9.0\\publish\\appsettings-SqlServer.json ',
					'rmdir bin\\Release\\net9.0\\publish\\wwwroot\\uploads\\ /s /q '
				].join('&&')
			},
			deploySqlServer: {
				command: [
					'@echo off',
					'dotnet publish /p:Configuration=Release ../YAF.NET-SqlServer.sln'
				].join('&&')
			},
			deployMySql: {
				command: [
					'@echo off',
					'dotnet publish /p:Configuration=Release ../YAF.NET-MySql.sln'
				].join('&&')
			},
			deployPostgreSQL: {
				command: [
					'@echo off',
					'dotnet publish /p:Configuration=Release ../YAF.NET-PostgreSQL.sln'
				].join('&&')
			},
			deploySqlite: {
				command: [
					'@echo off',
					'dotnet publish /p:Configuration=Release ../YAF.NET-Sqlite.sln'
				].join('&&')
			},
			emailTemplates: {
				command: [
					'@echo off',
					'cd ..\\Tools\\BootstrapEmail\\',
					'echo Build cerulean theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/cerulean/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/cerulean/bootstrap_email.json',
					'echo Build cosmo theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/cosmo/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/cosmo/bootstrap_email.json',
					'echo Build cyborg theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/cyborg/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/cyborg/bootstrap_email.json',
					'echo Build darkly theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/darkly/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/darkly/bootstrap_email.json',
					'echo Build flatly theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/flatly/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/flatly/bootstrap_email.json',
					'echo Build journal theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/journal/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/journal/bootstrap_email.json',
					'echo Build litera theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/litera/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/litera/bootstrap_email.json',
					'echo Build lumen theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/lumen/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/lumen/bootstrap_email.json',
					'echo Build lux theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/lux/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/lux/bootstrap_email.json',
					'echo Build materia theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/materia/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/materia/bootstrap_email.json',
					'echo Build minty theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/minty/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/minty/bootstrap_email.json',
					'echo Build morph theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/morph/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/morph/bootstrap_email.json',
					'echo Build pulse theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/pulse/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/pulse/bootstrap_email.json',
					'echo Build quartz theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/quartz/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/quartz/bootstrap_email.json',
					'echo Build sandstone theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/sandstone/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/sandstone/bootstrap_email.json',
					'echo Build simplex theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/simplex/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/simplex/bootstrap_email.json',
					'echo Build sketchy theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/sketchy/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/sketchy/bootstrap_email.json',
					'echo Build slate theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/slate/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/slate/bootstrap_email.json',
					'echo Build solar theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/solar/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/solar/bootstrap_email.json',
					'echo Build spacelab theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/spacelab/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/spacelab/bootstrap_email.json',
					'echo Build superhero theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/superhero/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/superhero/bootstrap_email.json',
					'echo Build united theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/united/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/united/bootstrap_email.json',
					'echo Build vapor theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/vapor/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/vapor/bootstrap_email.json',
					'echo Build yaf theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/yaf/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/yaf/bootstrap_email.json',
					'echo Build yeti theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/yeti/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/yeti/bootstrap_email.json',
					'echo Build zephyr theme email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/EmailTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/zephyr/EmailTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/zephyr/bootstrap_email.json',
					'rmdir .sass-cache /s /q'
				].join('&&')
			},
			emailDigestTemplates: {
				command: [
					'@echo off',
					'cd ..\\Tools\\BootstrapEmail\\',
					'echo Build cerulean theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/cerulean/DigestTopicTemplate.html  -c ../../YetAnotherForum.NET/wwwroot/lib/themes/cerulean/bootstrap_email-digest.json',
					'echo Build cosmo theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/cosmo/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/cosmo/bootstrap_email-digest.json',
					'echo Build cyborg theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/cyborg/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/cyborg/bootstrap_email-digest.json',
					'echo Build darkly theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/darkly/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/darkly/bootstrap_email-digest.json',
					'echo Build flatly theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/flatly/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/flatly/bootstrap_email-digest.json',
					'echo Build journal theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/journal/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/journal/bootstrap_email-digest.json',
					'echo Build litera theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/litera/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/litera/bootstrap_email-digest.json',
					'echo Build lumen theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/lumen/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/lumen/bootstrap_email-digest.json',
					'echo Build lux theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/lux/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/lux/bootstrap_email-digest.json',
					'echo Build materia theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/materia/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/materia/bootstrap_email-digest.json',
					'echo Build minty theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/minty/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/minty/bootstrap_email-digest.json',
					'echo Build morph theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/morph/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/morph/bootstrap_email-digest.json',
					'echo Build pulse theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/pulse/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/pulse/bootstrap_email-digest.json',
					'echo Build quartz theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/quartz/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/quartz/bootstrap_email-digest.json',
					'echo Build sandstone theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/sandstone/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/sandstone/bootstrap_email-digest.json',
					'echo Build simplex theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/simplex/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/simplex/bootstrap_email-digest.json',
					'echo Build sketchy theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/sketchy/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/sketchy/bootstrap_email-digest.json',
					'echo Build slate theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/slate/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/slate/bootstrap_email-digest.json',
					'echo Build solar theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/solar/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/solar/bootstrap_email-digest.json',
					'echo Build spacelab theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/spacelab/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/spacelab/bootstrap_email-digest.json',
					'echo Build superhero theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/superhero/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/superhero/bootstrap_email-digest.json',
					'echo Build united theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/united/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/united/bootstrap_email-digest.json',
					'echo Build vapor theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/vapor/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/vapor/bootstrap_email-digest.json',
					'echo Build yaf theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/yaf/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/yaf/bootstrap_email-digest.json',
					'echo Build yeti theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/yeti/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/yeti/bootstrap_email-digest.json',
					'echo Build zephyr theme digest email template',
					'BootstrapEmail.Cli -f ../../YetAnotherForum.NET/wwwroot/Resources/DigestTopicTemplate.html -d ../../YetAnotherForum.NET/wwwroot/css/themes/zephyr/DigestTopicTemplate.html -c ../../YetAnotherForum.NET/wwwroot/lib/themes/zephyr/bootstrap_email-digest.json',
					'rmdir .sass-cache /s /q'
				].join('&&')
			},
			publishNuGetPackages: {
				command: [
					'@echo off',
					'cd ..\\deploy\\',
					'echo publish Package YAFNET.Types to NuGet',
					'dotnet nuget push "YAFNET.Types.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.Configuration to NuGet',
					'dotnet nuget push "YAFNET.Configuration.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.Web to NuGet',
					'dotnet nuget push "YAFNET.Web.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.Core to NuGet',
					'dotnet nuget push "YAFNET.Core.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.Data.MySql to NuGet',
					'dotnet nuget push "YAFNET.Data.MySql.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.Data.PostgreSQL to NuGet',
					'dotnet nuget push "YAFNET.Data.PostgreSQL.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.Data.Sqlite to NuGet',
					'dotnet nuget push "YAFNET.Data.Sqlite.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.Data.SqlServer to NuGet',
					'dotnet nuget push "YAFNET.Data.SqlServer.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"'
				].join('&&')
			},
			publishNuGetUIPackages: {
				command: [
					'@echo off',
					'cd ..\\deploy\\',
					'echo publish Package YAFNET.RazorPages to NuGet',
					'dotnet nuget push "YAFNET.RazorPages.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"',
					'echo publish Package YAFNET.UI.Chat to NuGet',
					'dotnet nuget push "YAFNET.UI.Chat.<%= pkg.versionNuget %>.nupkg" --source "https://api.nuget.org/v3/index.json" --api-key "<%= secret.api %>"'
				].join('&&')
			},
			publishToGitHub: {
				command: [
					'@echo off',
					'cd ..\\Tools\\GitHubReleaser\\bin\\Release\\net9.0',
					'echo publish Packages to GitHub.com',
					'GitHubReleaser YAFNET.json'
				].join('&&')
			}
		},

		// Minimize JS
		uglify: {
			themeSelector: {
				options: {
					sourceMap: false,
					output: { beautify: true },
					mangle: false,
					compress: false
				},
				src: [
					'wwwroot/lib/bootstrap/color-modes.js'
				],
				dest: 'wwwroot/js/themeSelector.min.js'
			},
			installWizard: {
				options: {
					sourceMap: false,
					output: { beautify: true },
					mangle: false,
					compress: false
				},
				src: [
					'wwwroot/lib/bootstrap.bundle.js',
					'wwwroot/lib/forum/installWizard.js'
				],
				dest: 'wwwroot/js/InstallWizard.comb.js'
			},

			codeMirror: {
				options: {
					sourceMap: false,
					output: { beautify: true },
					mangle: false,
					compress: false
				},
				src: [
					'node_modules/codemirror/lib/codemirror.js',
					'node_modules/codemirror/mode/sql/sql.js',
					'node_modules/codemirror/addon/edit/matchbrackets.js',
					'node_modules/codemirror/addon/hint/show-hint.js',
					'node_modules/codemirror/addon/hint/sql-hint.js'
				],
				dest: 'wwwroot/js/codemirror.min.js'
			},
			yafEditor: {
				options: {
					sourceMap: false,
					output: { beautify: true },
					mangle: false,
					compress: false
				},
				src: [
					'wwwroot/lib/editor/editor.js',
					'wwwroot/lib/editor/undoManager.js',
					'wwwroot/lib/editor/autoCloseTags.js',
					'wwwroot/lib/editor/mentions.js'
				],
				dest: 'wwwroot/js/editor.comb.js'
			},
			SCEditor: {
				options: {
					sourceMap: false,
					output: { beautify: true },
					mangle: false,
					compress: false
				},
				src: [
					'node_modules/@yafnet/sceditor/minified/sceditor.min.js',
					'node_modules/@yafnet/sceditor/minified/formats/bbcode.js',
					'node_modules/@yafnet/sceditor/minified/icons/fontawesome.js',
					'node_modules/@yafnet/sceditor/minified/plugins/dragdrop.js',
					'node_modules/@yafnet/sceditor/minified/plugins/undo.js',
					'node_modules/@yafnet/sceditor/minified/plugins/plaintext.js',
					'wwwroot/lib/sceditor/mentions.js'
				],
				dest: 'wwwroot/js/sceditor/sceditor.min.js'
			},
			SCEditorLanguages: {
				options: {
					warnings: true,
					compress: true,
					mangle: true
				},
				files: [
					{
						expand: true,
						filter: 'isFile',
						cwd: 'wwwroot/js/sceditor/',
						src: 'languages/**.js',
						dest: 'wwwroot/js/sceditor'
					}
				]
			},
			forumExtensions: {
				options: {
					sourceMap: false,
					output: { beautify: true },
					mangle: false,
					compress: false
				},
				src: [
					'wwwroot/lib/bootstrap.bundle.js',
					'wwwroot/lib/bootbox.js',
					'wwwroot/lib/dark-editable.js',
					'wwwroot/lib/bootstrap-notify.js',
					'wwwroot/lib/forum/bootstrap-touchspin.js',
					'wwwroot/lib/choices/assets/scripts/choices.js',
					'wwwroot/lib/bs5-lightbox/dist/index.bundle.min.js',
					'wwwroot/lib/forum/hoverCard.js',
					'wwwroot/lib/prism.js',
					'node_modules/long-press-event/src/long-press-event.js',
					'node_modules/@microsoft/signalr/dist/browser/signalr.js',
					'wwwroot/lib/forum/utilities.js',
					'wwwroot/lib/forum/albums.js',
					'wwwroot/lib/forum/attachments.js',
					'wwwroot/lib/forum/notify.js',
					'wwwroot/lib/forum/searchResults.js',
					'wwwroot/lib/forum/similarTitles.js',
					'wwwroot/lib/forum/paging.js',
					'wwwroot/lib/forum/main.js',
					'wwwroot/lib/forum/modals.js',
					'wwwroot/lib/forum/notificationHub.js',
					'wwwroot/lib/forum/contextMenu.js',
					'wwwroot/lib/forum/chatHub.js',
					'wwwroot/lib/form-serialize/index.js'
				],
				dest: 'wwwroot/js/forumExtensions.js'
			},
			forumAdminExtensions: {
				options: {
					sourceMap: false,
					output: { beautify: true },
					mangle: false,
					compress: false
				},
				src: [
					'wwwroot/lib/bootstrap.bundle.js',
					'wwwroot/lib/bootbox.js',
					'wwwroot/lib/dark-editable.js',
					'wwwroot/lib/bootstrap-notify.js',
					'wwwroot/lib/forum/bootstrap-touchspin.js',
					'wwwroot/lib/choices/assets/scripts/choices.js',
					'wwwroot/lib/bs5-lightbox/dist/index.bundle.min.js',
					'wwwroot/lib/forum/hoverCard.js',
					'wwwroot/lib/prism.js',
					'node_modules/long-press-event/src/long-press-event.js',
					'node_modules/@microsoft/signalr/dist/browser/signalr.js',
					'wwwroot/lib/forum/utilities.js',
					'wwwroot/lib/forum/albums.js',
					'wwwroot/lib/forum/notify.js',
					'wwwroot/lib/forum/paging.js',
					'wwwroot/lib/forum/main.js',
					'wwwroot/lib/forum/modals.js',
					'wwwroot/lib/forum/notificationHub.js',
					'wwwroot/lib/forum/contextMenu.js',
					'wwwroot/lib/form-serialize/index.js'
				],
				dest: 'wwwroot/js/forumAdminExtensions.js'
			},
			minify: {
				files: {
					"wwwroot/js/themeSelector.min.js": 'wwwroot/js/themeSelector.min.js',
					"wwwroot/js/editor.min.js": 'wwwroot/js/editor.comb.js',
					"wwwroot/js/InstallWizard.comb.min.js": 'wwwroot/js/InstallWizard.comb.js',
					"wwwroot/js/codemirror.min.js": 'wwwroot/js/codemirror.min.js',
					"wwwroot/js/fileUploader.min.js": 'wwwroot/lib/fileUploader.js',
					"wwwroot/js/forumExtensions.min.js": 'wwwroot/js/forumExtensions.js',
					"wwwroot/js/forumAdminExtensions.min.js": 'wwwroot/js/forumAdminExtensions.js'

				}
			}
		},

		sass: {
			options: {
				implementation: sass,
				sourceMap: false
			},
			installWizard: {
				files: {
					"wwwroot/css/InstallWizard.css": 'wwwroot/lib/InstallWizard.scss'
				}
			},
			forum: {
				files: {
					"wwwroot/css/forum.css": 'wwwroot/lib/forum.scss'
				}
			},
			forumAdmin: {
				files: {
					"wwwroot/css/forum-admin.css": 'wwwroot/lib/forum-admin.scss'
				}
			},
			bootstrap: {
				files: {
					"wwwroot/lib/bootstrap/bootstrap.css": 'wwwroot/lib/bootstrap/bootstrap.scss'
				}
			},
			themes: {
				files: {
					"wwwroot/css/themes/zephyr/bootstrap-forum.css": 'wwwroot/lib/themes/zephyr/bootstrap-forum.scss',
					"wwwroot/css/themes/yaf/bootstrap-forum.css": 'wwwroot/lib/themes/yaf/bootstrap-forum.scss',
					"wwwroot/css/themes/yeti/bootstrap-forum.css": 'wwwroot/lib/themes/yeti/bootstrap-forum.scss',
					"wwwroot/css/themes/vapor/bootstrap-forum.css": 'wwwroot/lib/themes/vapor/bootstrap-forum.scss',
					"wwwroot/css/themes/united/bootstrap-forum.css": 'wwwroot/lib/themes/united/bootstrap-forum.scss',
					"wwwroot/css/themes/superhero/bootstrap-forum.css":
						'wwwroot/lib/themes/superhero/bootstrap-forum.scss',
					"wwwroot/css/themes/spacelab/bootstrap-forum.css":
						'wwwroot/lib/themes/spacelab/bootstrap-forum.scss',
					"wwwroot/css/themes/solar/bootstrap-forum.css": 'wwwroot/lib/themes/solar/bootstrap-forum.scss',
					"wwwroot/css/themes/slate/bootstrap-forum.css": 'wwwroot/lib/themes/slate/bootstrap-forum.scss',
					"wwwroot/css/themes/sketchy/bootstrap-forum.css": 'wwwroot/lib/themes/sketchy/bootstrap-forum.scss',
					"wwwroot/css/themes/simplex/bootstrap-forum.css": 'wwwroot/lib/themes/simplex/bootstrap-forum.scss',
					"wwwroot/css/themes/sandstone/bootstrap-forum.css":
						'wwwroot/lib/themes/sandstone/bootstrap-forum.scss',
					"wwwroot/css/themes/quartz/bootstrap-forum.css": 'wwwroot/lib/themes/quartz/bootstrap-forum.scss',
					"wwwroot/css/themes/pulse/bootstrap-forum.css": 'wwwroot/lib/themes/pulse/bootstrap-forum.scss',
					"wwwroot/css/themes/morph/bootstrap-forum.css": 'wwwroot/lib/themes/morph/bootstrap-forum.scss',
					"wwwroot/css/themes/minty/bootstrap-forum.css": 'wwwroot/lib/themes/minty/bootstrap-forum.scss',
					"wwwroot/css/themes/materia/bootstrap-forum.css": 'wwwroot/lib/themes/materia/bootstrap-forum.scss',
					"wwwroot/css/themes/lux/bootstrap-forum.css": 'wwwroot/lib/themes/lux/bootstrap-forum.scss',
					"wwwroot/css/themes/lumen/bootstrap-forum.css": 'wwwroot/lib/themes/lumen/bootstrap-forum.scss',
					"wwwroot/css/themes/litera/bootstrap-forum.css": 'wwwroot/lib/themes/litera/bootstrap-forum.scss',
					"wwwroot/css/themes/journal/bootstrap-forum.css": 'wwwroot/lib/themes/journal/bootstrap-forum.scss',
					"wwwroot/css/themes/flatly/bootstrap-forum.css": 'wwwroot/lib/themes/flatly/bootstrap-forum.scss',
					"wwwroot/css/themes/darkly/bootstrap-forum.css": 'wwwroot/lib/themes/darkly/bootstrap-forum.scss',
					"wwwroot/css/themes/cyborg/bootstrap-forum.css": 'wwwroot/lib/themes/cyborg/bootstrap-forum.scss',
					"wwwroot/css/themes/cosmo/bootstrap-forum.css": 'wwwroot/lib/themes/cosmo/bootstrap-forum.scss',
					"wwwroot/css/themes/cerulean/bootstrap-forum.css":
						'wwwroot/lib/themes/cerulean/bootstrap-forum.scss'
				}
			}
		},

		postcss: {
			options: {
				map: false,
				processors: [
					require('autoprefixer')({ overrideBrowserslist: 'last 2 versions' })
				]
			},
			installWizard: {
				src: 'wwwroot/css/InstallWizard.css'
			},
			forum: {
				src: 'wwwroot/css/forum.css'
			},
			forumAdmin: {
				src: 'wwwroot/css/forum-admin.css'
			},
			themes: {
				src: 'wwwroot/css/themes/**/*.css'
			}
		},

		// CSS Minify
		cssmin: {
			codeMirror: {
				files: {
					"wwwroot/css/codemirror.min.css": [
						'node_modules/codemirror/lib/codemirror.css',
						'node_modules/codemirror/theme/monokai.css',
						'node_modules/codemirror/addon/hint/show-hint.css'
					]
				}
			},
			other: {
				files: {
					"wwwroot/css/InstallWizard.min.css": 'wwwroot/css/InstallWizard.css',
					"wwwroot/css/forum.min.css": 'wwwroot/css/forum.css',
					"wwwroot/css/forum-admin.min.css": 'wwwroot/css/forum-admin.css'
				}
			},
			themes: {
				files: {
					"wwwroot/css/themes/zephyr/bootstrap-forum.min.css":
						'wwwroot/css/themes/zephyr/bootstrap-forum.css',
					"wwwroot/css/themes/yaf/bootstrap-forum.min.css": 'wwwroot/css/themes/yaf/bootstrap-forum.css',
					"wwwroot/css/themes/yeti/bootstrap-forum.min.css": 'wwwroot/css/themes/yeti/bootstrap-forum.css',
					"wwwroot/css/themes/vapor/bootstrap-forum.min.css": 'wwwroot/css/themes/vapor/bootstrap-forum.css',
					"wwwroot/css/themes/united/bootstrap-forum.min.css":
						'wwwroot/css/themes/united/bootstrap-forum.css',
					"wwwroot/css/themes/superhero/bootstrap-forum.min.css":
						'wwwroot/css/themes/superhero/bootstrap-forum.css',
					"wwwroot/css/themes/spacelab/bootstrap-forum.min.css":
						'wwwroot/css/themes/spacelab/bootstrap-forum.css',
					"wwwroot/css/themes/solar/bootstrap-forum.min.css": 'wwwroot/css/themes/solar/bootstrap-forum.css',
					"wwwroot/css/themes/slate/bootstrap-forum.min.css": 'wwwroot/css/themes/slate/bootstrap-forum.css',
					"wwwroot/css/themes/sketchy/bootstrap-forum.min.css":
						'wwwroot/css/themes/sketchy/bootstrap-forum.css',
					"wwwroot/css/themes/simplex/bootstrap-forum.min.css":
						'wwwroot/css/themes/simplex/bootstrap-forum.css',
					"wwwroot/css/themes/sandstone/bootstrap-forum.min.css":
						'wwwroot/css/themes/sandstone/bootstrap-forum.css',
					"wwwroot/css/themes/quartz/bootstrap-forum.min.css":
						'wwwroot/css/themes/quartz/bootstrap-forum.css',
					"wwwroot/css/themes/pulse/bootstrap-forum.min.css": 'wwwroot/css/themes/pulse/bootstrap-forum.css',
					"wwwroot/css/themes/morph/bootstrap-forum.min.css": 'wwwroot/css/themes/morph/bootstrap-forum.css',
					"wwwroot/css/themes/minty/bootstrap-forum.min.css": 'wwwroot/css/themes/minty/bootstrap-forum.css',
					"wwwroot/css/themes/materia/bootstrap-forum.min.css":
						'wwwroot/css/themes/materia/bootstrap-forum.css',
					"wwwroot/css/themes/lux/bootstrap-forum.min.css": 'wwwroot/css/themes/lux/bootstrap-forum.css',
					"wwwroot/css/themes/lumen/bootstrap-forum.min.css": 'wwwroot/css/themes/lumen/bootstrap-forum.css',
					"wwwroot/css/themes/litera/bootstrap-forum.min.css":
						'wwwroot/css/themes/litera/bootstrap-forum.css',
					"wwwroot/css/themes/journal/bootstrap-forum.min.css":
						'wwwroot/css/themes/journal/bootstrap-forum.css',
					"wwwroot/css/themes/flatly/bootstrap-forum.min.css":
						'wwwroot/css/themes/flatly/bootstrap-forum.css',
					"wwwroot/css/themes/darkly/bootstrap-forum.min.css":
						'wwwroot/css/themes/darkly/bootstrap-forum.css',
					"wwwroot/css/themes/cyborg/bootstrap-forum.min.css":
						'wwwroot/css/themes/cyborg/bootstrap-forum.css',
					"wwwroot/css/themes/cosmo/bootstrap-forum.min.css": 'wwwroot/css/themes/cosmo/bootstrap-forum.css',
					"wwwroot/css/themes/cerulean/bootstrap-forum.min.css":
						'wwwroot/css/themes/cerulean/bootstrap-forum.css'
				}
			}
		},
		devUpdate: {
			main: {
				options: {
					reportUpdated: true,
					updateType: 'force',
					semver: true,
					packages: {
						devDependencies: true, //only check for devDependencies
						dependencies: true
					}
				}
			}
		},
		zip: {
			"YAF-SqlServer-Deploy": {
				cwd: 'bin/Release/net9.0/publish/',
				src: ['bin/Release/net9.0/publish/**/*'],
				dest: '../deploy/YAF.SqlSever-v<%= pkg.version %>.zip'
			},
			"YAF-MySql-Deploy": {
				cwd: 'bin/Release/net9.0/publish/',
				src: ['bin/Release/net9.0/publish/**/*'],
				dest: '../deploy/YAF.MySql-v<%= pkg.version %>.zip'
			},
			"YAF-PostgreSQL-Deploy": {
				cwd: 'bin/Release/net9.0/publish/',
				src: ['bin/Release/net9.0/publish/**/*'],
				dest: '../deploy/YAF.PostgreSQL-v<%= pkg.version %>.zip'
			},
			"YAF-Sqlite-Deploy": {
				cwd: 'bin/Release/net9.0/publish/',
				src: ['bin/Release/net9.0/publish/**/*'],
				dest: '../deploy/YAF.Sqlite-v<%= pkg.version %>.zip'
			}
		}
	});

	// PLUGINS
	grunt.loadNpmTasks('@lodder/grunt-postcss');
	grunt.loadNpmTasks('grunt-contrib-copy');
	grunt.loadNpmTasks('grunt-contrib-uglify');
	grunt.loadNpmTasks('grunt-contrib-cssmin');
	grunt.loadNpmTasks('@w8tcha/grunt-dev-update');
	grunt.loadNpmTasks('grunt-sass');
	grunt.loadNpmTasks('grunt-shell');
	grunt.loadNpmTasks('grunt-replace');
	grunt.loadNpmTasks('grunt-zip');
	grunt.loadNpmTasks('grunt-file-append');
	grunt.loadNpmTasks('grunt-webpack');

	grunt.registerTask('default',
		[
			'webpack:lightBox', 'uglify', 'sass', 'postcss', 'cssmin'
		]);

	grunt.registerTask('updatePackages',
		[
			'devUpdate'
		]);

	grunt.registerTask('syncLanguages',
		[
			'shell:syncLanguages'
		]);

	grunt.registerTask('updateBootstrap',
		[
			'copy:bootstrap'
		]);

	grunt.registerTask('updateFontAwesome',
		[
			'copy:fontAwesome'
		]);

	grunt.registerTask('updateBootswatchThemes',
		[
			'copy:bootswatchThemes', 'replace:bootswatch'
		]);

	grunt.registerTask('updateFlagIcons',
		[
			'copy:flagIcons', 'replace:flagIcons'
		]);

	grunt.registerTask('updateSCEditor',
		[
			'copy:SCEditor'
		]);

	grunt.registerTask('emailTemplates',
		[
			'shell:emailTemplates', 'shell:emailDigestTemplates'
		]);

	grunt.registerTask('js',
		[
			'uglify'
		]);

	grunt.registerTask('css',
		[
			'sass', 'postcss', 'cssmin'
		]);

	grunt.registerTask('deploy-SqlServer',
		[
			'shell:deletePublish', 'shell:deploySqlServer', 'shell:compileLanguages', 'copy:appSettingsSqlServer',
			'shell:deleteAppSettings',
			'zip:YAF-SqlServer-Deploy'
		]);

	grunt.registerTask('deploy-MySql',
		[
			'shell:deletePublish', 'shell:deployMySql', 'shell:compileLanguages', 'copy:appSettingsMySql',
			'shell:deleteAppSettings',
			'zip:YAF-MySql-Deploy'
		]);

	grunt.registerTask('deploy-PostgreSQL',
		[
			'shell:deletePublish', 'shell:deployPostgreSQL', 'shell:compileLanguages', 'copy:appSettingsPostgreSQL',
			'shell:deleteAppSettings',
			'zip:YAF-PostgreSQL-Deploy'
		]);

	grunt.registerTask('deploy-Sqlite',
		[
			'shell:deletePublish', 'shell:deploySqlite', 'shell:compileLanguages', 'copy:appSettingsSqlite',
			'shell:deleteAppSettings',
			'zip:YAF-Sqlite-Deploy'
		]);

	grunt.registerTask('deploy',
		[
			'shell:deletePublish', 'shell:deploySqlite', 'shell:compileLanguages', 'copy:appSettingsSqlite',
			'shell:deleteAppSettings', 'zip:YAF-Sqlite-Deploy',
			'shell:deletePublish', 'shell:deploySqlServer', 'shell:compileLanguages', 'copy:appSettingsSqlServer',
			'shell:deleteAppSettings', 'zip:YAF-SqlServer-Deploy',
			'shell:deletePublish', 'shell:deployMySql', 'shell:compileLanguages', 'copy:appSettingsMySql',
			'shell:deleteAppSettings', 'zip:YAF-MySql-Deploy',
			'shell:deletePublish', 'shell:deployPostgreSQL', 'shell:compileLanguages', 'copy:appSettingsPostgreSQL',
			'shell:deleteAppSettings', 'zip:YAF-PostgreSQL-Deploy'
		]);

	grunt.registerTask('publishGitHub',
		[
			'shell:publishToGitHub'
		]);

	grunt.registerTask('publishNuget',
		[
			'shell:deleteOldNuGetPackages', 'shell:createNuGetPackages', 'shell:copyNuGetPackages',
			'shell:publishNuGetPackages'
		]);

	grunt.registerTask('publishNugetUI',
		[
			'shell:deleteOldNuGetPackages', 'shell:createNuGetUIPackages', 'shell:copyNuGetUIPackages',
			'shell:publishNuGetUIPackages'
		]);
};
