<template lang="pug">
.padding.center-align(style="min-height: 100vh; display: flex; flex-direction: column; justify-content: center; align-items: center;")
  router-link.bold.large(to="/") HomeTask
  p.small.padding-bottom Crie sua conta gratuitamente

  article.padding(style="width: 100%; max-width: 520px;")
    .field.border(v-if="erro")
      p.error {{ erro }}

    form(@submit.prevent="handleCadastro")
      //- Tipo de conta
      p.bold Tipo de conta
      .row.wrap.padding-bottom
        label.radio
          input(type="radio" v-model="form.tipoUsuario" value="1")
          span 🏠 Cliente
        label.radio
          input(type="radio" v-model="form.tipoUsuario" value="2")
          span 🔧 Prestador
        label.radio
          input(type="radio" v-model="form.tipoUsuario" value="3")
          span ⭐ Ambos

      //- Dados básicos
      .field.label.border
        input(v-model="form.nome" type="text" required)
        label Nome completo

      .field.label.border
        input(v-model="form.email" type="email" required)
        label E-mail

      .field.label.border
        input(v-model="form.documento" type="text" required maxlength="20")
        label Documento

      .field.label.border
        input(v-model="form.telefone" type="tel")
        label Telefone

      .field.label.border
        input(v-model="form.senha" type="password" required minlength="6")
        label Senha

      .divider
      p.bold.small Endereço

        .field.label.border
          input(v-model="form.cep" type="cep" required maxlength="10")
          label CEP

        .field.label.border.max
          input(v-model="form.endereco" type="text" required)
          label Logradouro

        .field.label.border
          input(v-model="form.bairro" type="text" required)
          label Bairro

        .field.label.border
          input(v-model="form.cidade" type="text" required)
          label Cidade

        .field.label.border.max
          input(v-model="form.estado" type="text" maxlength="2" required)
          label Estado

      //- Dados de prestador
      template(v-if="form.tipoUsuario === '2' || form.tipoUsuario === '3'")
        .divider
        p.bold.small Dados profissionais

        .field.label.textarea.border
          textarea(v-model="form.descricao" rows="3")
          label Descrição profissional

      button.responsive.padding-top(:disabled="carregando" type="submit")
        .progress.circle.small(v-if="carregando")
        span(v-else) Criar Conta

    .divider
    .center-align.small
      | Já tem conta?&nbsp;
      router-link(to="/login") Entrar
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import type { CadastroForm } from '@/types'

const router = useRouter()

const form = reactive<CadastroForm>({
  tipoUsuario: '1',
  nome: '',
  email: '',
  documento: '',
  telefone: '',
  senha: '',
  cep: '',
  endereco: '',
  bairro: '',
  cidade: '',
  estado: '',
  raioAtendimentoKm: 10,
  descricao: '',
})

const carregando = ref(false)
const erro = ref<string | null>(null)

async function handleCadastro() {
  erro.value = null
  carregando.value = true
  try {
    const { data: usuario } = await api.post('/api/Usuario/CriarUsuario', {
      nome: form.nome,
      email: form.email,
      documento: form.documento,
      telefone: form.telefone,
      senha: form.senha,
      tipo: Number(form.tipoUsuario),
      cep: form.cep,
      endereco: form.endereco,
      bairro: form.bairro,
      cidade: form.cidade,
      estado: form.estado,
    })

    if (form.tipoUsuario === '1' || form.tipoUsuario === '3') {
      await api.post('/api/Cliente/CriarCliente', { usuarioId: usuario.id })
    }

    if (form.tipoUsuario === '2' || form.tipoUsuario === '3') {
      await api.post('/api/Prestador/CriarPrestador', {
        usuarioId: usuario.id,
        raioAtendimentoKm: form.raioAtendimentoKm,
        descricao: form.descricao,
      })
    }

    router.push('/cadastro-sucesso')
  } catch (err: unknown) {
    const e = err as { response?: { status?: number; data?: unknown } }
    const status = e.response?.status
    const msg = e.response?.data
    if (status === 409) {
      erro.value = typeof msg === 'string' ? msg : 'E-mail ou Documento já cadastrado.'
    } else if (status === 400) {
      erro.value = typeof msg === 'string' ? msg : 'Dados inválidos. Verifique o formulário.'
    } else {
      erro.value = 'Erro ao cadastrar. Tente novamente em instantes.'
    }
  } finally {
    carregando.value = false
  }
}
</script>
