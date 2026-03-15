<template lang="pug">
.padding.center-align(style="min-height: 100vh; display: flex; flex-direction: column; justify-content: center; align-items: center;")
  router-link.bold.large(to="/") HomeTask
  p.small.padding-bottom Redefinição de senha

  //- Token inválido ou ausente
  article.padding(v-if="!token" style="width: 100%; max-width: 400px;")
    i.extra.error error
    p Link inválido ou expirado. Solicite um novo link de recuperação.
    router-link.button.padding-top(to="/esqueci-senha") Solicitar novo link

  article.padding(v-else style="width: 100%; max-width: 400px;")
    template(v-if="!concluido")
      .field.border(v-if="erro")
        p.error {{ erro }}

      form(@submit.prevent="handleRedefinir")
        .field.label.border
          input(v-model="novaSenha" :type="mostrarSenha ? 'text' : 'password'" required minlength="6")
          label Nova senha
          button.transparent.circle(type="button" @click="mostrarSenha = !mostrarSenha")
            i {{ mostrarSenha ? 'visibility_off' : 'visibility' }}

        .field.label.border
          input(v-model="confirmarSenha" :type="mostrarSenha ? 'text' : 'password'" required minlength="6")
          label Confirmar senha

        button.responsive(:disabled="carregando" type="submit")
          .progress.circle.small(v-if="carregando")
          span(v-else) Redefinir senha

    template(v-else)
      i.extra.primary check_circle
      h5.padding-top Senha redefinida!
      p Sua senha foi alterada com sucesso.
      router-link.button.padding-top(to="/login")
        i login
        span Fazer Login
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'

const route = useRoute()

const token = ref<string | null>(null)
const novaSenha = ref('')
const confirmarSenha = ref('')
const mostrarSenha = ref(false)
const carregando = ref(false)
const erro = ref<string | null>(null)
const concluido = ref(false)

onMounted(() => {
  token.value = (route.query.token as string) ?? null
})

async function handleRedefinir() {
  if (novaSenha.value !== confirmarSenha.value) {
    erro.value = 'As senhas não coincidem.'
    return
  }
  erro.value = null
  carregando.value = true
  try {
    await api.post('/api/Auth/RedefinirSenha', {
      token: token.value,
      novaSenha: novaSenha.value,
    })
    concluido.value = true
  } catch (err: unknown) {
    const e = err as { response?: { status?: number; data?: unknown } }
    if (e.response?.status === 400) {
      erro.value = 'Token inválido ou expirado. Solicite um novo link.'
    } else {
      erro.value = 'Erro ao redefinir a senha. Tente novamente.'
    }
  } finally {
    carregando.value = false
  }
}
</script>
