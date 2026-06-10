import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { initTheme } from './composables/useTheme'
import { useAuthStore } from './stores/auth'

async function bootstrap() {
  if (import.meta.env.VITE_USE_MOCK === 'true') {
    const { worker } = await import('@/mocks/browser')
    await worker.start({
      onUnhandledRequest: 'bypass', // deixa passar requisições sem handler (ex: assets)
    })
  }

  const app = createApp(App)
  const pinia = createPinia()

  app.use(pinia)

  const auth = useAuthStore(pinia)
  await auth.initialize()

  app.use(router)
  app.mount('#app')
  initTheme()
}

bootstrap()
