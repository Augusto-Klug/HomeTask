import { beforeEach, describe, expect, it, vi } from "vitest"
import { flushPromises, mount } from "@vue/test-utils"
import { createMemoryHistory, createRouter } from "vue-router"
import { createPinia, setActivePinia } from "pinia"
import NovoAgendamentoView from "../NovoAgendamentoView.vue"
import * as apiModule from "@/services/api"
import { useAuthStore } from "@/stores/auth"
import { UnidadeCobranca } from "@/types"

vi.mock("@/services/api", () => ({ default: { get: vi.fn(), post: vi.fn() } }))
vi.mock("@/shared/validacao", () => ({ validarCampos: vi.fn(() => true) }))

const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: "/servicos/buscar", component: { template: "<div>Busca</div>" } },
    { path: "/agendamento/novo/:servicoId", component: NovoAgendamentoView, props: true },
    { path: "/agendamento/sucesso", component: { template: "<div>Sucesso</div>" } },
  ],
})

describe("NovoAgendamentoView", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    setActivePinia(createPinia())
    useAuthStore().setUser({ userId: "1", nome: "Cliente", email: "cliente@test.com", tipo: 1 })

    vi.mocked(apiModule.default.get).mockImplementation(async (url: string) => {
      if (url === "/api/Cidade/Listar") {
        return {
          data: [
            { id: "cidade-ac", nome: "Rio Branco", estado: "AC" },
            { id: "cidade-sc", nome: "Blumenau", estado: "SC" },
          ],
        }
      }

      if (url === "/api/ServicoOferecido/ObterServicoPorId") {
        return {
          data: {
            id: "srv-1",
            titulo: "Faxina",
            descricao: "desc",
            precoBase: 150,
            unidadeCobranca: UnidadeCobranca.Total,
            tipoAnuncio: 1,
            prestadorId: "prest-1",
            prestadorNome: "Maria",
            categoria: 1,
            cidade: "Blumenau",
            estado: "SC",
          },
        }
      }

      if (url === "/api/Cliente/ObterClientesPorUsuarioId") {
        return { data: { id: "cli-1" } }
      }

      throw new Error(`GET inesperado: ${url}`)
    })

    vi.mocked(apiModule.default.post).mockResolvedValue({ data: {} })
  })

  it("habilita a cidade apenas depois de selecionar o estado", async () => {
    router.push("/agendamento/novo/srv-1")
    await router.isReady()

    const wrapper = mount(NovoAgendamentoView, {
      props: { servicoId: "srv-1" },
      global: {
        plugins: [router],
        stubs: {
          HtInput: {
            props: ["modelValue", "label", "type"],
            emits: ["update:modelValue"],
            template: "<input :data-label='label' :type='type || \"text\"' :value='modelValue' @input=\"$emit('update:modelValue', $event.target.value)\" />",
          },
          HtSearchSelect: {
            props: ["modelValue", "label", "options"],
            emits: ["update:modelValue"],
            template: "<select data-testid='estado' :value='modelValue' @change=\"$emit('update:modelValue', $event.target.value)\"><option value=''></option><option v-for='option in options' :key='option.value' :value='option.value'>{{ option.label }}</option></select>",
          },
          HtSelect: {
            props: ["modelValue", "options", "disabled", "placeholder", "hint"],
            emits: ["update:modelValue"],
            template: "<div><select data-testid='cidade' :disabled='disabled' :value='modelValue' @change=\"$emit('update:modelValue', $event.target.value)\"><option value=''></option><option v-for='option in options' :key='option.value' :value='option.value'>{{ option.label }}</option></select><span data-testid='cidade-placeholder'>{{ placeholder }}</span><span data-testid='cidade-hint'>{{ hint }}</span></div>",
          },
          HtTextarea: {
            props: ["modelValue"],
            emits: ["update:modelValue"],
            template: "<textarea :value='modelValue' @input=\"$emit('update:modelValue', $event.target.value)\" />",
          },
          HtButton: { template: "<button type='submit'><slot /></button>", props: ["loading", "variant"] },
          HtCard: { template: "<div><slot /></div>" },
          HtAlert: { template: "<div />", props: ["message"] },
          HtSpinner: true,
          HtDivider: true,
        },
      },
    })

    await flushPromises()

    expect(wrapper.get("[data-testid='cidade']").attributes("disabled")).toBeDefined()
    expect(wrapper.get("[data-testid='cidade-placeholder']").text()).toBe("Selecione primeiro o estado")

    await wrapper.get("[data-testid='estado']").setValue("SC")

    expect(wrapper.get("[data-testid='cidade']").attributes("disabled")).toBeUndefined()
    expect(wrapper.get("[data-testid='cidade-placeholder']").text()).toBe("Selecione a cidade")
    expect(wrapper.html()).toContain("Blumenau")
  })

  it("envia a cidade selecionada ao criar o agendamento", async () => {
    router.push("/agendamento/novo/srv-1")
    await router.isReady()

    const wrapper = mount(NovoAgendamentoView, {
      props: { servicoId: "srv-1" },
      global: {
        plugins: [router],
        stubs: {
          HtInput: {
            props: ["modelValue", "label", "type"],
            emits: ["update:modelValue"],
            template: "<input :data-label='label' :type='type || \"text\"' :value='modelValue' @input=\"$emit('update:modelValue', $event.target.value)\" />",
          },
          HtSearchSelect: {
            props: ["modelValue", "label", "options"],
            emits: ["update:modelValue"],
            template: "<select data-testid='estado' :value='modelValue' @change=\"$emit('update:modelValue', $event.target.value)\"><option value=''></option><option v-for='option in options' :key='option.value' :value='option.value'>{{ option.label }}</option></select>",
          },
          HtSelect: {
            props: ["modelValue", "options", "disabled"],
            emits: ["update:modelValue"],
            template: "<select data-testid='cidade' :disabled='disabled' :value='modelValue' @change=\"$emit('update:modelValue', $event.target.value)\"><option value=''></option><option v-for='option in options' :key='option.value' :value='option.value'>{{ option.label }}</option></select>",
          },
          HtTextarea: {
            props: ["modelValue"],
            emits: ["update:modelValue"],
            template: "<textarea :value='modelValue' @input=\"$emit('update:modelValue', $event.target.value)\" />",
          },
          HtButton: { template: "<button type='submit'><slot /></button>", props: ["loading", "variant"] },
          HtCard: { template: "<div><slot /></div>" },
          HtAlert: { template: "<div />", props: ["message"] },
          HtSpinner: true,
          HtDivider: true,
        },
      },
    })

    await flushPromises()

    const inputs = wrapper.findAll("input")
    const [inputData, inputHora, inputEndereco] = inputs

    expect(inputData).toBeDefined()
    expect(inputHora).toBeDefined()
    expect(inputEndereco).toBeDefined()

    await inputData!.setValue("2026-07-01")
    await inputHora!.setValue("09:30")
    await inputEndereco!.setValue("Rua das Palmeiras, 120")
    await wrapper.get("[data-testid='estado']").setValue("SC")
    await wrapper.get("[data-testid='cidade']").setValue("cidade-sc")
    await wrapper.get("form").trigger("submit.prevent")

    expect(apiModule.default.post).toHaveBeenCalledWith("/api/Agendamento/CriarAgendamento", expect.objectContaining({
      cidadeId: "cidade-sc",
      enderecoDescricao: "Rua das Palmeiras, 120",
    }))
  })
})
