import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import CadastrarServicoPrestadorView from '../CadastrarServicoPrestadorView.vue'
import HtInput from '@/components/ui/HtInput.vue'
import HtSelect from '@/components/ui/HtSelect.vue'
import HtAlert from '@/components/ui/HtAlert.vue'
import type { ServicoPrestadorForm } from '@/types'

vi.mock('@/services/api', () => ({
  default: { post: vi.fn(), get: vi.fn() },
}))

vi.mock('@/stores/auth', () => ({
  useAuthStore: vi.fn(() => ({
    isLoggedIn: true,
    user: { userId: 2, nome: 'Maria', tipo: 2 },
  })),
}))

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: { template: '<div/>' } },
    { path: '/servicos/buscar', component: { template: '<div/>' } },
    { path: '/servicos/novo-prestador', component: { template: '<div/>' } },
  ],
})

type ExposedVm = {
  form: ServicoPrestadorForm
  handleSubmit: () => Promise<void>
  sucesso: boolean
  erro: string | null
}

function mountView() {
  return shallowMount(CadastrarServicoPrestadorView, {
    global: {
      plugins: [createPinia(), router],
      // Não stubar HtCard para que o conteúdo dos slots seja renderizado
      stubs: { HtCard: false },
    },
  })
}

function preencherFormPrestador(vm: ExposedVm, overrides: Partial<ServicoPrestadorForm> = {}) {
  Object.assign(vm.form, {
    titulo: 'Corte de árvore',
    descricao: 'Serviço de poda e corte',
    categoria: '2',
    tipoValor: 'por_hora',
    valor: '80',
    aceitaPagamentoAposFinalizacao: false,
    ...overrides,
  })
}

describe('CadastrarServicoPrestadorView', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('exibe o título da tela', () => {
    const wrapper = mountView()
    expect(wrapper.text()).toContain('Anunciar Meu Serviço')
  })

  it('exibe campo de título do serviço', () => {
    const wrapper = mountView()
    const inputs = wrapper.findAllComponents(HtInput)
    const tituloInput = inputs.find(i => i.props('label') === 'Título do serviço')
    expect(tituloInput).toBeDefined()
  })

  it('exibe campo de valor', () => {
    const wrapper = mountView()
    const inputs = wrapper.findAllComponents(HtInput)
    const valorInput = inputs.find(i => i.props('label') === 'Valor (R$)')
    expect(valorInput).toBeDefined()
  })

  it('exibe ao menos dois selects (categoria e modalidade)', () => {
    const wrapper = mountView()
    expect(wrapper.findAllComponents(HtSelect).length).toBeGreaterThanOrEqual(2)
  })

  it('exibe checkbox de aceitar pagamento após finalização', () => {
    const wrapper = mountView()
    const checkbox = wrapper.find('input[type="checkbox"]')
    expect(checkbox.exists()).toBe(true)
  })

  it('chama a API ao submeter o formulário com dados válidos', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockResolvedValueOnce({ data: {} })

    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    preencherFormPrestador(vm)

    await vm.handleSubmit()
    await flushPromises()

    expect(api.default.post).toHaveBeenCalledOnce()
  })

  it('define sucesso como true após submissão bem-sucedida', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockResolvedValueOnce({ data: {} })

    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    preencherFormPrestador(vm)

    await vm.handleSubmit()
    await flushPromises()

    expect(vm.sucesso).toBe(true)
  })

  it('exibe HtAlert quando a API falha', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockRejectedValueOnce({ response: { data: 'Erro no servidor' } })

    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    preencherFormPrestador(vm)

    await vm.handleSubmit()
    await flushPromises()
    await wrapper.vm.$nextTick()

    expect(wrapper.findAllComponents(HtAlert).length).toBeGreaterThanOrEqual(1)
  })
})
