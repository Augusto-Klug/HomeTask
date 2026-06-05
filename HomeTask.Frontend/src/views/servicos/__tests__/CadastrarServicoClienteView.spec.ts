import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount, flushPromises } from '@vue/test-utils'
import { setActivePinia, createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import CadastrarServicoClienteView from '../CadastrarServicoClienteView.vue'
import HtTextarea from '@/components/ui/HtTextarea.vue'
import HtSelect from '@/components/ui/HtSelect.vue'
import HtInput from '@/components/ui/HtInput.vue'
import HtDateTimeInput from '@/components/ui/HtDateTimeInput.vue'
import HtAlert from '@/components/ui/HtAlert.vue'
import HtCard from '@/components/ui/HtCard.vue'
import { UnidadeCobranca, type ServicoClienteForm } from '@/types'

vi.mock('@/services/api', () => ({
  default: { post: vi.fn(), get: vi.fn() },
}))

vi.mock('@/stores/auth', () => ({
  useAuthStore: vi.fn(() => ({
    isLoggedIn: true,
    user: { userId: '1', nome: 'João', tipo: 1 },
  })),
}))

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: { template: '<div/>' } },
    { path: '/servicos/buscar', component: { template: '<div/>' } },
    { path: '/servicos/novo-cliente', component: { template: '<div/>' } },
  ],
})

type ExposedVm = {
  form: ServicoClienteForm
  handleSubmit: () => Promise<void>
  sucesso: boolean
  erro: string | null
}

function mountView() {
  return shallowMount(CadastrarServicoClienteView, {
    global: {
      plugins: [createPinia(), router],
      // Não stubar HtCard para que o conteúdo dos slots seja renderizado
      stubs: { HtCard: false },
    },
  })
}

function preencherFormCliente(vm: ExposedVm, overrides: Partial<ServicoClienteForm> = {}) {
  Object.assign(vm.form, {
    titulo: 'Faxina completa',
    descricao: 'Preciso de faxina na minha casa',
    categoria: '1',
    unidadeCobranca: String(UnidadeCobranca.PorHora),
    precoBase: '80',
    data: '',
    ...overrides,
  })
}

describe('CadastrarServicoClienteView', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('exibe o título da tela', () => {
    const wrapper = mountView()
    expect(wrapper.text()).toContain('Solicitar serviço')
  })

  it('exibe campo de descrição (HtTextarea)', () => {
    const wrapper = mountView()
    expect(wrapper.findAllComponents(HtTextarea).length).toBeGreaterThanOrEqual(1)
  })

  it('exibe ao menos dois selects (categoria e tipo de valor)', () => {
    const wrapper = mountView()
    expect(wrapper.findAllComponents(HtSelect).length).toBeGreaterThanOrEqual(2)
  })

  it('oculta campo de valor quando tipoValor é a_combinar', async () => {
    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    vm.form.unidadeCobranca = `${UnidadeCobranca.ACombinar}`
    await wrapper.vm.$nextTick()
    const inputs = wrapper.findAllComponents(HtInput)
    const valorInput = inputs.find(i => i.props('label') === 'Valor (R$)')
    expect(valorInput).toBeUndefined()
  })

  it('exibe campo de valor quando tipoValor é por_hora', async () => {
    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    vm.form.unidadeCobranca = `${UnidadeCobranca.PorHora}`
    await wrapper.vm.$nextTick()
    const inputs = wrapper.findAllComponents(HtInput)
    const valorInput = inputs.find(i => i.props('label') === 'Valor (R$)')
    expect(valorInput).toBeDefined()
  })

  it('exibe campo de valor quando tipoValor é total', async () => {
    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    vm.form.unidadeCobranca = `${UnidadeCobranca.Total}`
    await wrapper.vm.$nextTick()
    const inputs = wrapper.findAllComponents(HtInput)
    const valorInput = inputs.find(i => i.props('label') === 'Valor (R$)')
    expect(valorInput).toBeDefined()
  })

  it('exibe a data desejada com seletor nativo de data e hora', () => {
    const wrapper = mountView()
    const dateTimeInput = wrapper.findComponent(HtDateTimeInput)

    expect(dateTimeInput.exists()).toBe(true)
    expect(dateTimeInput.props('label')).toBe('Data desejada')
  })

  it('chama a API ao submeter o formulário com dados válidos', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockResolvedValueOnce({ data: {} })

    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    preencherFormCliente(vm)

    await vm.handleSubmit()
    await flushPromises()

    expect(api.default.post).toHaveBeenCalledOnce()
  })

  it('define sucesso como true após submissão bem-sucedida', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockResolvedValueOnce({ data: {} })

    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    preencherFormCliente(vm, { unidadeCobranca: `${UnidadeCobranca.ACombinar}`, valor: '' })

    await vm.handleSubmit()
    await flushPromises()

    expect(vm.sucesso).toBe(true)
  })

  it('exibe HtAlert quando a API falha', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockRejectedValueOnce({ response: { data: 'Erro no servidor' } })

    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    preencherFormCliente(vm, { unidadeCobranca: `${UnidadeCobranca.ACombinar}`, valor: '' })

    await vm.handleSubmit()
    await flushPromises()
    await wrapper.vm.$nextTick()

    expect(wrapper.findAllComponents(HtAlert).length).toBeGreaterThanOrEqual(1)
  })

  it('envia a data desejada com hora quando preenchida', async () => {
    const api = await import('@/services/api')
    vi.mocked(api.default.post).mockResolvedValueOnce({ data: {} })

    const wrapper = mountView()
    const vm = wrapper.vm as unknown as ExposedVm
    preencherFormCliente(vm, { data: '2026-04-20T14:30' })

    await vm.handleSubmit()
    await flushPromises()

    expect(api.default.post).toHaveBeenCalledWith(
      '/api/ServicoOferecido/CriarServicoCliente',
      expect.objectContaining({
        dataDesejada: expect.stringContaining('2026-04-20T'),
      }),
    )
  })
})
