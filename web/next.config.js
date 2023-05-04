/** @type {import('next').NextConfig} */

const nextConfig = {
    reactStrictMode: true,
    rewrites: process.env.NODE_ENV !== 'production' ? async () => {
        return [
            {
                source: '/api/:path*',
                destination: 'http://localhost:7099/api/:path*'
            }
        ]
    } : null,
    publicRuntimeConfig: {
        i18n: {
            languages: ["fr"],
            defaultLanguage: "fr",
            namespaces: ["accounting", "common", "configuration", "documents", "login"],
            defaultNamespace: "common",
        },
    },
    images: {
        unoptimized: true
    },
    output: process.env.NODE_ENV === 'production' ? 'export' : undefined
}

module.exports = nextConfig;
