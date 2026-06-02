export interface User {
  userId: string
  nome: string
  email: string
  tipo: number
}

export interface AuthResponse {
  userId: string
  nome: string
  email: string
  tipo: number
}

export interface Categoria {
  id: string
  nome: string
  icone: string
}

export enum TipoUsuario {
  Cliente = 1,
  Prestador = 2,
  Ambos = 3,
}

export enum TipoAnuncio {
  Oferta = 1,
  Pedido = 2,
}

export enum UnidadeCobranca {
  PorHora = 1,
  Total = 2,
  ACombinar = 3,
}

export interface Servico {
  id: string
  titulo: string
  descricao: string
  precoBase: number
  preco?: number
  unidadeCobranca: UnidadeCobranca
  tipoAnuncio?: TipoAnuncio
  prestadorId?: string
  prestadorNome?: string
  clienteId?: string
  clienteNome?: string
  categoria: number | { id: string; nome: string; icone: string }
  cidade: string
  estado: string
  mediaAvaliacoes?: number | null
  dataDesejada?: string | null
  aceitaPagamentoAposFinalizacao?: boolean
}

export interface Avaliacao {
  id: string
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
}

export interface ResultadoPaginado<T> {
  itens: T[]
  paginaAtual: number
  tamanhoPagina: number
  totalRegistros: number
  totalPaginas: number
}

export const CATEGORIAS_SERVICO = [
  { value: 1, label: 'Faxina' },
  { value: 2, label: 'Jardinagem' },
  { value: 3, label: 'Reparos' },
  { value: 4, label: 'Lavanderia' },
  { value: 5, label: 'Passadoria' },
  { value: 6, label: 'Babysitter' },
  { value: 7, label: 'Cuidador de Idosos' },
  { value: 8, label: 'Pet Sitter' },
  { value: 9, label: 'Cozinheiro' },
  { value: 10, label: 'Serviços Gerais' },
] as const

export type TipoValorCliente = UnidadeCobranca
export type TipoValorPrestador = UnidadeCobranca.PorHora | UnidadeCobranca.Total
export type TipoValorClienteForm = `${UnidadeCobranca}` | ''
export type TipoValorPrestadorForm = `${UnidadeCobranca.PorHora | UnidadeCobranca.Total}` | ''

export type StatusAgendamento =
  | 'Solicitado'
  | 'Confirmado'
  | 'Aceito'
  | 'EmAndamento'
  | 'Concluido'
  | 'Cancelado'
  | 'Recusado'

export type TipoAnuncioCliente = TipoAnuncio.Pedido
export type TipoAnuncioPrestador = TipoAnuncio.Oferta

export interface ServicoOferecido {
  id: string
  titulo: string
  descricao: string
  precoBase: number
  duracaoEstimadaMinutos: number | null
  unidadeCobranca: UnidadeCobranca
  tipoAnuncio: TipoAnuncioCliente | TipoAnuncioPrestador
  categoria: number | { id: string; nome: string; icone: string }
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
  aguardandoRespostaDe?: TipoUsuario | null
  servicos: Array<ServicoOferecido & { unidadeCobranca: UnidadeCobranca }>
}

export interface ServicoPrestadorDetalhe extends Servico {
  prestadorId: string
  prestadorNome?: string
  tipoAnuncio: TipoAnuncioPrestador
}

export interface ServicoClienteDetalhe extends Servico {
  clienteId: string
  clienteNome?: string
  dataDesejada?: string | null
  tipoAnuncio: TipoAnuncioCliente
}

export type ServicoDetalhe = ServicoPrestadorDetalhe | ServicoClienteDetalhe

export interface ServicoBuscaResumo extends Servico {
  precoBase: number
  unidadeCobranca: UnidadeCobranca
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
  unidadeCobranca: TipoValorClienteForm
  precoBase: string
  valor?: string
  data: string
  tipo?: TipoAnuncioCliente
}

export interface ServicoPrestadorForm {
  titulo: string
  descricao: string
  categoria: string
  unidadeCobranca: TipoValorPrestadorForm
  precoBase: string
  valor?: string
  aceitaPagamentoAposFinalizacao: boolean
  tipo?: TipoAnuncioPrestador
}
