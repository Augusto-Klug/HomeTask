import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import SolicitacoesPendentesView from "../SolicitacoesPendentesView.vue";

describe("SolicitacoesPendentesView", () => {
  it("aponta para minha operacao", () => {
    const wrapper = mount(SolicitacoesPendentesView, {
      global: {
        stubs: {
          HtButton: { template: "<button><slot /></button>" },
          RouterLink: { template: "<a><slot /></a>" },
        },
      },
    });

    expect(wrapper.text()).toContain("Ver minha operação");
  });
});
