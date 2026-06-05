import type { UnidadeCobranca } from "@/types";
import { UnidadeCobranca as UnidadeCobrancaEnum } from "@/types";

type DateLike = string | Date;

export function obterInicialNome(nome?: string | null): string {
  return nome?.trim().charAt(0).toUpperCase() || "?";
}

export function formatarMoeda(valor: number | null | undefined): string {
  return Number(valor ?? 0).toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL",
  });
}

export function formatarData(valor: DateLike, options?: Intl.DateTimeFormatOptions): string {
  return criarData(valor).toLocaleDateString("pt-BR", options);
}

export function formatarDataCurta(valor: DateLike): string {
  return formatarData(valor, {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

export function formatarDataLonga(valor: DateLike): string {
  return formatarData(valor, {
    weekday: "long",
    day: "2-digit",
    month: "long",
    year: "numeric",
  });
}

export function formatarHora(valor: DateLike): string {
  return criarData(valor).toLocaleTimeString("pt-BR", {
    hour: "2-digit",
    minute: "2-digit",
  });
}

export function obterDescricaoCobranca(unidadeCobranca: UnidadeCobranca): string {
  switch (unidadeCobranca) {
    case UnidadeCobrancaEnum.PorHora:
      return "/ hora";
    case UnidadeCobrancaEnum.ACombinar:
      return "a combinar";
    default:
      return "/ total";
  }
}

export function formatarPrecoServico(valor: number, unidadeCobranca: UnidadeCobranca): string {
  if (unidadeCobranca === UnidadeCobrancaEnum.ACombinar) {
    return "A combinar";
  }

  return `${formatarMoeda(valor)} ${obterDescricaoCobranca(unidadeCobranca)}`;
}

export async function logoutHandler(
  logout: () => Promise<void>,
  redirect: () => Promise<unknown> | unknown,
  beforeLogout?: () => void,
): Promise<void> {
  beforeLogout?.();
  await logout();
  await Promise.resolve(redirect());
}

function criarData(valor: DateLike): Date {
  return valor instanceof Date ? new Date(valor) : new Date(valor);
}

export default {
  fecharSidenav(setter: (v: boolean) => void) {
    setter(false);
  },
  abrirSidenav(setter: (v: boolean) => void) {
    setter(true);
  },
  toggleSidenav(setter: (v: boolean) => void, current: boolean) {
    setter(!current);
  },
};
