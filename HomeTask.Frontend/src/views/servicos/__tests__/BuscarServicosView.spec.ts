import { mount, flushPromises } from '@vue/test-utils'
import { describe, expect, it, vi, beforeEach } from 'vitest'
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
    vi.mocked(apiModule.default.get).mockResolvedValue({
      data: {
        itens: [],
        paginaAtual: 1,
        tamanhoPagina: 30,
        totalRegistros: 0,
        totalPaginas: 0,
      },
    })
  })

  it('carrega a busca inicial com 30 registros por página', async () => {
    router.push('/servicos/buscar')
    await router.isReady()

    mount(BuscarServicosView, {
      global: {
        plugins: [router],
      },
    })

    await flushPromises()

    expect(apiModule.default.get).toHaveBeenCalledWith(
      '/api/ServicoOferecido/BuscarServicos',
      expect.objectContaining({
        params: expect.objectContaining({ pagina: 1, tamanhoPagina: 30 }),
      }),
    )
  })

  it('preserva paginação e filtros vindos da URL', async () => {
    router.push('/servicos/buscar?cidade=Blumenau&pagina=3&tamanhoPagina=50')
    await router.isReady()

    mount(BuscarServicosView, {
      global: {
        plugins: [router],
      },
    })

    await flushPromises()

    expect(apiModule.default.get).toHaveBeenCalledWith(
      '/api/ServicoOferecido/BuscarServicos',
      expect.objectContaining({
        params: expect.objectContaining({
          cidade: 'Blumenau',
          pagina: 3,
          tamanhoPagina: 50,
        }),
      }),
    )
  })
})
