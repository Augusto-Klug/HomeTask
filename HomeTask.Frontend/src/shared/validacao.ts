// ─── Máscaras ──────────────────────────────────────────────────────────────

export function mascaraCPF(v: string): string {
  return v
    .replace(/\D/g, '')
    .slice(0, 11)
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d{1,2})$/, '$1-$2')
}

export function mascaraCNPJ(v: string): string {
  return v
    .replace(/\D/g, '')
    .slice(0, 14)
    .replace(/(\d{2})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1/$2')
    .replace(/(\d{4})(\d{1,2})$/, '$1-$2')
}

export function mascaraCEP(v: string): string {
  return v
    .replace(/\D/g, '')
    .slice(0, 8)
    .replace(/(\d{5})(\d{1,3})$/, '$1-$2')
}

export function mascaraTelefone(v: string): string {
  const d = v.replace(/\D/g, '').slice(0, 11)
  if (d.length <= 10) {
    return d
      .replace(/(\d{2})(\d)/, '($1) $2')
      .replace(/(\d{4})(\d{1,4})$/, '$1-$2')
  }
  return d
    .replace(/(\d{2})(\d)/, '($1) $2')
    .replace(/(\d{5})(\d{1,4})$/, '$1-$2')
}

export function mascaraDocumento(v: string): string {
  const d = v.replace(/\D/g, '')
  return d.length <= 11 ? mascaraCPF(v) : mascaraCNPJ(v)
}

// ─── Validações ────────────────────────────────────────────────────────────

export function validarCPF(cpf: string): boolean {
  const n = cpf.replace(/\D/g, '')
  if (n.length !== 11 || /^(\d)\1+$/.test(n)) return false
  let soma = 0
  for (let i = 0; i < 9; i++) soma += parseInt(n[i] ?? '0', 10) * (10 - i)
  let r = (soma * 10) % 11
  if (r === 10 || r === 11) r = 0
  if (r !== parseInt(n[9] ?? '0', 10)) return false
  soma = 0
  for (let i = 0; i < 10; i++) soma += parseInt(n[i] ?? '0', 10) * (11 - i)
  r = (soma * 10) % 11
  if (r === 10 || r === 11) r = 0
  return r === parseInt(n[10] ?? '0', 10)
}

export function validarCNPJ(cnpj: string): boolean {
  const n = cnpj.replace(/\D/g, '')
  if (n.length !== 14 || /^(\d)\1+$/.test(n)) return false
  const calc = (str: string, weights: number[]) =>
    weights.reduce((acc, w, i) => acc + parseInt(str[i] ?? '0', 10) * w, 0)
  const mod = (v: number) => {
    const r = v % 11
    return r < 2 ? 0 : 11 - r
  }
  const d1 = mod(calc(n, [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]))
  const d2 = mod(calc(n, [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]))
  return d1 === parseInt(n[12] ?? '0', 10) && d2 === parseInt(n[13] ?? '0', 10)
}

export function validarEmail(email: string): boolean {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())
}

export function validarCEP(cep: string): boolean {
  return /^\d{5}-?\d{3}$/.test(cep.trim())
}

export function validarTelefone(tel: string): boolean {
  const d = tel.replace(/\D/g, '')
  return d.length === 10 || d.length === 11
}

export function validarDocumento(doc: string): boolean {
  const d = doc.replace(/\D/g, '')
  return d.length === 11 ? validarCPF(doc) : d.length === 14 ? validarCNPJ(doc) : false
}

// ─── Mensagens de erro padrão ──────────────────────────────────────────────

export const mensagensPadrao: Record<string, string> = {
  required:   'Campo obrigatório',
  email:      'E-mail inválido',
  cpf:        'CPF inválido',
  cnpj:       'CNPJ inválido',
  documento:  'CPF ou CNPJ inválido',
  cep:        'CEP inválido',
  telefone:   'Telefone inválido',
  senha:      'Mínimo de 6 caracteres',
}

// ─── Interface do componente de campo ─────────────────────────────────────

export interface CampoValidavel {
  validar: () => boolean
}

// ─── validarCampos ─────────────────────────────────────────────────────────
/**
 * Chama validar() em cada campo passado por parâmetro.
 * Retorna true se TODOS forem válidos; false se houver ao menos um inválido.
 * Campos nulos/undefined são ignorados.
 *
 * Exemplo de uso:
 *   const ok = validarCampos([inputEmail.value, inputCPF.value, inputCEP.value])
 *   if (!ok) return
 */
export function validarCampos(
  campos: Array<CampoValidavel | null | undefined>,
): boolean {
  let tudo = true
  for (const campo of campos) {
    if (campo && typeof campo.validar === 'function') {
      if (!campo.validar()) tudo = false
    }
  }
  return tudo
}
