/**
 * Build process for YetAnotherForum.NET
 *
 * Don't know where to start?
 * Try: http://24ways.org/2013/grunt-is-not-weird-and-hard/
 */

var webpackConfig = require('./webpack.config.js');
var sass = require('sass');

module.exports = function(grunt) {
	require('@lodder/time-grunt')(grunt);
	// CONFIGURATION
	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),

		webpack: {
			main: webpackConfig
		},

		copy: {
			bootstrap: {
				files: [
					{
						expand: true,
						src: '**/*.scss',
						cwd: 'node_modules/bootstrap/scss',
						dest: 'wwwroot/lib/bootstrap/'
					}
				]
			},
			bootswatchThemes: {
				files: [
					// includes files within path
					{ expand: true, src: '**/*.scss', cwd: 'node_modules/bootswatch/dist', dest: 'wwwroot/lib/themes/' }
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
						cwd: 'node_modules/@yafnet/sceditor/dist',
						dest: 'wwwroot/css/'
					}, {
						expand: true,
						src: '**/*.js',
						cwd: 'node_modules/@yafnet/sceditor/dist',
						dest: 'wwwroot/js/sceditor/'
					}
				]
			}
		},

		replace: {
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
			},

			version: {
				options: {
					patterns: [
						{
							match: /Version>([\d.]+)<\//,
							replacement: 'Version><%= pkg.version%></'
						},
						{
							match: /AssemblyVersion>([\d.]+)<\//,
							replacement: 'AssemblyVersion><%= pkg.version%></'
						},
						{
							match: /FileVersion>([\d.]+)<\//,
							replacement: 'FileVersion><%= pkg.version%></'
						}
					]
				},
				files: [
					{
						expand: true,
						flatten: true,
						src: ['../Directory.Build.props'],
						dest: '../'
					}, {
						expand: true,
						flatten: true,
						src: ['../ServiceStack/Directory.Build.props'],
						dest: '../ServiceStack/'
					}, {
						expand: true,
						flatten: true,
						src: ['../Tests/Directory.Build.props'],
						dest: '../Tests/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.UI/YAFNET.RazorPages/YAFNET.RazorPages.csproj'],
						dest: '../YAFNET.UI/YAFNET.RazorPages/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.UI/YAFNET.UI.Chat/YAFNET.UI.Chat.csproj'],
						dest: '../YAFNET.UI/YAFNET.UI.Chat/'
					}
				]
			},
			versionNuget: {
				options: {
					patterns: [
						{
							match: /version>([\d.]+)<\//,
							replacement: 'version><%= pkg.version%></'
						}
					]
				},
				files: [
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Configuration/YAFNET.Configuration.nuspec'],
						dest: '../YAFNET.Configuration/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Core/YAFNET.Core.nuspec'],
						dest: '../YAFNET.Core/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.MySql/YAFNET.Data.MySql.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.MySql/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.PostgreSQL/YAFNET.Data.PostgreSQL.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.PostgreSQL/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.Sqlite/YAFNET.Data.Sqlite.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.Sqlite/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.SqlServer/YAFNET.Data.SqlServer.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.SqlServer/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Types/YAFNET.Types.nuspec'],
						dest: '../YAFNET.Types/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Web/YAFNET.Web.nuspec'],
						dest: '../YAFNET.Web/'
					}
				]
			},
			versionNugetPackages: {
				options: {
					patterns: [
						{
							match: /version="([\d.]+)" exclude/,
							replacement: 'version="<%= pkg.version%>" exclude'
						}
					]
				},
				files: [
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Configuration/YAFNET.Configuration.nuspec'],
						dest: '../YAFNET.Configuration/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Core/YAFNET.Core.nuspec'],
						dest: '../YAFNET.Core/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.MySql/YAFNET.Data.MySql.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.MySql/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.PostgreSQL/YAFNET.Data.PostgreSQL.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.PostgreSQL/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.Sqlite/YAFNET.Data.Sqlite.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.Sqlite/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.SqlServer/YAFNET.Data.SqlServer.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.SqlServer/'
					}
				]
			},
			versionNugetDependencies: {
				options: {
					patterns: [
						{
							match: /id="YAFNET.Configuration" version="([\d.]+)"/,
							replacement: 'id="YAFNET.Configuration" version="<%= pkg.version%>"'
						},
						{
							match: /id="YAFNET.Core" version="([\d.]+)"/,
							replacement: 'id="YAFNET.Core" version="<%= pkg.version%>"'
						},
						{
							match: /id="YAFNET.Types" version="([\d.]+)"/,
							replacement: 'id="YAFNET.Types" version="<%= pkg.version%>"'
						}
					]
				},
				files: [
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Configuration/YAFNET.Configuration.nuspec'],
						dest: '../YAFNET.Configuration/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Core/YAFNET.Core.nuspec'],
						dest: '../YAFNET.Core/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.MySql/YAFNET.Data.MySql.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.MySql/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.PostgreSQL/YAFNET.Data.PostgreSQL.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.PostgreSQL/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.Sqlite/YAFNET.Data.Sqlite.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.Sqlite/'
					},
					{
						expand: true,
						flatten: true,
						src: ['../YAFNET.Data/YAFNET.Data.SqlServer/YAFNET.Data.SqlServer.nuspec'],
						dest: '../YAFNET.Data/YAFNET.Data.SqlServer/'
					}
				]
			}
		},

		shell: {
			dockerBuildSqlServer: {
				command: [
					'@echo off',
					'cd ..\\ ',
					'docker build -t yafnet/yafnet:latest-sqlserver -f docker/yaf-sqlserver/Dockerfile . ',
					'docker push yafnet/yafnet:latest-sqlserver '
				].join('&&')
			},
			dockerBuildMySql: {
				command: [
					'@echo off',
					'cd ..\\ ',
					'docker build -t yafnet/yafnet:latest-mysql -f docker/yaf-mysql/Dockerfile . ',
					'docker push yafnet/yafnet:latest-mysql '
				].join('&&')
			},
			dockerBuildPostgreSQL: {
				command: [
					'@echo off',
					'cd ..\\ ',
					'docker build -t yafnet/yafnet:latest-postgresql -f docker/yaf-postgresql/Dockerfile . ',
					'docker push yafnet/yafnet:latest-postgresql '
				].join('&&')
			},
			dockerBuildSqlite: {
				command: [
					'@echo off',
					'cd ..\\ ',
					'docker build -t yafnet/yafnet:latest-sqlite -f docker/yaf-sqlite/Dockerfile . ',
					'docker push yafnet/yafnet:latest-sqlite '
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
			}
		},

		sass: {
			options: {
				implementation: sass,
				sourceMap: false,
				api: 'modern',
				silenceDeprecations: ['color-functions', 'global-builtin', 'import', 'legacy-js-api', 'if-function']
			},
			flagIcons: {
				files: {
					"wwwroot/css/flag-icons.min.css": 'wwwroot/lib/flag-icons/flag-icons.scss'
				}
			},
			installWizard: {
				files: {
					"wwwroot/css/InstallWizard.min.css": 'wwwroot/lib/InstallWizard.scss'
				}
			},
			forum: {
				files: {
					"wwwroot/css/forum.min.css": 'wwwroot/lib/forum.scss'
				}
			},
			forumAdmin: {
				files: {
					"wwwroot/css/forum-admin.min.css": 'wwwroot/lib/forum-admin.scss'
				}
			},
			bootstrap: {
				files: {
					"wwwroot/lib/bootstrap/bootstrap.css": 'wwwroot/lib/bootstrap/bootstrap.scss'
				}
			},
			themes: {
				files: {
					"wwwroot/css/themes/zephyr/bootstrap-forum.min.css":
						'wwwroot/lib/themes/zephyr/bootstrap-forum.scss',
					"wwwroot/css/themes/yaf/bootstrap-forum.min.css": 'wwwroot/lib/themes/yaf/bootstrap-forum.scss',
					"wwwroot/css/themes/yeti/bootstrap-forum.min.css": 'wwwroot/lib/themes/yeti/bootstrap-forum.scss',
					"wwwroot/css/themes/vapor/bootstrap-forum.min.css": 'wwwroot/lib/themes/vapor/bootstrap-forum.scss',
					"wwwroot/css/themes/united/bootstrap-forum.min.css":
						'wwwroot/lib/themes/united/bootstrap-forum.scss',
					"wwwroot/css/themes/superhero/bootstrap-forum.min.css":
						'wwwroot/lib/themes/superhero/bootstrap-forum.scss',
					"wwwroot/css/themes/spacelab/bootstrap-forum.min.css":
						'wwwroot/lib/themes/spacelab/bootstrap-forum.scss',
					"wwwroot/css/themes/solar/bootstrap-forum.min.css": 'wwwroot/lib/themes/solar/bootstrap-forum.scss',
					"wwwroot/css/themes/slate/bootstrap-forum.min.css": 'wwwroot/lib/themes/slate/bootstrap-forum.scss',
					"wwwroot/css/themes/sketchy/bootstrap-forum.min.css":
						'wwwroot/lib/themes/sketchy/bootstrap-forum.scss',
					"wwwroot/css/themes/simplex/bootstrap-forum.min.css":
						'wwwroot/lib/themes/simplex/bootstrap-forum.scss',
					"wwwroot/css/themes/sandstone/bootstrap-forum.min.css":
						'wwwroot/lib/themes/sandstone/bootstrap-forum.scss',
					"wwwroot/css/themes/quartz/bootstrap-forum.min.css":
						'wwwroot/lib/themes/quartz/bootstrap-forum.scss',
					"wwwroot/css/themes/pulse/bootstrap-forum.min.css": 'wwwroot/lib/themes/pulse/bootstrap-forum.scss',
					"wwwroot/css/themes/morph/bootstrap-forum.min.css": 'wwwroot/lib/themes/morph/bootstrap-forum.scss',
					"wwwroot/css/themes/minty/bootstrap-forum.min.css": 'wwwroot/lib/themes/minty/bootstrap-forum.scss',
					"wwwroot/css/themes/materia/bootstrap-forum.min.css":
						'wwwroot/lib/themes/materia/bootstrap-forum.scss',
					"wwwroot/css/themes/lux/bootstrap-forum.min.css": 'wwwroot/lib/themes/lux/bootstrap-forum.scss',
					"wwwroot/css/themes/lumen/bootstrap-forum.min.css": 'wwwroot/lib/themes/lumen/bootstrap-forum.scss',
					"wwwroot/css/themes/litera/bootstrap-forum.min.css":
						'wwwroot/lib/themes/litera/bootstrap-forum.scss',
					"wwwroot/css/themes/journal/bootstrap-forum.min.css":
						'wwwroot/lib/themes/journal/bootstrap-forum.scss',
					"wwwroot/css/themes/flatly/bootstrap-forum.min.css":
						'wwwroot/lib/themes/flatly/bootstrap-forum.scss',
					"wwwroot/css/themes/darkly/bootstrap-forum.min.css":
						'wwwroot/lib/themes/darkly/bootstrap-forum.scss',
					"wwwroot/css/themes/cyborg/bootstrap-forum.min.css":
						'wwwroot/lib/themes/cyborg/bootstrap-forum.scss',
					"wwwroot/css/themes/cosmo/bootstrap-forum.min.css": 'wwwroot/lib/themes/cosmo/bootstrap-forum.scss',
					"wwwroot/css/themes/cerulean/bootstrap-forum.min.css":
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
			flagIcons: {
				src: 'wwwroot/css/flag-icons.min.css'
			},
			installWizard: {
				src: 'wwwroot/css/InstallWizard.min.css'
			},
			forum: {
				src: 'wwwroot/css/forum.min.css'
			},
			forumAdmin: {
				src: 'wwwroot/css/forum-admin.min.css'
			},
			themes: {
				src: 'wwwroot/css/themes/**/*.css'
			}
		},

		// CSS Minify
		cssmin: {
			other: {
				files: {
					"wwwroot/css/InstallWizard.min.css": 'wwwroot/css/InstallWizard.min.css',
					"wwwroot/css/flag-icons.min.css": 'wwwroot/css/flag-icons.min.css',
					"wwwroot/css/forum.min.css": 'wwwroot/css/forum.min.css',
					"wwwroot/css/forum-admin.min.css": 'wwwroot/css/forum-admin.min.css'
				}
			},
			themes: {
				files: {
					"wwwroot/css/themes/zephyr/bootstrap-forum.min.css":
						'wwwroot/css/themes/zephyr/bootstrap-forum.min.css',
					"wwwroot/css/themes/yaf/bootstrap-forum.min.css": 'wwwroot/css/themes/yaf/bootstrap-forum.min.css',
					"wwwroot/css/themes/yeti/bootstrap-forum.min.css":
						'wwwroot/css/themes/yeti/bootstrap-forum.min.css',
					"wwwroot/css/themes/vapor/bootstrap-forum.min.css":
						'wwwroot/css/themes/vapor/bootstrap-forum.min.css',
					"wwwroot/css/themes/united/bootstrap-forum.min.css":
						'wwwroot/css/themes/united/bootstrap-forum.min.css',
					"wwwroot/css/themes/superhero/bootstrap-forum.min.css":
						'wwwroot/css/themes/superhero/bootstrap-forum.min.css',
					"wwwroot/css/themes/spacelab/bootstrap-forum.min.css":
						'wwwroot/css/themes/spacelab/bootstrap-forum.min.css',
					"wwwroot/css/themes/solar/bootstrap-forum.min.css":
						'wwwroot/css/themes/solar/bootstrap-forum.min.css',
					"wwwroot/css/themes/slate/bootstrap-forum.min.css":
						'wwwroot/css/themes/slate/bootstrap-forum.min.css',
					"wwwroot/css/themes/sketchy/bootstrap-forum.min.css":
						'wwwroot/css/themes/sketchy/bootstrap-forum.min.css',
					"wwwroot/css/themes/simplex/bootstrap-forum.min.css":
						'wwwroot/css/themes/simplex/bootstrap-forum.min.css',
					"wwwroot/css/themes/sandstone/bootstrap-forum.min.css":
						'wwwroot/css/themes/sandstone/bootstrap-forum.min.css',
					"wwwroot/css/themes/quartz/bootstrap-forum.min.css":
						'wwwroot/css/themes/quartz/bootstrap-forum.min.css',
					"wwwroot/css/themes/pulse/bootstrap-forum.min.css":
						'wwwroot/css/themes/pulse/bootstrap-forum.min.css',
					"wwwroot/css/themes/morph/bootstrap-forum.min.css":
						'wwwroot/css/themes/morph/bootstrap-forum.min.css',
					"wwwroot/css/themes/minty/bootstrap-forum.min.css":
						'wwwroot/css/themes/minty/bootstrap-forum.min.css',
					"wwwroot/css/themes/materia/bootstrap-forum.min.css":
						'wwwroot/css/themes/materia/bootstrap-forum.min.css',
					"wwwroot/css/themes/lux/bootstrap-forum.min.css": 'wwwroot/css/themes/lux/bootstrap-forum.min.css',
					"wwwroot/css/themes/lumen/bootstrap-forum.min.css":
						'wwwroot/css/themes/lumen/bootstrap-forum.min.css',
					"wwwroot/css/themes/litera/bootstrap-forum.min.css":
						'wwwroot/css/themes/litera/bootstrap-forum.min.css',
					"wwwroot/css/themes/journal/bootstrap-forum.min.css":
						'wwwroot/css/themes/journal/bootstrap-forum.min.css',
					"wwwroot/css/themes/flatly/bootstrap-forum.min.css":
						'wwwroot/css/themes/flatly/bootstrap-forum.min.css',
					"wwwroot/css/themes/darkly/bootstrap-forum.min.css":
						'wwwroot/css/themes/darkly/bootstrap-forum.min.css',
					"wwwroot/css/themes/cyborg/bootstrap-forum.min.css":
						'wwwroot/css/themes/cyborg/bootstrap-forum.min.css',
					"wwwroot/css/themes/cosmo/bootstrap-forum.min.css":
						'wwwroot/css/themes/cosmo/bootstrap-forum.min.css',
					"wwwroot/css/themes/cerulean/bootstrap-forum.min.css":
						'wwwroot/css/themes/cerulean/bootstrap-forum.min.css'
				}
			}
		}
	});

	// PLUGINS
	grunt.loadNpmTasks('@lodder/grunt-postcss');
	grunt.loadNpmTasks('grunt-contrib-copy');
	grunt.loadNpmTasks('grunt-contrib-cssmin');
	grunt.loadNpmTasks('grunt-sass');
	grunt.loadNpmTasks('grunt-shell');
	grunt.loadNpmTasks('grunt-replace');
	grunt.loadNpmTasks('grunt-zip');
	grunt.loadNpmTasks('grunt-webpack');

	grunt.registerTask('default',
		[
			'webpack', 'sass', 'postcss', 'cssmin'
		]);

	grunt.registerTask('updateBootstrap',
		[
			'copy:bootstrap'
		]);

	grunt.registerTask('updateBootswatchThemes',
		[
			'copy:bootswatchThemes'
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

	grunt.registerTask('css',
		[
			'sass', 'postcss', 'cssmin'
		]);

	grunt.registerTask('publishDocker',
		[
			'shell:dockerBuildSqlServer', 'shell:dockerBuildMySql', 'shell:dockerBuildPostgreSQL',
			'shell:dockerBuildSqlite'
		]);

	grunt.registerTask('updateVersionNumber',
		[
			'replace:version', 'replace:versionNuget', 'replace:versionNugetPackages',
			'replace:versionNugetDependencies'
		]);
};
