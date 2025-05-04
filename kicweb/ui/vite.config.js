import { resolve } from 'path'

export default {
  resolve: {
    alias: {
      '~': resolve(__dirname),
    },
  },
  build: {
    outDir: '../wwwroot/dist',
    assetsDir: '',
    manifest: true,
    rollupOptions: {
      input: {
        main: resolve(__dirname, 'index.js')
      }
    }
  },
  css: {
    preprocessorOptions: {
      scss: {
        additionalData: `
          @use "./_variables" as *;
          @use "./_mixins" as *;
        `
      }
    }
  }
};