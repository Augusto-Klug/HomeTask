import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import HtDateTimeInput from '../HtDateTimeInput.vue'

describe('HtDateTimeInput', () => {
  it('separa a model unica em data e hora', () => {
    const wrapper = mount(HtDateTimeInput, {
      props: {
        modelValue: '2026-04-20T14:30',
      },
    })

    const [inputData, inputHora] = wrapper.findAll('input')

    expect(inputData).toBeDefined()
    expect(inputHora).toBeDefined()
    expect((inputData!.element as HTMLInputElement).value).toBe('2026-04-20')
    expect((inputHora!.element as HTMLInputElement).value).toBe('14:30')
  })

  it('emite model unica ao preencher data e hora', async () => {
    const wrapper = mount(HtDateTimeInput, {
      props: {
        modelValue: '',
      },
    })

    const [inputData, inputHora] = wrapper.findAll('input')

    expect(inputData).toBeDefined()
    expect(inputHora).toBeDefined()

    await inputData!.setValue('2026-04-20')
    await inputHora!.setValue('14:30')

    const eventos = wrapper.emitted('update:modelValue') ?? []

    expect(eventos[eventos.length - 1]).toEqual(['2026-04-20T14:30'])
  })
})
