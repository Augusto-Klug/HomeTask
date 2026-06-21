<template>
  <div class="mx-auto max-w-3xl px-4 py-8">
    <div v-if="carregando" class="flex justify-center py-20">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <div v-else-if="!servico" class="flex flex-col items-center gap-4 py-16 text-center">
      <span class="material-symbols-rounded text-5xl text-error">error</span>
      <p class="text-sm text-muted">Serviço não encontrado.</p>
      <router-link to="/servicos/buscar">
        <HtButton variant="outline">Voltar à busca</HtButton>
      </router-link>
    </div>

    <template v-else>
      <router-link
        to="/servicos/buscar"
        class="mb-6 inline-flex items-center gap-1.5 text-sm text-muted transition-colors hover:text-primary"
      >
        <span class="material-symbols-rounded text-base">arrow_back</span>
        Voltar à busca
      </router-link>

      <HtCard class="mb-6">
        <div class="mb-4 flex flex-wrap items-start justify-between gap-3">
          <div>
            <h1 class="text-xl font-bold text-foreground">{{ servico.titulo }}</h1>
            <p v-if="ehServicoPrestador" class="mt-0.5 text-sm text-muted">
              Prestador:
              <router-link :to="`/prestadores/${servico.prestadorId}`" class="font-medium text-primary hover:underline">
                {{ nomeResponsavel }}
              </router-link>
            </p>
            <div v-else class="mt-0.5 flex flex-wrap items-center gap-x-3 gap-y-1 text-sm text-muted">
              <p>
                Solicitante:
                <router-link :to="`/clientes/${servico.clienteId}`" class="font-medium text-primary hover:underline">
                  {{ nomeResponsavel }}
                </router-link>
              </p>
              <router-link :to="`/clientes/${servico.clienteId}`" class="text-primary hover:underline">
                Ver perfil do cliente
              </router-link>
            </div>
          </div>
          <HtBadge :variant="HtBadgeVariant.Primary">{{ obterNomeCategoria(servico.categoria) }}</HtBadge>
        </div>

        <HtDivider />

        <div class="mb-2 flex items-center gap-2 text-sm text-muted">
          <span class="material-symbols-rounded text-base">location_on</span>
          {{ formatarEnderecoServico(servico) }}
        </div>

        <div class="mb-4 flex items-center justify-between">
          <span class="text-lg font-bold text-primary">
            {{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}
          </span>
          <div class="text-right text-sm">
            <p v-if="typeof servico.mediaAvaliacoes === 'number'" class="text-yellow-500">
              Serviço {{ estrelas(servico.mediaAvaliacoes) }}
            </p>
            <p v-if="ehServicoPrestador && typeof servico.mediaAvaliacoesPrestador === 'number'" class="text-yellow-500">
              Prestador {{ formatarEstrelas(servico.mediaAvaliacoesPrestador) }}
            </p>
          </div>
        </div>

        <div
          v-if="!ehServicoPrestador && servico.dataDesejada"
          class="mb-4 rounded-lg border border-primary/10 bg-primary/5 p-3"
        >
          <p class="mb-1 text-xs font-semibold uppercase text-primary">Data desejada para execução</p>
          <p class="flex items-center gap-2 text-sm text-foreground">
            <span class="material-symbols-rounded text-base">event</span>
            {{ formatarData(servico.dataDesejada) }} às {{ formatarHora(servico.dataDesejada) }}
          </p>
        </div>

        <p v-if="servico.descricao" class="mb-6 text-sm leading-relaxed text-foreground">
          {{ servico.descricao }}
        </p>

        <HtButton v-if="auth.isLoggedIn" @click="irParaFluxo">
          <span class="material-symbols-rounded text-base">calendar_month</span>
          {{ ehServicoPrestador ? "Agendar agora" : "Enviar proposta" }}
        </HtButton>
        <router-link v-else :to="`/login?redirect=/servicos/detalhes/${props.id}`">
          <HtButton variant="outline">
            <span class="material-symbols-rounded text-base">login</span>
            Faça login para prosseguir
          </HtButton>
        </router-link>
      </HtCard>

      <div v-if="avaliacoes.length">
        <h2 class="mb-4 text-title font-semibold text-foreground">Avaliações</h2>
        <div class="flex flex-col gap-3">
          <HtCard v-for="avaliacao in avaliacoes" :key="avaliacao.id">
            <div class="mb-1 flex items-center justify-between">
              <span class="text-sm font-semibold text-foreground">{{ avaliacao.clienteNome }}</span>
              <div class="text-right text-xs text-yellow-500">
                <p>Serviço {{ estrelas(avaliacao.notaServico) }}</p>
                <p>Prestador {{ estrelas(avaliacao.notaPrestador) }}</p>
              </div>
            </div>
            <p class="mb-1 text-xs text-muted">{{ avaliacao.servicoTitulo }}</p>
            <p class="mb-1 text-sm text-foreground">{{ avaliacao.comentario }}</p>
            <p class="text-xs text-muted">{{ formatarData(avaliacao.dataAvaliacao) }}</p>
          </HtCard>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue"
import { useRouter } from "vue-router"
import api from "@/services/api"
import { formatarData, formatarEstrelas, formatarHora, formatarPrecoServico } from "@/shared/utils"
import { useAuthStore } from "@/stores/auth"
import { CATEGORIAS_SERVICO, type Avaliacao, type ServicoDetalhe } from "@/types"
import HtButton from "@/components/ui/HtButton.vue"
import HtCard from "@/components/ui/HtCard.vue"
import HtBadge, { HtBadgeVariant } from "@/components/ui/HtBadge.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"
import HtDivider from "@/components/ui/HtDivider.vue"

const props = defineProps<{ id: string }>()

const auth = useAuthStore()
const router = useRouter()

const servico = ref<ServicoDetalhe | null>(null)
const avaliacoes = ref<Avaliacao[]>([])
const carregando = ref(true)

const ehServicoPrestador = computed(() => !!servico.value && "prestadorId" in servico.value)
const nomeResponsavel = computed(() => {
  if (!servico.value) return "N/A"
  return "prestadorNome" in servico.value
    ? servico.value.prestadorNome || "N/A"
    : servico.value.clienteNome || "N/A"
})

onMounted(async () => {
  try {
    const { data } = await api.get<ServicoDetalhe>("/api/ServicoOferecido/ObterServicoPorId", {
      params: { id: props.id },
    })
    servico.value = data

    if (data && "prestadorId" in data) {
      const resposta = await api
        .get<Avaliacao[]>("/api/Avaliacao/ObterAvaliacoesPorPrestador", {
          params: { prestadorId: data.prestadorId },
        })
        .catch(() => ({ data: [] as Avaliacao[] }))
      avaliacoes.value = resposta.data ?? []
    }
  } catch {
    servico.value = null
  } finally {
    carregando.value = false
  }
})

function irParaFluxo() {
  if (!servico.value) return

  if ("prestadorId" in servico.value) {
    router.push({
      name: "agendamento-novo",
      params: { servicoId: props.id },
    })
    return
  }

  router.push({
    name: "proposta-nova",
    params: { servicoId: props.id },
  })
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

function formatarEnderecoServico(detalhe: ServicoDetalhe): string {
  const base = [detalhe.logradouro, detalhe.numero].filter(Boolean).join(", ")
  const bairro = detalhe.bairro ? ` - ${detalhe.bairro}` : ""
  const cidadeEstado = [detalhe.cidade || "N/A", detalhe.estado || "N/A"].join("/")

  return base ? `${base}${bairro} - ${cidadeEstado}` : cidadeEstado
}
</script>
