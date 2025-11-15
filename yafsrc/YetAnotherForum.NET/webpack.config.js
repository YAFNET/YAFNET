const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');

const pckg = require('./package.json');


module.exports = [
	{
		entry: {
			albums: './wwwroot/lib/pages/albums.ts',
			choices: './wwwroot/lib/choices/choices.ts',
			codemirror: './wwwroot/lib/codemirror.ts',
			editor: './wwwroot/lib/editor/editor.ts',
			fileUploader: './wwwroot/lib/fileUploader.ts',
			messages: './wwwroot/lib/pages/messages.ts',
			persianPicker: './wwwroot/lib/persiandatetimepicker/persianPicker.ts',
			post: './wwwroot/lib/pages/post.ts',
			search: './wwwroot/lib/pages/search.ts',
			subscriptions: './wwwroot/lib/pages/subscriptions.ts',
			themeSelector: './wwwroot/lib/bootstrap/themeSelector.ts',
			'admin-dashboard': './wwwroot/lib/pages/admin-dashboard.ts',
			"serviceWorker": './wwwroot/lib/serviceWorker.ts'
		},
		output: {
			filename: '[name].min.js',
			path: path.resolve(__dirname, 'wwwroot', 'js'),
			clean: false
		},
		externals: {
			bootstrap: 'bootstrap',
			'microsoft/signalr': 'microsoft/signalr',
			'w8tcha/bootbox': '@w8tcha/bootbox'
		},
		devtool: 'source-map',
		mode: process.env.NODE_ENV === 'production' ? 'production' : 'development',
		resolve: {
			extensions: ['.ts', '.js'],
			extensionAlias: { '.js': ['.js', '.ts'] }
		},
		optimization: {
			minimize: true,
			minimizer: [new TerserPlugin({
				terserOptions: {
					format: {
						preamble: `/*! Yet Another Forum.NET v${pckg.version} | © ${pckg.author} | ${pckg.homepage} */\n`,
						comments: false
					}
				}, extractComments: false })]
		},
		stats: {
			orphanModules: true
		},
		module: {
			rules: [
				{
					test: /\.css$/i,
					use: ['style-loader', 'css-loader']
				},
				{
					test: /\.ts$/i,
					use: ['ts-loader'],
					exclude: /node_modules/
				}
			]
		}
	}, {
		entry: {
			forumExtensions: './wwwroot/lib/forumExtensions.ts'
		},
		output: {
			filename: '[name].min.js',
			path: path.resolve(__dirname, 'wwwroot', 'js'),
			clean: false
		},
		devtool: 'source-map',
		mode: process.env.NODE_ENV === 'production' ? 'production' : 'development',
		resolve: {
			extensions: ['.ts', '.js'],
			extensionAlias: { '.js': ['.js', '.ts'] }
		},
		optimization: {
			minimize: true,
			minimizer: [
				new TerserPlugin({
					terserOptions: {
						format: {
							preamble: `/*! Yet Another Forum.NET v${pckg.version} | © ${pckg.author} | ${pckg.homepage} */\n`,
							comments: false
						}
					},
					extractComments: false
				})
			]
		},
		stats: {
			orphanModules: true
		},
		module: {
			rules: [
				{
					test: /\.css$/i,
					use: ['style-loader', 'css-loader']
				},
				{
					test: /\.ts$/i,
					use: ['ts-loader'],
					exclude: /node_modules/
				}
			]
		}
	}, {
		entry: {
			install: './wwwroot/lib/pages/install.ts'
		},
		output: {
			filename: '[name].min.js',
			path: path.resolve(__dirname, 'wwwroot', 'js'),
			clean: false
		},
		devtool: 'source-map',
		mode: process.env.NODE_ENV === 'production' ? 'production' : 'development',
		resolve: {
			extensions: ['.ts', '.js'],
			extensionAlias: { '.js': ['.js', '.ts'] }
		},
		optimization: {
			minimize: true,
			minimizer: [
				new TerserPlugin({
					terserOptions: {
						format: {
							preamble: `/*! Yet Another Forum.NET v${pckg.version} | © ${pckg.author} | ${pckg.homepage} */\n`,
							comments: false
						}
					},
					extractComments: false
				})
			]
		},
		stats: {
			orphanModules: true
		},
		module: {
			rules: [
				{
					test: /\.css$/i,
					use: ['style-loader', 'css-loader']
				}, {
					test: /\.ts$/i,
					use: ['ts-loader'],
					exclude: /node_modules/
				}
			]
		}
	}
];
