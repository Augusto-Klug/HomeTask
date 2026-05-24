import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { createRouter, createMemoryHistory } from "vue-router";
import MeusAgendamentosView from "../MeusAgendamentosView.vue";
import * as apiModule from "@/services/api";
import type { AgendamentoResumo } from "@/types";

vi.mock("@/services/api", () => ({ default: { get: vi.fn() } }));

const makeAgendamento = (overrides: Partial<AgendamentoResumo> = {}): AgendamentoResumo => ({
  id: "1",
  clienteId: "10",
  clienteNome: "João Silva",
  prestadorId: "20",
  prestadorNome: "Maria Costa",
  dataHoraAgendada: new Date(Date.now() + 86400000).toISOString(),
  duracaoMinutos: 60,
  status: "Solicitado",
  endereco: { logradouro: "Rua A, 100", bairro: "Centro", cidade: "Florianópolis", estado: "SC" },
  observacoes: null,
  valorTotal: 150,
  dataSolicitacao: new Date().toISOString(),
  dataResposta: null,
  dataConclusao: null,
  motivoRecusa: null,
  aguardandoRespostaDe: "Prestador",
  servicos: [
    {
      id: "srv-1",
      titulo: "Faxina",
      descricao: "Limpeza residencial",
      precoBase: 80,
      duracaoEstimadaMinutos: 60,
      unidadeCobranca: "total",
      tipoAnuncio: 1,
      categoria: 1,
    },
  ],
  ...overrides,
});

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: "/", component: { template: "<div />" } }],
});

function mountView() {
  setActivePinia(createPinia());

  return mount(MeusAgendamentosView, {
    global: {
      plugins: [router],
      stubs: {
        HtSpinner: true,
        HtCard: { template: "<div><slot /></div>" },
        HtBadge: { template: "<span><slot /></span>" },
      },
    },
  });
}

describe("MeusAgendamentosView", () => {
  beforeEach(() => vi.clearAllMocks());

  it("exibe a visão de cliente", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] });
    const wrapper = mountView();
    await flushPromises();
    expect(wrapper.text()).toContain("Cliente");
    expect(wrapper.text()).not.toContain("Prestador");
  });

  it("não exibe agendamentos passados", async () => {
    const passado = makeAgendamento({
      dataHoraAgendada: new Date(Date.now() - 86400000).toISOString(),
    });
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [passado] });
    const wrapper = mountView();
    await flushPromises();
    expect(wrapper.text()).toContain("Nenhum agendamento");
  });

  it("exibe nome do prestador no card do cliente", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] });
    const wrapper = mountView();
    await flushPromises();
    expect(wrapper.text()).toContain("Maria Costa");
  });

  it("mostra quando a proposta aguarda aprovação do cliente", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({
      data: [makeAgendamento({ aguardandoRespostaDe: "Cliente" })],
    });
    const wrapper = mountView();
    await flushPromises();
    expect(wrapper.text()).toContain("Aguardando sua aprovação");
  });
});
