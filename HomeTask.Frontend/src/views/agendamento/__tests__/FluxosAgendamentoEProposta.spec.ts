import { flushPromises, mount } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMemoryHistory, createRouter } from "vue-router";
import NovaPropostaView from "../NovaPropostaView.vue";
import NovoAgendamentoView from "../NovoAgendamentoView.vue";
import DetalhesServicoView from "@/views/servicos/DetalhesServicoView.vue";
import * as apiModule from "@/services/api";
import { useAuthStore } from "@/stores/auth";

vi.mock("@/services/api", () => ({
  default: { get: vi.fn(), post: vi.fn() },
}));

const servicoPrestador = {
  id: "srv-prestador-1",
  titulo: "Faxina Residencial",
  descricao: "Faxina completa.",
  precoBase: 120,
  unidadeCobranca: "por_hora",
  categoria: 1,
  ativo: true,
  cidade: "Blumenau",
  estado: "SC",
  prestadorId: "prest-123",
  prestadorNome: "Maria Silva",
  duracaoEstimadaMinutos: 120,
  mediaAvaliacoes: 4.7,
  tipoAnuncio: 1 as const,
};

const servicoCliente = {
  id: "srv-cliente-1",
  titulo: "Limpeza Pós-Obra",
  descricao: "Apartamento após reforma.",
  precoBase: 180,
  unidadeCobranca: "total",
  categoria: 1,
  ativo: true,
  cidade: "Blumenau",
  estado: "SC",
  clienteId: "cli-123",
  clienteNome: "Pedro Martins",
  dataDesejada: "2026-05-30T14:00:00",
  tipoAnuncio: 2 as const,
};

function criarRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      {
        path: "/servicos/detalhes/:id",
        name: "servicos-detalhes",
        component: DetalhesServicoView,
        props: true,
      },
      {
        path: "/agendamento/novo/:servicoId",
        name: "agendamento-novo",
        component: NovoAgendamentoView,
        props: true,
      },
      {
        path: "/proposta/nova/:servicoId",
        name: "proposta-nova",
        component: NovaPropostaView,
        props: true,
      },
      {
        path: "/agendamento/sucesso",
        name: "agendamento-sucesso",
        component: { template: "<div>sucesso</div>" },
      },
    ],
  });
}

function configurarAuth() {
  setActivePinia(createPinia());
  const auth = useAuthStore();
  auth.setUser({
    userId: 99,
    nome: "Prestador Demo",
    email: "demo@teste.com",
    tipo: 3,
  });
}

describe("Fluxos de agendamento e proposta", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    configurarAuth();
  });

  it("redireciona detalhe de serviço de prestador para agendamento", async () => {
    const router = criarRouter();
    vi.mocked(apiModule.default.get)
      .mockResolvedValueOnce({ data: servicoPrestador })
      .mockResolvedValueOnce({ data: [] });

    router.push("/servicos/detalhes/srv-prestador-1");
    await router.isReady();

    const wrapper = mount(DetalhesServicoView, {
      props: { id: "srv-prestador-1" },
      global: { plugins: [router] },
    });

    await flushPromises();
    expect(wrapper.text()).toContain("Agendar agora");

    await wrapper.get("button").trigger("click");
    await flushPromises();

    expect(router.currentRoute.value.name).toBe("agendamento-novo");
    expect(router.currentRoute.value.params.servicoId).toBe("srv-prestador-1");
  });

  it("redireciona detalhe de serviço de cliente para proposta", async () => {
    const router = criarRouter();
    vi.mocked(apiModule.default.get).mockResolvedValueOnce({ data: servicoCliente });

    router.push("/servicos/detalhes/srv-cliente-1");
    await router.isReady();

    const wrapper = mount(DetalhesServicoView, {
      props: { id: "srv-cliente-1" },
      global: { plugins: [router] },
    });

    await flushPromises();
    expect(wrapper.text()).toContain("Enviar proposta");

    await wrapper.get("button").trigger("click");
    await flushPromises();

    expect(router.currentRoute.value.name).toBe("proposta-nova");
    expect(router.currentRoute.value.params.servicoId).toBe("srv-cliente-1");
  });

  it("cria agendamento usando o prestadorId do serviço ofertado", async () => {
    const router = criarRouter();
    vi.mocked(apiModule.default.get)
      .mockResolvedValueOnce({ data: servicoPrestador })
      .mockResolvedValueOnce({ data: { id: "cli-999" } });
    vi.mocked(apiModule.default.post).mockResolvedValue({ data: { id: "ag-1" } });

    router.push("/agendamento/novo/srv-prestador-1");
    await router.isReady();

    const wrapper = mount(NovoAgendamentoView, {
      props: { servicoId: "srv-prestador-1" },
      global: { plugins: [router] },
    });

    await flushPromises();
    await wrapper.get('input[type="date"]').setValue("2026-06-01");
    await wrapper.get('input[type="time"]').setValue("09:30");
    await wrapper.get('input[placeholder="Rua, número, bairro..."]').setValue(
      "Rua XV, 100",
    );
    await wrapper.get("form").trigger("submit.prevent");
    await flushPromises();

    expect(apiModule.default.post).toHaveBeenCalledWith(
      "/api/Agendamento/CriarAgendamento",
      expect.objectContaining({
        clienteId: "cli-999",
        prestadorId: "prest-123",
        servicosOferecidosIds: ["srv-prestador-1"],
        dataHoraAgendada: "2026-06-01T09:30:00",
      }),
    );
    expect(router.currentRoute.value.name).toBe("agendamento-sucesso");
  });

  it("cria proposta usando o prestador autenticado e o serviço do cliente", async () => {
    const router = criarRouter();
    vi.mocked(apiModule.default.get)
      .mockResolvedValueOnce({ data: servicoCliente })
      .mockResolvedValueOnce({ data: { id: "prest-999" } });
    vi.mocked(apiModule.default.post).mockResolvedValue({ data: { id: "ag-2" } });

    router.push("/proposta/nova/srv-cliente-1");
    await router.isReady();

    const wrapper = mount(NovaPropostaView, {
      props: { servicoId: "srv-cliente-1" },
      global: { plugins: [router] },
    });

    await flushPromises();
    await wrapper.get('input[type="date"]').setValue("2026-06-02");
    await wrapper.get('input[type="time"]').setValue("15:00");
    await wrapper.get("textarea").setValue("Posso executar no período da tarde.");
    await wrapper.get("form").trigger("submit.prevent");
    await flushPromises();

    expect(apiModule.default.post).toHaveBeenCalledWith(
      "/api/Agendamento/CriarAgendamento",
      expect.objectContaining({
        prestadorId: "prest-999",
        servicosOferecidosIds: ["srv-cliente-1"],
        dataHoraAgendada: "2026-06-02T15:00:00",
        observacoes: "Posso executar no período da tarde.",
      }),
    );
    expect(
      vi.mocked(apiModule.default.post).mock.calls[0]?.[1],
    ).not.toHaveProperty("clienteId");
    expect(router.currentRoute.value.name).toBe("agendamento-sucesso");
  });
});
