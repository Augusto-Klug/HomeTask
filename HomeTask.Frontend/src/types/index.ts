export interface User {
  userId: number
  nome: string
  email: string
  tipo: number // 1=Cliente, 2=Prestador, 3=Ambos
}

export interface AuthResponse {
  userId: number
  nome: string
  email: string
  tipo: number
}

export interface Categoria {
  id: number
  nome: string
  icone: string
}

export interface Servico {
  id: number
  titulo: string
  descricao: string
  preco: number
  prestadorId: number
  prestadorNome: string
  categoria: string
  cidade: string
  estado: string
  mediaAvaliacoes: number
}

export interface Avaliacao {
  id: number
  clienteNome: string
  nota: number
  comentario: string
  data: string
}

export interface AgendamentoForm {
  data: string
  hora: string
  endereco: string
  observacoes: string
}

export interface CadastroForm {
  tipoUsuario: number
  nome: string
  email: string
  documento: string
  telefone: string
  senha: string
  cep: string
  endereco: string
  bairro: string
  cidade: string
  estado: string
  raioAtendimentoKm: number
  descricao: string
}

export interface BuscarFiltro {
  categoria: string
  cidade: string
  precoMaximo: number | null
  avaliacaoMinima: number
}

export const CATEGORIAS_SERVICO = [
  { value: '1',  label: 'Faxina' },
  { value: '2',  label: 'Jardinagem' },
  { value: '3',  label: 'Reparos' },
  { value: '4',  label: 'Lavanderia' },
  { value: '5',  label: 'Passadoria' },
  { value: '6',  label: 'Babysitter' },
  { value: '7',  label: 'Cuidador de Idosos' },
  { value: '8',  label: 'Pet Sitter' },
  { value: '9',  label: 'Cozinheiro' },
  { value: '10', label: 'Serviços Gerais' },
] as const

export type TipoValorCliente = 'por_hora' | 'total' | 'a_combinar'
export type TipoValorPrestador = 'por_hora' | 'total'

export interface ServicoClienteForm {
  titulo: string
  descricao: string
  categoria: string
  unidadeCobranca: TipoValorCliente | ''
  valor: string
  data: string
}

export interface ServicoPrestadorForm {
  titulo: string
  descricao: string
  categoria: string
  unidadeCobranca: TipoValorPrestador | ''
  valor: string
  aceitaPagamentoAposFinalizacao: boolean
}
