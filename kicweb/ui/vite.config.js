import { resolve } from 'path'

export default {
  base: '/dist/',
  build: {
    outDir: '../wwwroot/dist',
    assetsDir: '',
    manifest: true,
    rollupOptions: {
      input: {
        main: resolve(__dirname, 'index.js')
      }
    }
  }
}