import { flushPromises, mount } from "@vue/test-utils";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMemoryHistory, createRouter } from "vue-router";
import BuscarServicosView from "../BuscarServicosView.vue";
import * as apiModule from "@/services/api";

vi.mock("@/services/api", () => ({
  default: { get: vi.fn() },
}));

function criarRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [{ path: "/servicos/buscar", component: BuscarServicosView }],
  });
}

describe("BuscarServicosView consumo", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renderiza precoBase retornado pela API sem NaN", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValueOnce({
      data: [
        {
          id: "srv-1",
          titulo: "Faxina",
          descricao: "Faxina completa",
          precoBase: 120,
          unidadeCobranca: "por_hora",
          prestadorId: "prest-1",
          prestadorNome: "Maria",
          categoria: 1,
          cidade: "Blumenau",
          estado: "SC",
          mediaAvaliacoes: 4.5,
          tipoAnuncio: 1,
        },
      ],
    });

    const router = criarRouter();
    router.push("/servicos/buscar");
    await router.isReady();

    const wrapper = mount(BuscarServicosView, {
      global: { plugins: [router] },
    });

    await wrapper.get("button").trigger("click");
    await flushPromises();

    expect(wrapper.text()).toContain("R$ 120,00/h");
    expect(wrapper.text()).not.toContain("NaN");
  });
});
