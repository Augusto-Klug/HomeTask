<template>
  <div class="min-h-[calc(100vh-7rem)] flex items-center justify-center px-4 py-12">
    <div class="w-full max-w-sm">
      <div class="text-center mb-8">
        <router-link to="/" class="text-2xl font-bold text-primary">HomeTask</router-link>
        <p class="text-sm text-muted mt-1">Redefinição de senha</p>
      </div>

      <!-- Token inválido -->
      <HtCard v-if="!token">
        <div class="text-center py-4 flex flex-col items-center gap-3">
          <span class="material-symbols-rounded text-5xl text-error">error</span>
          <p class="text-sm text-muted">Link inválido ou expirado. Solicite um novo link de recuperação.</p>
          <router-link to="/esqueci-senha">
            <HtButton class="mt-2">Solicitar novo link</HtButton>
          </router-link>
        </div>
      </HtCard>

      <HtCard v-else>
        <!-- Concluído -->
        <template v-if="concluido">
          <div class="text-center py-4 flex flex-col items-center gap-3">
            <span class="material-symbols-rounded text-5xl text-success">check_circle</span>
            <h2 class="text-title font-semibold text-foreground">Senha redefinida!</h2>
            <p class="text-sm text-muted">Sua senha foi alterada com sucesso.</p>
            <router-link to="/login">
              <HtButton class="mt-2">
                <span class="material-symbols-rounded text-base">login</span>
                Fazer Login
              </HtButton>
            </router-link>
          </div>
        </template>

        <!-- Formulário -->
        <template v-else>
          <HtAlert v-if="erro" :message="erro" class="mb-4" />

          <form class="flex flex-col gap-4" @submit.prevent="handleRedefinir">
            <HtInput
              ref="inputNovaSenha"
              v-model="novaSenha"
              label="Nova senha"
              type="password"
              regra="senha"
              placeholder="Mínimo 6 caracteres"
              required
            />

            <HtInput
              ref="inputConfirmar"
              v-model="confirmarSenha"
              label="Confirmar senha"
              type="password"
              regra="senha"
              placeholder="Repita a senha"
              required
            />

            <HtButton type="submit" :loading="carregando" class="w-full mt-2">
              Redefinir senha
            </HtButton>
          </form>
        </template>
      </HtCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'
import { validarCampos } from '@/shared/validacao'
import HtInput from '@/components/ui/HtInput.vue'
import HtButton from '@/components/ui/HtButton.vue'
import HtCard from '@/components/ui/HtCard.vue'
import HtAlert from '@/components/ui/HtAlert.vue'

const route = useRoute()

const token = ref<string | null>(null)
const novaSenha = ref('')
const confirmarSenha = ref('')
const carregando = ref(false)
const erro = ref<string | null>(null)
const concluido = ref(false)

const inputNovaSenha = ref<InstanceType<typeof HtInput> | null>(null)
const inputConfirmar = ref<InstanceType<typeof HtInput> | null>(null)

onMounted(() => {
  token.value = (route.query.token as string) ?? null
})

async function handleRedefinir() {
  if (!validarCampos([inputNovaSenha.value, inputConfirmar.value])) return

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
    const e = err as { response?: { status?: number } }
    erro.value = e.response?.status === 400
      ? 'Token inválido ou expirado. Solicite um novo link.'
      : 'Erro ao redefinir a senha. Tente novamente.'
  } finally {
    carregando.value = false
  }
}
</script>
