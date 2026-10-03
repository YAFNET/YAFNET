// Copies the shared forum files from the YetAnotherForum.NET project into this project.
// Replaces the former "copy-files-from-to" package. Usage: npm run copy
const fs = require('node:fs');
const path = require('node:path');

const source = path.resolve(__dirname, '../../YetAnotherForum.NET');

const copyFiles = [
	{ from: 'Pages', to: 'Areas/Forums/Pages' },
	{ from: 'ViewComponents', to: 'ViewComponents' },
	{ from: 'wwwroot/css', to: 'wwwroot/css' },
	{ from: 'wwwroot/assets', to: 'wwwroot/assets' },
	{ from: 'wwwroot/images', to: 'wwwroot/Forums/images' },
	{ from: 'wwwroot/js', to: 'wwwroot/js' },
	{ from: 'wwwroot/languages', to: 'wwwroot/languages' },
	{ from: 'wwwroot/resources', to: 'wwwroot/resources' },
	{ from: 'wwwroot/webfonts', to: 'wwwroot/webfonts' }
];

let failed = false;

for (const { from, to } of copyFiles) {
	const src = path.join(source, from);
	const dest = path.resolve(__dirname, to);

	if (!fs.existsSync(src)) {
		console.error(`Source not found: ${src}`);
		failed = true;
		continue;
	}

	let count = 0;

	fs.cpSync(src, dest, {
		recursive: true,
		force: true,
		// Same as the old "**/*.*" glob: only files with an extension
		filter: (file) => {
			if (fs.statSync(file).isDirectory()) {
				return true;
			}

			if (!path.basename(file).includes('.')) {
				return false;
			}

			count++;
			return true;
		}
	});

	console.log(`${from} -> ${to} (${count} files)`);
}

if (failed) {
	process.exit(1);
}
