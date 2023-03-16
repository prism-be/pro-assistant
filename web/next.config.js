/** @type {import('next').NextConfig} */

const nextTranslate = require('next-translate-plugin');

const nextConfig = {
    reactStrictMode: true,
    async rewrites() {
        return [
            {
                source: '/api/:path*',
                destination: 'http://localhost:7099/api/:path*'
            }
        ]
    }
}

module.exports = nextTranslate(nextConfig);
