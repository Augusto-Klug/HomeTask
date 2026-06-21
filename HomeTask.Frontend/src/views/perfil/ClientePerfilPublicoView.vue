<template>
  <div class="mx-auto max-w-4xl px-4 py-8">
    <div v-if="carregando" class="flex justify-center py-20">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <div v-else-if="!perfil" class="py-16 text-center">
      <p class="text-sm text-muted">Cliente nao encontrado.</p>
    </div>

    <template v-else>
      <router-link
        to="/servicos/buscar"
        class="mb-6 inline-flex items-center gap-1.5 text-sm text-muted transition-colors hover:text-primary"
      >
        <span class="material-symbols-rounded text-base">arrow_back</span>
        Voltar
      </router-link>

      <HtCard class="mb-6">
        <div class="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
          <div>
            <h1 class="text-2xl font-bold text-foreground">{{ perfil.nome }}</h1>
            <p v-if="perfil.cidade || perfil.estado" class="mt-1 text-sm text-muted">
              {{ [perfil.cidade, perfil.estado].filter(Boolean).join("/") }}
            </p>
          </div>

          <div class="grid grid-cols-1 gap-3 text-sm md:min-w-72">
            <div class="rounded-xl border border-border/60 bg-muted/10 p-4">
              <p class="text-xs uppercase tracking-wider text-muted">Avaliacao do cliente</p>
              <p class="mt-1 text-lg font-semibold text-yellow-500">{{ formatarEstrelas(perfil.mediaAvaliacoes, "round") }}</p>
              <p class="text-sm text-foreground">{{ formatarNota(perfil.mediaAvaliacoes) }} de 5</p>
              <p class="text-xs text-muted">{{ perfil.totalAvaliacoes }} avaliacao(oes)</p>
            </div>
            <div class="rounded-xl border border-border/60 bg-muted/10 p-4">
              <p class="text-xs uppercase tracking-wider text-muted">Servicos contratados</p>
              <p class="mt-1 text-2xl font-bold text-foreground">{{ perfil.totalServicosContratados }}</p>
            </div>
          </div>
        </div>
      </HtCard>
    </template>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue"
import api from "@/services/api"
import { formatarEstrelas } from "@/shared/utils"
import type { ClientePerfilPublico } from "@/types"
import HtCard from "@/components/ui/HtCard.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"

const props = defineProps<{ id: string }>()

const perfil = ref<ClientePerfilPublico | null>(null)
const carregando = ref(true)

onMounted(async () => {
  try {
    const { data } = await api.get<ClientePerfilPublico>("/api/Cliente/ObterPerfilPublico", {
      params: { clienteId: props.id },
    })
    perfil.value = data
  } catch {
    perfil.value = null
  } finally {
    carregando.value = false
  }
})

function formatarNota(nota: number): string {
  return Number(nota ?? 0).toFixed(1)
}
</script>
