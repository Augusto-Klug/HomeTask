import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import OperacaoPrestadorView from "../OperacaoPrestadorView.vue";
import * as apiModule from "@/services/api";
import { UnidadeCobranca, type AgendamentoResumo } from "@/types";

vi.mock("@/services/api", () => ({ default: { get: vi.fn() } }));

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: "/", component: { template: "<div />" } }],
});

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
  aguardandoRespostaDe: "Prestador",
  servicos: [{ id: "srv-1", titulo: "Faxina", descricao: "", precoBase: 120, duracaoEstimadaMinutos: 90, unidadeCobranca: UnidadeCobranca.Total, tipoAnuncio: 1, categoria: 1 }],
  ...overrides,
});

describe("OperacaoPrestadorView", () => {
  beforeEach(() => vi.clearAllMocks());

  it("agrupa solicitado do prestador em agendados", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({
      data: [makeAgendamento()],
    });

    const wrapper = mount(OperacaoPrestadorView, {
      global: {
        plugins: [router],
        stubs: {
          HtSpinner: true,
          HtCard: { template: "<div><slot /></div>" },
          HtBadge: { template: "<span><slot /></span>", props: ["variant"] },
        },
      },
    });

    await flushPromises();
    expect(wrapper.text()).toContain("Aguardando sua resposta");
  });
});
