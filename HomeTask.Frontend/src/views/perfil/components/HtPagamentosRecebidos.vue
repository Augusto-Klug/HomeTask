<template>
  <div class="space-y-6">
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <HtCard class="border-success/20 bg-success/5 p-5">
        <p class="text-sm text-muted mb-2">Total recebido</p>
        <p class="text-3xl font-semibold text-foreground">{{ formatCurrency(resumo.saldoRecebidoTotal) }}</p>
        <p class="text-xs text-muted mt-2">Somatório dos serviços concluídos com pagamento confirmado.</p>
      </HtCard>

      <HtCard class="border-primary/20 bg-primary/5 p-5">
        <p class="text-sm text-muted mb-2">Serviços pagos</p>
        <p class="text-3xl font-semibold text-foreground">{{ resumo.totalServicosRecebidos }}</p>
        <p class="text-xs text-muted mt-2">Quantidade de serviços já concluídos e contabilizados.</p>
      </HtCard>
    </div>

    <HtCard class="overflow-hidden">
      <div class="flex items-center justify-between px-5 py-4 border-b border-border">
        <div>
          <h3 class="text-lg font-semibold text-foreground">Histórico de recebimentos</h3>
          <p class="text-sm text-muted">Acompanhe quanto entrou por serviço concluído.</p>
        </div>
      </div>

      <div v-if="carregando" class="flex justify-center py-12">
        <HtSpinner />
      </div>

      <div v-else-if="resumo.servicosRecebidos.length === 0" class="px-5 py-12 text-center text-sm text-muted">
        Nenhum recebimento encontrado até o momento.
      </div>

      <div v-else class="divide-y divide-border">
        <article
          v-for="item in resumo.servicosRecebidos"
          :key="item.agendamentoId"
          class="px-5 py-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between"
        >
          <div class="space-y-1">
            <p class="font-medium text-foreground">{{ item.tituloServico }}</p>
            <p class="text-sm text-muted">{{ item.clienteNome }} • {{ item.cidade }} - {{ item.estado }}</p>
            <p class="text-xs text-muted">
              {{ item.dataConclusao ? `Concluído em ${formatDate(item.dataConclusao)}` : 'Conclusão sem data informada' }}
            </p>
          </div>

          <div class="text-left md:text-right">
            <p class="text-lg font-semibold text-success">{{ formatCurrency(item.valorRecebido) }}</p>
            <router-link
              :to="`/agendamento/detalhes/${item.agendamentoId}`"
              class="text-sm text-primary hover:underline"
            >
              Ver agendamento
            </router-link>
          </div>
        </article>
      </div>
    </HtCard>
  </div>
</template>

<script setup lang="ts">
import type { PrestadorRecebimentosResumo } from '@/types'
import HtCard from '@/components/ui/HtCard.vue'
import HtSpinner from '@/components/ui/HtSpinner.vue'

defineProps<{
  resumo: PrestadorRecebimentosResumo
  carregando: boolean
}>()

function formatCurrency(value: number) {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(value)
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'medium',
  }).format(new Date(value))
}
</script>
