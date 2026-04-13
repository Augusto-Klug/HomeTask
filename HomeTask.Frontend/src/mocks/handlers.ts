/**
 * handlers.ts — Handlers MSW para desenvolvimento sem backend.
 * Ativados via VITE_USE_MOCK=true no arquivo .env.
 * As requisições aparecem normalmente no DevTools → Network.
 */

import { http, HttpResponse, delay } from 'msw'
import type { Servico, Avaliacao, AuthResponse } from '@/types'

// ---------------------------------------------------------------------------
// Dados fictícios
// ---------------------------------------------------------------------------

const MOCK_SERVICOS: Servico[] = [
  {
    id: 1,
    titulo: 'Faxina Residencial Completa',
    descricao:
      'Limpeza completa de residências com produtos de qualidade. Inclui cozinha, banheiros, quartos e áreas de circulação.',
    preco: 80,
    prestadorId: 10,
    prestadorNome: 'Maria Silva',
    categoria: 'Faxina',
    cidade: 'Blumenau',
    estado: 'SC',
    mediaAvaliacoes: 4.5,
  },
  {
    id: 2,
    titulo: 'Jardinagem e Poda de Árvores',
    descricao:
      'Serviços de jardinagem, poda, plantio e manutenção de jardins residenciais e comerciais.',
    preco: 60,
    prestadorId: 11,
    prestadorNome: 'João Santos',
    categoria: 'Jardinagem',
    cidade: 'Blumenau',
    estado: 'SC',
    mediaAvaliacoes: 4.0,
  },
  {
    id: 3,
    titulo: 'Reparos Gerais Residenciais',
    descricao:
      'Pequenos reparos elétricos, hidráulicos e de alvenaria. Montagem de móveis e instalação de equipamentos.',
    preco: 90,
    prestadorId: 12,
    prestadorNome: 'Carlos Pereira',
    categoria: 'Reparos',
    cidade: 'Gaspar',
    estado: 'SC',
    mediaAvaliacoes: 3.5,
  },
  {
    id: 4,
    titulo: 'Lavanderia — Entrega no Mesmo Dia',
    descricao:
      'Lavagem, secagem e passadoria de roupas. Entrega no mesmo dia para pedidos feitos até 10h.',
    preco: 40,
    prestadorId: 13,
    prestadorNome: 'Ana Lima',
    categoria: 'Lavanderia',
    cidade: 'Indaial',
    estado: 'SC',
    mediaAvaliacoes: 5.0,
  },
  {
    id: 5,
    titulo: 'Babysitter Experiente',
    descricao:
      'Cuidados com crianças de 0 a 12 anos. Experiência com primeiros socorros e educação infantil.',
    preco: 50,
    prestadorId: 14,
    prestadorNome: 'Fernanda Costa',
    categoria: 'Babysitter',
    cidade: 'Blumenau',
    estado: 'SC',
    mediaAvaliacoes: 4.8,
  },
  {
    id: 6,
    titulo: 'Cuidador de Idosos — Período Integral',
    descricao:
      'Acompanhamento e cuidados para idosos. Auxílio com medicamentos, higiene pessoal e atividades diárias.',
    preco: 70,
    prestadorId: 15,
    prestadorNome: 'Roberto Alves',
    categoria: 'Cuidador de Idosos',
    cidade: 'Blumenau',
    estado: 'SC',
    mediaAvaliacoes: 4.2,
  },
  {
    id: 7,
    titulo: 'Passadoria a Domicílio',
    descricao: 'Serviço de passadoria caprichada na sua residência. Roupas entregues em cabide.',
    preco: 35,
    prestadorId: 16,
    prestadorNome: 'Sônia Ramos',
    categoria: 'Passadoria',
    cidade: 'Blumenau',
    estado: 'SC',
    mediaAvaliacoes: 4.3,
  },
  {
    id: 8,
    titulo: 'Pet Sitter — Cuidados para seu Animal',
    descricao:
      'Cuidados para cães e gatos enquanto você viaja: alimentação, passeios e banho.',
    preco: 45,
    prestadorId: 17,
    prestadorNome: 'Lucas Mendes',
    categoria: 'Pet Sitter',
    cidade: 'Gaspar',
    estado: 'SC',
    mediaAvaliacoes: 4.6,
  },
]

const MOCK_AVALIACOES: Record<number, Avaliacao[]> = {
  10: [
    { id: 1, clienteNome: 'Pedro Martins', nota: 5, comentario: 'Excelente trabalho! Casa ficou impecável.', data: '2025-03-10T14:00:00Z' },
    { id: 2, clienteNome: 'Luisa Ferreira', nota: 4, comentario: 'Muito boa, pontual e cuidadosa.', data: '2025-02-20T09:30:00Z' },
  ],
  11: [
    { id: 3, clienteNome: 'Paulo Gomes', nota: 4, comentario: 'Jardim ficou bonito. Recomendo!', data: '2025-03-05T16:00:00Z' },
  ],
  12: [
    { id: 4, clienteNome: 'Marcia Souza', nota: 3, comentario: 'Serviço ok, mas demorou mais que o esperado.', data: '2025-01-18T11:00:00Z' },
    { id: 5, clienteNome: 'Henrique Silva', nota: 4, comentario: 'Resolveu o problema da pia rapidinho.', data: '2025-02-28T08:00:00Z' },
  ],
  13: [
    { id: 6, clienteNome: 'Camila Ramos', nota: 5, comentario: 'Roupas impecáveis, entrega super rápida!', data: '2025-03-12T17:30:00Z' },
    { id: 7, clienteNome: 'Tiago Nunes', nota: 5, comentario: 'Melhor serviço de lavanderia da cidade.', data: '2025-03-08T10:00:00Z' },
  ],
  14: [
    { id: 8, clienteNome: 'Carla Mendes', nota: 5, comentario: 'Minha filha adorou! Muito atenciosa.', data: '2025-03-01T19:00:00Z' },
  ],
  15: [
    { id: 9, clienteNome: 'Beatriz Oliveira', nota: 4, comentario: 'Cuidou muito bem da minha mãe.', data: '2025-02-15T12:00:00Z' },
  ],
  16: [
    { id: 10, clienteNome: 'Renata Costa', nota: 4, comentario: 'Roupas bem passadas, sem vincos.', data: '2025-03-20T10:00:00Z' },
  ],
  17: [
    { id: 11, clienteNome: 'Diego Faria', nota: 5, comentario: 'Meu cachorro foi muito bem cuidado!', data: '2025-03-18T15:00:00Z' },
  ],
}

const MOCK_USER: AuthResponse = {
  userId: 1,
  nome: 'Usuário Demo',
  email: 'demo@hometask.com',
  tipo: 3,
}

const MOCK_CLIENTE = { id: 1, usuarioId: 1, nome: 'Usuário Demo' }

const CATEGORIA_LABELS: Record<string, string> = {
  '1': 'Faxina',
  '2': 'Jardinagem',
  '3': 'Reparos',
  '4': 'Lavanderia',
  '5': 'Passadoria',
  '6': 'Babysitter',
  '7': 'Cuidador de Idosos',
  '8': 'Pet Sitter',
  '9': 'Cozinheiro',
  '10': 'Serviços Gerais',
}

// Latência simulada (ms) para dar a sensação de requisição real
const MOCK_DELAY = 400

// ---------------------------------------------------------------------------
// Handlers
// ---------------------------------------------------------------------------

export const handlers = [
  // GET /api/ServicoOferecido/BuscarServicos
  http.get('*/api/ServicoOferecido/BuscarServicos', async ({ request }) => {
    await delay(MOCK_DELAY)
    const url = new URL(request.url)
    let result = [...MOCK_SERVICOS]

    const categoria = url.searchParams.get('categoria')
    if (categoria) {
      const label = CATEGORIA_LABELS[categoria] ?? ''
      result = result.filter((s) => s.categoria === label)
    }

    const cidade = url.searchParams.get('cidade')
    if (cidade) {
      result = result.filter((s) =>
        s.cidade.toLowerCase().includes(cidade.toLowerCase()),
      )
    }

    const precoMaximo = url.searchParams.get('precoMaximo')
    if (precoMaximo) {
      result = result.filter((s) => s.preco <= Number(precoMaximo))
    }

    return HttpResponse.json(result)
  }),

  // GET /api/ServicoOferecido/ObterServicoPorId
  http.get('*/api/ServicoOferecido/ObterServicoPorId', async ({ request }) => {
    await delay(MOCK_DELAY)
    const url = new URL(request.url)
    const id = url.searchParams.get('id')
    const servico = MOCK_SERVICOS.find((s) => String(s.id) === id)
    if (!servico) return new HttpResponse(null, { status: 404 })
    return HttpResponse.json(servico)
  }),

  // GET /api/Avaliacao/ObterAvaliacoesPorPrestador
  http.get('*/api/Avaliacao/ObterAvaliacoesPorPrestador', async ({ request }) => {
    await delay(MOCK_DELAY)
    const url = new URL(request.url)
    const prestadorId = Number(url.searchParams.get('prestadorId'))
    return HttpResponse.json(MOCK_AVALIACOES[prestadorId] ?? [])
  }),

  // POST /api/Auth/Login — aceita qualquer credencial
  http.post('*/api/Auth/Login', async () => {
    await delay(MOCK_DELAY)
    return HttpResponse.json(MOCK_USER)
  }),

  // POST /api/Auth/Logout
  http.post('*/api/Auth/Logout', async () => {
    await delay(MOCK_DELAY)
    return new HttpResponse(null, { status: 200 })
  }),

  // POST /api/Auth/Refresh
  http.post('*/api/Auth/Refresh', async () => {
    await delay(MOCK_DELAY)
    return new HttpResponse(null, { status: 200 })
  }),

  // POST /api/Auth/EsqueciSenha
  http.post('*/api/Auth/EsqueciSenha', async () => {
    await delay(MOCK_DELAY)
    return new HttpResponse(null, { status: 200 })
  }),

  // POST /api/Auth/RedefinirSenha
  http.post('*/api/Auth/RedefinirSenha', async () => {
    await delay(MOCK_DELAY)
    return new HttpResponse(null, { status: 200 })
  }),

  // POST /api/Usuario/CriarUsuario
  http.post('*/api/Usuario/CriarUsuario', async () => {
    await delay(MOCK_DELAY)
    return HttpResponse.json(MOCK_USER, { status: 201 })
  }),

  // GET /api/Cliente/ObterClientesPorUsuarioId
  http.get('*/api/Cliente/ObterClientesPorUsuarioId', async () => {
    await delay(MOCK_DELAY)
    return HttpResponse.json(MOCK_CLIENTE)
  }),

  // POST /api/Agendamento/CriarAgendamento
  http.post('*/api/Agendamento/CriarAgendamento', async () => {
    await delay(MOCK_DELAY)
    return HttpResponse.json(
      { id: Math.floor(Math.random() * 9000) + 1000 },
      { status: 201 },
    )
  }),

  // POST /api/ServicoOferecido/CriarServico
  http.post('*/api/ServicoOferecido/CriarServico', async () => {
    await delay(MOCK_DELAY)
    return HttpResponse.json(
      { id: Math.floor(Math.random() * 9000) + 1000 },
      { status: 201 },
    )
  }),
]
