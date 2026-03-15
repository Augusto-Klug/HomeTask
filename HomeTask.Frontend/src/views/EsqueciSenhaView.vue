<template lang="pug">
.padding.center-align(style="min-height: 100vh; display: flex; flex-direction: column; justify-content: center; align-items: center;")
  router-link.bold.large(to="/") HomeTask
  p.small.padding-bottom Recuperação de senha

  article.padding(style="width: 100%; max-width: 400px;")
    template(v-if="!enviado")
      p.padding-bottom Informe seu e-mail e enviaremos um link para redefinir sua senha.

      .field.border(v-if="erro")
        p.error {{ erro }}

      form(@submit.prevent="handleEnviar")
        .field.label.border
          input(v-model="email" type="email" required)
          label E-mail

        button.responsive(:disabled="carregando" type="submit")
          .progress.circle.small(v-if="carregando")
          span(v-else) Enviar link de recuperação

      .center-align.small.padding-top
        router-link(to="/login") Voltar para o login

    template(v-else)
      i.extra.primary mark_email_read
      h5.padding-top Link enviado!
      p Verifique sua caixa de entrada e clique no link para redefinir sua senha.
      router-link.button.padding-top(to="/login") Voltar para o login
</template>

<script setup lang="ts">
import { ref } from 'vue'
import api from '@/services/api'

const email = ref('')
const carregando = ref(false)
const erro = ref<string | null>(null)
const enviado = ref(false)

async function handleEnviar() {
  erro.value = null
  carregando.value = true
  try {
    await api.post('/api/Auth/EsqueciSenha', { email: email.value })
    enviado.value = true
  } catch (err: unknown) {
    const e = err as { response?: { status?: number; data?: unknown } }
    if (e.response?.status === 404) {
      erro.value = 'E-mail não encontrado.'
    } else {
      erro.value = 'Erro ao enviar o e-mail. Tente novamente.'
    }
  } finally {
    carregando.value = false
  }
}
</script>
