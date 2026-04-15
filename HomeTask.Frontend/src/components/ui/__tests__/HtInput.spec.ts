import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import HtInput from '../HtInput.vue'

describe('HtInput', () => {
  it('keeps the text readable when disabled', () => {
    const wrapper = mount(HtInput, {
      props: {
        modelValue: 'Thiago',
        disabled: true,
      },
    })

    const field = wrapper.get('label.input')
    const input = wrapper.get('input')

    expect(field.classes()).not.toContain('opacity-50')
    expect(field.classes()).toContain('bg-base-200/60')
    expect(field.classes()).toContain('border-base-300/70')
    expect(input.classes()).toContain('disabled:text-base-content')
    expect(input.classes()).toContain('disabled:opacity-100')
  })
})
