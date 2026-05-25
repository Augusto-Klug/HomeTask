import { describe, it, expect, vi } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import BuscarServicosView from "../BuscarServicosView.vue";
import * as apiModule from "@/services/api";
import { UnidadeCobranca } from "@/types";

vi.mock("@/services/api", () => ({ default: { get: vi.fn() } }));

const router = createRouter({
  history: createMemoryHistory(),
  routes: [{ path: "/servicos/buscar", component: BuscarServicosView }],
});

describe("BuscarServicosConsumo", () => {
  it("consome precoBase da busca", async () => {
    vi.mocked(apiModule.default.get).mockResolvedValue({
      data: {
        itens: [
          {
            id: "1",
            titulo: "Faxina",
            descricao: "desc",
            precoBase: 120,
            unidadeCobranca: UnidadeCobranca.Total,
            prestadorId: "10",
            prestadorNome: "Maria",
            categoria: "Faxina",
            cidade: "Blumenau",
            estado: "SC",
          },
        ],
        paginaAtual: 1,
        tamanhoPagina: 30,
        totalRegistros: 1,
        totalPaginas: 1,
      },
    });

    router.push("/servicos/buscar");
    await router.isReady();

    const wrapper = mount(BuscarServicosView, {
      global: { plugins: [router] },
    });

    await flushPromises();
    expect(wrapper.text()).toContain("R$");
  });
});
