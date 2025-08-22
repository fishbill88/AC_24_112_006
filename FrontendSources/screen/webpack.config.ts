// tslint:disable: object-literal-sort-keys
import * as path from 'path';
import * as webpack from 'webpack';
import * as webpackDevServer from 'webpack-dev-server';
import * as HtmlWebpackPlugin from 'html-webpack-plugin';
import * as CopyWebpackPlugin from 'copy-webpack-plugin';
import * as MiniCssExtractPlugin from 'mini-css-extract-plugin';
import * as TerserPlugin from 'terser-webpack-plugin';
import * as nodeExternals from 'webpack-node-externals';
import * as fs from 'fs';
import { AureliaPlugin, ModuleDependenciesPlugin } from 'aurelia-webpack-plugin';
import { BundleAnalyzerPlugin } from 'webpack-bundle-analyzer';
import { CleanWebpackPlugin } from 'clean-webpack-plugin';
import {
	IBuildOptions,
	IEnvironment,
	FilesWatcherPlugin,
	getScreensWebpackInfo,
	IScreensWebpackInfo,
	RunLinterPlugin,
	ScreenConfigPlugin,
	AureliaCacheWorkaroundPlugin,
	ScreenInfoDeployPlugin
} from 'build-tools';
import {
	fileName as buildToolsLog,
	logger
} from 'build-tools/logger';
import {
	fileName as transformersToolsLog,
} from 'transformers/logger';
import * as currentPackage from './package.json';
import * as tsconfig from './tsconfig.json';

// tslint:disable-next-line: variable-name
const DuplicatePackageCheckerPlugin = require('duplicate-package-checker-webpack-plugin');
// tslint:disable-next-line: variable-name
const CircularDependencyPlugin = require('circular-dependency-plugin');

let outDir = path.resolve(__dirname, "../../Scripts/Screens");
const port = 8080;
const host = '127.0.0.1';
const open = false;

const srcDir = path.resolve(__dirname, 'src');
const scriptDir = '';
const controlsDir = "node_modules/client-controls";

let screenInfoTargetPath = path.resolve(__dirname, '../../App_Data/TSScreenInfo');
const lockFileName = 'lock.json';
const screenSrcFolder = path.resolve(srcDir, 'screens');
const getTenantSrcFolder = (env: IEnvironment) => `Screens_Customization/${env.tenant}/Screens`;
const screenNameLength = 8;

const getBaseConfig = (env: IEnvironment & IBuildOptions, screenInfos: IScreensWebpackInfo): webpack.Configuration => ({
	resolve: {
		extensions: ['.ts', '.js'],
		modules: [srcDir, path.resolve(__dirname, 'node_modules'), 'node_modules'],
		alias: {
			'client-controls': path.resolve(__dirname, controlsDir)
		}
	},
	entry: screenInfos.entry,
	mode: env.production ? 'production' : 'development',
	output: {
		path: outDir,
		filename: `${scriptDir}[name].[chunkhash].bundle.js`,
		sourceMapFilename: '[file].map[query]',
		chunkFilename: `${scriptDir}[name].[chunkhash].chunk.js`
	},
	performance: { hints: false },
	stats: env.production ? "errors-only" : "minimal",
	devServer: {
		hot: !env.noreload && env.watch,
		liveReload: !env.noreload && env.watch,
		devMiddleware: {
			writeToDisk: true,
		},
		historyApiFallback: true,
		static: {
			directory: outDir,
			watch: env.watch && {
				ignored: '**/node_modules',
				poll: 1000
			} || undefined
		},
		port: env.port || port,
		open,
		host: env.host || host,
		client: {
			webSocketURL: { hostname: env.host || host, pathname: undefined, port: env.port || port },
			reconnect: env.watch
		},
	},
	devtool: env.production ? false : 'source-map',
	resolveLoader: {
		alias: {
		  'merge-html': path.resolve(__dirname, 'node_modules/build-tools', 'html-merge-loader.js'),
		  'wg-loader': path.resolve(__dirname, 'node_modules/build-tools', 'html-wrapper-generation.js'),
		  'localization-loader': path.resolve(__dirname, 'node_modules/build-tools', 'html-localization-extractor.js')
		}
	  }
});

const getOptimization = (): webpack.Configuration => ({
	optimization: {
		minimizer: [
			new TerserPlugin({ terserOptions: { keep_classnames: true, keep_fnames: true } })
		],
		moduleIds: 'named',
		runtimeChunk: false,
		concatenateModules: false,
		splitChunks: {
			hidePathInfo: true, // prevents the path from being used in the filename when using maxSize
			chunks: "initial",
			cacheGroups: {
				default: false,
				controls: {
					test: /[\\/]client-controls[\\/]/,
					name: 'controls',
					priority: 19,
					enforce: true,
					chunks: 'all',
				},
				vendors: { // picks up everything from node_modules as long as the sum of node modules is larger than minSize
					test: /[\\/]node_modules[\\/]/,
					name: 'vendors',
					priority: 19,
					enforce: true, // causes maxInitialRequests to be ignored, minSize still respected if specified in cacheGroup
					minSize: 30000, // use the default minSize
					chunks: 'all',
				}
			}
		},
		usedExports: false,
	}
});

const getModule = (env: IEnvironment & IBuildOptions, screenInfos: IScreensWebpackInfo): webpack.Configuration => {
	const cssRules = [{
		loader: 'css-loader',
		options: { esModule: false }
	}];

	const sassRules = [{
		loader: "sass-loader",
		options: {
			sassOptions: { includePaths: ['node_modules'] }
		}
	}];

	const htmlLoaders = new Array<any>(
		{
			loader: 'html-loader',
			options: {
				minimize: false
			}
		},
		{
			loader: 'wg-loader',
		},
		{
			loader: 'localization-loader',
		}
	);

	if (screenInfos.mergeHtml) {
		const customizations: string[] = [];
		if (env.customizations && env.customizations.length) {
			customizations.push(...env.customizations);
		}
		if (env.tenant) {
			customizations.push(env.tenant);
		}
		htmlLoaders.push({
			loader: 'merge-html',
			options: { customizations }
		});
	}

	const tenantSrcFolder = env.tenant ? path.resolve(__dirname, getTenantSrcFolder(env)) : undefined;
	const screenFolders: string[] = screenInfos.allScreens ? [] :
		screenInfos.screenIds.map(f => {
			const screenId = f.toUpperCase();
			return path.join(screenSrcFolder, `${screenId.substring(0, 2)}\\${screenId}`);
		});
	if (tenantSrcFolder) {
		screenInfos.screenIds.forEach(f => {
			const screenId = f.toUpperCase();
			screenFolders.push(path.join(tenantSrcFolder, `${screenId.substring(0, 2)}\\${screenId}`));
		});
	}

	const excludeOtherScreens = (value: string) => {
		if (screenInfos.allScreens) {
			return false;
		}

		if (!value.startsWith(screenSrcFolder) && (!tenantSrcFolder || !value.startsWith(tenantSrcFolder))) {
			return false;
		}

		const folderEnding = value.substring(screenSrcFolder.length + 1).split('\\');
		if (folderEnding.length < 2 ||
			folderEnding[0].length !== 2 ||
			folderEnding[1].length !== screenNameLength ||
			!folderEnding[1].startsWith(folderEnding[0])) {
			// not screen
			return false;
		}

		return screenFolders.every(f => !value.startsWith(f));
	};

	return {
		module: {
			unsafeCache: !screenInfos.allScreens ? true : undefined,
			rules: [{
				test: /\.css$/i,
				issuer: [{ not: /\.html$/i }],
				use: env.extractCss ? [{
					loader: MiniCssExtractPlugin.loader
				}, ...cssRules
				] : ['style-loader', ...cssRules]
			}, {
				test: /\.css$/i,
				issuer: /\.html$/i,
				use: cssRules
			}, {
				test: /\.scss$/,
				use: env.extractCss ? [{
					loader: MiniCssExtractPlugin.loader
				}, ...cssRules, ...sassRules
				] : ['style-loader', ...cssRules, ...sassRules],
				issuer: /\.[tj]s$/i
			}, {
				test: /\.scss$/,
				use: [...cssRules, ...sassRules],
				issuer: /\.html?$/i
			}, {
				// TODO: I think, for optimization reasons, we should separate processing of screens html templates
				// from all other html files.
				test: /\.html$/i,
				use: htmlLoaders,
				exclude: excludeOtherScreens
			}, {
				test: /(?<!\.d)\.ts?$/,
				loader: "ts-loader",
				options: {
					compiler: 'ttypescript',
					configFile: !screenInfos.allScreens ?
						ScreenConfigPlugin.tempConfigFile :
						undefined,
				},
				exclude: excludeOtherScreens
			},
			{
				test: excludeOtherScreens,
				loader: 'ignore-loader'
			},
			{
				test: /\.d\.ts|\.map$/,
				loader: 'ignore-loader'
			 }, {
				test: /\.(png|gif|jpg|cur)$/i, loader: 'url-loader', options: { limit: 8192 }
			}, {
				test: /\.woff2(\?v=[0-9]\.[0-9]\.[0-9])?$/i,
				loader: 'url-loader',
				options: { limit: 10000, mimetype: 'application/font-woff2' }
			}, {
				test: /\.woff(\?v=[0-9]\.[0-9]\.[0-9])?$/i,
				loader: 'url-loader',
				options: { limit: 10000, mimetype: 'application/font-woff' }
			}, {
				test: /\.(ttf|eot|svg|otf)(\?v=[0-9]\.[0-9]\.[0-9])?$/i, loader: 'file-loader'
			}, {
				test: /environment\.json$/i, use: [{
					loader: "app-settings-loader",
					options: { env: env.production ? 'production' : 'development' }
				}]
			}, {
				test: /\.js$/,
				use: ["source-map-loader"],
				enforce: "pre"
			}],
		}
	};
};

const getPlugins = (env: IEnvironment & IBuildOptions, screenInfos: IScreensWebpackInfo): webpack.Configuration => {
	const viewsFor = `{src,${controlsDir}}/**/!(tslib)*.{ts,js}`;
	const plugins: webpack.WebpackPluginInstance[] = [
		new AureliaPlugin({
			viewsFor,
			entry: Object.keys(screenInfos.entry)
		}),
		...screenInfos.htmlPlugins,
		new webpack.ProvidePlugin({
			process: 'process/browser',
		}),
		new AureliaCacheWorkaroundPlugin(),
		new ScreenInfoDeployPlugin({
			sourceDir: path.resolve(controlsDir, 'screenInfos'),
			targetDir: path.resolve(__dirname, screenInfoTargetPath)
		}),
		new webpack.DefinePlugin({
			HTML_MERGED: screenInfos.mergeHtml
		}),
		{
			apply: (compiler: webpack.Compiler) => {
				const lockFilePath = path.join(screenInfoTargetPath, lockFileName);
				compiler.hooks.beforeCompile.tap('Write jsons lock', () => {
					logger.info(`Writing lock file ${lockFilePath}`);
					const lockJson = JSON.stringify({ pid: process.pid, ppid: process.ppid, });
					try {
						if (!fs.existsSync(screenInfoTargetPath)) {
							fs.mkdirSync(screenInfoTargetPath);
						}
						fs.writeFileSync(lockFilePath, lockJson);
					}
					catch (error) {
						console.error(error);
					}
				});
				compiler.hooks.failed.tap('Write jsons lock', () => {
					logger.info(`Removing lock file ${lockFilePath}`);
					try {
						fs.unlinkSync(lockFilePath);
					}
					catch (error) {
						console.error(error);
					}
				});
				compiler.hooks.done.tap('Write jsons lock', () => {
					logger.info(`Removing lock file ${lockFilePath}`);
					console.log(`More build info can be found in logs:\n${buildToolsLog}\n${transformersToolsLog}`);
					try {
						fs.unlinkSync(lockFilePath);
					}
					catch (error) {
						console.error(error);
					}
				});
			}
		},
	];

	if (!env.production) {
		plugins.push(new RunLinterPlugin('../../../../../'));
	}

	if (env.watch) {
		plugins.push(new FilesWatcherPlugin({
			addDeletePaths: [path.resolve('./src/screens')],
			changePaths: [
				path.resolve('./build'),
				path.resolve('./webpack.config.ts'),
				path.resolve('./tsconfig.json'),
				path.resolve('./node_modules/client-controls')
			]
		}));
	}

	if (screenInfos.allScreens && !env.tenant) {
		plugins.push(new HtmlWebpackPlugin({
			// template: 'index2.ejs',
			filename: 'enhance.html',
			minify: false
		}));
		plugins.push(new CleanWebpackPlugin({ cleanStaleWebpackAssets: !!env.production }));
	}
	else {
		const defaultFoldersToInclude = ["src/*",
			"src/extensions",
			"src/resources",
			"test",
		];
		if (env.tenant && env.screenIds) {
			const tenantSrcFolder = getTenantSrcFolder(env);
			env.screenIds.forEach((screenId: string) => {
				const tenantScreenPath = `${tenantSrcFolder}/${screenId.substring(0, 2)}/${screenId}`;
				if (fs.existsSync(path.resolve(__dirname, tenantScreenPath))) {
					defaultFoldersToInclude.push(tenantScreenPath);
				}
			});
		}
		plugins.push(new ScreenConfigPlugin({
			...screenInfos,
			defaultFoldersToInclude,
			tsconfig,
			baseDir: path.resolve(__dirname)
		}));
	}

	if (!env.tests) {
		if (screenInfos.allScreens) {
			plugins.push(new DuplicatePackageCheckerPlugin());
		}
		plugins.push(new CopyWebpackPlugin({
			patterns: [
				{ from: 'static', to: outDir, globOptions: { ignore: ['**/*.css', '**/.*'] } }
			]
		}));
	}

	if (env.analyze) {
		plugins.push(new BundleAnalyzerPlugin());
	}

	if (env.extractCss) {
		plugins.push(new MiniCssExtractPlugin({ // updated to match the naming conventions for the js files
			filename: '[name].[contenthash].bundle.css',
			chunkFilename: '[name].[contenthash].chunk.css'
		}));
	}

	if (env.analyzeDependencies) {
		plugins.push(new CircularDependencyPlugin());
	}

	return { plugins };
};

const processScreenIds = (env: IEnvironment & IBuildOptions) => {
	if (typeof env.screenIds === 'string') {
		env.screenIds = (env.screenIds as string).split(',');
	}

	if (typeof env.customizations === 'string') {
		env.customizations = (env.customizations as string).split(',');
	}

	if (env.tenant) {
		outDir = path.resolve(__dirname, `../../Scripts/Screens/${env.tenant}`);
		process.env.TENANT = env.tenant;
		process.env.SCREEN_INFO_PATH = `../../App_Data/TSScreenInfo/${env.tenant}`;
		screenInfoTargetPath = path.resolve(__dirname, process.env.SCREEN_INFO_PATH);
	}

	if (env.modules && !env.screenIds) {
		const modules = typeof env.modules === 'string' ? (env.modules as string).split(',') : env.modules;
		const screenIds: string[] = [];
		modules.forEach((m) => {
			const folder = path.join(screenSrcFolder, m);
			if (!fs.existsSync(folder)) {
				return;
			}
			const subfolders = fs.readdirSync(folder);
			if (!subfolders) {
				return;
			}
			screenIds.push(...subfolders.filter((subfolder) =>
				subfolder.length === screenNameLength && subfolder.startsWith(m)));
		});
		env.screenIds = screenIds;
	}

	if (env.screenIds) {
		process.env.SCREEN_IDS = env.screenIds.join(',');
	}
};

const config = (env: IEnvironment & IBuildOptions = {}): webpack.Configuration => {
	processScreenIds(env);

	const screensInfo = getScreensWebpackInfo(env);

	return {
		...getBaseConfig(env, screensInfo),
		...getOptimization(),
		...getModule(env, screensInfo),
		...getPlugins(env, screensInfo)
	};
};

export default config;
