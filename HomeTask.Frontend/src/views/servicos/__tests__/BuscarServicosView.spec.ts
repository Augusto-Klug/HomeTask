import { mount, flushPromises } from '@vue/test-utils'
import { describe, expect, it, vi, beforeEach, afterEach } from 'vitest'
import { createRouter, createMemoryHistory } from 'vue-router'

import BuscarServicosView from '../BuscarServicosView.vue'
import * as apiModule from '@/services/api'

vi.mock('@/services/api', () => ({
  default: { get: vi.fn() },
}))

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: '/servicos/buscar', component: BuscarServicosView }],
})

describe('BuscarServicosView', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.useFakeTimers()
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [] })
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('debounces service search by 700ms when the city filter changes', async () => {
    router.push('/servicos/buscar')
    await router.isReady()

    const wrapper = mount(BuscarServicosView, {
      global: {
        plugins: [router],
      },
    })

    await flushPromises()
    expect(apiModule.default.get).toHaveBeenCalledTimes(1)

    const cityInput = wrapper.get('input[placeholder="Ex: Blumenau"]')
    await cityInput.setValue('Blu')

    vi.advanceTimersByTime(699)
    await flushPromises()
    expect(apiModule.default.get).toHaveBeenCalledTimes(1)

    vi.advanceTimersByTime(1)
    await flushPromises()
    expect(apiModule.default.get).toHaveBeenCalledTimes(2)
    expect(apiModule.default.get).toHaveBeenLastCalledWith(
      '/api/ServicoOferecido/BuscarServicos',
      expect.objectContaining({
        params: expect.objectContaining({ cidade: 'Blu' }),
      }),
    )
  })
})
