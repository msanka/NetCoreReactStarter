var path = require('path');
var webpack = require('webpack');
var pkg = require('./package.json');

// bundle dependencies in separate vendor bundle
var vendorPackages = Object.keys(pkg.dependencies).filter(function (el) {
    return el.indexOf('font') === -1; // exclude font packages from vendor bundle
});

var config =
    {
        devtool: 'source-map',
        cache: false,
        context: path.join(__dirname,"App", "src"),
        entry:
        {
            app: "./index",
            vendor: vendorPackages
        },
        output: 
        {
            path: path.join(__dirname, "wwwroot", "js", "app"),
            filename: '[name].js',
            sourceMapFilename: '[file].map',
            publicPath: 'http://localhost:5000/js/app/' //Server Path for LazyLoading the chunks. Thanks : https://toddmotto.com/lazy-loading-angular-code-splitting-webpack
        },
        resolve:
        {
            modules: ['node_modules']
        },
        optimization: 
        {
            splitChunks: 
            {
                cacheGroups: 
                {
                    default: false,
                    commons: {
                        test: /[\\/]node_modules[\\/]/,
                        name: "vendor",
                        chunks: "all"
                    }
                }
            }
        },
        plugins: 
        [
            new webpack.SourceMapDevToolPlugin(
            {
                filename: '[name].js.map',
                exclude: ['vendor.js']
            }),
            new webpack.DefinePlugin(
                { 
                    'process.env': 
                    {
                        'NODE_ENV': JSON.stringify('local')
                    },
                    '__globals':
                    {
                        PROXY_URI: JSON.stringify('/api/proxy'),
                        SPA_IMAGES_ROOT: JSON.stringify('/images/spa'),
                    }
                }),
                new webpack.IgnorePlugin(/^\.\/locale$/, [/moment$/])
        ],
        module: 
        {
            rules: 
            [
              {
                test: /\.css$/,
                use: 
                    [
                        { loader: 'style-loader' },
                        { loader: 'css-loader', options: { modules: true } }
                    ]
              },
              { 
                test: /(\.jsx|\.js)$/,
                exclude: /node_modules/, 
                use : 
                    [ 
                        {loader: 'eslint-loader'}
                    ]
               },
               { 
                    test: /\.js$/,
                    exclude: /node_modules/, 
                    use : 
                        [ 
                            {loader: 'babel-loader'},
                        ]
               },
               {
                    test: /\.(gif|png|jpe?g|svg)$/i,
                    use: 
                        [
                            {loader: 'file-loader', options: { bypassOnDebug: true } },
                            {loader: 'image-webpack-loader',
                             options: 
                             {
                                mozjpeg: { progressive: true, quality: 65 },
                                // optipng.enabled: false will disable optipng
                                optipng: { enabled: false },
                                pngquant: { quality: '65-90', speed: 4 },
                                gifsicle: { interlaced: false },
                                // the webp option will enable WEBP
                                webp: { quality: 75}
                             }
                            }
                        ]
               }
            ]
        }
}

module.exports = config;