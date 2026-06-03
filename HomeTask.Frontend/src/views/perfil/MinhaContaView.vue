<template>
  <div class="container mx-auto px-4 py-8 max-w-2xl">
    <!-- Cabeçalho -->
    <div class="flex items-center justify-between mb-8">
      <h1 class="text-2xl font-bold">Minha conta</h1>
      <button
        v-if="!editando && !carregando"
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

      <form @submit.prevent="salvar" class="flex flex-col gap-4">
        <section>
          <h2
            class="text-xs font-semibold text-base-content/50 uppercase tracking-wide mb-3"
          >
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
          <h2
            class="text-xs font-semibold text-base-content/50 uppercase tracking-wide mb-3"
          >
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
              />
            </div>
            <HtInput
              v-model="form.cidade"
              label="Cidade"
              :disabled="!editando"
            />
          </div>
        </section>

        <section v-if="isPrestador">
          <h2
            class="text-xs font-semibold text-base-content/50 uppercase tracking-wide mb-3"
          >
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
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from "vue";
import { useAuthStore } from "@/stores/auth";
import api from "@/services/api";
import type { PerfilForm } from "@/types";
import HtInput from "@/components/ui/HtInput.vue";
import HtSearchSelect from "@/components/ui/HtSearchSelect.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtAlert from "@/components/ui/HtAlert.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import { UF_OPTIONS } from "@/statics/selects";

const auth = useAuthStore();

const carregando = ref(true);
const editando = ref(false);
const salvando = ref(false);
const erro = ref("");
const sucesso = ref(false);

const form = reactive<PerfilForm>({
  nome: "",
  email: "",
  telefone: "",
  documento: "",
  cep: "",
  logradouro: "",
  bairro: "",
  cidade: "",
  estado: "",
  descricao: "",
  raioAtendimentoKm: null,
});

const isPrestador = computed(
  () => auth.user?.tipo === 2 || auth.user?.tipo === 3,
);

onMounted(async () => {
  try {
    const { data } = await api.get<PerfilForm>(
      "/api/Usuario/ObterPerfilUsuario",
    );
    Object.assign(form, data);
  } catch {
    erro.value = "Não foi possível carregar seus dados.";
  } finally {
    carregando.value = false;
  }
});

function iniciarEdicao() {
  editando.value = true;
  sucesso.value = false;
  erro.value = "";
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
</script>
