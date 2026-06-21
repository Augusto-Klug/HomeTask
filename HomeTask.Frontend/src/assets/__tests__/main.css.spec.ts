import { readFileSync } from 'node:fs'
import { resolve } from 'node:path'

import { describe, expect, it } from 'vitest'

describe('main.css typography scale', () => {
  it('increases the global font sizes by 2px', () => {
    const css = readFileSync(resolve(__dirname, '../main.css'), 'utf-8')

    expect(css).toMatch(/--text-xs:\s*14px;/)
    expect(css).toMatch(/--text-sm:\s*15px;/)
    expect(css).toMatch(/--text-base:\s*16px;/)
    expect(css).toMatch(/--text-title:\s*18px;/)
    expect(css).toMatch(/--text-lg:\s*20px;/)
    expect(css).toMatch(/--text-xl:\s*22px;/)
    expect(css).toMatch(/--text-2xl:\s*26px;/)
    expect(css).toMatch(/--text-3xl:\s*32px;/)
    expect(css).toMatch(/html\s*\{\s*font-size:\s*16px;/)
    expect(css).toMatch(/body\s*\{[\s\S]*font-family:[\s\S]*["']Inter["'],[\s\S]*["']Roboto["'],[\s\S]*system-ui,[\s\S]*-apple-system,[\s\S]*sans-serif;[\s\S]*font-size:\s*16px;/)
  })
})
