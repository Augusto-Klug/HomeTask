import { beforeEach, describe, expect, it, vi } from "vitest"
import { flushPromises, mount } from "@vue/test-utils"
import { createMemoryHistory, createRouter } from "vue-router"
import { createPinia, setActivePinia } from "pinia"
import DetalhesAgendamentoView from "../DetalhesAgendamentoView.vue"
import { useAuthStore } from "@/stores/auth"
import * as apiModule from "@/services/api"
import { StatusAgendamento, StatusPagamento, TipoUsuario, UnidadeCobranca, type AgendamentoResumo } from "@/types"

vi.mock("@/services/api", () => ({ default: { get: vi.fn(), post: vi.fn() }, API_BASE_URL: "http://localhost:5000" }))

const makeAgendamento = (overrides: Partial<AgendamentoResumo> = {}): AgendamentoResumo => ({
  id: "ag-1",
  clienteId: "cli-1",
  clienteNome: "Maria Cliente",
  prestadorId: "prest-1",
  prestadorNome: "Prestador Demo",
  dataHoraAgendada: new Date(Date.now() + 86400000).toISOString(),
  duracaoMinutos: 90,
  status: StatusAgendamento.Concluido,
  endereco: { logradouro: "Rua 1", bairro: "Centro", cidade: "Blumenau", estado: "SC" },
  observacoes: null,
  valorTotal: 120,
  dataSolicitacao: new Date().toISOString(),
  dataResposta: null,
  dataConclusao: new Date().toISOString(),
  motivoRecusa: null,
  aguardandoRespostaDe: TipoUsuario.Cliente,
  podeClienteAvaliarPrestador: true,
  clienteJaAvaliouPrestador: false,
  podePrestadorAvaliarCliente: true,
  prestadorJaAvaliouCliente: false,
  servicos: [
    {
      id: "srv-1",
      titulo: "Faxina",
      descricao: "",
      precoBase: 120,
      duracaoEstimadaMinutos: 90,
      unidadeCobranca: UnidadeCobranca.Total,
      tipoAnuncio: 1,
      categoria: 1,
    },
  ],
  ...overrides,
})

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: "/agendamento/detalhes/:id", component: DetalhesAgendamentoView }],
})

describe("DetalhesAgendamentoView", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    setActivePinia(createPinia())
    const auth = useAuthStore()
    auth.setUser({ userId: "1", nome: "Cliente", email: "c@c.com", tipo: 1 })
  })

  it("abre a avaliacao do cliente automaticamente quando o pagamento foi aprovado e o agendamento foi concluido", async () => {
    vi.mocked(apiModule.default.get)
      .mockResolvedValueOnce({ data: { id: "cli-1" } })
      .mockRejectedValueOnce(new Error("sem prestador"))
      .mockResolvedValueOnce({ data: makeAgendamento() })
      .mockResolvedValueOnce({
        data: {
          id: "pag-1",
          agendamentoId: "ag-1",
          valor: 120,
          status: StatusPagamento.Aprovado,
          dataCriacao: new Date().toISOString(),
        },
      })
      .mockRejectedValueOnce(new Error("sem avaliacao"))
      .mockRejectedValueOnce(new Error("sem avaliacao cliente"))

    router.push("/agendamento/detalhes/ag-1")
    await router.isReady()

    const wrapper = mount(DetalhesAgendamentoView, {
      global: {
        plugins: [router],
        stubs: {
          HtSpinner: true,
          HtAlert: { template: "<div><slot /></div>", props: ["variant", "title"] },
          HtCard: { template: "<div><slot /></div>" },
          HtBadge: { template: "<span><slot /></span>", props: ["variant"] },
          HtButton: { template: "<button><slot /></button>", props: ["loading", "variant"] },
          HtTextarea: { template: "<textarea />", props: ["modelValue"] },
          AgendamentoChat: true,
        },
      },
    })

    await flushPromises()

    expect(wrapper.text()).toContain("Avaliar atendimento")
    expect(wrapper.text()).toContain("Avalie o serviço")
    expect(wrapper.text()).toContain("Avalie o prestador")
  })

  it("abre a avaliacao do prestador automaticamente quando o usuario logado e o prestador", async () => {
    const auth = useAuthStore()
    auth.setUser({ userId: "2", nome: "Prestador", email: "p@p.com", tipo: 2 })

    vi.mocked(apiModule.default.get)
      .mockRejectedValueOnce(new Error("sem cliente"))
      .mockResolvedValueOnce({ data: { id: "prest-1" } })
      .mockResolvedValueOnce({ data: makeAgendamento() })
      .mockResolvedValueOnce({
        data: {
          id: "pag-1",
          agendamentoId: "ag-1",
          valor: 120,
          status: StatusPagamento.Aprovado,
          dataCriacao: new Date().toISOString(),
        },
      })
      .mockRejectedValueOnce(new Error("sem avaliacao"))
      .mockRejectedValueOnce(new Error("sem avaliacao cliente"))

    router.push("/agendamento/detalhes/ag-1")
    await router.isReady()

    const wrapper = mount(DetalhesAgendamentoView, {
      global: {
        plugins: [router],
        stubs: {
          HtSpinner: true,
          HtAlert: { template: "<div><slot /></div>", props: ["variant", "title"] },
          HtCard: { template: "<div><slot /></div>" },
          HtBadge: { template: "<span><slot /></span>", props: ["variant"] },
          HtButton: { template: "<button><slot /></button>", props: ["loading", "variant"] },
          HtTextarea: { template: "<textarea />", props: ["modelValue"] },
        },
      },
    })

    await flushPromises()

    expect(wrapper.text()).toContain("Avaliar cliente")
    expect(wrapper.text()).toContain("Avalie o cliente")
  })
})
