/**
 * auth.ts — Pinia store de autenticação.
 * Os tokens (access + refresh) ficam em cookies HttpOnly gerenciados pelo servidor.
 * O store mantém apenas os dados do usuário logado em memória.
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, AuthResponse } from '@/types'
import api from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)

  const isLoggedIn = computed(() => user.value !== null)

  function setUser(data: AuthResponse) {
    user.value = {
      userId: data.userId,
      nome: data.nome,
      email: data.email,
      tipo: data.tipo,
    }
  }

  async function login(email: string, senha: string): Promise<void> {
    const { data } = await api.post<AuthResponse>('/api/Auth/Login', { email, senha })
    setUser(data)
  }

  async function logout(): Promise<void> {
    try {
      await api.post('/api/Auth/Logout')
    } finally {
      user.value = null
    }
  }

  async function fetchMe(): Promise<void> {
    try {
      const { data } = await api.get<AuthResponse>('/api/Auth/Me')
      setUser(data)
    } catch {
      user.value = null
    }
  }

  return { user, isLoggedIn, login, logout, fetchMe }
})
