/**
 * Build process for YetAnotherForum.NET
 *
 * Usage: node build.js [task], or npm run [task] (see the scripts in package.json)
 * Without a task the js and css files are built.
 */

const { execFileSync, execSync } = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');

const autoprefixer = require('autoprefixer');
const CleanCSS = require('clean-css');
const postcss = require('postcss');
const sass = require('sass');

const generateFontAwesomeIcons = require('./fontawesome-icons.js');
const pkg = require('./package.json');

process.chdir(__dirname);

const themesFolder = 'wwwroot/lib/themes';

const themes = fs.readdirSync(themesFolder, { withFileTypes: true })
	.filter(entry => entry.isDirectory())
	.map(entry => entry.name);

/**
 * The css files as [source, destination].
 */
const stylesheets = [
	['wwwroot/lib/flag-icons/flag-icons.scss', 'wwwroot/css/flag-icons.min.css'],
	['wwwroot/lib/InstallWizard.scss', 'wwwroot/css/InstallWizard.min.css'],
	['wwwroot/lib/forum.scss', 'wwwroot/css/forum.min.css'],
	['wwwroot/lib/forum-admin.scss', 'wwwroot/css/forum-admin.min.css'],
	...themes.map(theme => [`${themesFolder}/${theme}/bootstrap-forum.scss`, `wwwroot/css/themes/${theme}/bootstrap-forum.min.css`])
];

const sassOptions = {
	silenceDeprecations: ['color-functions', 'global-builtin', 'import', 'legacy-js-api', 'if-function', 'color-adjust']
};

/**
 * The nuspec files of the packages which depend on other YAF.NET packages.
 */
const nuspecsWithDependencies = [
	'../YAFNET.Configuration/YAFNET.Configuration.nuspec',
	'../YAFNET.Core/YAFNET.Core.nuspec',
	'../YAFNET.Data/YAFNET.Data.MySql/YAFNET.Data.MySql.nuspec',
	'../YAFNET.Data/YAFNET.Data.PostgreSQL/YAFNET.Data.PostgreSQL.nuspec',
	'../YAFNET.Data/YAFNET.Data.Sqlite/YAFNET.Data.Sqlite.nuspec',
	'../YAFNET.Data/YAFNET.Data.SqlServer/YAFNET.Data.SqlServer.nuspec'
];

/**
 * The patterns have no g flag on purpose: only the first match of each pattern is a YAF.NET version,
 * e.g. the first dependency with exclude in a nuspec is a YAF.NET package, the others are third party packages.
 */
const versionReplacements = [
	{
		files: [
			'../Directory.Build.props',
			'../ServiceStack/Directory.Build.props',
			'../Tests/Directory.Build.props',
			'../YAFNET.UI/YAFNET.RazorPages/YAFNET.RazorPages.csproj',
			'../YAFNET.UI/YAFNET.UI.Chat/YAFNET.UI.Chat.csproj'
		],
		patterns: [
			[/Version>([\d.]+)<\//, `Version>${pkg.version}</`],
			[/AssemblyVersion>([\d.]+)<\//, `AssemblyVersion>${pkg.version}</`],
			[/FileVersion>([\d.]+)<\//, `FileVersion>${pkg.version}</`]
		]
	},
	{
		files: [
			...nuspecsWithDependencies,
			'../YAFNET.Types/YAFNET.Types.nuspec',
			'../YAFNET.Web/YAFNET.Web.nuspec'
		],
		patterns: [
			[/version>([\d.]+)<\//, `version>${pkg.version}</`]
		]
	},
	{
		files: nuspecsWithDependencies,
		patterns: [
			[/version="([\d.]+)" exclude/, `version="${pkg.version}" exclude`],
			[/id="YAFNET.Configuration" version="([\d.]+)"/, `id="YAFNET.Configuration" version="${pkg.version}"`],
			[/id="YAFNET.Core" version="([\d.]+)"/, `id="YAFNET.Core" version="${pkg.version}"`],
			[/id="YAFNET.Types" version="([\d.]+)"/, `id="YAFNET.Types" version="${pkg.version}"`]
		]
	}
];

/**
 * Copies the files of a folder (and its sub folders) which match the filter.
 */
function copyFiles(from, to, filter) {
	let count = 0;

	for (const entry of fs.readdirSync(from, { withFileTypes: true, recursive: true })) {
		if (entry.isFile() && filter(entry.name)) {
			const source = path.join(entry.parentPath, entry.name);
			const destination = path.join(to, path.relative(from, source));

			fs.mkdirSync(path.dirname(destination), { recursive: true });
			fs.copyFileSync(source, destination);
			count++;
		}
	}

	console.log(`${from} -> ${to} (${count} files)`);
}

const hasExtension = (...extensions) => name => extensions.includes(path.extname(name));

/**
 * Compiles a scss file, adds the vendor prefixes and minifies the result.
 */
async function buildStylesheet([source, destination], prefixer) {
	const compiled = await sass.compileAsync(source, sassOptions);
	const prefixed = await prefixer.process(compiled.css, { from: destination, to: destination, map: false });

	for (const warning of prefixed.warnings()) {
		console.warn(`${destination}: ${warning}`);
	}

	fs.mkdirSync(path.dirname(destination), { recursive: true });
	fs.writeFileSync(destination, prefixed.css);

	const minified = new CleanCSS().minify([destination]);

	if (minified.errors.length > 0) {
		throw new Error(`${destination}: ${minified.errors.join(', ')}`);
	}

	// the themes import their web fonts from Google Fonts, these imports are kept on purpose
	for (const warning of minified.warnings.filter(w => !w.startsWith('Skipping remote @import'))) {
		console.warn(`${destination}: ${warning}`);
	}

	fs.writeFileSync(destination, minified.styles);
}

function runWebpack() {
	execFileSync(process.execPath, [require.resolve('webpack-cli/bin/cli.js')], { stdio: 'inherit' });
}

// writes the Font Awesome icons used by the forum to wwwroot/lib/_fontawesome-icons.scss
// (after webpack, the built scripts are scanned too)
function findFontAwesomeIcons() {
	generateFontAwesomeIcons(console.log);
}

async function buildStylesheets() {
	const prefixer = postcss([autoprefixer({ overrideBrowserslist: 'defaults' })]);

	await Promise.all(stylesheets.map(stylesheet => buildStylesheet(stylesheet, prefixer)));

	console.log(`${stylesheets.length} css files created`);
}

function buildEmailTemplates(template, config) {
	for (const theme of themes) {
		console.log(`Build ${theme} theme ${template}`);

		execSync(
			`bootstrap-email -f wwwroot/resources/${template} -d wwwroot/css/themes/${theme}/${template} -c ${themesFolder}/${theme}/${config}`,
			{ stdio: 'inherit' });
	}

	fs.rmSync('.sass-cache', { recursive: true, force: true });
}

const tasks = {
	async default() {
		runWebpack();
		findFontAwesomeIcons();
		await buildStylesheets();
	},

	async css() {
		findFontAwesomeIcons();
		await buildStylesheets();
	},

	webpack: runWebpack,

	fontAwesomeIcons: findFontAwesomeIcons,

	updateFontAwesome() {
		copyFiles('node_modules/@fortawesome/fontawesome-free/webfonts', 'wwwroot/webfonts', name => name.includes('.'));
	},

	updateBootswatchThemes() {
		copyFiles('node_modules/bootswatch/dist', themesFolder, hasExtension('.scss'));
	},

	updateFlagIcons() {
		copyFiles('node_modules/flag-icons/sass', 'wwwroot/lib/flag-icons', hasExtension('.scss'));
		copyFiles('node_modules/flag-icons/flags', 'wwwroot/css/flags', hasExtension('.svg'));

		const variables = 'wwwroot/lib/flag-icons/_variables.scss';

		fs.writeFileSync(variables, fs.readFileSync(variables, 'utf8').replaceAll('../flags', 'flags'));
	},

	updateSCEditor() {
		const dist = 'node_modules/@yafnet/sceditor/dist';

		copyFiles(dist, 'wwwroot/css', hasExtension('.css'));
		copyFiles(`${dist}/languages`, 'wwwroot/js/sceditor/languages', hasExtension('.js'));
		copyFiles(dist, 'wwwroot/js/sceditor', name => name === 'sceditor.min.js');
	},

	emailTemplates() {
		buildEmailTemplates('EmailTemplate.html', 'bootstrap_email.json');
		buildEmailTemplates('DigestTopicTemplate.html', 'bootstrap_email-digest.json');
	},

	publishDocker() {
		for (const database of ['sqlserver', 'mysql', 'postgresql', 'sqlite']) {
			const image = `yafnet/yafnet:latest-${database}`;

			execSync(`docker build -t ${image} -f docker/yaf-${database}/Dockerfile .`, { cwd: '..', stdio: 'inherit' });
			execSync(`docker push ${image}`, { cwd: '..', stdio: 'inherit' });
		}
	},

	updateVersionNumber() {
		for (const { files, patterns } of versionReplacements) {
			for (const file of files) {
				const content = fs.readFileSync(file, 'utf8');

				fs.writeFileSync(file, patterns.reduce((text, [match, replacement]) => text.replace(match, replacement), content));
			}
		}

		console.log(`version set to ${pkg.version}`);
	}
};

async function main() {
	const name = process.argv[2] ?? 'default';

	if (!Object.hasOwn(tasks, name)) {
		throw new Error(`Unknown task "${name}", available tasks: ${Object.keys(tasks).join(', ')}`);
	}

	const start = performance.now();

	await tasks[name]();

	console.log(`${name} finished in ${((performance.now() - start) / 1000).toFixed(1)}s`);
}

main().catch(error => {
	console.error(error.formatted ?? error.message ?? error);
	process.exitCode = 1;
});
