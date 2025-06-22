const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');

const pckg = require('./package.json');

module.exports = [
	{
		entry: {
			albums: './wwwroot/lib/pages/albums.ts',
			messages: './wwwroot/lib/pages/messages.ts',
			search: './wwwroot/lib/pages/search.ts',
			persianPicker: './wwwroot/lib/persiandatetimepicker/persianPicker.ts',
			fileUploader: './wwwroot/lib/fileUploader.ts',
			themeSelector: './wwwroot/lib/bootstrap/themeSelector.ts',
			editor: './wwwroot/lib/editor/editor.ts',
			codemirror: './wwwroot/lib/codemirror.ts'
		},
		output: {
			filename: '[name].min.js',
			path: path.resolve(__dirname, 'wwwroot', 'js'),
			clean: false
		},
		externals: {
			bootstrap: 'bootstrap',
			'microsoft/signalr': 'microsoft/signalr'
		},
		devtool: 'source-map',
		mode: 'development',
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
				},
				{
					test: /\.(png|svg|jpg|jpeg|gif|webp)$/i,
					type: 'asset'
				},
				{
					test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
					type: 'asset'
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
		mode: 'development',
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
				},
				{
					test: /\.(png|svg|jpg|jpeg|gif|webp)$/i,
					type: 'asset'
				},
				{
					test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
					type: 'asset'
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
		mode: 'development',
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
					test: /\.ts$/i,
					use: ['ts-loader'],
					exclude: /node_modules/
				},
				{
					test: /\.(png|svg|jpg|jpeg|gif|webp)$/i,
					type: 'asset'
				},
				{
					test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
					type: 'asset'
				}
			]
		}
	}
];
