import { ref, computed } from 'vue'

type ThemeMode = 'auto' | 'light' | 'dark'

const STORAGE_KEY = 'ht_theme_mode'
const CYCLE: ThemeMode[] = ['auto', 'light', 'dark']

const mode = ref<ThemeMode>(
  (localStorage.getItem(STORAGE_KEY) as ThemeMode) ?? 'auto',
)

function applyMode(m: ThemeMode) {
  mode.value = m
  localStorage.setItem(STORAGE_KEY, m)

  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
  const isDark = m === 'dark' || (m === 'auto' && prefersDark)

  // DaisyUI usa data-theme no elemento <html>
  document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light')
}

function cycleMode() {
  const next = CYCLE[(CYCLE.indexOf(mode.value) + 1) % CYCLE.length] as ThemeMode
  applyMode(next)
}

const modeIcon = computed(() => {
  if (mode.value === 'light') return 'light_mode'
  if (mode.value === 'dark') return 'dark_mode'
  return 'brightness_auto'
})

const modeLabel = computed(() => {
  if (mode.value === 'light') return 'Claro'
  if (mode.value === 'dark') return 'Escuro'
  return 'Sistema'
})

export function useTheme() {
  return { mode, modeIcon, modeLabel, applyMode, cycleMode }
}

export function initTheme() {
  applyMode(mode.value)

  // Reage a mudanças do sistema quando em modo auto
  window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
    if (mode.value === 'auto') applyMode('auto')
  })
}
