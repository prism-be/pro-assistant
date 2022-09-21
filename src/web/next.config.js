/** @type {import('next').NextConfig} */

const nextTranslate = require('next-translate')

const nextConfig = {
    reactStrictMode: true,
    swcMinify: true,
    output: "standalone",
    publicRuntimeConfig: {
        apiRoot: process.env.NEXT_PUBLIC_API_URL
    }
}

module.exports = nextTranslate(nextConfig);
