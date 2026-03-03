<template lang="pug">
.padding.center-align(style="min-height: 100vh; display: flex; flex-direction: column; justify-content: center; align-items: center;")
  router-link.bold.large(to="/") HomeTask
  p.small.padding-bottom Acesse sua conta

  article.padding(style="width: 100%; max-width: 400px;")
    .field.border(v-if="erro")
      p.error {{ erro }}

    form(@submit.prevent="handleLogin")
      .field.label.border
        input(v-model="email" type="email" required)
        label E-mail

      .field.label.border
        input(v-model="senha" :type="mostrarSenha ? 'text' : 'password'" required)
        label Senha
        button.transparent.circle(type="button" @click="mostrarSenha = !mostrarSenha")
          i {{ mostrarSenha ? 'visibility_off' : 'visibility' }}

      button.responsive(:disabled="carregando" type="submit")
        .progress.circle.small(v-if="carregando")
        span(v-else) Entrar

    .divider
    .center-align.small
      | Não tem conta?&nbsp;
      router-link(to="/cadastro") Cadastre-se
    .center-align.small.padding-top
      router-link(to="/esqueci-senha") Esqueci minha senha
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const email = ref('')
const senha = ref('')
const mostrarSenha = ref(false)
const carregando = ref(false)
const erro = ref<string | null>(null)

async function handleLogin() {
  erro.value = null
  carregando.value = true
  try {
    await auth.login(email.value, senha.value)
    const redirect = (route.query.redirect as string) ?? '/'
    router.push(redirect)
  } catch (err: unknown) {
    const e = err as { response?: { status?: number } }
    if (e.response?.status === 401) {
      erro.value = 'E-mail ou senha inválidos.'
    } else {
      erro.value = 'Erro ao conectar com o servidor. Tente novamente.'
    }
  } finally {
    carregando.value = false
  }
}
</script>
