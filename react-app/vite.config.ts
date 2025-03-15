import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import mkcert from 'vite-plugin-mkcert'
import tsconfigPaths from 'vite-tsconfig-paths'

export default defineConfig({
  plugins: [react(), tsconfigPaths(), mkcert()],
  server: {
    port: 8080,
    proxy: {
      '/api': {
        target: 'https://localhost:7225',
        changeOrigin: true,
        secure: false,
      },
    },
  },
  build: {
    minify: 'terser',
    chunkSizeWarningLimit: 1024,
    outDir: '../AspNotes/wwwroot',
    emptyOutDir: true,
  },
})
