const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');

const pckg = require('./package.json');

module.exports = [
	{
		entry: {
			codemirror: './scripts/codemirror.ts'
		},
		output: {
			filename: '[name].min.js',
			path: path.resolve(__dirname, 'scripts'),
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
					// TypeScript 7 (native) has no JS compiler API, ts-loader needs TypeScript 6
					use: [{ loader: 'ts-loader', options: { compiler: 'typescript6' } }],
					exclude: /node_modules/
				}
			]
		}
	}
];
