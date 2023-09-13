const path = require("path");
const webpack = require("webpack");
const TerserPlugin = require("terser-webpack-plugin");

module.exports = {
	entry: "./Scripts/bs5-lightbox/index.js",
	devtool: "source-map",
	plugins: [
		new webpack.BannerPlugin({
			raw: true,
			banner: `
/*!
 * Lightbox for Bootstrap 5 v${process.env.npm_package_version} (https://trvswgnr.github.io/bs5-lightbox/)
 * Copyright ${new Date().getFullYear()} Travis Aaron Wagner (https://github.com/trvswgnr/)
 * Licensed under MIT (https://github.com/trvswgnr/bs5-lightbox/blob/main/LICENSE)
 */`
		})
	],
	output: {
		path: path.resolve(__dirname, "./dist")
	},
	externals: {
		bootstrap: "bootstrap"
	},
	optimization: {
		minimizer: [new TerserPlugin({ extractComments: false })]
	}
};
