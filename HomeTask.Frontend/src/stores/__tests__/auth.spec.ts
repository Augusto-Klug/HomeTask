import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../auth'

// Mock the api module
vi.mock('@/services/api', () => ({
  default: {
    post: vi.fn(),
    get: vi.fn(),
  },
}))

// Mock the router
vi.mock('@/router', () => ({
  default: { push: vi.fn() },
}))

describe('useAuthStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('should start with no user logged in', () => {
    const auth = useAuthStore()
    expect(auth.isLoggedIn).toBe(false)
    expect(auth.user).toBeNull()
  })

  it('should set user after login', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockResolvedValueOnce({
      data: { userId: '1', nome: 'João', email: 'joao@test.com', tipo: 1 },
    })

    const auth = useAuthStore()
    await auth.login('joao@test.com', '123456')

    expect(auth.isLoggedIn).toBe(true)
    expect(auth.user?.nome).toBe('João')
  })

  it('should clear user after logout', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockResolvedValueOnce({
      data: { userId: '1', nome: 'João', email: 'joao@test.com', tipo: 1 },
    })
    vi.mocked(api.default.post).mockResolvedValueOnce({}) // logout

    const auth = useAuthStore()
    await auth.login('joao@test.com', '123456')
    await auth.logout()

    expect(auth.isLoggedIn).toBe(false)
    expect(auth.user).toBeNull()
  })
})
