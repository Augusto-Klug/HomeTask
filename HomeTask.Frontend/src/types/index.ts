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
  tipoUsuario: string
  nome: string
  email: string
  cpf: string
  telefone: string
  senha: string
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
