import { ref, computed } from "vue";
import { ui } from "beercss";

type ThemeMode = "auto" | "light" | "dark";

const STORAGE_KEY = "ht_theme_mode";
const CYCLE: ThemeMode[] = ["auto", "light", "dark"];

const mode = ref<ThemeMode>(
  (localStorage.getItem(STORAGE_KEY) as ThemeMode) ?? "auto",
);

function applyMode(m: ThemeMode) {
  mode.value = m;
  localStorage.setItem(STORAGE_KEY, m);
  ui("mode", m);
}

function cycleMode() {
  const next = CYCLE[
    (CYCLE.indexOf(mode.value) + 1) % CYCLE.length
  ] as ThemeMode;
  applyMode(next);
}

const modeIcon = computed(() => {
  if (mode.value === "light") return "light_mode";
  if (mode.value === "dark") return "dark_mode";
  return "brightness_auto";
});

const modeLabel = computed(() => {
  if (mode.value === "light") return "Claro";
  if (mode.value === "dark") return "Escuro";
  return "Sistema";
});

export function useTheme() {
  return { mode, modeIcon, modeLabel, applyMode, cycleMode };
}

/** Aplica o modo (auto/light/dark) salvo. Chamar no main.ts. */
export function initTheme() {
  applyMode(mode.value);
}
