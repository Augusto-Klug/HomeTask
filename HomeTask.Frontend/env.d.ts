/// <reference types="vite/client" />

// Extend RouteMeta for navigation guards
import 'vue-router'
declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    guestOnly?: boolean
  }
}

// Extend Vite's ImportMetaEnv with custom variables
interface ImportMetaEnv {
  readonly VITE_API_BASE_URL?: string
}

