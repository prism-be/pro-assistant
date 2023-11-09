import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tsconfigPaths from 'vite-tsconfig-paths'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), tsconfigPaths()],
  server: {
    port: 3000,
    proxy: {
      '/api' : {
        target: 'http://localhost:7099',
      }
    }
  },
  build: {
    chunkSizeWarningLimit: 1000,
  }
})
