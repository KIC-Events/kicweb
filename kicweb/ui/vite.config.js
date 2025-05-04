import { resolve } from 'path'
import { viteStaticCopy } from 'vite-plugin-static-copy';

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
  },
  plugins: [
    viteStaticCopy({
      targets: [
        {
          src: 'public/*',
          dest: '',
        },
      ],
    }),
  ],
};