/** @type {import('next').NextConfig} */

const nextTranslate = require('next-translate')

const nextConfig = {
    reactStrictMode: true,
    swcMinify: true,
    output: "standalone",
    publicRuntimeConfig: {
        clientId: process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID,
        tenantId: process.env.NEXT_PUBLIC_AZURE_AD_TENANT_ID,
        tenantName: process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME,
        userFlow: process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW
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
