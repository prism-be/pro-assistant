/** @type {import('next').NextConfig} */

const {i18n} = require('./next-i18next.config');

const nextConfig = {
    reactStrictMode: true,
    swcMinify: true,
    output: "standalone",
    i18n,
    publicRuntimeConfig: {
        apiRoot: process.env.NEXT_PUBLIC_API_URL
    }
}

module.exports = nextConfig
