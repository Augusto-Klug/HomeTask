<template>
  <div class="container mx-auto px-4 py-8 max-w-2xl">
    <div class="flex items-center justify-between mb-8">
      <h1 class="text-2xl font-bold">Minha conta</h1>
      <button
        v-if="!editando && !carregando && abaAtiva === 'informacoes'"
        type="button"
        data-testid="btn-editar"
        class="btn btn-ghost btn-sm btn-square"
        title="Editar dados"
        @click="iniciarEdicao"
      >
        <span class="material-symbols-rounded text-xl">edit</span>
      </button>
    </div>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <template v-else>
      <HtAlert v-if="erro" variant="error" :message="erro" class="mb-4" />
      <HtAlert
        v-if="sucesso"
        variant="success"
        message="Dados salvos com sucesso!"
        class="mb-4"
      />

      <div v-if="isPrestador" class="tabs tabs-bordered mb-6">
        <input
          type="radio"
          name="minha_conta_tabs"
          class="tab"
          aria-label="Informações"
          :checked="abaAtiva === 'informacoes'"
          @change="selecionarAba('informacoes')"
        />
        <div class="tab-content p-0"></div>

        <input
          type="radio"
          name="minha_conta_tabs"
          class="tab"
          aria-label="Certificações"
          :checked="abaAtiva === 'certificacoes'"
          @change="selecionarAba('certificacoes')"
        />
        <div class="tab-content p-0"></div>

        <input
          type="radio"
          name="minha_conta_tabs"
          class="tab"
          aria-label="Portfólio"
          :checked="abaAtiva === 'portfolio'"
          @change="selecionarAba('portfolio')"
        />
        <div class="tab-content p-0"></div>

        <input
          type="radio"
          name="minha_conta_tabs"
          class="tab"
          aria-label="Recebimentos"
          :checked="abaAtiva === 'recebimentos'"
          @change="selecionarAba('recebimentos')"
        />
        <div class="tab-content p-0"></div>
      </div>

      <form
        v-if="abaAtiva === 'informacoes'"
        class="flex flex-col gap-4"
        @submit.prevent="salvar"
      >
        <section>
          <h2 class="text-xs font-semibold text-base-content/50 uppercase tracking-wide mb-3">
            Dados pessoais
          </h2>
          <div class="flex flex-col gap-3">
            <HtInput
              v-model="form.nome"
              label="Nome completo"
              :disabled="!editando"
            />
            <HtInput
              v-model="form.email"
              label="E-mail"
              type="email"
              :disabled="!editando"
            />
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <HtInput
                v-model="form.documento"
                label="CPF / CNPJ"
                regra="documento"
                :disabled="!editando"
              />
              <HtInput
                v-model="form.telefone"
                label="Telefone"
                regra="telefone"
                :disabled="!editando"
              />
            </div>
          </div>
        </section>

        <section>
          <h2 class="text-xs font-semibold text-base-content/50 uppercase tracking-wide mb-3">
            Endereço
          </h2>
          <div class="flex flex-col gap-3">
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
              <div class="sm:col-span-1">
                <HtInput
                  v-model="form.cep"
                  label="CEP"
                  regra="cep"
                  :disabled="!editando"
                />
              </div>
              <div class="sm:col-span-2">
                <HtInput
                  v-model="form.logradouro"
                  label="Logradouro"
                  :disabled="!editando"
                />
              </div>
            </div>
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
              <div class="sm:col-span-2">
                <HtInput
                  v-model="form.bairro"
                  label="Bairro"
                  :disabled="!editando"
                />
              </div>
              <HtSearchSelect
                v-model="form.estado"
                label="Estado"
                :options="UF_OPTIONS"
                required
                :disabled="!editando"
                placeholder="Busque pela UF"
              />
            </div>
            <HtSelect
              v-model="form.cidadeId"
              label="Cidade"
              :options="cidadesDisponiveis"
              :placeholder="form.estado ? 'Selecione a cidade' : 'Selecione primeiro o estado'"
              :hint="cidadeHint"
              :disabled="!editando || !form.estado || carregandoCidades"
            />
          </div>
        </section>

        <section v-if="isPrestador">
          <h2 class="text-xs font-semibold text-base-content/50 uppercase tracking-wide mb-3">
            Perfil profissional
          </h2>
          <div class="flex flex-col gap-3">
            <HtInput
              v-model="form.descricao"
              label="Descrição dos serviços"
              :disabled="!editando"
            />
            <HtInput
              v-model.number="form.raioAtendimentoKm"
              label="Raio de atendimento (km)"
              type="number"
              :disabled="!editando"
            />
          </div>
        </section>

        <div v-if="editando" class="flex gap-3 justify-end pt-2">
          <HtButton variant="ghost" type="button" @click="editando = false">
            Cancelar
          </HtButton>
          <HtButton
            data-testid="btn-salvar"
            variant="primary"
            type="submit"
            :loading="salvando"
          >
            Salvar
          </HtButton>
        </div>
      </form>

      <div v-if="abaAtiva === 'certificacoes' && isPrestador" class="pt-4">
        <HtCertificacaoForm
          :prestador-id="prestadorId"
          :is-prestador="isPrestador"
          :certificacoes="form.certificacoes || []"
          @certificacao-adicionada="onCertificacaoAdicionada"
          @certificacao-removida="onCertificacaoRemovida"
          @certificacao-clicada="onCertificacaoClicada"
        />
      </div>

      <div v-if="abaAtiva === 'portfolio' && isPrestador" class="pt-4">
        <HtPortfolioGaleria
          :prestador-id="prestadorId"
          :is-prestador="isPrestador"
          :portfolios="form.portfolios || []"
          @portfolio-adicionado="onPortfolioAdicionado"
          @portfolio-removido="onPortfolioRemovido"
        />
      </div>

      <div v-if="abaAtiva === 'recebimentos' && isPrestador" class="pt-4">
        <HtPagamentosRecebidos
          :resumo="recebimentos"
          :carregando="carregandoRecebimentos"
        />
      </div>
    </template>

    <HtCertificacaoDetail
      :certificacao="certificacaoSelecionada"
      :is-open="mostrarDetalhesCertificacao"
      @close="mostrarDetalhesCertificacao = false"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from "vue";
import { useAuthStore } from "@/stores/auth";
import api from "@/services/api";
import type {
  Certificacao,
  PerfilForm,
  Portfolio,
  PrestadorRecebimentosResumo,
} from "@/types";
import HtInput from "@/components/ui/HtInput.vue";
import HtSearchSelect from "@/components/ui/HtSearchSelect.vue";
import HtSelect from "@/components/ui/HtSelect.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtAlert from "@/components/ui/HtAlert.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import HtCertificacaoForm from "./components/HtCertificacaoForm.vue";
import HtPortfolioGaleria from "./components/HtPortfolioGaleria.vue";
import HtCertificacaoDetail from "./components/HtCertificacaoDetail.vue";
import HtPagamentosRecebidos from "./components/HtPagamentosRecebidos.vue";
import { useCidadeEstadoOptions } from "@/composables/useCidadeEstadoOptions";
import { UF_OPTIONS } from "@/statics/selects";

const auth = useAuthStore();

const carregando = ref(true);
const editando = ref(false);
const salvando = ref(false);
const erro = ref("");
const sucesso = ref(false);
const abaAtiva = ref<"informacoes" | "certificacoes" | "portfolio" | "recebimentos">("informacoes");
const carregandoRecebimentos = ref(false);

const prestadorId = ref("");
const certificacaoSelecionada = ref<any | null>(null);
const mostrarDetalhesCertificacao = ref(false);

const form = reactive<PerfilForm>({
  nome: "",
  email: "",
  telefone: "",
  documento: "",
  cep: "",
  logradouro: "",
  bairro: "",
  cidade: "",
  cidadeId: "",
  estado: "",
  descricao: "",
  raioAtendimentoKm: null,
  certificacoes: [],
  portfolios: [],
});

const recebimentos = reactive<PrestadorRecebimentosResumo>({
  saldoRecebidoTotal: 0,
  totalServicosRecebidos: 0,
  servicosRecebidos: [],
});
const {
  carregandoCidades,
  cidadesDisponiveis,
  cidadeHint,
  carregarCidades,
} = useCidadeEstadoOptions(() => form.estado);

const isPrestador = computed(
  () => auth.user?.tipo === 2 || auth.user?.tipo === 3,
);

async function carregarDados() {
  try {
    const { data } = await api.get<PerfilForm>(
      "/api/Usuario/ObterPerfilUsuario",
    );
    Object.assign(form, data);

    if (isPrestador.value && auth.user?.userId) {
      try {
        const prestadorResponse = await api.get<{ id: string }>(
          "/api/Prestador/ObterPrestadorPorUsuarioId",
          {
            params: { usuarioId: auth.user.userId },
          },
        );
        prestadorId.value = prestadorResponse.data.id;
      } catch {
        // Silenciar erro de busca de prestador
      }
    }
  } catch {
    erro.value = "Não foi possível carregar seus dados.";
  }
}

async function carregarRecebimentos() {
  if (!prestadorId.value) {
    return;
  }

  carregandoRecebimentos.value = true;

  try {
    const { data } = await api.get<PrestadorRecebimentosResumo>(
      "/api/Prestador/ObterRecebimentos",
      {
        params: { prestadorId: prestadorId.value },
      },
    );

    recebimentos.saldoRecebidoTotal = data.saldoRecebidoTotal;
    recebimentos.totalServicosRecebidos = data.totalServicosRecebidos;
    recebimentos.servicosRecebidos = data.servicosRecebidos;
  } catch {
    erro.value = "Não foi possível carregar os recebimentos.";
  } finally {
    carregandoRecebimentos.value = false;
  }
}

onMounted(async () => {
  await carregarCidades();
  await carregarDados();
  carregando.value = false;
});

watch(
  () => form.estado,
  (estadoAtual, estadoAnterior) => {
    const cidadeSelecionadaPermaneceValida = cidadesDisponiveis.value.some(
      (cidade) => cidade.value === form.cidadeId,
    );

    if (!cidadeSelecionadaPermaneceValida) {
      form.cidadeId = "";
      form.cidade = "";
    }

    if (estadoAnterior && estadoAtual !== estadoAnterior) {
      sucesso.value = false;
    }
  },
);

watch(
  () => form.cidadeId,
  (cidadeId) => {
    const cidadeSelecionada = cidadesDisponiveis.value.find(
      (cidade) => cidade.value === cidadeId,
    );
    form.cidade = cidadeSelecionada?.label ?? "";
  },
);

watch(abaAtiva, async (aba) => {
  if ((aba === "certificacoes" || aba === "portfolio")
    && form.certificacoes?.length === 0
    && form.portfolios?.length === 0) {
    await carregarDados();
  }

  if (aba === "recebimentos") {
    await carregarRecebimentos();
  }
});

function iniciarEdicao() {
  editando.value = true;
  sucesso.value = false;
  erro.value = "";
}

function selecionarAba(aba: "informacoes" | "certificacoes" | "portfolio" | "recebimentos") {
  abaAtiva.value = aba;
}

async function salvar() {
  salvando.value = true;
  erro.value = "";
  try {
    await api.put("/api/Usuario/AtualizarPrefilUsuario", { ...form });
    sucesso.value = true;
    editando.value = false;
  } catch (e: any) {
    erro.value =
      e.response?.data?.message ?? "Erro ao salvar. Tente novamente.";
  } finally {
    salvando.value = false;
  }
}

function onCertificacaoAdicionada(cert: Certificacao) {
  if (!form.certificacoes) {
    form.certificacoes = [];
  }
  form.certificacoes.push(cert);
}

function onCertificacaoRemovida(id: string) {
  if (form.certificacoes) {
    form.certificacoes = form.certificacoes.filter((c) => c.id !== id);
  }
}

function onPortfolioAdicionado(portfolio: Portfolio) {
  if (!form.portfolios) {
    form.portfolios = [];
  }
  form.portfolios.push(portfolio);
}

function onPortfolioRemovido(id: string) {
  if (form.portfolios) {
    form.portfolios = form.portfolios.filter((p) => p.id !== id);
  }
}

function onCertificacaoClicada(cert: Certificacao) {
  certificacaoSelecionada.value = cert;
  mostrarDetalhesCertificacao.value = true;
}
</script>
