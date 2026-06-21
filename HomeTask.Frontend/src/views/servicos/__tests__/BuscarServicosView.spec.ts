import { mount, flushPromises } from '@vue/test-utils'
import { describe, expect, it, vi, beforeEach } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { createRouter, createMemoryHistory } from 'vue-router'

import BuscarServicosView from '../BuscarServicosView.vue'
import * as apiModule from '@/services/api'
import { useAuthStore } from '@/stores/auth'
import { TipoAnuncio, TipoUsuario } from '@/types'

vi.mock('@/services/api', () => ({
  default: { get: vi.fn() },
}))

describe('BuscarServicosView', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
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
    const router = criarRouter()
    router.push('/servicos/buscar')
    await router.isReady()

    mount(BuscarServicosView, {
      global: {
        plugins: [router, createPinia()],
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
    const router = criarRouter()
    router.push('/servicos/buscar?cidade=Blumenau&pagina=3&tamanhoPagina=50')
    await router.isReady()

    mount(BuscarServicosView, {
      global: {
        plugins: [router, createPinia()],
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

  it('envia filtro de pedidos quando usuário logado é prestador', async () => {
    const router = criarRouter()
    const pinia = createPinia()
    setActivePinia(pinia)
    useAuthStore().setUser({ userId: '1', nome: 'Prestador', email: 'prestador@test.com', tipo: TipoUsuario.Prestador })
    router.push('/servicos/buscar')
    await router.isReady()

    mount(BuscarServicosView, {
      global: {
        plugins: [router, pinia],
      },
    })

    await flushPromises()

    expect(apiModule.default.get).toHaveBeenCalledWith(
      '/api/ServicoOferecido/BuscarServicos',
      expect.objectContaining({
        params: expect.objectContaining({ tipoAnuncio: TipoAnuncio.Pedido }),
      }),
    )
  })

  it('envia filtro de ofertas quando usuário logado é cliente', async () => {
    const router = criarRouter()
    const pinia = createPinia()
    setActivePinia(pinia)
    useAuthStore().setUser({ userId: '1', nome: 'Cliente', email: 'cliente@test.com', tipo: TipoUsuario.Cliente })
    router.push('/servicos/buscar')
    await router.isReady()

    mount(BuscarServicosView, {
      global: {
        plugins: [router, pinia],
      },
    })

    await flushPromises()

    expect(apiModule.default.get).toHaveBeenCalledWith(
      '/api/ServicoOferecido/BuscarServicos',
      expect.objectContaining({
        params: expect.objectContaining({ tipoAnuncio: TipoAnuncio.Oferta }),
      }),
    )
  })
})

function criarRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [{ path: '/servicos/buscar', component: BuscarServicosView }],
  })
}
