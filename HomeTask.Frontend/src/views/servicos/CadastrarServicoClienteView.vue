<template>
  <div class="max-w-2xl mx-auto px-4 py-8">
    <!-- Voltar -->
    <router-link
      to="/servicos/buscar"
      class="inline-flex items-center gap-1.5 text-sm text-muted hover:text-primary mb-6 transition-colors"
    >
      <span class="material-symbols-rounded text-base">arrow_back</span>
      Voltar à busca
    </router-link>

    <!-- Sucesso -->
    <HtCard v-if="sucesso" class="text-center py-8">
      <span class="material-symbols-rounded text-5xl text-success mb-4 block"
        >check_circle</span
      >
      <h2 class="text-title font-semibold text-foreground mb-2">
        Anúncio publicado!
      </h2>
      <p class="text-sm text-muted mb-6">
        Seu pedido de serviço foi publicado. Prestadores poderão visualizá-lo e
        entrar em contato.
      </p>
      <div class="flex flex-wrap justify-center gap-3">
        <router-link to="/servicos/buscar">
          <HtButton variant="outline">Explorar serviços</HtButton>
        </router-link>
        <HtButton @click="reiniciar">Publicar outro</HtButton>
      </div>
    </HtCard>

    <template v-else>
      <h1 class="text-title font-semibold text-foreground mb-1">
        Anunciar Serviço Desejado
      </h1>
      <p class="text-sm text-muted mb-6">
        Descreva o serviço que você precisa. Prestadores cadastrados poderão
        visualizar seu anúncio e entrar em contato com uma proposta.
      </p>

      <HtCard>
        <HtAlert v-if="erro" :message="erro" class="mb-4" />

        <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
          <!-- Titulo do serviço -->
          <HtInput
            ref="refTitulo"
            v-model="form.titulo"
            label="Título do serviço"
            placeholder="Ex: Faxina mensal na residência"
            required
            regra="required"
          />
          <!-- Descrição -->
          <HtTextarea
            ref="refDescricao"
            v-model="form.descricao"
            label="Descrição do serviço"
            placeholder="Descreva detalhadamente o que precisa ser feito, condições do local, materiais necessários..."
            :rows="4"
            required
            regra="required"
          />

          <!-- Categoria -->
          <HtSelect
            ref="refCategoria"
            v-model="form.categoria"
            :options="categoriasOpcoes"
            label="Categoria"
            placeholder="Selecione uma categoria"
            required
          />

          <!-- Tipo de remuneração -->
          <HtSelect
            ref="refTipoValor"
            v-model="form.unidadeCobranca"
            :options="tiposValorOpcoes"
            label="Tipo de remuneração"
            placeholder="Como deseja pagar?"
            required
          />

          <!-- Valor (condicional) -->
          <HtInput
            v-if="
              form.unidadeCobranca &&
              form.unidadeCobranca !== String(UnidadeCobranca.ACombinar)
            "
            ref="refValor"
            v-model="form.precoBase"
            label="Valor (R$)"
            type="number"
            :allowNegative="false"
            :placeholder="
              form.unidadeCobranca === String(UnidadeCobranca.PorHora)
                ? 'Ex: 80,00 por hora'
                : 'Ex: 250,00 total'
            "
            :hint="
              form.unidadeCobranca === String(UnidadeCobranca.PorHora)
                ? 'Valor por hora de trabalho'
                : 'Valor total do serviço'
            "
            required
            regra="required"
          />
          <!-- Informação sobre valor a combinar -->
          <HtAlert
            v-if="form.unidadeCobranca === String(UnidadeCobranca.ACombinar)"
            variant="info"
            message="O prestador poderá fazer uma oferta com o valor e horário que desejar. Você receberá uma notificação com a proposta."
          />

          <!-- Data desejada -->
          <HtDateTimeInput
            v-model="form.data"
            label="Data desejada"
            hint="Opcional — deixe em branco para combinar com o prestador"
          />

          <!-- Informação sobre data em branco -->
          <HtAlert
            v-if="!form.data"
            variant="info"
            message="Sem data definida, o prestador poderá propor o horário que melhor se adequa à agenda dele."
          />

          <HtDivider />

          <HtButton type="submit" :loading="carregando" class="w-full">
            <span class="material-symbols-rounded text-base">campaign</span>
            Publicar Anúncio
          </HtButton>
        </form>
      </HtCard>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import api from "@/services/api";
import { validarCampos, type CampoValidavel } from "@/shared/validacao";
import {
  CATEGORIAS_SERVICO,
  UnidadeCobranca,
  type ServicoClienteForm,
} from "@/types";
import HtInput from "@/components/ui/HtInput.vue";
import HtTextarea from "@/components/ui/HtTextarea.vue";
import HtSelect from "@/components/ui/HtSelect.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtAlert from "@/components/ui/HtAlert.vue";
import HtDateTimeInput from "@/components/ui/HtDateTimeInput.vue";
import HtDivider from "@/components/ui/HtDivider.vue";

const categoriasOpcoes = CATEGORIAS_SERVICO.map((c) => ({
  value: c.value,
  label: c.label,
}));

const tiposValorOpcoes = [
  { value: UnidadeCobranca.PorHora, label: "Valor por hora" },
  { value: UnidadeCobranca.Total, label: "Valor total fixo" },
  { value: UnidadeCobranca.ACombinar, label: "A combinar com o prestador" },
];

const form = reactive<ServicoClienteForm>({
  titulo: "",
  descricao: "",
  categoria: "",
  unidadeCobranca: "",
  precoBase: "",
  data: "",
  tipo: 2,
});

const refTitulo = ref<InstanceType<typeof HtInput> | null>(null);
const refDescricao = ref<InstanceType<typeof HtTextarea> | null>(null);
const refCategoria = ref<InstanceType<typeof HtSelect> | null>(null);
const refTipoValor = ref<InstanceType<typeof HtSelect> | null>(null);
const refValor = ref<InstanceType<typeof HtInput> | null>(null);

const carregando = ref(false);
const erro = ref<string | null>(null);
const sucesso = ref(false);

async function handleSubmit() {
  const campos: Array<CampoValidavel | null> = [
    refTitulo.value,
    refDescricao.value,
    refCategoria.value,
    refTipoValor.value,
  ];
  if (
    form.unidadeCobranca &&
    form.unidadeCobranca !== String(UnidadeCobranca.ACombinar)
  )
    campos.push(refValor.value);
  if (!validarCampos(campos)) return;

  erro.value = null;
  carregando.value = true;
  try {
    const categoriaSelecionada = Number(form.categoria);
    if (!categoriaSelecionada) {
      throw new Error("Selecione uma categoria válida.");
    }

    const payload: Record<string, unknown> = {
      titulo: form.titulo,
      descricao: form.descricao,
      categoria: categoriaSelecionada,
      unidadeCobranca: Number(form.unidadeCobranca),
    };
    if (
      form.unidadeCobranca !== String(UnidadeCobranca.ACombinar) &&
      form.precoBase
    ) {
      payload.precoBase = Number(form.precoBase.toString().replace(",", "."));
    }
    if (form.data) {
      payload.dataDesejada = new Date(form.data).toISOString();
    }

    await api.post("/api/ServicoOferecido/CriarServicoCliente", payload);
    sucesso.value = true;
  } catch (err: unknown) {
    const e = err as { response?: { data?: unknown } };
    const msg = e.response?.data;
    erro.value =
      typeof msg === "string" && msg
        ? msg
        : "Erro ao publicar o anúncio. Verifique os dados e tente novamente.";
  } finally {
    carregando.value = false;
  }
}

function reiniciar() {
  form.titulo = "";
  form.descricao = "";
  form.categoria = "";
  form.unidadeCobranca = "";
  form.precoBase = "";
  form.data = "";
  erro.value = null;
  sucesso.value = false;
}

defineExpose({ form, handleSubmit, sucesso, erro });
</script>
