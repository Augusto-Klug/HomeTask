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
  logradouro: string
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
  logradouro: string
  bairro: string
  cidadeId: string
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
  { value: 1,  label: 'Faxina' },
  { value: 2,  label: 'Jardinagem' },
  { value: 3,  label: 'Reparos' },
  { value: 4,  label: 'Lavanderia' },
  { value: 5,  label: 'Passadoria' },
  { value: 6,  label: 'Babysitter' },
  { value: 7,  label: 'Cuidador de Idosos' },
  { value: 8,  label: 'Pet Sitter' },
  { value: 9,  label: 'Cozinheiro' },
  { value: 10, label: 'Serviços Gerais' },
] as const

export type TipoValorCliente = 'por_hora' | 'total' | 'a_combinar'
export type TipoValorPrestador = 'por_hora' | 'total'

export type StatusAgendamento =
  | 'Solicitado'
  | 'Confirmado'
  | 'Aceito'
  | 'EmAndamento'
  | 'Concluido'
  | 'Cancelado'
  | 'Recusado'

export type TipoAnuncioCliente = 2
export type TipoAnuncioPrestador = 1
export type AguardandoRespostaDe = 'Cliente' | 'Prestador'

export interface ServicoDetalheBase {
  id: string
  titulo: string
  descricao: string
  precoBase: number
  unidadeCobranca: string
  categoria: number
  ativo: boolean
  cidade?: string | null
  estado?: string | null
}

export interface ServicoBuscaResumo {
  id: string
  titulo: string
  descricao: string
  precoBase: number
  unidadeCobranca: string
  prestadorId: string
  prestadorNome?: string | null
  categoria: number
  cidade?: string | null
  estado?: string | null
  mediaAvaliacoes?: number | null
  tipoAnuncio: TipoAnuncioPrestador
}

export interface ServicoPrestadorDetalhe extends ServicoDetalheBase {
  prestadorId: string
  prestadorNome?: string | null
  duracaoEstimadaMinutos?: number | null
  aceitaPagamentoAposFinalizacao?: boolean
  mediaAvaliacoes?: number | null
  tipoAnuncio: TipoAnuncioPrestador
}

export interface ServicoClienteDetalhe extends ServicoDetalheBase {
  clienteId: string
  clienteNome?: string | null
  dataDesejada?: string | null
  tipoAnuncio: TipoAnuncioCliente
}

export type ServicoDetalhe = ServicoPrestadorDetalhe | ServicoClienteDetalhe

export interface ServicoOferecido {
  id: string
  titulo: string
  descricao: string
  precoBase: number
  duracaoEstimadaMinutos: number | null
  unidadeCobranca: string
  tipoAnuncio: TipoAnuncioCliente | TipoAnuncioPrestador
  categoria: number
  dataDesejada?: string | null
  aceitaPagamentoAposFinalizacao?: boolean
}

export interface AgendamentoEndereco {
  logradouro: string
  bairro: string
  cidade: string
  estado: string
}

export interface AgendamentoResumo {
  id: string
  clienteId: string
  clienteNome: string
  prestadorId: string
  prestadorNome: string
  dataHoraAgendada: string
  duracaoMinutos: number
  status: StatusAgendamento
  endereco: AgendamentoEndereco
  observacoes: string | null
  valorTotal: number
  dataSolicitacao: string
  dataResposta: string | null
  dataConclusao: string | null
  motivoRecusa: string | null
  aguardandoRespostaDe: AguardandoRespostaDe | null
  servicos: ServicoOferecido[]
}

export interface PerfilForm {
  nome: string
  email: string
  telefone: string
  documento: string
  cep: string
  logradouro: string
  bairro: string
  cidade: string
  estado: string
  descricao: string
  raioAtendimentoKm: number | null
}

export interface ServicoClienteForm {
  titulo: string
  descricao: string
  categoria: string
  unidadeCobranca: TipoValorCliente | ''
  precoBase: string
  data: string
  tipo? : TipoAnuncioCliente
}

export interface ServicoPrestadorForm {
  titulo: string
  descricao: string
  categoria: string
  unidadeCobranca: TipoValorPrestador | ''
  precoBase: string
  aceitaPagamentoAposFinalizacao: boolean
  tipo? : TipoAnuncioPrestador
}
