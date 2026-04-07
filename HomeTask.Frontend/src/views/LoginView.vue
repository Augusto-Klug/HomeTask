<template>
  <div class="min-h-[calc(100vh-7rem)] flex items-center justify-center px-4 py-12">
    <div class="w-full max-w-sm">
      <!-- Logo -->
      <div class="text-center mb-8">
        <router-link to="/" class="text-2xl font-bold text-primary">HomeTask</router-link>
        <p class="text-sm text-muted mt-1">Acesse sua conta</p>
      </div>

      <HtCard>
        <HtAlert v-if="erro" :message="erro" class="mb-4" />

        <form class="flex flex-col gap-4" @submit.prevent="handleLogin">
          <HtInput
            ref="inputEmail"
            v-model="email"
            label="E-mail"
            type="email"
            regra="email"
            placeholder="seu@email.com"
            required
          />

          <HtInput
            ref="inputSenha"
            v-model="senha"
            label="Senha"
            type="password"
            regra="senha"
            placeholder="••••••"
            required
          />

          <HtButton type="submit" :loading="carregando" class="w-full mt-2">
            Entrar
          </HtButton>
        </form>

        <HtDivider />

        <div class="text-center text-sm text-muted flex flex-col gap-2">
          <span>
            Não tem conta?
            <router-link to="/cadastro" class="text-primary font-medium hover:underline">Cadastre-se</router-link>
          </span>
          <router-link to="/esqueci-senha" class="text-primary hover:underline">
            Esqueci minha senha
          </router-link>
        </div>
      </HtCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { validarCampos } from '@/shared/validacao'
import HtInput from '@/components/ui/HtInput.vue'
import HtButton from '@/components/ui/HtButton.vue'
import HtCard from '@/components/ui/HtCard.vue'
import HtAlert from '@/components/ui/HtAlert.vue'
import HtDivider from '@/components/ui/HtDivider.vue'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const email = ref('')
const senha = ref('')
const carregando = ref(false)
const erro = ref<string | null>(null)

const inputEmail = ref<InstanceType<typeof HtInput> | null>(null)
const inputSenha = ref<InstanceType<typeof HtInput> | null>(null)

async function handleLogin() {
  if (!validarCampos([inputEmail.value, inputSenha.value])) return

  erro.value = null
  carregando.value = true
  try {
    await auth.login(email.value, senha.value)
    const redirect = (route.query.redirect as string) ?? '/'
    router.push(redirect)
  } catch (err: unknown) {
    const e = err as { response?: { status?: number } }
    erro.value = e.response?.status === 401
      ? 'E-mail ou senha inválidos.'
      : 'Erro ao conectar com o servidor. Tente novamente.'
  } finally {
    carregando.value = false
  }
}
</script>
