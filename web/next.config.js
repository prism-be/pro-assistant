/** @type {import('next').NextConfig} */

const nextTranslate = require('next-translate-plugin');

const nextConfig = {
    reactStrictMode: true,
    async rewrites() {
        return [
            {
                source: '/api/data/:path*',
                destination: 'http://localhost:7099/api/data/:path*'
            }
        ]
    }
}

module.exports = nextTranslate(nextConfig);
