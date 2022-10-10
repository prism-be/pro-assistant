/** @type {import('next').NextConfig} */

const nextTranslate = require('next-translate')

const nextConfig = {
    reactStrictMode: true,
    swcMinify: true,
    output: "standalone",
    publicRuntimeConfig: {
        apiRoot: process.env.NEXT_PUBLIC_API_URL
    },
    async rewrites() {
        return [
            {
                source: '/api/:path*',
                destination: 'http://localhost:7013/api/:path*'
                //destination: 'http://localhost:7071/api/:path*'
            }
        ]
    }
}

module.exports = nextTranslate(nextConfig);
