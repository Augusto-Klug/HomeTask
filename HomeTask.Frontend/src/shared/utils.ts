/** Utilitários de sidenav e formatação compartilhada */
import type { UnidadeCobranca } from '@/types'
import { UnidadeCobranca as UnidadeCobrancaEnum } from '@/types'

export function obterInicialNome(nome?: string | null): string {
  return nome?.trim().charAt(0).toUpperCase() || '?'
}

export function formatarMoeda(valor: number | null | undefined): string {
  return Number(valor ?? 0).toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  })
}

export function obterDescricaoCobranca(unidadeCobranca: UnidadeCobranca): string {
  switch (unidadeCobranca) {
    case UnidadeCobrancaEnum.PorHora:
      return '/ hora'
    case UnidadeCobrancaEnum.ACombinar:
      return 'a combinar'
    default:
      return '/ total'
  }
}

export function formatarPrecoServico(valor: number, unidadeCobranca: UnidadeCobranca): string {
  if (unidadeCobranca === UnidadeCobrancaEnum.ACombinar) {
    return 'A combinar'
  }

  return `${formatarMoeda(valor)} ${obterDescricaoCobranca(unidadeCobranca)}`
}

export default {
  fecharSidenav(setter: (v: boolean) => void) {
    setter(false)
  },
  abrirSidenav(setter: (v: boolean) => void) {
    setter(true)
  },
  toggleSidenav(setter: (v: boolean) => void, current: boolean) {
    setter(!current)
  },
}
