import { StatusAgendamento, StatusPagamento, TipoAnuncio, TipoUsuario, UnidadeCobranca } from "./enums";

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

export interface Certificacao {
  id: string
  prestadorId: string
  nome: string
  instituicao?: string
  dataEmissao?: string
  dataValidade?: string
  urlDocumento?: string
  verificada: boolean
  dataCadastro: string
}

export interface Portfolio {
  id: string
  prestadorId: string
  titulo?: string
  descricao?: string
  urlImagem: string
  dataCadastro: string
  ordem: number
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
  logradouro?: string | null
  numero?: string | null
  bairro?: string | null
  cidade: string
  estado: string
  mediaAvaliacoes?: number | null
  totalAvaliacoes?: number
  mediaAvaliacoesPrestador?: number | null
  totalAvaliacoesPrestador?: number | null
  dataDesejada?: string | null
}

export interface Avaliacao {
  id: string
  agendamentoId: string
  clienteId?: string
  clienteNome?: string
  prestadorId?: string
  prestadorNome?: string
  servicoPrestadorId: string
  servicoTitulo?: string
  notaServico: number
  notaPrestador: number
  comentario?: string | null
  dataAvaliacao: string
  visivel?: boolean
}

export interface AvaliacaoCliente {
  id: string
  agendamentoId: string
  clienteId: string
  clienteNome?: string
  prestadorId: string
  prestadorNome?: string
  nota: number
  comentario?: string | null
  dataAvaliacao: string
  visivel?: boolean
}

export interface AgendamentoForm {
  data: string
  hora: string
  logradouro: string
  cidadeId: string
  estado: string
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
}

export interface AgendamentoEndereco {
  logradouro: string
  bairro: string
  cidade: string
  estado: string
  descricao?: string | null
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
  enderecoDescricao?: string | null
  observacoes: string | null
  valorTotal: number
  dataSolicitacao: string
  dataResposta: string | null
  dataConclusao: string | null
  motivoRecusa: string | null
  aguardandoRespostaDe?: TipoUsuario | null
  podeClienteAvaliarPrestador?: boolean
  clienteJaAvaliouPrestador?: boolean
  podePrestadorAvaliarCliente?: boolean
  prestadorJaAvaliouCliente?: boolean
  servicos: Array<ServicoOferecido & { unidadeCobranca: UnidadeCobranca }>
}

export interface PagamentoResumo {
  id: string
  agendamentoId: string
  valor: number
  status: StatusPagamento
  checkoutExternoId?: string | null
  checkoutUrl?: string | null
  statusExterno?: string | null
  dataCriacao: string
  dataProcessamento?: string | null
  dataConfirmacao?: string | null
  motivoRecusa?: string | null
}

export interface MensagemChat {
  id: string
  remetenteId: string
  agendamentoId?: string | null
  conversaId?: string | null
  conteudo: string
  dataEnvio: string
  dataLeitura?: string | null
  lida: boolean
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
  cidadeId: string
  estado: string
  descricao: string
  raioAtendimentoKm: number | null
  certificacoes?: Certificacao[]
  portfolios?: Portfolio[]
}

export interface PrestadorHistoricoPublico {
  agendamentoId: string
  servicoPrestadorId: string
  tituloServico: string
  dataHoraAgendada: string
  cidade: string
  estado: string
  notaServico?: number | null
  notaPrestador?: number | null
}

export interface PrestadorPerfilPublico {
  id: string
  nome: string
  descricao?: string | null
  cidade?: string | null
  estado?: string | null
  mediaAvaliacoes: number
  totalAvaliacoes: number
  totalServicosConcluidos: number
  certificacoes: Certificacao[]
  portfolios: Portfolio[]
  servicosOferecidos: Servico[]
  historicoConcluido: PrestadorHistoricoPublico[]
}

export interface PrestadorRecebimentoItem {
  agendamentoId: string
  tituloServico: string
  clienteNome: string
  dataConclusao?: string | null
  valorRecebido: number
  cidade: string
  estado: string
}

export interface PrestadorRecebimentosResumo {
  saldoRecebidoTotal: number
  totalServicosRecebidos: number
  servicosRecebidos: PrestadorRecebimentoItem[]
}

export interface ClientePerfilPublico {
  id: string
  nome: string
  cidade?: string | null
  estado?: string | null
  mediaAvaliacoes: number
  totalAvaliacoes: number
  totalServicosContratados: number
}

export interface CertificacaoForm {
  nome: string
  instituicao: string
  dataEmissao: string
  dataValidade: string
  documento?: File
}

export interface PortfolioForm {
  titulo: string
  descricao: string
  imagem?: File
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
  tipo?: TipoAnuncioPrestador
}
