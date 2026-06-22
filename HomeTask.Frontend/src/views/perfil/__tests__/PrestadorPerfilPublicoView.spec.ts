import { beforeEach, describe, expect, it, vi } from "vitest"
import { flushPromises, mount } from "@vue/test-utils"
import PrestadorPerfilPublicoView from "../PrestadorPerfilPublicoView.vue"
import * as apiModule from "@/services/api"

vi.mock("@/services/api", () => ({ default: { get: vi.fn() } }))

const mockPerfil = {
  id: "prest-1",
  nome: "Maria Silva",
  descricao: "Profissional com experiencia.",
  cidade: "Blumenau",
  estado: "SC",
  mediaAvaliacoes: 4.7,
  totalAvaliacoes: 12,
  totalServicosConcluidos: 28,
  certificacoes: [
    {
      id: "cert-1",
      prestadorId: "prest-1",
      nome: "Curso de Limpeza Profissional",
      instituicao: "Instituto Casa em Ordem",
      dataEmissao: "2025-01-15T00:00:00Z",
      dataValidade: "2027-01-15T00:00:00Z",
      urlDocumento: "https://example.com/certificado.jpg",
      verificada: true,
      dataCadastro: "2025-01-20T00:00:00Z",
    },
  ],
  portfolios: [
    {
      id: "port-1",
      prestadorId: "prest-1",
      titulo: "Cozinha finalizada",
      descricao: "Organizacao e limpeza completa apos reforma.",
      urlImagem: "https://example.com/portfolio.jpg",
      dataCadastro: "2026-05-10T00:00:00Z",
      ordem: 1,
    },
  ],
  servicosOferecidos: [
    {
      id: "srv-1",
      titulo: "Faxina Residencial Completa",
      descricao: "Limpeza completa",
      precoBase: 80,
      unidadeCobranca: 2,
      tipoAnuncio: 1,
      categoria: 1,
      cidade: "Blumenau",
      estado: "SC",
      totalAvaliacoes: 0,
      mediaAvaliacoes: 4.5,
    },
  ],
  historicoConcluido: [
    {
      agendamentoId: "ag-1",
      servicoPrestadorId: "srv-1",
      tituloServico: "Faxina Residencial Completa",
      dataHoraAgendada: "2026-05-20T12:00:00Z",
      cidade: "Blumenau",
      estado: "SC",
      notaServico: 5,
      notaPrestador: 5,
    },
  ],
}

function mountView() {
  vi.mocked(apiModule.default.get).mockResolvedValue({ data: mockPerfil })

  return mount(PrestadorPerfilPublicoView, {
    props: { id: "prest-1" },
    global: {
      stubs: {
        RouterLink: { template: "<a><slot /></a>" },
        HtCard: { template: "<div><slot /></div>" },
        HtSpinner: true,
        HtCertificacaoDetail: { template: "<div data-testid='certificacao-detail'></div>" },
      },
    },
  })
}

describe("PrestadorPerfilPublicoView", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renderiza certificacoes e portfolio no perfil publico", async () => {
    const wrapper = mountView()
    await flushPromises()

    expect(wrapper.text()).toContain("Certifica")
    expect(wrapper.text()).toContain("Curso de Limpeza Profissional")
    expect(wrapper.text()).toContain("Portf")
    expect(wrapper.text()).toContain("Cozinha finalizada")
  })

  it("abre modal da imagem ao clicar em ver foto", async () => {
    const wrapper = mountView()
    await flushPromises()

    await wrapper.find("button.btn.btn-ghost.btn-xs").trigger("click")

    expect(wrapper.text()).toContain("Abrir imagem")
    expect(wrapper.find("img[alt='Cozinha finalizada']").exists()).toBe(true)
  })
})
