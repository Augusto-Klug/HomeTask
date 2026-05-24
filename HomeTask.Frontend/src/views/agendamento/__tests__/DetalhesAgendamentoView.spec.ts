import { flushPromises, mount } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMemoryHistory, createRouter } from "vue-router";
import DetalhesAgendamentoView from "../DetalhesAgendamentoView.vue";
import { useAuthStore } from "@/stores/auth";
import * as apiModule from "@/services/api";

vi.mock("@/services/api", () => ({
  default: { get: vi.fn(), post: vi.fn() },
}));

const agendamentoPropostaCliente = {
  id: "ag-1",
  clienteId: "cli-123",
  clienteNome: "Cliente Demo",
  prestadorId: "prest-999",
  prestadorNome: "Prestador Demo",
  dataHoraAgendada: "2026-06-01T15:00:00",
  duracaoMinutos: 90,
  status: "Solicitado" as const,
  endereco: {
    logradouro: "Rua A",
    bairro: "Centro",
    cidade: "Blumenau",
    estado: "SC",
  },
  observacoes: "Pode ser no período da tarde.",
  valorTotal: 180,
  dataSolicitacao: "2026-05-29T10:00:00",
  dataResposta: null,
  dataConclusao: null,
  motivoRecusa: null,
  aguardandoRespostaDe: "Cliente" as const,
  servicos: [
    {
      id: "srv-cliente-1",
      titulo: "Limpeza Pós-Obra",
      descricao: "",
      precoBase: 180,
      duracaoEstimadaMinutos: 90,
      unidadeCobranca: "total",
      tipoAnuncio: 2,
      categoria: 1,
    },
  ],
};

function criarRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      {
        path: "/agendamento/detalhes/:id",
        name: "agendamento-detalhes",
        component: DetalhesAgendamentoView,
      },
    ],
  });
}

describe("DetalhesAgendamentoView", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setActivePinia(createPinia());
  });

  it("exibe aceitar proposta para o cliente dono do pedido", async () => {
    const auth = useAuthStore();
    auth.setUser({
      userId: 101,
      nome: "Cliente Demo",
      email: "cliente@teste.com",
      tipo: 1,
    });

    vi.mocked(apiModule.default.get)
      .mockResolvedValueOnce({ data: { id: "cli-123" } })
      .mockResolvedValueOnce({ data: agendamentoPropostaCliente });

    const router = criarRouter();
    router.push("/agendamento/detalhes/ag-1");
    await router.isReady();

    const wrapper = mount(DetalhesAgendamentoView, {
      global: { plugins: [router] },
    });

    await flushPromises();

    expect(wrapper.text()).toContain("Aceitar Proposta");
    expect(wrapper.text()).not.toContain("Aceitar Agendamento");
  });
});
