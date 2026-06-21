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
  routes: [
    { path: '/servicos/buscar', component: BuscarServicosView },
    { path: '/clientes/:id', component: { template: '<div>Cliente</div>' } },
    { path: '/prestadores/:id', component: { template: '<div>Prestador</div>' } },
  ],
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

  it('carrega a busca inicial com 30 registros por pagina', async () => {
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

  it('preserva paginacao e filtros vindos da URL', async () => {
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

  it('exibe endereco detalhado quando o servico o informa', async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({
      data: {
        itens: [{
          id: '11',
          titulo: 'Limpeza Pos-obra',
          descricao: 'desc',
          precoBase: 160,
          unidadeCobranca: 2,
          tipoAnuncio: 2,
          clienteId: '2',
          clienteNome: 'Pedro',
          categoria: 1,
          logradouro: 'Rua XV de Novembro',
          numero: '320',
          bairro: 'Centro',
          cidade: 'Blumenau',
          estado: 'SC',
        }],
        paginaAtual: 1,
        tamanhoPagina: 30,
        totalRegistros: 1,
        totalPaginas: 1,
      },
    })

    router.push('/servicos/buscar')
    await router.isReady()

    const wrapper = mount(BuscarServicosView, {
      global: {
        plugins: [router],
      },
    })

    await flushPromises()

    expect(wrapper.text()).toContain('Rua XV de Novembro, 320 - Centro - Blumenau/SC')
  })
})
