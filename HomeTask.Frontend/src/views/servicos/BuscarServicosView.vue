<template lang="pug">
.padding
  h4 Buscar Serviços

  //- Filtros
  article.padding
    .row.wrap
      .field.label.border.max
        select(v-model="filtro.categoria")
          option(value="") Todas as categorias
          option(v-for="cat in categorias" :key="cat.id" :value="cat.id") {{ cat.nome }}
        label Categoria

      .field.label.border.max
        input(v-model="filtro.cidade" type="text")
        label Cidade

      .field.label.border.max
        input(v-model.number="filtro.precoMaximo" type="number" min="0")
        label Preço máximo (R$)

    button(@click="buscar" :disabled="carregando")
      i search
      span Buscar

  //- Loading
  .center-align.padding(v-if="carregando")
    .progress.circle

  //- Resultados
  template(v-else-if="buscou")
    p.padding(v-if="servicos.length === 0") Nenhum serviço encontrado com os filtros selecionados.

    .grid.padding(v-else)
      article.s12.m6.l4(v-for="s in servicos" :key="s.id")
        .padding
          .row
            .max
              h6 {{ s.titulo }}
              p.small {{ s.prestadorNome }}
            span.chip {{ s.categoria }}
          p.small {{ s.cidade }}/{{ s.estado }}
          .row
            span.bold R$ {{ formatarPreco(s.preco) }}/h
            .max
            span {{ estrelas(s.mediaAvaliacoes) }}
          router-link.button.responsive(:to="`/servicos/detalhes/${s.id}`")
            span Ver detalhes
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'
import type { Servico, Categoria, BuscarFiltro } from '@/types'

const route = useRoute()

const categorias: Categoria[] = [
  { id: 1,  nome: 'Faxina',            icone: 'cleaning_services' },
  { id: 2,  nome: 'Jardinagem',         icone: 'yard' },
  { id: 3,  nome: 'Reparos',            icone: 'handyman' },
  { id: 4,  nome: 'Lavanderia',         icone: 'local_laundry_service' },
  { id: 5,  nome: 'Passadoria',         icone: 'iron' },
  { id: 6,  nome: 'Babysitter',         icone: 'child_care' },
  { id: 7,  nome: 'Cuidador de Idosos', icone: 'elderly' },
  { id: 8,  nome: 'Pet Sitter',         icone: 'pets' },
  { id: 9,  nome: 'Cozinheiro',         icone: 'restaurant' },
  { id: 10, nome: 'Serviços Gerais',    icone: 'build' },
]

const filtro = reactive<BuscarFiltro>({
  categoria: (route.query.categoria as string) ?? '',
  cidade: '',
  precoMaximo: null,
  avaliacaoMinima: 0,
})

const servicos = ref<Servico[]>([])
const carregando = ref(false)
const buscou = ref(false)

onMounted(() => buscar())

async function buscar() {
  carregando.value = true
  buscou.value = false
  try {
    const params: Record<string, unknown> = {}
    if (filtro.categoria)   params.categoria   = filtro.categoria
    if (filtro.cidade)      params.cidade      = filtro.cidade
    if (filtro.precoMaximo) params.precoMaximo = filtro.precoMaximo

    const { data } = await api.get<Servico[]>('/api/ServicoOferecido/BuscarServicos', { params })
    servicos.value = filtro.avaliacaoMinima > 0
      ? data.filter((s) => s.mediaAvaliacoes >= filtro.avaliacaoMinima)
      : data
  } catch {
    servicos.value = []
  } finally {
    carregando.value = false
    buscou.value = true
  }
}

function formatarPreco(valor: number): string {
  return Number(valor).toFixed(2).replace('.', ',')
}

function estrelas(media: number): string {
  const cheias = Math.floor(media ?? 0)
  return '★'.repeat(cheias) + '☆'.repeat(5 - cheias)
}
</script>
