import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  server:{
    host: '0.0.0.0',
    port: 80,
    // 因使用Docker建置，導致原本監聽檔案修改的事件無法正常運作，因此需要設定polling並500毫秒觸發一次使熱重載正常運作
    watch: {
      usePolling: true,
      interval: 500,
    }
  }
})
