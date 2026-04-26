/**
 * handlers.ts — Handlers MSW para desenvolvimento sem backend.
 * Ativados via VITE_USE_MOCK=true no arquivo .env.
 * As requisições aparecem normalmente no DevTools → Network.
 */

import { http, HttpResponse, delay } from "msw";
import type {
  Servico,
  Avaliacao,
  AuthResponse,
  AgendamentoResumo,
  PerfilForm,
} from "@/types";

// ---------------------------------------------------------------------------
// Dados fictícios
// ---------------------------------------------------------------------------

const MOCK_SERVICOS: Servico[] = [
  {
    id: 1,
    titulo: "Faxina Residencial Completa",
    descricao:
      "Limpeza completa de residências com produtos de qualidade. Inclui cozinha, banheiros, quartos e áreas de circulação.",
    preco: 80,
    prestadorId: 10,
    prestadorNome: "Maria Silva",
    categoria: "Faxina",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.5,
  },
  {
    id: 2,
    titulo: "Jardinagem e Poda de Árvores",
    descricao:
      "Serviços de jardinagem, poda, plantio e manutenção de jardins residenciais e comerciais.",
    preco: 60,
    prestadorId: 11,
    prestadorNome: "João Santos",
    categoria: "Jardinagem",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.0,
  },
  {
    id: 3,
    titulo: "Reparos Gerais Residenciais",
    descricao:
      "Pequenos reparos elétricos, hidráulicos e de alvenaria. Montagem de móveis e instalação de equipamentos.",
    preco: 90,
    prestadorId: 12,
    prestadorNome: "Carlos Pereira",
    categoria: "Reparos",
    cidade: "Gaspar",
    estado: "SC",
    mediaAvaliacoes: 3.5,
  },
  {
    id: 4,
    titulo: "Lavanderia — Entrega no Mesmo Dia",
    descricao:
      "Lavagem, secagem e passadoria de roupas. Entrega no mesmo dia para pedidos feitos até 10h.",
    preco: 40,
    prestadorId: 13,
    prestadorNome: "Ana Lima",
    categoria: "Lavanderia",
    cidade: "Indaial",
    estado: "SC",
    mediaAvaliacoes: 5.0,
  },
  {
    id: 5,
    titulo: "Babysitter Experiente",
    descricao:
      "Cuidados com crianças de 0 a 12 anos. Experiência com primeiros socorros e educação infantil.",
    preco: 50,
    prestadorId: 14,
    prestadorNome: "Fernanda Costa",
    categoria: "Babysitter",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.8,
  },
  {
    id: 6,
    titulo: "Cuidador de Idosos — Período Integral",
    descricao:
      "Acompanhamento e cuidados para idosos. Auxílio com medicamentos, higiene pessoal e atividades diárias.",
    preco: 70,
    prestadorId: 15,
    prestadorNome: "Roberto Alves",
    categoria: "Cuidador de Idosos",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.2,
  },
  {
    id: 7,
    titulo: "Passadoria a Domicílio",
    descricao:
      "Serviço de passadoria caprichada na sua residência. Roupas entregues em cabide.",
    preco: 35,
    prestadorId: 16,
    prestadorNome: "Sônia Ramos",
    categoria: "Passadoria",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.3,
  },
  {
    id: 8,
    titulo: "Pet Sitter — Cuidados para seu Animal",
    descricao:
      "Cuidados para cães e gatos enquanto você viaja: alimentação, passeios e banho.",
    preco: 45,
    prestadorId: 17,
    prestadorNome: "Lucas Mendes",
    categoria: "Pet Sitter",
    cidade: "Gaspar",
    estado: "SC",
    mediaAvaliacoes: 4.6,
  },
];

const MOCK_AVALIACOES: Record<number, Avaliacao[]> = {
  10: [
    {
      id: 1,
      clienteNome: "Pedro Martins",
      nota: 5,
      comentario: "Excelente trabalho! Casa ficou impecável.",
      data: "2025-03-10T14:00:00Z",
    },
    {
      id: 2,
      clienteNome: "Luisa Ferreira",
      nota: 4,
      comentario: "Muito boa, pontual e cuidadosa.",
      data: "2025-02-20T09:30:00Z",
    },
  ],
  11: [
    {
      id: 3,
      clienteNome: "Paulo Gomes",
      nota: 4,
      comentario: "Jardim ficou bonito. Recomendo!",
      data: "2025-03-05T16:00:00Z",
    },
  ],
  12: [
    {
      id: 4,
      clienteNome: "Marcia Souza",
      nota: 3,
      comentario: "Serviço ok, mas demorou mais que o esperado.",
      data: "2025-01-18T11:00:00Z",
    },
    {
      id: 5,
      clienteNome: "Henrique Silva",
      nota: 4,
      comentario: "Resolveu o problema da pia rapidinho.",
      data: "2025-02-28T08:00:00Z",
    },
  ],
  13: [
    {
      id: 6,
      clienteNome: "Camila Ramos",
      nota: 5,
      comentario: "Roupas impecáveis, entrega super rápida!",
      data: "2025-03-12T17:30:00Z",
    },
    {
      id: 7,
      clienteNome: "Tiago Nunes",
      nota: 5,
      comentario: "Melhor serviço de lavanderia da cidade.",
      data: "2025-03-08T10:00:00Z",
    },
  ],
  14: [
    {
      id: 8,
      clienteNome: "Carla Mendes",
      nota: 5,
      comentario: "Minha filha adorou! Muito atenciosa.",
      data: "2025-03-01T19:00:00Z",
    },
  ],
  15: [
    {
      id: 9,
      clienteNome: "Beatriz Oliveira",
      nota: 4,
      comentario: "Cuidou muito bem da minha mãe.",
      data: "2025-02-15T12:00:00Z",
    },
  ],
  16: [
    {
      id: 10,
      clienteNome: "Renata Costa",
      nota: 4,
      comentario: "Roupas bem passadas, sem vincos.",
      data: "2025-03-20T10:00:00Z",
    },
  ],
  17: [
    {
      id: 11,
      clienteNome: "Diego Faria",
      nota: 5,
      comentario: "Meu cachorro foi muito bem cuidado!",
      data: "2025-03-18T15:00:00Z",
    },
  ],
};

const MOCK_USER: AuthResponse = {
  userId: 1,
  nome: "Usuário Demo",
  email: "demo@hometask.com",
  tipo: 3,
};

const MOCK_CLIENTE = { id: 1, usuarioId: 1, nome: "Usuário Demo" };
const MOCK_PRESTADOR = { id: 1, usuarioId: 1, nome: "Usuário Demo" };

// ---------------------------------------------------------------------------
// Mocks — Agendamentos
// ---------------------------------------------------------------------------

function diasAPartirDeHoje(dias: number, hora = "09:00") {
  const d = new Date();
  d.setDate(d.getDate() + dias);
  const [h, m] = hora.split(":").map(Number);
  d.setHours(h!, m, 0, 0);
  return d.toISOString();
}

const MOCK_AGENDAMENTOS_CLIENTE: AgendamentoResumo[] = [
  {
    id: "ag-c-001",
    clienteId: "1",
    clienteNome: "Usuário Demo",
    prestadorId: "10",
    prestadorNome: "Maria Silva",
    dataHoraAgendada: diasAPartirDeHoje(2, "09:00"),
    duracaoMinutos: 120,
    status: "Confirmado",
    endereco: {
      logradouro: "Rua XV de Novembro, 320",
      bairro: "Centro",
      cidade: "Blumenau",
      estado: "SC",
    },
    observacoes: "Dar atenção especial à cozinha e banheiros.",
    valorTotal: 160,
    dataSolicitacao: diasAPartirDeHoje(-3),
    dataResposta: diasAPartirDeHoje(-2),
    dataConclusao: null,
    motivoRecusa: null,
    servicos: [
      {
        id: "srv-001",
        titulo: "Faxina Residencial Completa",
        descricao: "Limpeza completa de residências com produtos de qualidade.",
        precoBase: 80,
        duracaoEstimadaMinutos: 120,
        unidadeCobranca: "total",
        tipoAnuncio: "PrestadorOferece",
        categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
      },
    ],
  },
  {
    id: "ag-c-002",
    clienteId: "1",
    clienteNome: "Usuário Demo",
    prestadorId: "14",
    prestadorNome: "Fernanda Costa",
    dataHoraAgendada: diasAPartirDeHoje(5, "14:30"),
    duracaoMinutos: 180,
    status: "Solicitado",
    endereco: {
      logradouro: "Rua Alameda Rio Branco, 45",
      bairro: "Velha",
      cidade: "Blumenau",
      estado: "SC",
    },
    observacoes: null,
    valorTotal: 150,
    dataSolicitacao: diasAPartirDeHoje(-1),
    dataResposta: null,
    dataConclusao: null,
    motivoRecusa: null,
    servicos: [
      {
        id: "srv-005",
        titulo: "Babysitter Experiente",
        descricao: "Cuidados com crianças de 0 a 12 anos.",
        precoBase: 50,
        duracaoEstimadaMinutos: 180,
        unidadeCobranca: "por_hora",
        tipoAnuncio: 1,
        categoria: { id: "cat-6", nome: "Babysitter", icone: "child_care" },
      },
    ],
  },
  {
    id: "ag-c-003",
    clienteId: "1",
    clienteNome: "Usuário Demo",
    prestadorId: "12",
    prestadorNome: "Carlos Pereira",
    dataHoraAgendada: diasAPartirDeHoje(10, "08:00"),
    duracaoMinutos: 90,
    status: "Confirmado",
    endereco: {
      logradouro: "Rua XV de Novembro, 320",
      bairro: "Centro",
      cidade: "Blumenau",
      estado: "SC",
    },
    observacoes: "Vazamento embaixo da pia da cozinha e torneira do banheiro.",
    valorTotal: 90,
    dataSolicitacao: diasAPartirDeHoje(-5),
    dataResposta: diasAPartirDeHoje(-4),
    dataConclusao: null,
    motivoRecusa: null,
    servicos: [
      {
        id: "srv-003",
        titulo: "Reparos Gerais Residenciais",
        descricao: "Pequenos reparos elétricos, hidráulicos e de alvenaria.",
        precoBase: 90,
        duracaoEstimadaMinutos: 90,
        unidadeCobranca: "total",
        tipoAnuncio: "PrestadorOferece",
        categoria: { id: "cat-3", nome: "Reparos", icone: "handyman" },
      },
    ],
  },
];

const MOCK_AGENDAMENTOS_PRESTADOR: AgendamentoResumo[] = [
  {
    id: "ag-p-001",
    clienteId: "2",
    clienteNome: "Pedro Martins",
    prestadorId: "1",
    prestadorNome: "Usuário Demo",
    dataHoraAgendada: diasAPartirDeHoje(1, "10:00"),
    duracaoMinutos: 120,
    status: "Confirmado",
    endereco: {
      logradouro: "Rua Hermann Hering, 1800",
      bairro: "Itoupava Norte",
      cidade: "Blumenau",
      estado: "SC",
    },
    observacoes: "Residência de 3 andares, foco no térreo.",
    valorTotal: 160,
    dataSolicitacao: diasAPartirDeHoje(-4),
    dataResposta: diasAPartirDeHoje(-3),
    dataConclusao: null,
    motivoRecusa: null,
    servicos: [
      {
        id: "srv-011",
        titulo: "Limpeza Pós-Obra",
        descricao: "Limpeza profunda após reformas e obras.",
        precoBase: 160,
        duracaoEstimadaMinutos: 240,
        unidadeCobranca: "total",
        tipoAnuncio: "ClienteSolicitou",
        categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
      },
    ],
  },
  {
    id: "ag-p-002",
    clienteId: "3",
    clienteNome: "Luisa Ferreira",
    prestadorId: "1",
    prestadorNome: "Usuário Demo",
    dataHoraAgendada: diasAPartirDeHoje(7, "16:00"),
    duracaoMinutos: 60,
    status: "Solicitado",
    endereco: {
      logradouro: "Av. Brasil, 1001",
      bairro: "Ponta Aguda",
      cidade: "Blumenau",
      estado: "SC",
    },
    observacoes: null,
    valorTotal: 80,
    dataSolicitacao: diasAPartirDeHoje(-1),
    dataResposta: null,
    dataConclusao: null,
    motivoRecusa: null,
    servicos: [
      {
        id: "srv-012",
        titulo: "Faxina Rápida — Apartamento",
        descricao: "Limpeza de apartamentos de até 60m².",
        precoBase: 80,
        duracaoEstimadaMinutos: 60,
        unidadeCobranca: "total",
        tipoAnuncio: "PrestadorOferece",
        categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
      },
    ],
  },
];

const MOCK_PERFIL: PerfilForm = {
  nome: "Usuário Demo",
  email: "demo@hometask.com",
  telefone: "(47) 99123-4567",
  documento: "123.456.789-00",
  cep: "89010-001",
  logradouro: "Rua XV de Novembro, 320",
  bairro: "Centro",
  cidade: "Blumenau",
  estado: "SC",
  descricao:
    "Profissional de limpeza com 5 anos de experiência. Atendo residências e pequenos comércios.",
  raioAtendimentoKm: 15,
};

const CATEGORIA_LABELS: Record<string, string> = {
  "1": "Faxina",
  "2": "Jardinagem",
  "3": "Reparos",
  "4": "Lavanderia",
  "5": "Passadoria",
  "6": "Babysitter",
  "7": "Cuidador de Idosos",
  "8": "Pet Sitter",
  "9": "Cozinheiro",
  "10": "Serviços Gerais",
};

// Latência simulada (ms) para dar a sensação de requisição real
const MOCK_DELAY = 400;

// ---------------------------------------------------------------------------
// Handlers
// ---------------------------------------------------------------------------

export const handlers = [
  // GET /api/ServicoOferecido/BuscarServicos
  http.get("*/api/ServicoOferecido/BuscarServicos", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    let result = [...MOCK_SERVICOS];

    const categoria = url.searchParams.get("categoria");
    if (categoria) {
      const label = CATEGORIA_LABELS[categoria] ?? "";
      result = result.filter((s) => s.categoria === label);
    }

    const cidade = url.searchParams.get("cidade");
    if (cidade) {
      result = result.filter((s) =>
        s.cidade.toLowerCase().includes(cidade.toLowerCase()),
      );
    }

    const precoMaximo = url.searchParams.get("precoMaximo");
    if (precoMaximo) {
      result = result.filter((s) => s.preco <= Number(precoMaximo));
    }

    return HttpResponse.json(result);
  }),

  // GET /api/ServicoOferecido/ObterServicoPorId
  http.get("*/api/ServicoOferecido/ObterServicoPorId", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const id = url.searchParams.get("id");
    const servico = MOCK_SERVICOS.find((s) => String(s.id) === id);
    if (!servico) return new HttpResponse(null, { status: 404 });
    return HttpResponse.json(servico);
  }),

  // GET /api/Avaliacao/ObterAvaliacoesPorPrestador
  http.get(
    "*/api/Avaliacao/ObterAvaliacoesPorPrestador",
    async ({ request }) => {
      await delay(MOCK_DELAY);
      const url = new URL(request.url);
      const prestadorId = Number(url.searchParams.get("prestadorId"));
      return HttpResponse.json(MOCK_AVALIACOES[prestadorId] ?? []);
    },
  ),

  // POST /api/Auth/Login — aceita qualquer credencial
  http.post("*/api/Auth/Login", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_USER);
  }),

  // POST /api/Auth/Logout
  http.post("*/api/Auth/Logout", async () => {
    await delay(MOCK_DELAY);
    return new HttpResponse(null, { status: 200 });
  }),

  // POST /api/Auth/Refresh
  http.post("*/api/Auth/Refresh", async () => {
    await delay(MOCK_DELAY);
    return new HttpResponse(null, { status: 200 });
  }),

  // POST /api/Auth/EsqueciSenha
  http.post("*/api/Auth/EsqueciSenha", async () => {
    await delay(MOCK_DELAY);
    return new HttpResponse(null, { status: 200 });
  }),

  // POST /api/Auth/RedefinirSenha
  http.post("*/api/Auth/RedefinirSenha", async () => {
    await delay(MOCK_DELAY);
    return new HttpResponse(null, { status: 200 });
  }),

  // POST /api/Usuario/CriarUsuario
  http.post("*/api/Usuario/CriarUsuario", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_USER, { status: 201 });
  }),

  // GET /api/Cliente/ObterClientesPorUsuarioId
  http.get("*/api/Cliente/ObterClientesPorUsuarioId", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_CLIENTE);
  }),

  // GET /api/Prestador/ObterPrestadorPorUsuarioId
  http.get("*/api/Prestador/ObterPrestadorPorUsuarioId", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_PRESTADOR);
  }),

  // POST /api/Agendamento/CriarAgendamento
  http.post("*/api/Agendamento/CriarAgendamento", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(
      { id: Math.floor(Math.random() * 9000) + 1000 },
      { status: 201 },
    );
  }),

  // GET /api/Agendamento/ObterAgendamentosPorPrestador
  http.get("*/api/Agendamento/ObterAgendamentosPorPrestador", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_AGENDAMENTOS_PRESTADOR);
  }),

  // GET /api/Agendamento/ObterMeusAgendamentosPrestador
  http.get("*/api/Agendamento/ObterMeusAgendamentosPrestador", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_AGENDAMENTOS_PRESTADOR);
  }),

  // GET /api/Agendamento/ObterMeusAgendamentosCliente
  http.get("*/api/Agendamento/ObterMeusAgendamentosCliente", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_AGENDAMENTOS_CLIENTE);
  }),

  // GET /api/Agendamento/ObterAgendamentoPorId
  http.get("*/api/Agendamento/ObterAgendamentoPorId", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const id = url.searchParams.get("id");
    const todos = [...MOCK_AGENDAMENTOS_CLIENTE, ...MOCK_AGENDAMENTOS_PRESTADOR];
    const alvo = todos.find((a) => a.id === id);
    return HttpResponse.json(alvo ?? null, { status: alvo ? 200 : 404 });
  }),

  // GET /api/Agendamento/ObterSolicitacoesPendentesPrestador
  http.get("*/api/Agendamento/ObterSolicitacoesPendentesPrestador", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(
      MOCK_AGENDAMENTOS_PRESTADOR.filter((a) => a.status === "Solicitado"),
    );
  }),

  // POST /api/Agendamento/AceitarAgendamento
  http.post("*/api/Agendamento/AceitarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string };
    const alvo = MOCK_AGENDAMENTOS_PRESTADOR.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = "Aceito";
      alvo.dataResposta = new Date().toISOString();
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  // POST /api/Agendamento/RecusarAgendamento
  http.post("*/api/Agendamento/RecusarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string; motivoRecusa?: string };
    const alvo = MOCK_AGENDAMENTOS_PRESTADOR.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = "Recusado";
      alvo.dataResposta = new Date().toISOString();
      alvo.motivoRecusa = body.motivoRecusa ?? "Sem motivo informado";
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  // POST /api/ServicoOferecido/CriarServicoCliente
  http.post("*/api/ServicoOferecido/CriarServicoCliente", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(
      { id: Math.floor(Math.random() * 9000) + 1000 },
      { status: 201 },
    );
  }),

  // GET /api/Agendamentos/cliente
  http.get("*/api/Agendamentos/cliente", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_AGENDAMENTOS_CLIENTE);
  }),

  // GET /api/Agendamentos/prestador
  http.get("*/api/Agendamentos/prestador", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_AGENDAMENTOS_PRESTADOR);
  }),

  // GET /api/Usuarios/perfil
  http.get("*/api/Usuarios/perfil", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_PERFIL);
  }),

  // PUT /api/Usuarios/perfil
  http.put("*/api/Usuarios/perfil", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as Partial<PerfilForm>;
    Object.assign(MOCK_PERFIL, body);
    return HttpResponse.json(MOCK_PERFIL);
  }),
];
