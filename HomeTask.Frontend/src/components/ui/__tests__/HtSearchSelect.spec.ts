import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import HtSearchSelect from '../HtSearchSelect.vue'

const ufOptions = [
  { value: 'SC', label: 'Santa Catarina' },
  { value: 'SP', label: 'Sao Paulo' },
  { value: 'PR', label: 'Parana' },
]

describe('HtSearchSelect', () => {
  it('filters options by label and only accepts a listed option', async () => {
    const wrapper = mount(HtSearchSelect, {
      props: {
        modelValue: '',
        options: ufOptions,
        label: 'UF',
        required: true,
      },
    })

    await wrapper.get('input').setValue('Santa')

    expect(wrapper.text()).toContain('Santa Catarina')
    expect(wrapper.text()).not.toContain('Parana')

    await wrapper.get('[data-testid="ht-search-select-option-SC"]').trigger('mousedown')

    expect(wrapper.emitted('update:modelValue')).toEqual([['SC']])
  })

  it('clears a custom value on blur and reports validation error', async () => {
    const wrapper = mount(HtSearchSelect, {
      props: {
        modelValue: '',
        options: ufOptions,
        label: 'UF',
        required: true,
      },
    })

    await wrapper.get('input').setValue('Estado inexistente')
    await wrapper.get('input').trigger('blur')

    expect(wrapper.emitted('update:modelValue')).toEqual([['']])
    expect(wrapper.text()).toContain('Campo obrigatório')
  })
})
