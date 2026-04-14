import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { createRouter, createMemoryHistory } from 'vue-router'
import MeusAgendamentosView from '../MeusAgendamentosView.vue'
import { useAuthStore } from '@/stores/auth'
import * as apiModule from '@/services/api'
import type { AgendamentoResumo } from '@/types'

vi.mock('@/services/api', () => ({ default: { get: vi.fn() } }))

const makeAgendamento = (overrides: Partial<AgendamentoResumo> = {}): AgendamentoResumo => ({
  id: '1',
  clienteId: '10',
  clienteNome: 'João Silva',
  prestadorId: '20',
  prestadorNome: 'Maria Costa',
  dataHoraAgendada: new Date(Date.now() + 86400000).toISOString(),
  duracaoMinutos: 60,
  status: 'Solicitado',
  endereco: { logradouro: 'Rua A, 100', bairro: 'Centro', cidade: 'Florianópolis', estado: 'SC' },
  observacoes: null,
  valorTotal: 150,
  dataSolicitacao: new Date().toISOString(),
  dataResposta: null,
  dataConclusao: null,
  motivoRecusa: null,
  servicos: [
    {
      id: 'srv-1',
      titulo: 'Faxina',
      descricao: 'Limpeza residencial',
      precoBase: 80,
      duracaoEstimadaMinutos: 60,
      unidadeCobranca: 'total',
      tipoAnuncio: 'PrestadorOferece',
      categoria: { id: 'cat-1', nome: 'Faxina', icone: 'cleaning_services' },
    },
  ],
  ...overrides,
})

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: '/', component: { template: '<div />' } }],
})

function mountView(userTipo: number) {
  setActivePinia(createPinia())
  const auth = useAuthStore()
  auth.setUser({ userId: 10, nome: 'Teste', email: 'a@b.com', tipo: userTipo })

  return mount(MeusAgendamentosView, {
    global: {
      plugins: [router],
      stubs: {
        HtSpinner: true,
        HtCard: { template: '<div><slot /></div>' },
        HtBadge: { template: '<span><slot /></span>' },
      },
    },
  })
}

describe('MeusAgendamentosView', () => {
  beforeEach(() => vi.clearAllMocks())

  it('exibe seção "Cliente" para tipo 1', async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] })
    const wrapper = mountView(1)
    await flushPromises()
    expect(wrapper.text()).toContain('Cliente')
    expect(wrapper.text()).not.toContain('Prestador')
  })

  it('exibe seção "Prestador" para tipo 2', async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] })
    const wrapper = mountView(2)
    await flushPromises()
    expect(wrapper.text()).toContain('Prestador')
    expect(wrapper.text()).not.toContain('Cliente')
  })

  it('exibe ambas as seções para tipo 3', async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] })
    const wrapper = mountView(3)
    await flushPromises()
    expect(wrapper.text()).toContain('Cliente')
    expect(wrapper.text()).toContain('Prestador')
  })

  it('não exibe agendamentos passados', async () => {
    const passado = makeAgendamento({
      dataHoraAgendada: new Date(Date.now() - 86400000).toISOString(),
    })
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [passado] })
    const wrapper = mountView(1)
    await flushPromises()
    expect(wrapper.text()).toContain('Nenhum agendamento')
  })

  it('exibe nome do prestador no card do cliente', async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] })
    const wrapper = mountView(1)
    await flushPromises()
    expect(wrapper.text()).toContain('Maria Costa')
  })

  it('exibe mensagem vazia quando não há agendamentos futuros', async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [] })
    const wrapper = mountView(1)
    await flushPromises()
    expect(wrapper.text()).toContain('Nenhum agendamento')
  })
})
