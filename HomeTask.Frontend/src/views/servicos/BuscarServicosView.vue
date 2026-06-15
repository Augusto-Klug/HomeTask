<template>
  <div class="mx-auto max-w-6xl px-4 py-8">
    <div class="mb-6">
      <h1 class="text-title font-semibold text-foreground">Buscar Servicos</h1>
      <p class="mt-1 text-sm text-muted">{{ totalLabel }}</p>
    </div>

    <HtCard class="mb-6">
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-5">
        <HtSelect
          v-model="filtro.categoria"
          :options="categoriasOpcoes"
          label="Categoria"
          placeholder="Todas as categorias"
        />

        <HtInput v-model="filtro.cidade" label="Cidade" placeholder="Ex: Blumenau" />

        <HtInput
          v-model="filtroPrecoStr"
          label="Preco maximo (R$)"
          type="number"
          :allowNegative="false"
          placeholder="Ex: 150"
        />

        <HtSelect
          v-model="tamanhoPaginaSelecionado"
          :options="opcoesTamanhoPagina"
          label="Registros por pagina"
        />

        <div class="flex items-end gap-2">
          <HtButton class="flex-1" :loading="carregando" @click="aplicarFiltros">
            <span class="material-symbols-rounded text-base">search</span>
            Buscar
          </HtButton>
          <HtButton variant="outline" :disabled="carregando" @click="limparFiltros">
            Limpar
          </HtButton>
        </div>
      </div>
    </HtCard>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <template v-else>
      <p v-if="resultado.itens.length === 0" class="py-12 text-center text-sm text-muted">
        Nenhum servico encontrado com os filtros selecionados.
      </p>

      <div v-else class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <HtCard
          v-for="servico in resultado.itens"
          :key="servico.id"
          class="transition-colors hover:border-primary"
        >
          <div class="mb-2 flex items-start justify-between gap-3">
            <div>
              <h3 class="text-sm font-semibold text-foreground">{{ servico.titulo }}</h3>
              <p class="mt-0.5 text-xs text-muted">
                {{ servico.prestadorId ? "Prestador:" : "Solicitante:" }}
                {{ servico.prestadorNome || servico.clienteNome || "N/A" }}
              </p>
            </div>
            <HtBadge :variant="HtBadgeVariant.Primary">{{ obterNomeCategoria(servico.categoria) }}</HtBadge>
          </div>

          <p class="mb-3 flex items-center gap-1 text-xs text-muted">
            <span class="material-symbols-rounded text-sm">location_on</span>
            {{ servico.cidade || "N/A" }}/{{ servico.estado || "N/A" }}
          </p>

          <div class="mb-4 flex items-center justify-between gap-3">
            <span class="text-sm font-bold text-primary">
              {{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}
            </span>
            <div v-if="servico.prestadorId" class="text-right text-xs">
              <p v-if="typeof servico.mediaAvaliacoes === 'number'" class="text-yellow-500">
                Servico {{ estrelas(servico.mediaAvaliacoes) }}
              </p>
              <p v-if="typeof servico.mediaAvaliacoesPrestador === 'number'" class="text-yellow-500">
                Prestador {{ estrelas(servico.mediaAvaliacoesPrestador) }}
              </p>
            </div>
          </div>

          <router-link :to="`/servicos/detalhes/${servico.id}`">
            <HtButton variant="outline" size="sm" class="w-full">Ver detalhes</HtButton>
          </router-link>
        </HtCard>
      </div>

      <HtCard class="mt-6">
        <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <p class="text-sm text-muted">
            Pagina {{ resultado.paginaAtual }} de {{ Math.max(resultado.totalPaginas, 1) }}
          </p>

          <div class="flex flex-col gap-3 sm:flex-row sm:items-end">
            <HtInput
              v-model="paginaDigitada"
              label="Ir para a pagina"
              type="number"
              :allowNegative="false"
              placeholder="Ex: 3"
            />

            <div class="flex gap-2">
              <HtButton
                variant="outline"
                :disabled="resultado.paginaAtual <= 1 || carregando"
                @click="irParaPagina(resultado.paginaAtual - 1)"
              >
                Anterior
              </HtButton>
              <HtButton
                variant="outline"
                :disabled="carregando || resultado.totalPaginas === 0"
                @click="irParaPaginaDigitada"
              >
                Ir
              </HtButton>
              <HtButton
                :disabled="resultado.paginaAtual >= resultado.totalPaginas || carregando"
                @click="irParaPagina(resultado.paginaAtual + 1)"
              >
                Proxima
              </HtButton>
            </div>
          </div>
        </div>
      </HtCard>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from "vue"
import { useRoute, useRouter } from "vue-router"
import api from "@/services/api"
import { formatarPrecoServico } from "@/shared/utils"
import { CATEGORIAS_SERVICO, type BuscarFiltro, type ResultadoPaginado, type ServicoBuscaResumo } from "@/types"
import HtInput from "@/components/ui/HtInput.vue"
import HtSelect from "@/components/ui/HtSelect.vue"
import HtButton from "@/components/ui/HtButton.vue"
import HtCard from "@/components/ui/HtCard.vue"
import HtBadge, { HtBadgeVariant } from "@/components/ui/HtBadge.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"

const route = useRoute()
const router = useRouter()

const categoriasOpcoes = [
  { value: "", label: "Todas as categorias" },
  ...CATEGORIAS_SERVICO.map((categoria) => ({ value: String(categoria.value), label: categoria.label })),
]

const opcoesTamanhoPagina = [
  { value: "10", label: "10 registros" },
  { value: "30", label: "30 registros" },
  { value: "50", label: "50 registros" },
]

const filtro = reactive<BuscarFiltro>({
  categoria: "",
  cidade: "",
  precoMaximo: null,
})

const filtroPrecoStr = ref("")
const tamanhoPaginaSelecionado = ref("30")
const paginaDigitada = ref("1")
const carregando = ref(false)
let ultimaBuscaId = 0

const resultado = reactive<ResultadoPaginado<ServicoBuscaResumo>>({
  itens: [],
  paginaAtual: 1,
  tamanhoPagina: 30,
  totalRegistros: 0,
  totalPaginas: 0,
})

const totalLabel = computed(() => {
  if (resultado.totalRegistros === 0) {
    return "Nenhum resultado para esta consulta."
  }

  return `${resultado.totalRegistros} servico(s) encontrados nesta consulta.`
})

watch(
  () => route.query,
  async (query) => {
    filtro.categoria = typeof query.categoria === "string" ? query.categoria : ""
    filtro.cidade = typeof query.cidade === "string" ? query.cidade : ""
    filtroPrecoStr.value = typeof query.precoMaximo === "string" ? query.precoMaximo : ""
    filtro.precoMaximo = filtroPrecoStr.value ? Number(filtroPrecoStr.value) : null
    tamanhoPaginaSelecionado.value = normalizarTamanhoPagina(query.tamanhoPagina)

    const paginaAtual = normalizarPagina(query.pagina)
    paginaDigitada.value = String(paginaAtual)

    await buscar(paginaAtual)
  },
  { immediate: true },
)

async function buscar(pagina: number) {
  const buscaAtualId = ++ultimaBuscaId
  carregando.value = true

  try {
    const params: Record<string, unknown> = {
      pagina,
      tamanhoPagina: Number(tamanhoPaginaSelecionado.value),
    }

    if (filtro.categoria) params.categoria = filtro.categoria
    if (filtro.cidade) params.cidade = filtro.cidade
    if (filtroPrecoStr.value) params.precoMaximo = Number(filtroPrecoStr.value)

    const { data } = await api.get<ResultadoPaginado<ServicoBuscaResumo> | ServicoBuscaResumo[]>(
      "/api/ServicoOferecido/BuscarServicos",
      { params },
    )
    if (buscaAtualId !== ultimaBuscaId) return

    const normalizado = normalizarResultadoBusca(data, pagina, Number(tamanhoPaginaSelecionado.value))
    Object.assign(resultado, normalizado)
    paginaDigitada.value = String(normalizado.paginaAtual)
  } catch {
    if (buscaAtualId !== ultimaBuscaId) return

    Object.assign(resultado, {
      itens: [],
      paginaAtual: pagina,
      tamanhoPagina: Number(tamanhoPaginaSelecionado.value),
      totalRegistros: 0,
      totalPaginas: 0,
    })
  } finally {
    if (buscaAtualId === ultimaBuscaId) {
      carregando.value = false
    }
  }
}

async function aplicarFiltros() {
  await sincronizarBusca(1)
}

async function limparFiltros() {
  filtro.categoria = ""
  filtro.cidade = ""
  filtro.precoMaximo = null
  filtroPrecoStr.value = ""
  tamanhoPaginaSelecionado.value = "30"
  await sincronizarBusca(1)
}

async function irParaPagina(pagina: number) {
  if (pagina < 1 || (resultado.totalPaginas > 0 && pagina > resultado.totalPaginas)) {
    return
  }

  await sincronizarBusca(pagina)
}

async function irParaPaginaDigitada() {
  const pagina = normalizarPagina(paginaDigitada.value)
  await irParaPagina(resultado.totalPaginas > 0 ? Math.min(pagina, resultado.totalPaginas) : pagina)
}

function montarQuery(pagina: number): Record<string, string> {
  const query: Record<string, string> = {
    pagina: String(pagina),
    tamanhoPagina: tamanhoPaginaSelecionado.value,
  }

  if (filtro.categoria) query.categoria = filtro.categoria
  if (filtro.cidade) query.cidade = filtro.cidade
  if (filtroPrecoStr.value) query.precoMaximo = filtroPrecoStr.value

  return query
}

async function sincronizarBusca(pagina: number) {
  const query = montarQuery(pagina)

  if (queriesSaoIguais(route.query, query)) {
    await buscar(pagina)
    return
  }

  await router.push({ query })
}

function queriesSaoIguais(queryAtual: Record<string, unknown>, novaQuery: Record<string, string>): boolean {
  const chavesAtuais = Object.keys(queryAtual).filter((chave) => typeof queryAtual[chave] !== "undefined")
  const chavesNovas = Object.keys(novaQuery)

  if (chavesAtuais.length !== chavesNovas.length) {
    return false
  }

  return chavesNovas.every((chave) => String(queryAtual[chave] ?? "") === novaQuery[chave])
}

function normalizarPagina(valor: unknown): number {
  const pagina = Number(valor)
  return Number.isFinite(pagina) && pagina > 0 ? Math.trunc(pagina) : 1
}

function normalizarTamanhoPagina(valor: unknown): string {
  return valor === "10" || valor === "50" ? String(valor) : "30"
}

function estrelas(media: number): string {
  const cheias = Math.floor(media ?? 0)
  return "★".repeat(cheias) + "☆".repeat(5 - cheias)
}

function obterNomeCategoria(categoria: number | { id: string; nome: string; icone: string }): string {
  if (typeof categoria === "object" && categoria !== null) {
    return categoria.nome
  }

  const encontrada = CATEGORIAS_SERVICO.find((item) => String(item.value) === String(categoria))
  return encontrada?.label ?? String(categoria)
}

function normalizarResultadoBusca(
  data: ResultadoPaginado<ServicoBuscaResumo> | ServicoBuscaResumo[],
  pagina: number,
  tamanhoPagina: number,
): ResultadoPaginado<ServicoBuscaResumo> {
  if (Array.isArray(data)) {
    return {
      itens: data,
      paginaAtual: pagina,
      tamanhoPagina,
      totalRegistros: data.length,
      totalPaginas: data.length === 0 ? 0 : 1,
    }
  }

  const payload = data as unknown as Record<string, unknown>
  const itens = Array.isArray(payload.itens)
    ? (payload.itens as ServicoBuscaResumo[])
    : Array.isArray(payload.Itens)
      ? (payload.Itens as ServicoBuscaResumo[])
      : []

  const paginaAtual = obterNumeroPayload(payload.paginaAtual, payload.PaginaAtual, pagina)
  const tamanhoPaginaAtual = obterNumeroPayload(payload.tamanhoPagina, payload.TamanhoPagina, tamanhoPagina)
  const totalRegistros = obterNumeroPayload(payload.totalRegistros, payload.TotalRegistros, itens.length)
  const totalPaginas = obterNumeroPayload(
    payload.totalPaginas,
    payload.TotalPaginas,
    totalRegistros === 0 ? 0 : Math.ceil(totalRegistros / Math.max(tamanhoPaginaAtual, 1)),
  )

  return {
    itens,
    paginaAtual,
    tamanhoPagina: tamanhoPaginaAtual,
    totalRegistros,
    totalPaginas,
  }
}

function obterNumeroPayload(valorCamel: unknown, valorPascal: unknown, fallback: number): number {
  const valor = typeof valorCamel !== "undefined" ? valorCamel : valorPascal
  const numero = Number(valor)
  return Number.isFinite(numero) ? numero : fallback
}
</script>
