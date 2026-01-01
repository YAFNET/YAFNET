const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');

const pckg = require('./package.json');


module.exports = [
	{
		entry: {
			chat: './wwwroot/lib/chat.ts'
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
	}
];
