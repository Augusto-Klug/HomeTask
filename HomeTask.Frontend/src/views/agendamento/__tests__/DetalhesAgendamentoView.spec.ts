import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { createRouter, createMemoryHistory } from "vue-router";
import DetalhesAgendamentoView from "../DetalhesAgendamentoView.vue";
import { useAuthStore } from "@/stores/auth";
import * as apiModule from "@/services/api";
import { TipoUsuario, UnidadeCobranca, type AgendamentoResumo } from "@/types";

vi.mock("@/services/api", () => ({ default: { get: vi.fn(), post: vi.fn() } }));

const makeAgendamento = (overrides: Partial<AgendamentoResumo> = {}): AgendamentoResumo => ({
  id: "ag-1",
  clienteId: "cli-1",
  clienteNome: "Maria Cliente",
  prestadorId: "prest-1",
  prestadorNome: "Prestador Demo",
  dataHoraAgendada: new Date(Date.now() + 86400000).toISOString(),
  duracaoMinutos: 90,
  status: "Solicitado",
  endereco: { logradouro: "Rua 1", bairro: "Centro", cidade: "Blumenau", estado: "SC" },
  observacoes: null,
  valorTotal: 120,
  dataSolicitacao: new Date().toISOString(),
  dataResposta: null,
  dataConclusao: null,
  motivoRecusa: null,
  aguardandoRespostaDe: TipoUsuario.Cliente,
  servicos: [{ id: "srv-1", titulo: "Faxina", descricao: "", precoBase: 120, duracaoEstimadaMinutos: 90, unidadeCobranca: UnidadeCobranca.Total, tipoAnuncio: 2, categoria: 1 }],
  ...overrides,
});

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: "/agendamento/detalhes/:id", component: DetalhesAgendamentoView }],
});

describe("DetalhesAgendamentoView", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setActivePinia(createPinia());
    const auth = useAuthStore();
    auth.setUser({ userId: "1", nome: "Cliente", email: "c@c.com", tipo: 1 });
  });

  it("mostra aceitar proposta para o cliente quando aguardando resposta dele", async () => {
    vi.mocked(apiModule.default.get)
      .mockResolvedValueOnce({ data: { id: "cli-1" } })
      .mockRejectedValueOnce(new Error("sem prestador"))
      .mockResolvedValueOnce({ data: makeAgendamento() });

    router.push("/agendamento/detalhes/ag-1");
    await router.isReady();

    const wrapper = mount(DetalhesAgendamentoView, {
      global: {
        plugins: [router],
        stubs: {
          HtSpinner: true,
          HtCard: { template: "<div><slot /></div>" },
          HtBadge: { template: "<span><slot /></span>", props: ["variant"] },
          HtButton: { template: "<button><slot /></button>", props: ["loading", "variant"] },
          HtTextarea: { template: "<textarea />", props: ["modelValue"] },
        },
      },
    });

    await flushPromises();
    expect(wrapper.text()).toContain("Aceitar Proposta");
  });
});
