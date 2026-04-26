import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { createRouter, createMemoryHistory } from "vue-router";
import SolicitacoesPendentesView from "../SolicitacoesPendentesView.vue";
import { useAuthStore } from "@/stores/auth";
import * as apiModule from "@/services/api";
import { UnidadeCobranca, type AgendamentoResumo } from "@/types";

vi.mock("@/services/api", () => ({ default: { get: vi.fn(), post: vi.fn() } }));

const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: "/", component: { template: "<div />" } },
    { path: "/perfil/agendamentos", component: { template: "<div />" } },
    { path: "/agendamento/detalhes/:id", component: { template: "<div />" } },
  ],
});

const makeSolicitacao = (
  overrides: Partial<AgendamentoResumo> = {},
): AgendamentoResumo => ({
  id: "ag-1",
  clienteId: "cli-1",
  clienteNome: "Maria Cliente",
  prestadorId: "prest-1",
  prestadorNome: "Usuário Demo",
  dataHoraAgendada: new Date(Date.now() + 86400000).toISOString(),
  duracaoMinutos: 90,
  status: "Solicitado",
  endereco: {
    logradouro: "Rua 1, 10",
    bairro: "Centro",
    cidade: "Blumenau",
    estado: "SC",
  },
  observacoes: null,
  valorTotal: 120,
  dataSolicitacao: new Date().toISOString(),
  dataResposta: null,
  dataConclusao: null,
  motivoRecusa: null,
  servicos: [{ id: "srv-1", titulo: "Faxina", descricao: "", precoBase: 120, duracaoEstimadaMinutos: 90, unidadeCobranca: UnidadeCobranca.Total, tipoAnuncio: 1, categoria: 1 }],
  ...overrides,
});

function mountView() {
  setActivePinia(createPinia());
  const auth = useAuthStore();
  auth.setUser({
    userId: 1,
    nome: "Prestador",
    email: "prestador@teste.com",
    tipo: 2,
  });

  return mount(SolicitacoesPendentesView, {
    global: {
      plugins: [router],
      stubs: {
        HtSpinner: true,
        HtAlert: { template: "<div><slot /></div>" },
        HtBadge: { template: "<span><slot /></span>" },
        HtCard: { template: "<div><slot /></div>" },
        HtTextarea: {
          props: ["modelValue"],
          emits: ["update:modelValue"],
          template:
            '<textarea :value="modelValue" @input="$emit(\'update:modelValue\', $event.target.value)" />',
        },
        HtButton: {
          props: ["loading", "disabled"],
          emits: ["click"],
          template:
            '<button :disabled="loading || disabled" @click="$emit(\'click\')"><slot /></button>',
        },
      },
    },
  });
}

describe("SolicitacoesPendentesView", () => {
  beforeEach(() => vi.clearAllMocks());

  it("lista apenas solicitações com status Solicitado", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({
      data: [makeSolicitacao({ id: "ag-1", clienteNome: "Maria Cliente" })],
    });

    const wrapper = mountView();
    await flushPromises();

    expect(apiModule.default.get).toHaveBeenCalledWith(
      "/api/Agendamento/ObterSolicitacoesPendentesPrestador",
    );
    expect(wrapper.text()).toContain("Maria Cliente");
  });

  it("aceita solicitação e remove da lista", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeSolicitacao()] });
    vi.mocked(apiModule.default.post).mockResolvedValue({ data: {} });

    const wrapper = mountView();
    await flushPromises();

    const botaoAceitar = wrapper
      .findAll("button")
      .find((b) => b.text().includes("Aceitar"));

    expect(botaoAceitar).toBeTruthy();
    await botaoAceitar!.trigger("click");
    await flushPromises();

    expect(apiModule.default.post).toHaveBeenCalledWith(
      "/api/Agendamento/AceitarAgendamento",
      { id: "ag-1" },
    );
    expect(wrapper.text()).not.toContain("Maria Cliente");
  });
});
