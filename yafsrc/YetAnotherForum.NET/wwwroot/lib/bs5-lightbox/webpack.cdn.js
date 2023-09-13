const { merge } = require("webpack-merge");
const common = require("./webpack.common.js");

module.exports = merge(common, {
	target: ["web", "es5"],
	module: {
		rules: [
			{
				test: /\.js$/,
				use: "babel-loader",
				exclude: /node_modules/
			}
		]
	},
	output: {
		filename: "index.bundle.min.js",
		library: {
			name: "Lightbox",
			type: "window",
			export: "default"
		}
	}
});
