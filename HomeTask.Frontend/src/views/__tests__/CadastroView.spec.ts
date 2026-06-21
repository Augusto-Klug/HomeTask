import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import CadastroView from '../CadastroView.vue'
import api from '@/services/api'

vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
  },
}))

const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: '/', component: { template: '<div />' } },
    { path: '/cadastro-sucesso', component: { template: '<div />' } },
  ],
})

const HtInputStub = {
  template: '<input />',
  props: ['modelValue', 'label', 'placeholder', 'regra', 'required', 'type'],
  methods: { validar: () => true },
}

const HtTextareaStub = {
  template: '<textarea />',
  props: ['modelValue', 'label', 'placeholder', 'rows'],
  methods: { validar: () => true },
}

const HtSearchSelectStub = {
  template: `
    <div>
      <span data-testid="estado-atual">{{ modelValue }}</span>
      <button type="button" data-testid="selecionar-sc" @click="$emit('update:modelValue', 'SC')">SC</button>
      <button type="button" data-testid="selecionar-ac" @click="$emit('update:modelValue', 'AC')">AC</button>
    </div>
  `,
  props: ['modelValue', 'label', 'options', 'placeholder', 'required'],
  methods: { validar: () => true },
}

const HtSelectStub = {
  template: `
    <div>
      <span data-testid="cidade-disabled">{{ disabled ? 'true' : 'false' }}</span>
      <span data-testid="cidade-options">{{ options.map((option) => option.label).join('|') }}</span>
      <span data-testid="cidade-placeholder">{{ placeholder }}</span>
      <span data-testid="cidade-hint">{{ hint }}</span>
    </div>
  `,
  props: ['modelValue', 'label', 'options', 'placeholder', 'hint', 'disabled', 'required'],
  methods: { validar: () => true },
}

function mountView() {
  return mount(CadastroView, {
    global: {
      plugins: [router],
      stubs: {
        HtInput: HtInputStub,
        HtTextarea: HtTextareaStub,
        HtSearchSelect: HtSearchSelectStub,
        HtSelect: HtSelectStub,
        HtButton: { template: '<button><slot /></button>', props: ['loading', 'type'] },
        HtCard: { template: '<div><slot /></div>' },
        HtAlert: { template: '<div />', props: ['message'] },
        HtDivider: { template: '<hr />' },
      },
    },
  })
}

describe('CadastroView', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(api.get).mockResolvedValue({
      data: [
        { id: '1', nome: 'Rio Branco', estado: 'AC' },
        { id: '2', nome: 'Blumenau', estado: 'SC' },
        { id: '3', nome: 'Florianopolis', estado: 'SC' },
      ],
    })
  })

  it('exige estado antes de habilitar e listar cidades', async () => {
    const wrapper = mountView()
    await flushPromises()

    expect(wrapper.get('[data-testid="cidade-disabled"]').text()).toBe('true')
    expect(wrapper.get('[data-testid="cidade-options"]').text()).toBe('')
    expect(wrapper.get('[data-testid="cidade-placeholder"]').text()).toBe('Selecione primeiro o estado')

    await wrapper.get('[data-testid="selecionar-sc"]').trigger('click')
    await flushPromises()

    expect(wrapper.get('[data-testid="cidade-disabled"]').text()).toBe('false')
    expect(wrapper.get('[data-testid="cidade-options"]').text()).toBe('Blumenau|Florianopolis')
    expect(wrapper.get('[data-testid="cidade-placeholder"]').text()).toBe('Selecione a cidade')
  })
})
