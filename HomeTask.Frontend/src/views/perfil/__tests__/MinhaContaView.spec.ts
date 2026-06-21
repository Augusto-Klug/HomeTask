import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { createMemoryHistory, createRouter } from 'vue-router'
import MinhaContaView from '../MinhaContaView.vue'
import * as apiModule from '@/services/api'

vi.mock('@/services/api', () => ({ default: { get: vi.fn(), put: vi.fn() } }))
vi.mock('@/stores/auth', () => ({
  useAuthStore: vi.fn(() => ({
    user: { userId: 'user-1', tipo: 2 },
  })),
}))

const mockPerfil = {
  nome: 'João Silva',
  email: 'joao@email.com',
  telefone: '(47) 99999-9999',
  documento: '123.456.789-00',
  cep: '89010-000',
  logradouro: 'Rua XV de Novembro, 100',
  bairro: 'Centro',
  cidade: 'Blumenau',
  estado: 'SC',
  descricao: '',
  raioAtendimentoKm: null,
}

const mockRecebimentos = {
  saldoRecebidoTotal: 420,
  totalServicosRecebidos: 2,
  servicosRecebidos: [
    {
      agendamentoId: 'ag-1',
      tituloServico: 'Faxina completa',
      clienteNome: 'Maria',
      dataConclusao: '2026-06-20T00:00:00Z',
      valorRecebido: 220,
      cidade: 'Blumenau',
      estado: 'SC',
    },
  ],
}

const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: '/', component: { template: '<div />' } },
    { path: '/agendamento/detalhes/:id', component: { template: '<div />' } },
  ],
})

const HtInputStub = {
  template: '<div><span class="input-value">{{ modelValue }}</span><input :disabled="disabled || undefined" /></div>',
  props: ['modelValue', 'disabled', 'label', 'regra', 'type'],
}

const HtSearchSelectStub = {
  template: '<div class="search-select"><span class="search-select-value">{{ modelValue }}</span><input :disabled="disabled || undefined" /></div>',
  props: ['modelValue', 'disabled', 'label', 'options', 'required'],
}

const HtButtonStub = {
  template: '<button :type="type || \'button\'" v-bind="$attrs"><slot /></button>',
  inheritAttrs: true,
  props: ['loading', 'variant', 'type'],
}

function mockApi() {
  vi.mocked(apiModule.default.get).mockImplementation((url: string) => {
    if (url === '/api/Usuario/ObterPerfilUsuario') {
      return Promise.resolve({ data: mockPerfil })
    }

    if (url === '/api/Prestador/ObterPrestadorPorUsuarioId') {
      return Promise.resolve({ data: { id: 'prest-1' } })
    }

    if (url === '/api/Prestador/ObterRecebimentos') {
      return Promise.resolve({ data: mockRecebimentos })
    }

    return Promise.resolve({ data: {} })
  })
}

function mountView() {
  setActivePinia(createPinia())
  mockApi()

  return mount(MinhaContaView, {
    global: {
      plugins: [router],
      stubs: {
        HtInput: HtInputStub,
        HtSearchSelect: HtSearchSelectStub,
        HtButton: HtButtonStub,
        HtSpinner: true,
        HtCard: { template: '<div><slot /></div>' },
        HtAlert: { template: '<div><slot /></div>', props: ['variant', 'message', 'title'] },
        HtCertificacaoForm: true,
        HtPortfolioGaleria: true,
        HtCertificacaoDetail: true,
      },
    },
  })
}

describe('MinhaContaView', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('carrega e exibe os dados do perfil', async () => {
    const wrapper = mountView()
    await flushPromises()
    expect(wrapper.text()).toContain('João Silva')
  })

  it('campos estão desabilitados no modo de visualização', async () => {
    const wrapper = mountView()
    await flushPromises()
    const inputs = wrapper.findAll('.input-value + input, .search-select input')
    expect(inputs.length).toBeGreaterThan(0)
    inputs.forEach(input => expect(input.attributes('disabled')).toBeDefined())
  })

  it('botão de lápis habilita o modo de edição', async () => {
    const wrapper = mountView()
    await flushPromises()
    await wrapper.find('[data-testid="btn-editar"]').trigger('click')
    const inputs = wrapper.findAll('.input-value + input, .search-select input')
    inputs.forEach(input => expect(input.attributes('disabled')).toBeUndefined())
  })

  it('exibe botão Salvar apenas no modo de edição', async () => {
    const wrapper = mountView()
    await flushPromises()
    expect(wrapper.find('[data-testid="btn-salvar"]').exists()).toBe(false)
    await wrapper.find('[data-testid="btn-editar"]').trigger('click')
    expect(wrapper.find('[data-testid="btn-salvar"]').exists()).toBe(true)
  })

  it('chama PUT /api/Usuarios/perfil ao salvar', async () => {
    vi.mocked(apiModule.default.put).mockResolvedValue({ data: mockPerfil })
    const wrapper = mountView()
    await flushPromises()
    await wrapper.find('[data-testid="btn-editar"]').trigger('click')
    await wrapper.find('form').trigger('submit')
    await flushPromises()
    expect(apiModule.default.put).toHaveBeenCalledWith('/api/Usuario/AtualizarPrefilUsuario', expect.any(Object))
  })

  it('usa um select pesquisável para a UF', async () => {
    const wrapper = mountView()
    await flushPromises()

    expect(wrapper.findAll('.search-select').length).toBeGreaterThanOrEqual(1)
    expect(wrapper.text()).toContain('SC')
  })

  it('carrega recebimentos ao abrir a aba do prestador', async () => {
    const wrapper = mountView()
    await flushPromises()

    const abaRecebimentos = wrapper.find('input[aria-label="Recebimentos"]')
    await abaRecebimentos.trigger('change')
    await flushPromises()

    expect(apiModule.default.get).toHaveBeenCalledWith('/api/Prestador/ObterRecebimentos', {
      params: { prestadorId: 'prest-1' },
    })
    expect(wrapper.text()).toContain('Faxina completa')
    expect(wrapper.text()).toContain('Maria')
  })
})
