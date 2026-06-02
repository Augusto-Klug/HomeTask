import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { createRouter, createMemoryHistory } from "vue-router";
import DetalhesServicoView from "@/views/servicos/DetalhesServicoView.vue";
import * as apiModule from "@/services/api";
import { useAuthStore } from "@/stores/auth";
import { UnidadeCobranca } from "@/types";

vi.mock("@/services/api", () => ({ default: { get: vi.fn() } }));

const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: "/servicos/buscar", component: { template: "<div>Busca</div>" } },
    { path: "/servicos/detalhes/:id", component: DetalhesServicoView, props: true },
    { path: "/agendamento/novo/:servicoId", name: "agendamento-novo", component: { template: "<div>Agendamento</div>" } },
    { path: "/proposta/nova/:servicoId", name: "proposta-nova", component: { template: "<div>Proposta</div>" } },
  ],
});

describe("FluxosAgendamentoEProposta", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setActivePinia(createPinia());
    useAuthStore().setUser({ userId: "1", nome: "Demo", email: "d@d.com", tipo: 3 });
  });

  it("mostra CTA de agendamento quando o servico e de prestador", async () => {
    vi.mocked(apiModule.default.get)
      .mockResolvedValueOnce({
        data: {
          id: "1",
          titulo: "Faxina",
          descricao: "desc",
          precoBase: 100,
          unidadeCobranca: UnidadeCobranca.Total,
          tipoAnuncio: 1,
          prestadorId: "10",
          prestadorNome: "Maria",
          categoria: 1,
          cidade: "Blumenau",
          estado: "SC",
          mediaAvaliacoes: 4,
        },
      })
      .mockResolvedValueOnce({ data: [] });

    router.push("/servicos/detalhes/1");
    await router.isReady();

    const wrapper = mount(DetalhesServicoView, {
      props: { id: "1" },
      global: {
        plugins: [router],
        stubs: {
          HtButton: { template: "<button @click=\"$emit('click')\"><slot /></button>", emits: ["click"] },
          HtCard: { template: "<div><slot /></div>" },
          HtBadge: { template: "<span><slot /></span>" },
          HtSpinner: true,
          HtDivider: true,
        },
      },
    });

    await flushPromises();
    expect(wrapper.text()).toContain("Agendar agora");
  });
});
