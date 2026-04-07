/** Utilitários de sidenav — controle via ref/emit, sem dependência de BeerCSS */
export default {
  fecharSidenav(setter: (v: boolean) => void) {
    setter(false)
  },
  abrirSidenav(setter: (v: boolean) => void) {
    setter(true)
  },
  toggleSidenav(setter: (v: boolean) => void, current: boolean) {
    setter(!current)
  },
}
