import { mount } from '@vue/test-utils'
import { describe, expect, it, vi } from 'vitest'

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

  it('blocks negative values when allowNegative is false', async () => {
    const wrapper = mount(HtInput, {
      props: {
        modelValue: '',
        type: 'number',
        allowNegative: false,
      },
    })

    await wrapper.get('input').setValue('-15')

    expect(wrapper.emitted('update:modelValue')).toEqual([['0']])
    expect(wrapper.get('input').attributes('min')).toBe('0')
  })

  it('opens the native picker on first focus when configured', async () => {
    const showPicker = vi.fn()

    const wrapper = mount(HtInput, {
      props: {
        modelValue: '',
        type: 'datetime-local',
        openPickerOnFocus: true,
      },
      attachTo: document.body,
    })

    Object.defineProperty(wrapper.get('input').element, 'showPicker', {
      value: showPicker,
      configurable: true,
    })

    await wrapper.get('input').trigger('focus')

    expect(showPicker).toHaveBeenCalledOnce()
  })
})
