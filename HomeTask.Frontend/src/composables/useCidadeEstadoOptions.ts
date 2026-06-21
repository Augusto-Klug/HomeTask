import { computed, ref } from "vue"
import api from "@/services/api"

type CidadeApi = {
  id?: string
  Id?: string
  cidadeId?: string
  CidadeId?: string
  nome?: string
  Nome?: string
  descricao?: string
  Descricao?: string
  estado?: string
  Estado?: string
  uf?: string
  Uf?: string
  UF?: string
}

type CidadeOption = {
  label: string
  value: string
  estado: string
}

function mapearCidade(cidade: CidadeApi): CidadeOption {
  const value = cidade.id ?? cidade.Id ?? cidade.cidadeId ?? cidade.CidadeId ?? ""
  const nome = cidade.nome ?? cidade.Nome ?? cidade.descricao ?? cidade.Descricao ?? ""
  const uf = cidade.uf ?? cidade.Uf ?? cidade.UF ?? cidade.estado ?? cidade.Estado ?? ""

  return {
    value: String(value),
    label: nome || String(value),
    estado: String(uf).toUpperCase(),
  }
}

export function useCidadeEstadoOptions(estadoSelecionado: () => string) {
  const carregandoCidades = ref(false)
  const todasCidades = ref<CidadeOption[]>([])

  const cidadesDisponiveis = computed(() =>
    todasCidades.value
      .filter((cidade) => cidade.estado === estadoSelecionado())
      .map(({ value, label }) => ({ value, label })),
  )

  const cidadeHint = computed(() => {
    if (carregandoCidades.value) return "Carregando cidades..."
    if (!estadoSelecionado()) return "Selecione um estado para listar as cidades."
    if (cidadesDisponiveis.value.length === 0) return "Nenhuma cidade encontrada para o estado selecionado."
    return undefined
  })

  async function carregarCidades() {
    carregandoCidades.value = true

    try {
      const response = await api.get("/api/Cidade/Listar")
      const lista = (Array.isArray(response.data)
        ? response.data
        : response.data?.cidades ?? response.data?.data ?? response.data?.value ?? []) as CidadeApi[]
      todasCidades.value = lista.map(mapearCidade).filter((cidade) => cidade.value && cidade.estado)
    } catch (err: unknown) {
      const e = err as { response?: { status?: number } }

      if (e.response?.status === 404) {
        try {
          const response = await api.get("/api/Cidades/Listar")
          const lista = (Array.isArray(response.data)
            ? response.data
            : response.data?.cidades ?? response.data?.data ?? response.data?.value ?? []) as CidadeApi[]
          todasCidades.value = lista.map(mapearCidade).filter((cidade) => cidade.value && cidade.estado)
          return
        } catch {
          todasCidades.value = []
        }
      } else {
        todasCidades.value = []
      }
    } finally {
      carregandoCidades.value = false
    }
  }

  return {
    carregandoCidades,
    cidadesDisponiveis,
    cidadeHint,
    carregarCidades,
  }
}
