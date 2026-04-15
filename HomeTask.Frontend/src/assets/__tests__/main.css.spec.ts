import { readFileSync } from 'node:fs'
import { resolve } from 'node:path'

import { describe, expect, it } from 'vitest'

describe('main.css typography scale', () => {
  it('increases the global font sizes by 2px', () => {
    const css = readFileSync(resolve(__dirname, '../main.css'), 'utf-8')

    expect(css).toContain('--text-xs:    14px;')
    expect(css).toContain('--text-sm:    15px;')
    expect(css).toContain('--text-base:  16px;')
    expect(css).toContain('--text-title: 18px;')
    expect(css).toContain('--text-lg:    20px;')
    expect(css).toContain('--text-xl:    22px;')
    expect(css).toContain('--text-2xl:   26px;')
    expect(css).toContain('--text-3xl:   32px;')
    expect(css).toMatch(/html\s*\{\s*font-size:\s*16px;/)
    expect(css).toMatch(/body\s*\{\s*font-family:\s*'Inter', 'Roboto', system-ui, -apple-system, sans-serif;\s*font-size:\s*16px;/)
  })
})
