import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import OperacaoPrestadorView from "../OperacaoPrestadorView.vue";
import * as apiModule from "@/services/api";

vi.mock("@/services/api", () => ({ default: { get: vi.fn() } }));

const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: "/", component: { template: "<div />" } },
    { path: "/perfil/solicitacoes-pendentes", component: { template: "<div />" } },
    { path: "/agendamento/detalhes/:id", component: { template: "<div />" } },
  ],
});

function makeAgendamento(status: string) {
  return {
    id: `${status}-1`,
    clienteId: "cli-1",
    clienteNome: "Cliente Demo",
    prestadorId: "prest-1",
    prestadorNome: "Prestador Demo",
    dataHoraAgendada: new Date(Date.now() + 86400000).toISOString(),
    duracaoMinutos: 90,
    status,
    endereco: {
      logradouro: "Rua 1",
      bairro: "Centro",
      cidade: "Blumenau",
      estado: "SC",
    },
    observacoes: null,
    valorTotal: 180,
    dataSolicitacao: new Date().toISOString(),
    dataResposta: null,
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: null,
    servicos: [
      {
        id: "srv-1",
        titulo: "Limpeza",
        descricao: "",
        precoBase: 180,
        duracaoEstimadaMinutos: 90,
        unidadeCobranca: "total",
        tipoAnuncio: 2,
        categoria: 1,
      },
    ],
  };
}

describe("OperacaoPrestadorView", () => {
  beforeEach(() => vi.clearAllMocks());

  it("separa os serviços por etapa operacional", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({
      data: [
        { ...makeAgendamento("Solicitado"), aguardandoRespostaDe: "Prestador" },
        makeAgendamento("Aceito"),
        makeAgendamento("EmAndamento"),
        makeAgendamento("Concluido"),
      ],
    });

    const wrapper = mount(OperacaoPrestadorView, {
      global: {
        plugins: [router],
        stubs: {
          HtSpinner: true,
          HtCard: { template: "<div><slot /></div>" },
          HtBadge: { template: "<span><slot /></span>" },
          HtButton: { template: "<button><slot /></button>" },
        },
      },
    });

    await flushPromises();

    expect(wrapper.text()).toContain("Agendados");
    expect(wrapper.text()).toContain("Em andamento");
    expect(wrapper.text()).toContain("Concluídos");
    expect(wrapper.text()).toContain("Aguardando sua resposta");
    expect(wrapper.text()).toContain("Pronto para iniciar");
    expect(wrapper.text()).toContain("Concluído");
  });
});
