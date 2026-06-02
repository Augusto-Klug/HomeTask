import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { createRouter, createMemoryHistory } from "vue-router";
import MeusAgendamentosView from "../MeusAgendamentosView.vue";
import { useAuthStore } from "@/stores/auth";
import * as apiModule from "@/services/api";
import { TipoUsuario, UnidadeCobranca, type AgendamentoResumo } from "@/types";

vi.mock("@/services/api", () => ({ default: { get: vi.fn() } }));

const makeAgendamento = (overrides: Partial<AgendamentoResumo> = {}): AgendamentoResumo => ({
  id: "1",
  clienteId: "10",
  clienteNome: "Joao Silva",
  prestadorId: "20",
  prestadorNome: "Maria Costa",
  dataHoraAgendada: new Date(Date.now() + 86400000).toISOString(),
  duracaoMinutos: 60,
  status: "Solicitado",
  endereco: { logradouro: "Rua A, 100", bairro: "Centro", cidade: "Florianopolis", estado: "SC" },
  observacoes: null,
  valorTotal: 150,
  dataSolicitacao: new Date().toISOString(),
  dataResposta: null,
  dataConclusao: null,
  motivoRecusa: null,
  aguardandoRespostaDe: TipoUsuario.Cliente,
  servicos: [
    {
      id: "srv-1",
      titulo: "Faxina",
      descricao: "Limpeza residencial",
      precoBase: 80,
      duracaoEstimadaMinutos: 60,
      unidadeCobranca: UnidadeCobranca.Total,
      tipoAnuncio: 2,
      categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
    },
  ],
  ...overrides,
});

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: "/", component: { template: "<div />" } }],
});

async function mountView() {
  setActivePinia(createPinia());
  const auth = useAuthStore();
  auth.setUser({ userId: "10", nome: "Teste", email: "a@b.com", tipo: 1 });

  await router.push("/");
  await router.isReady();

  return mount(MeusAgendamentosView, {
    global: {
      plugins: [router],
      stubs: {
        HtSpinner: true,
        HtCard: { template: "<div><slot /></div>" },
        HtBadge: { template: "<span><slot /></span>", props: ["variant"] },
      },
    },
  });
}

describe("MeusAgendamentosView", () => {
  beforeEach(() => vi.clearAllMocks());

  it("exibe a secao de aguardando confirmacao", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] });
    const wrapper = await mountView();
    await flushPromises();

    expect(wrapper.text()).toContain("Aguardando sua confirmacao");
    expect(wrapper.text()).not.toContain("Cliente");
  });

  it("mostra aprovacao do cliente quando a pendencia e dele", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({ data: [makeAgendamento()] });
    const wrapper = await mountView();
    await flushPromises();

    expect(wrapper.text()).toContain("Aguardando sua confirmacao");
  });
});
