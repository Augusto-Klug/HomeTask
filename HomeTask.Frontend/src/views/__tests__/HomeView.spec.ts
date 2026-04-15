import { describe, expect, it } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'

import HomeView from '../HomeView.vue'

const RouterLinkStub = {
  template: '<a :href="to"><slot /></a>',
  props: ['to'],
}

const HtButtonStub = {
  template: '<button><slot /></button>',
}

function mountView() {
  setActivePinia(createPinia())

  return mount(HomeView, {
    global: {
      stubs: {
        RouterLink: RouterLinkStub,
        HtButton: HtButtonStub,
      },
    },
  })
}

describe('HomeView', () => {
  it('renders the hero with the decorative tools background and adaptive overlay', () => {
    const wrapper = mountView()

    expect(wrapper.text()).toContain('Encontre o profissional ideal para sua casa')

    const hero = wrapper.get('[data-testid="home-hero"]')
    const heroOverlay = wrapper.get('[data-testid="home-hero-overlay"]')
    const heroPanel = wrapper.get('[data-testid="home-hero-panel"]')

    expect(hero.attributes('style')).toContain('background-image')
    expect(hero.attributes('style')).toContain('postura-plana-de-varias-ferramentas-tecnicas-isoladas-no-fundo-branco.png')
    expect(hero.attributes('style')).toContain('background-size: cover;')
    expect(heroOverlay.attributes('class')).toContain('bg-[color-mix(in_oklch,var(--color-base-100)_72%,transparent)]')
    expect(heroPanel.attributes('class')).toContain('bg-[color-mix(in_oklch,var(--color-base-100)_82%,transparent)]')
    expect(heroPanel.attributes('class')).toContain('border-border/60')
    expect(hero.attributes('class')).toContain('min-h-[26rem]')
    expect(hero.attributes('class')).toContain('sm:min-h-[30rem]')
  })

  it('shows platform benefits instead of the categories section', () => {
    const wrapper = mountView()

    expect(wrapper.text()).not.toContain('Categorias')
    expect(wrapper.text()).toContain('Tudo para contratar ou oferecer serviços com mais segurança')
    expect(wrapper.text()).toContain('Pagamento com mais segurança pela plataforma')
    expect(wrapper.text()).toContain('Publique o serviço que você precisa como cliente')
    expect(wrapper.text()).toContain('Anuncie seus serviços e alcance novos clientes')
    expect(wrapper.text()).toContain('Converse pelo chat com clientes e prestadores')
  })
})
