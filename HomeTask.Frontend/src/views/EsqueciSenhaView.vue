<template>
  <div class="min-h-[calc(100vh-7rem)] flex items-center justify-center px-4 py-12">
    <div class="w-full max-w-sm">
      <div class="text-center mb-8">
        <router-link to="/" class="text-2xl font-bold text-primary">HomeTask</router-link>
        <p class="text-sm text-muted mt-1">Recuperação de senha</p>
      </div>

      <HtCard>
        <!-- Sucesso -->
        <template v-if="enviado">
          <div class="text-center py-4 flex flex-col items-center gap-3">
            <span class="material-symbols-rounded text-5xl text-success">mark_email_read</span>
            <h2 class="text-title font-semibold text-foreground">Link enviado!</h2>
            <p class="text-sm text-muted">Verifique sua caixa de entrada e clique no link para redefinir sua senha.</p>
            <router-link to="/login">
              <HtButton variant="outline" class="mt-2">Voltar para o login</HtButton>
            </router-link>
          </div>
        </template>

        <!-- Formulário -->
        <template v-else>
          <p class="text-sm text-muted mb-4">Informe seu e-mail e enviaremos um link para redefinir sua senha.</p>

          <HtAlert v-if="erro" :message="erro" class="mb-4" />

          <form class="flex flex-col gap-4" @submit.prevent="handleEnviar">
            <HtInput
              ref="inputEmail"
              v-model="email"
              label="E-mail"
              type="email"
              regra="email"
              placeholder="seu@email.com"
              required
            />

            <HtButton type="submit" :loading="carregando" class="w-full">
              Enviar link de recuperação
            </HtButton>
          </form>

          <HtDivider />

          <div class="text-center">
            <router-link to="/login" class="text-sm text-primary hover:underline">
              Voltar para o login
            </router-link>
          </div>
        </template>
      </HtCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import api from '@/services/api'
import { validarCampos } from '@/shared/validacao'
import HtInput from '@/components/ui/HtInput.vue'
import HtButton from '@/components/ui/HtButton.vue'
import HtCard from '@/components/ui/HtCard.vue'
import HtAlert from '@/components/ui/HtAlert.vue'
import HtDivider from '@/components/ui/HtDivider.vue'

const email = ref('')
const carregando = ref(false)
const erro = ref<string | null>(null)
const enviado = ref(false)
const inputEmail = ref<InstanceType<typeof HtInput> | null>(null)

async function handleEnviar() {
  if (!validarCampos([inputEmail.value])) return

  erro.value = null
  carregando.value = true
  try {
    await api.post('/api/Auth/EsqueciSenha', { email: email.value }, { skipAuthRedirect: true })
    enviado.value = true
  } catch {
    erro.value = 'Erro ao enviar o e-mail. Tente novamente.'
  } finally {
    carregando.value = false
  }
}
</script>
