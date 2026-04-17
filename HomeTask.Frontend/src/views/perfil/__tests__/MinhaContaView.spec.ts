import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { createRouter, createMemoryHistory } from 'vue-router'
import MinhaContaView from '../MinhaContaView.vue'
import * as apiModule from '@/services/api'

vi.mock('@/services/api', () => ({ default: { get: vi.fn(), put: vi.fn() } }))

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

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: '/', component: { template: '<div />' } }],
})

// Stub que expõe o valor via texto para facilitar asserções
const HtInputStub = {
  template: '<div><span class="input-value">{{ modelValue }}</span><input :disabled="disabled || undefined" /></div>',
  props: ['modelValue', 'disabled', 'label', 'regra', 'type'],
}

const HtSearchSelectStub = {
  template: '<div class="search-select"><span class="search-select-value">{{ modelValue }}</span><input :disabled="disabled || undefined" /></div>',
  props: ['modelValue', 'disabled', 'label', 'options', 'required'],
}

// Stub que preserva data-testid e type
const HtButtonStub = {
  template: '<button :type="type || \'button\'" v-bind="$attrs"><slot /></button>',
  inheritAttrs: true,
  props: ['loading', 'variant', 'type'],
}

function mountView() {
  setActivePinia(createPinia())
  vi.mocked(apiModule.default.get).mockResolvedValue({ data: mockPerfil })

  return mount(MinhaContaView, {
    global: {
      plugins: [router],
      stubs: {
        HtInput: HtInputStub,
        HtSearchSelect: HtSearchSelectStub,
        HtButton: HtButtonStub,
        HtSpinner: true,
        HtAlert: { template: '<div><slot /></div>', props: ['variant', 'message', 'title'] },
      },
    },
  })
}

describe('MinhaContaView', () => {
  beforeEach(() => vi.clearAllMocks())

  it('carrega e exibe os dados do perfil', async () => {
    const wrapper = mountView()
    await flushPromises()
    expect(wrapper.text()).toContain('João Silva')
  })

  it('campos estão desabilitados no modo de visualização', async () => {
    const wrapper = mountView()
    await flushPromises()
    const inputs = wrapper.findAll('input')
    expect(inputs.length).toBeGreaterThan(0)
    inputs.forEach(input => expect(input.attributes('disabled')).toBeDefined())
  })

  it('botão de lápis habilita o modo de edição', async () => {
    const wrapper = mountView()
    await flushPromises()
    await wrapper.find('[data-testid="btn-editar"]').trigger('click')
    const inputs = wrapper.findAll('input')
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
    expect(apiModule.default.put).toHaveBeenCalledWith('/api/Usuarios/perfil', expect.any(Object))
  })

  it('usa um select pesquisável para a UF', async () => {
    const wrapper = mountView()
    await flushPromises()

    expect(wrapper.findAll('.search-select').length).toBeGreaterThanOrEqual(1)
    expect(wrapper.text()).toContain('SC')
  })
})
