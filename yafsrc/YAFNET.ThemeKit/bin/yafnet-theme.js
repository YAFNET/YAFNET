#!/usr/bin/env node

/**
 * yafnet-theme: creates and builds custom themes for YetAnotherForum.NET.
 *
 * The kit contains the theme sources and the forum styles of the YAF.NET version with the same version number,
 * so a theme can be built without the YAF.NET source code.
 */

const { spawnSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { parseArgs } = require('node:util');

const kit = path.resolve(__dirname, '..');
const kitPackage = require('../package.json');

const kitThemes = path.join(kit, 'themes');
const kitScss = path.join(kit, 'scss');
const kitEmail = path.join(kit, 'email');

const themeConfigFile = 'yafnet-theme.json';

const usage = `yafnet-theme ${kitPackage.version} - custom themes for YetAnotherForum.NET ${kitPackage.version}

Usage:
  yafnet-theme list                          list the themes of the kit, to start from
  yafnet-theme new <name> [--from <theme>]   create the theme <name> in the themes folder (default from: yaf)
  yafnet-theme build [<name>...]             build all themes in the themes folder (or the named ones)
  yafnet-theme watch [<name>...]             build the css again when a file in the themes folder changes

Options:
  --themes <folder>   folder with the theme sources (default: themes)
  --out <folder>      output folder (default: dist), e.g. the wwwroot/css/themes folder of a forum
  --from <theme>      new: the theme of the kit to start from (default: yaf)
  --no-email          build: only the css, not the email templates
  -v, --version       show the version
  -h, --help          show this help

A built theme is a folder <out>/<name> with bootstrap-forum.min.css, EmailTemplate.html and
DigestTopicTemplate.html. Copy it to the wwwroot/css/themes folder of the forum.

The email templates are built with the .NET tool bootstrap-email
(dotnet tool install --global BootstrapEmail.Net.Cli). Without it the prebuilt email templates of the
theme you started from are used.`;

const sassOptions = {
	silenceDeprecations: ['color-functions', 'global-builtin', 'import', 'legacy-js-api', 'if-function'],
	// the themes import "bootstrap/scss/bootstrap" and the forum styles ("forum/chat.scss", ...) from here
	loadPaths: [path.dirname(path.dirname(require.resolve('bootstrap/package.json'))), kitScss]
};

class UserError extends Error {}

const isThemeFolder = folder => fs.existsSync(path.join(folder, 'bootstrap-forum.scss'));

function listFolders(folder) {
	return fs.readdirSync(folder, { withFileTypes: true })
		.filter(entry => entry.isDirectory())
		.map(entry => entry.name)
		.sort();
}

function kitThemeNames() {
	return listFolders(kitThemes);
}

/**
 * The themes to build: the named ones, or every theme folder in the themes folder.
 */
function selectThemes(themesFolder, names) {
	if (!fs.existsSync(themesFolder)) {
		throw new UserError(`The themes folder ${themesFolder} doesn't exist. Create a theme with: yafnet-theme new <name>`);
	}

	if (names.length > 0) {
		for (const name of names) {
			if (!isThemeFolder(path.join(themesFolder, name))) {
				throw new UserError(`${path.join(themesFolder, name)} is not a theme (bootstrap-forum.scss is missing).`);
			}
		}

		return names;
	}

	const themes = listFolders(themesFolder).filter(name => isThemeFolder(path.join(themesFolder, name)));

	if (themes.length === 0) {
		throw new UserError(`No themes found in ${themesFolder}. Create a theme with: yafnet-theme new <name>`);
	}

	return themes;
}

/**
 * The theme of the kit a theme was created from, its prebuilt email templates are used without bootstrap-email.
 */
function baseTheme(themeFolder, name) {
	const configFile = path.join(themeFolder, themeConfigFile);

	if (fs.existsSync(configFile)) {
		const { from } = JSON.parse(fs.readFileSync(configFile, 'utf8'));

		if (from && fs.existsSync(path.join(kitEmail, 'prebuilt', from))) {
			return from;
		}
	}

	return fs.existsSync(path.join(kitEmail, 'prebuilt', name)) ? name : 'yaf';
}

/**
 * The theme name in the paths of the email sass files and the bootstrap-email configs.
 */
function renameEmailPaths(themeFolder, name) {
	const replacements = [
		['bootstrap-email.scss', /(@import\s+['"]\.\.\/themes\/)[^/'"]+\//g],
		['bootstrap_email.json', /("sass_email_location"\s*:\s*"[^"]*themes\/)[^/"]+\//gi],
		['bootstrap_email-digest.json', /("sass_email_location"\s*:\s*"[^"]*themes\/)[^/"]+\//gi]
	];

	for (const [file, pattern] of replacements) {
		const fullName = path.join(themeFolder, file);

		if (fs.existsSync(fullName)) {
			fs.writeFileSync(fullName, fs.readFileSync(fullName, 'utf8').replace(pattern, `$1${name}/`));
		}
	}
}

function newTheme(name, { themes, from }) {
	if (!name) {
		throw new UserError('Missing theme name: yafnet-theme new <name> [--from <theme>]');
	}

	if (!/^[a-z0-9][a-z0-9_-]*$/i.test(name)) {
		throw new UserError(`"${name}" is not a valid theme name, use letters, digits, - and _ only.`);
	}

	if (!kitThemeNames().includes(from)) {
		throw new UserError(`The kit has no theme "${from}". Available: ${kitThemeNames().join(', ')}`);
	}

	const destination = path.join(themes, name);

	if (fs.existsSync(destination)) {
		throw new UserError(`${destination} already exists.`);
	}

	fs.cpSync(path.join(kitThemes, from), destination, { recursive: true });

	renameEmailPaths(destination, name);

	fs.writeFileSync(
		path.join(destination, themeConfigFile),
		`${JSON.stringify({ from, kitVersion: kitPackage.version }, null, '\t')}\n`);

	console.log(`Created ${destination} from the theme ${from}.

Next steps:
  1. Change the variables in ${path.join(destination, '_variables.scss')}
     (Bootstrap variables, e.g. $primary, $font-family-sans-serif, $border-radius)
  2. Build the theme:   npx yafnet-theme build ${name}
  3. Copy the folder dist/${name} to the wwwroot/css/themes folder of your forum`);
}

async function buildCss(themesFolder, name, out) {
	const sass = require('sass');
	const postcss = require('postcss');
	const autoprefixer = require('autoprefixer');
	const CleanCSS = require('clean-css');

	const source = path.join(themesFolder, name, 'bootstrap-forum.scss');
	const destination = path.join(out, name, 'bootstrap-forum.min.css');

	// themes created in the source of YAF.NET 4.0.10 and older import Bootstrap and the forum styles with relative paths
	if (/@import\s+["'](\.\.\/)+(node_modules\/bootstrap|forum\/|choices\/|netStack)/.test(fs.readFileSync(source, 'utf8'))) {
		throw new UserError(`${source} imports Bootstrap or the forum styles with a relative path (../../...).
Change these imports to:
  @import "bootstrap/scss/bootstrap";
  @import "forum/chat.scss";
  @import "choices/bs-theme.scss";
  @import "netStack.scss";`);
	}

	const compiled = await sass.compileAsync(source, sassOptions);
	const prefixed = await postcss([autoprefixer({ overrideBrowserslist: 'defaults' })])
		.process(compiled.css, { from: destination, to: destination, map: false });

	for (const warning of prefixed.warnings()) {
		console.warn(`${name}: ${warning}`);
	}

	fs.mkdirSync(path.dirname(destination), { recursive: true });
	fs.writeFileSync(destination, prefixed.css);

	const minified = new CleanCSS().minify([destination]);

	if (minified.errors.length > 0) {
		throw new Error(`${destination}: ${minified.errors.join(', ')}`);
	}

	// the Bootswatch themes import their web fonts from Google Fonts, these imports are kept on purpose
	for (const warning of minified.warnings.filter(w => !w.startsWith('Skipping remote @import'))) {
		console.warn(`${name}: ${warning}`);
	}

	fs.writeFileSync(destination, minified.styles);

	const license = path.join(themesFolder, name, 'LICENSE.md');

	if (fs.existsSync(license)) {
		fs.copyFileSync(license, path.join(out, name, 'LICENSE.md'));
	}

	console.log(`${destination} (${Math.round(minified.styles.length / 1024)} KB)`);
}

/**
 * The command of the bootstrap-email tool: the global tool, or the local tool of a dotnet tool manifest.
 */
function findBootstrapEmail() {
	for (const [command, args] of [['bootstrap-email', []], ['dotnet', ['bootstrap-email']]]) {
		const result = spawnSync(command, [...args, '--version'], { encoding: 'utf8' });

		if (!result.error && result.status === 0) {
			return { command, args, version: result.stdout.trim() };
		}
	}

	return null;
}

const emailTemplates = [
	['EmailTemplate.html', {}],
	['DigestTopicTemplate.html', { CompileOnlyBody: true }]
];

/**
 * bootstrap-email resolves the theme import (../themes/[name]/variables) from its sass folder, so the
 * theme is compiled in a temporary folder with the same layout as the YAF.NET source.
 */
function buildEmailTemplates(themesFolder, name, out, tool) {
	const work = fs.mkdtempSync(path.join(os.tmpdir(), 'yafnet-theme-'));
	const slash = folder => `${folder.replaceAll('\\', '/')}/`;

	try {
		const sassFolder = path.join(work, 'bootstrap-email');
		const themeFolder = path.join(work, 'themes', name);

		fs.cpSync(path.join(kitEmail, 'bootstrap-email'), sassFolder, { recursive: true });
		fs.cpSync(path.join(themesFolder, name), themeFolder, { recursive: true });
		renameEmailPaths(themeFolder, name);

		for (const [template, options] of emailTemplates) {
			const config = path.join(work, `${template}.json`);
			const destination = path.join(out, name, template);

			fs.writeFileSync(config, JSON.stringify({
				sass_email_location: slash(themeFolder),
				sass_location: slash(sassFolder),
				...options
			}));

			// bootstrap-email writes its .sass-cache into the working folder
			const result = spawnSync(
				tool.command,
				[...tool.args, '-f', path.join(kitEmail, 'resources', template), '-d', destination, '-c', config],
				{ cwd: work, encoding: 'utf8' });

			if (result.error || result.status !== 0 || !fs.existsSync(destination)) {
				const output = `${result.stdout ?? ''}${result.stderr ?? ''}`.split('\n').slice(0, 5).join('\n');
				const hint = tool.version.startsWith('1.')
					? `\nbootstrap-email ${tool.version} can't be used, update it: dotnet tool update --global BootstrapEmail.Net.Cli`
					: '';

				throw new UserError(`bootstrap-email failed for ${name}/${template}:\n${result.error?.message ?? output}${hint}`);
			}

			console.log(destination);
		}
	} finally {
		fs.rmSync(work, { recursive: true, force: true });
	}
}

function copyPrebuiltEmailTemplates(themesFolder, name, out) {
	const from = baseTheme(path.join(themesFolder, name), name);

	for (const [template] of emailTemplates) {
		fs.copyFileSync(path.join(kitEmail, 'prebuilt', from, template), path.join(out, name, template));
	}

	console.log(`${path.join(out, name)}: email templates of the theme ${from} copied`);
}

async function build(names, { themes, out, 'no-email': noEmail }) {
	const selected = selectThemes(themes, names);

	for (const name of selected) {
		await buildCss(themes, name, out);
	}

	if (noEmail) {
		return;
	}

	const tool = findBootstrapEmail();

	if (!tool) {
		console.warn(`
bootstrap-email was not found, the prebuilt email templates are used instead: they have the colors of the
theme you started from. To build the email templates with the colors of your theme, install the .NET SDK and
  dotnet tool install --global BootstrapEmail.Net.Cli
`);
	}

	for (const name of selected) {
		if (tool) {
			buildEmailTemplates(themes, name, out, tool);
		} else {
			copyPrebuiltEmailTemplates(themes, name, out);
		}
	}
}

async function watch(names, options) {
	const selected = selectThemes(options.themes, names);
	const pending = new Set();
	let timer;

	async function rebuild() {
		const themes = [...pending];
		pending.clear();

		for (const name of themes) {
			try {
				await buildCss(options.themes, name, options.out);
			} catch (error) {
				console.error(error.formatted ?? error.message ?? error);
			}
		}
	}

	await build(selected, { ...options, 'no-email': true });

	fs.watch(options.themes, { recursive: true }, (_event, file) => {
		const name = file?.split(/[\\/]/)[0];

		if (name && selected.includes(name)) {
			pending.add(name);
			clearTimeout(timer);
			timer = setTimeout(rebuild, 200);
		}
	});

	console.log(`Watching ${options.themes} for changes of ${selected.join(', ')} (Ctrl+C to stop)`);
}

async function main() {
	const { values, positionals } = parseArgs({
		allowPositionals: true,
		options: {
			themes: { type: 'string', default: 'themes' },
			out: { type: 'string', default: 'dist' },
			from: { type: 'string', default: 'yaf' },
			'no-email': { type: 'boolean', default: false },
			version: { type: 'boolean', short: 'v' },
			help: { type: 'boolean', short: 'h' }
		}
	});

	const [command, ...args] = positionals;

	if (values.version) {
		console.log(kitPackage.version);
		return;
	}

	if (values.help || !command) {
		console.log(usage);
		return;
	}

	const options = { ...values, themes: path.resolve(values.themes), out: path.resolve(values.out) };

	switch (command) {
		case 'list':
			console.log(kitThemeNames().join('\n'));
			break;
		case 'new':
			newTheme(args[0], options);
			break;
		case 'build':
			await build(args, options);
			break;
		case 'watch':
			await watch(args, options);
			break;
		default:
			throw new UserError(`Unknown command "${command}".\n\n${usage}`);
	}
}

main().catch(error => {
	console.error(error instanceof UserError ? error.message : (error.formatted ?? error.message ?? error));
	process.exitCode = 1;
});
