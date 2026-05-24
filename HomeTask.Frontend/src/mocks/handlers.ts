import { delay, http, HttpResponse } from "msw";
import type { AgendamentoResumo, AuthResponse, Avaliacao, PerfilForm, Servico } from "@/types";
import { UnidadeCobranca } from "@/types";

const MOCK_DELAY = 200;

const MOCK_USER: AuthResponse = {
  userId: 1,
  nome: "Usuario Demo",
  email: "demo@hometask.com",
  tipo: 3,
};

const MOCK_CLIENTE = { id: 1, usuarioId: 1, nome: "Usuario Demo" };
const MOCK_PRESTADOR = { id: 1, usuarioId: 1, nome: "Usuario Demo" };

const MOCK_SERVICOS: Servico[] = [
  {
    id: "1",
    titulo: "Faxina Residencial Completa",
    descricao: "Limpeza completa de residencias com produtos de qualidade.",
    precoBase: 80,
    unidadeCobranca: UnidadeCobranca.Total,
    tipoAnuncio: 1,
    prestadorId: "10",
    prestadorNome: "Maria Silva",
    categoria: "Faxina",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.5,
  },
  {
    id: "2",
    titulo: "Jardinagem e Poda",
    descricao: "Servicos de jardinagem, poda e manutencao.",
    precoBase: 60,
    unidadeCobranca: UnidadeCobranca.PorHora,
    tipoAnuncio: 1,
    prestadorId: "11",
    prestadorNome: "Joao Santos",
    categoria: "Jardinagem",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4,
  },
  {
    id: "11",
    titulo: "Limpeza Pos-obra",
    descricao: "Preciso de uma limpeza completa depois da reforma.",
    precoBase: 160,
    unidadeCobranca: UnidadeCobranca.Total,
    tipoAnuncio: 2,
    clienteId: "2",
    clienteNome: "Pedro Martins",
    categoria: "Faxina",
    cidade: "Blumenau",
    estado: "SC",
    dataDesejada: diasAPartirDeHoje(3, "10:00"),
  },
];

const MOCK_AVALIACOES: Record<number, Avaliacao[]> = {
  10: [
    { id: 1, clienteNome: "Pedro Martins", nota: 5, comentario: "Excelente trabalho", data: "2025-03-10T14:00:00Z" },
  ],
  11: [
    { id: 2, clienteNome: "Luisa Ferreira", nota: 4, comentario: "Bom atendimento", data: "2025-03-05T16:00:00Z" },
  ],
};

function diasAPartirDeHoje(dias: number, hora = "09:00") {
  const d = new Date();
  d.setDate(d.getDate() + dias);
  const [h, m] = hora.split(":").map(Number);
  d.setHours(h ?? 0, m ?? 0, 0, 0);
  return d.toISOString();
}

const MOCK_AGENDAMENTOS_CLIENTE: AgendamentoResumo[] = [
  {
    id: "ag-c-001",
    clienteId: "1",
    clienteNome: "Usuario Demo",
    prestadorId: "10",
    prestadorNome: "Maria Silva",
    dataHoraAgendada: diasAPartirDeHoje(2, "09:00"),
    duracaoMinutos: 120,
    status: "Aceito",
    endereco: { logradouro: "Rua XV, 320", bairro: "Centro", cidade: "Blumenau", estado: "SC" },
    observacoes: "Dar atencao especial a cozinha.",
    valorTotal: 160,
    dataSolicitacao: diasAPartirDeHoje(-3),
    dataResposta: diasAPartirDeHoje(-2),
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: null,
    servicos: [
      {
        id: "srv-001",
        titulo: "Faxina Residencial Completa",
        descricao: "Limpeza completa",
        precoBase: 80,
        duracaoEstimadaMinutos: 120,
        unidadeCobranca: UnidadeCobranca.Total,
        tipoAnuncio: 1,
        categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
      },
    ],
  },
  {
    id: "ag-c-002",
    clienteId: "1",
    clienteNome: "Usuario Demo",
    prestadorId: "1",
    prestadorNome: "Usuario Demo",
    dataHoraAgendada: diasAPartirDeHoje(5, "14:30"),
    duracaoMinutos: 180,
    status: "Solicitado",
    endereco: { logradouro: "Rua Alameda, 45", bairro: "Velha", cidade: "Blumenau", estado: "SC" },
    observacoes: null,
    valorTotal: 150,
    dataSolicitacao: diasAPartirDeHoje(-1),
    dataResposta: null,
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: "Cliente",
    servicos: [
      {
        id: "11",
        titulo: "Limpeza Pos-obra",
        descricao: "Pedido do cliente",
        precoBase: 150,
        duracaoEstimadaMinutos: 180,
        unidadeCobranca: UnidadeCobranca.Total,
        tipoAnuncio: 2,
        categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
      },
    ],
  },
];

const MOCK_AGENDAMENTOS_PRESTADOR: AgendamentoResumo[] = [
  {
    id: "ag-p-001",
    clienteId: "3",
    clienteNome: "Luisa Ferreira",
    prestadorId: "1",
    prestadorNome: "Usuario Demo",
    dataHoraAgendada: diasAPartirDeHoje(1, "10:00"),
    duracaoMinutos: 120,
    status: "Solicitado",
    endereco: { logradouro: "Av. Brasil, 1001", bairro: "Ponta Aguda", cidade: "Blumenau", estado: "SC" },
    observacoes: null,
    valorTotal: 80,
    dataSolicitacao: diasAPartirDeHoje(-1),
    dataResposta: null,
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: "Prestador",
    servicos: [
      {
        id: "srv-012",
        titulo: "Faxina Rapida",
        descricao: "Servico de prestador",
        precoBase: 80,
        duracaoEstimadaMinutos: 60,
        unidadeCobranca: UnidadeCobranca.Total,
        tipoAnuncio: 1,
        categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
      },
    ],
  },
  {
    id: "ag-p-002",
    clienteId: "2",
    clienteNome: "Pedro Martins",
    prestadorId: "1",
    prestadorNome: "Usuario Demo",
    dataHoraAgendada: diasAPartirDeHoje(3, "15:00"),
    duracaoMinutos: 180,
    status: "Aceito",
    endereco: { logradouro: "Rua Hermann Hering, 1800", bairro: "Itoupava Norte", cidade: "Blumenau", estado: "SC" },
    observacoes: "Pronto para iniciar",
    valorTotal: 160,
    dataSolicitacao: diasAPartirDeHoje(-3),
    dataResposta: diasAPartirDeHoje(-2),
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: null,
    servicos: [
      {
        id: "11",
        titulo: "Limpeza Pos-obra",
        descricao: "Pedido do cliente",
        precoBase: 160,
        duracaoEstimadaMinutos: 180,
        unidadeCobranca: UnidadeCobranca.Total,
        tipoAnuncio: 2,
        categoria: { id: "cat-1", nome: "Faxina", icone: "cleaning_services" },
      },
    ],
  },
];

const MOCK_PERFIL: PerfilForm = {
  nome: "Usuario Demo",
  email: "demo@hometask.com",
  telefone: "(47) 99123-4567",
  documento: "123.456.789-00",
  cep: "89010-001",
  logradouro: "Rua XV de Novembro, 320",
  bairro: "Centro",
  cidade: "Blumenau",
  estado: "SC",
  descricao: "Profissional com experiencia.",
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
  "10": "Servicos Gerais",
};

export const handlers = [
  http.get("*/api/ServicoOferecido/BuscarServicos", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    let result = [...MOCK_SERVICOS].filter((s) => !!s.prestadorId);
    const pagina = Math.max(Number(url.searchParams.get("pagina") ?? "1"), 1);
    const tamanhoPagina = Number(url.searchParams.get("tamanhoPagina") ?? "30");

    const categoria = url.searchParams.get("categoria");
    if (categoria) {
      const label = CATEGORIA_LABELS[categoria] ?? "";
      result = result.filter((s) => s.categoria === label);
    }

    const cidade = url.searchParams.get("cidade");
    if (cidade) {
      result = result.filter((s) => (s.cidade ?? "").toLowerCase().includes(cidade.toLowerCase()));
    }

    const precoMaximo = url.searchParams.get("precoMaximo");
    if (precoMaximo) {
      result = result.filter((s) => s.precoBase <= Number(precoMaximo));
    }

    const totalRegistros = result.length;
    const itens = result.slice((pagina - 1) * tamanhoPagina, pagina * tamanhoPagina);

    return HttpResponse.json({
      itens,
      paginaAtual: pagina,
      tamanhoPagina,
      totalRegistros,
      totalPaginas: totalRegistros === 0 ? 0 : Math.ceil(totalRegistros / tamanhoPagina),
    });
  }),

  http.get("*/api/ServicoOferecido/ObterServicoPorId", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const id = url.searchParams.get("id");
    const servico = MOCK_SERVICOS.find((s) => String(s.id) === id);
    if (!servico) return new HttpResponse(null, { status: 404 });
    return HttpResponse.json(servico);
  }),

  http.get("*/api/Avaliacao/ObterAvaliacoesPorPrestador", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const prestadorId = Number(url.searchParams.get("prestadorId"));
    return HttpResponse.json(MOCK_AVALIACOES[prestadorId] ?? []);
  }),

  http.post("*/api/Auth/Login", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_USER);
  }),
  http.post("*/api/Auth/Logout", async () => new HttpResponse(null, { status: 200 })),
  http.post("*/api/Auth/Refresh", async () => new HttpResponse(null, { status: 200 })),
  http.post("*/api/Auth/EsqueciSenha", async () => new HttpResponse(null, { status: 200 })),
  http.post("*/api/Auth/RedefinirSenha", async () => new HttpResponse(null, { status: 200 })),
  http.post("*/api/Usuario/CriarUsuario", async () => HttpResponse.json(MOCK_USER, { status: 201 })),

  http.get("*/api/Cliente/ObterClientesPorUsuarioId", async () => HttpResponse.json(MOCK_CLIENTE)),
  http.get("*/api/Prestador/ObterPrestadorPorUsuarioId", async () => HttpResponse.json(MOCK_PRESTADOR)),

  http.post("*/api/Agendamento/CriarAgendamento", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json({ id: Math.floor(Math.random() * 9000) + 1000 }, { status: 201 });
  }),

  http.get("*/api/Agendamento/ObterMeusAgendamentosPrestador", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_AGENDAMENTOS_PRESTADOR);
  }),

  http.get("*/api/Agendamento/ObterMeusAgendamentosCliente", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(MOCK_AGENDAMENTOS_CLIENTE);
  }),

  http.get("*/api/Agendamento/ObterAgendamentoPorId", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const id = url.searchParams.get("id");
    const todos = [...MOCK_AGENDAMENTOS_CLIENTE, ...MOCK_AGENDAMENTOS_PRESTADOR];
    const alvo = todos.find((a) => a.id === id);
    return HttpResponse.json(alvo ?? null, { status: alvo ? 200 : 404 });
  }),

  http.get("*/api/Agendamento/ObterSolicitacoesPendentesPrestador", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(
      MOCK_AGENDAMENTOS_PRESTADOR.filter((a) => a.status === "Solicitado" && a.aguardandoRespostaDe === "Prestador"),
    );
  }),

  http.post("*/api/Agendamento/AceitarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string };
    const todos = [...MOCK_AGENDAMENTOS_PRESTADOR, ...MOCK_AGENDAMENTOS_CLIENTE];
    const alvo = todos.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = "Aceito";
      alvo.aguardandoRespostaDe = null;
      alvo.dataResposta = new Date().toISOString();
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/Agendamento/RecusarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string; motivoRecusa?: string };
    const todos = [...MOCK_AGENDAMENTOS_PRESTADOR, ...MOCK_AGENDAMENTOS_CLIENTE];
    const alvo = todos.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = "Recusado";
      alvo.dataResposta = new Date().toISOString();
      alvo.motivoRecusa = body.motivoRecusa ?? "Sem motivo informado";
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/Agendamento/IniciarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string };
    const alvo = MOCK_AGENDAMENTOS_PRESTADOR.find((a) => a.id === body.id);
    if (alvo) alvo.status = "EmAndamento";
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/Agendamento/ConcluirAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string };
    const alvo = MOCK_AGENDAMENTOS_PRESTADOR.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = "Concluido";
      alvo.dataConclusao = new Date().toISOString();
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/Agendamento/CancelarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string; motivoRecusa?: string };
    const todos = [...MOCK_AGENDAMENTOS_PRESTADOR, ...MOCK_AGENDAMENTOS_CLIENTE];
    const alvo = todos.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = "Cancelado";
      alvo.motivoRecusa = body.motivoRecusa ?? "Cancelado";
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/ServicoOferecido/CriarServicoCliente", async () => HttpResponse.json({ id: 999 }, { status: 201 })),
  http.get("*/api/Usuarios/perfil", async () => HttpResponse.json(MOCK_PERFIL)),
  http.put("*/api/Usuarios/perfil", async ({ request }) => {
    const body = (await request.json()) as Partial<PerfilForm>;
    Object.assign(MOCK_PERFIL, body);
    return HttpResponse.json(MOCK_PERFIL);
  }),
];
