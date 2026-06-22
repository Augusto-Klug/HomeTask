import { delay, http, HttpResponse } from "msw";
import type { AgendamentoResumo, AuthResponse, Avaliacao, AvaliacaoCliente, ClientePerfilPublico, PagamentoResumo, PerfilForm, PrestadorPerfilPublico, Servico } from "@/types";
import { StatusAgendamento, StatusPagamento, TipoUsuario, UnidadeCobranca } from "@/types";

const MOCK_DELAY = 200;

const MOCK_USER: AuthResponse = {
  userId: "1",
  nome: "Usuário Demo",
  email: "demo@hometask.com",
  tipo: 2,
};

const MOCK_CLIENTE = { id: "1", usuarioId: "1", nome: "Usuário Demo" };
const MOCK_PRESTADOR = { id: "1", usuarioId: "1", nome: "Usuário Demo" };

const MOCK_SERVICOS: Servico[] = [
  {
    id: "1",
    titulo: "Faxina Residencial Completa",
    descricao: "Limpeza completa de residências com produtos de qualidade.",
    precoBase: 80,
    unidadeCobranca: UnidadeCobranca.Total,
    tipoAnuncio: 1,
    prestadorId: "10",
    prestadorNome: "Maria Silva",
    categoria: 1,
    logradouro: "Rua das Palmeiras",
    numero: "120",
    bairro: "Centro",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.5,
    mediaAvaliacoesPrestador: 4.7,
  },
  {
    id: "2",
    titulo: "Jardinagem e Poda",
    descricao: "Serviços de jardinagem, poda e manutenção.",
    precoBase: 60,
    unidadeCobranca: UnidadeCobranca.PorHora,
    tipoAnuncio: 1,
    prestadorId: "11",
    prestadorNome: "Joao Santos",
    categoria: 2,
    logradouro: "Rua Sao Paulo",
    numero: "88",
    bairro: "Victor Konder",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4,
    mediaAvaliacoesPrestador: 4.2,
  },
  {
    id: "11",
    titulo: "Limpeza Pós-obra",
    descricao: "Preciso de uma limpeza completa depois da reforma.",
    precoBase: 160,
    unidadeCobranca: UnidadeCobranca.Total,
    tipoAnuncio: 2,
    clienteId: "2",
    clienteNome: "Pedro Martins",
    categoria: 1,
    logradouro: "Rua XV de Novembro",
    numero: "320",
    bairro: "Centro",
    cidade: "Blumenau",
    estado: "SC",
    mediaAvaliacoes: 4.4,
    totalAvaliacoes: 8,
    dataDesejada: diasAPartirDeHoje(3, "10:00"),
  },
];

const MOCK_AVALIACOES: Record<string, Avaliacao[]> = {
  "10": [
    {
      id: "1",
      agendamentoId: "ag-c-010",
      clienteNome: "Pedro Martins",
      servicoPrestadorId: "1",
      servicoTitulo: "Faxina Residencial Completa",
      notaServico: 5,
      notaPrestador: 5,
      comentario: "Excelente trabalho",
      dataAvaliacao: "2025-03-10T14:00:00Z",
    },
  ],
  "11": [
    {
      id: "2",
      agendamentoId: "ag-c-011",
      clienteNome: "Luisa Ferreira",
      servicoPrestadorId: "2",
      servicoTitulo: "Jardinagem e Poda",
      notaServico: 4,
      notaPrestador: 4,
      comentario: "Bom atendimento",
      dataAvaliacao: "2025-03-05T16:00:00Z",
    },
  ],
};

const MOCK_AVALIACOES_POR_AGENDAMENTO: Record<string, Avaliacao> = {};
const MOCK_AVALIACOES_CLIENTE_POR_AGENDAMENTO: Record<string, AvaliacaoCliente> = {};

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
    clienteNome: "Usuário Demo",
    prestadorId: "10",
    prestadorNome: "Maria Silva",
    dataHoraAgendada: diasAPartirDeHoje(2, "09:00"),
    duracaoMinutos: 120,
    status: StatusAgendamento.Aceito,
    endereco: { logradouro: "Rua XV, 320", bairro: "Centro", cidade: "Blumenau", estado: "SC" },
    observacoes: "Dar atenção especial à cozinha.",
    valorTotal: 160,
    dataSolicitacao: diasAPartirDeHoje(-3),
    dataResposta: diasAPartirDeHoje(-2),
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: null,
    podeClienteAvaliarPrestador: false,
    clienteJaAvaliouPrestador: false,
    podePrestadorAvaliarCliente: false,
    prestadorJaAvaliouCliente: false,
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
    clienteNome: "Usuário Demo",
    prestadorId: "1",
    prestadorNome: "Usuário Demo",
    dataHoraAgendada: diasAPartirDeHoje(5, "14:30"),
    duracaoMinutos: 180,
    status: StatusAgendamento.Solicitado,
    endereco: { logradouro: "Rua Alameda, 45", bairro: "Velha", cidade: "Blumenau", estado: "SC" },
    observacoes: null,
    valorTotal: 150,
    dataSolicitacao: diasAPartirDeHoje(-1),
    dataResposta: null,
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: TipoUsuario.Cliente,
    podeClienteAvaliarPrestador: false,
    clienteJaAvaliouPrestador: false,
    podePrestadorAvaliarCliente: false,
    prestadorJaAvaliouCliente: false,
    servicos: [
      {
        id: "11",
        titulo: "Limpeza Pós-obra",
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
    prestadorNome: "Usuário Demo",
    dataHoraAgendada: diasAPartirDeHoje(1, "10:00"),
    duracaoMinutos: 120,
    status: StatusAgendamento.Solicitado,
    endereco: { logradouro: "Av. Brasil, 1001", bairro: "Ponta Aguda", cidade: "Blumenau", estado: "SC" },
    observacoes: null,
    valorTotal: 80,
    dataSolicitacao: diasAPartirDeHoje(-1),
    dataResposta: null,
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: TipoUsuario.Prestador,
    podeClienteAvaliarPrestador: false,
    clienteJaAvaliouPrestador: false,
    podePrestadorAvaliarCliente: false,
    prestadorJaAvaliouCliente: false,
    servicos: [
      {
        id: "srv-012",
        titulo: "Faxina Rápida",
        descricao: "Serviço de prestador",
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
    prestadorNome: "Usuário Demo",
    dataHoraAgendada: diasAPartirDeHoje(3, "15:00"),
    duracaoMinutos: 180,
    status: StatusAgendamento.Aceito,
    endereco: { logradouro: "Rua Hermann Hering, 1800", bairro: "Itoupava Norte", cidade: "Blumenau", estado: "SC" },
    observacoes: "Pronto para iniciar",
    valorTotal: 160,
    dataSolicitacao: diasAPartirDeHoje(-3),
    dataResposta: diasAPartirDeHoje(-2),
    dataConclusao: null,
    motivoRecusa: null,
    aguardandoRespostaDe: null,
    podeClienteAvaliarPrestador: false,
    clienteJaAvaliouPrestador: false,
    podePrestadorAvaliarCliente: false,
    prestadorJaAvaliouCliente: false,
    servicos: [
      {
        id: "11",
        titulo: "Limpeza Pós-obra",
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

const MOCK_PAGAMENTOS: PagamentoResumo[] = [];

const MOCK_PERFIL: PerfilForm = {
  nome: "Usuário Demo",
  email: "demo@hometask.com",
  telefone: "(47) 99123-4567",
  documento: "123.456.789-00",
  cep: "89010-001",
  logradouro: "Rua XV de Novembro, 320",
  bairro: "Centro",
  cidade: "Blumenau",
  cidadeId: "cidade-blumenau",
  estado: "SC",
  descricao: "Profissional com experiência.",
  raioAtendimentoKm: 15,
};

const MOCK_PERFIL_PUBLICO: PrestadorPerfilPublico = {
  id: "10",
  nome: "Maria Silva",
  descricao: "Profissional com experiência.",
  cidade: "Blumenau",
  estado: "SC",
  mediaAvaliacoes: 4.7,
  totalAvaliacoes: 12,
  totalServicosConcluidos: 28,
  servicosOferecidos: MOCK_SERVICOS.filter((item) => item.prestadorId === "10"),
  historicoConcluido: [
    {
      agendamentoId: "ag-h-1",
      servicoPrestadorId: "1",
      tituloServico: "Faxina Residencial Completa",
      dataHoraAgendada: diasAPartirDeHoje(-4, "09:00"),
      cidade: "Blumenau",
      estado: "SC",
      notaServico: 5,
      notaPrestador: 5,
    },
  ],
};

const MOCK_PERFIL_CLIENTE_PUBLICO: ClientePerfilPublico = {
  id: "2",
  nome: "Pedro Martins",
  cidade: "Blumenau",
  estado: "SC",
  mediaAvaliacoes: 4.4,
  totalAvaliacoes: 8,
  totalServicosContratados: 14,
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

export const handlers = [
  http.get("*/api/ServicoOferecido/BuscarServicos", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    let result = [...MOCK_SERVICOS];
    const pagina = Math.max(Number(url.searchParams.get("pagina") ?? "1"), 1);
    const tamanhoPagina = Number(url.searchParams.get("tamanhoPagina") ?? "30");

    const categoria = url.searchParams.get("categoria");
    if (categoria) {
      const valorCategoria = Number(categoria);
      result = result.filter((s) => s.categoria === valorCategoria);
    }

    const cidade = url.searchParams.get("cidade");
    if (cidade) {
      result = result.filter((s) => (s.cidade ?? "").toLowerCase().includes(cidade.toLowerCase()));
    }

    const precoMaximo = url.searchParams.get("precoMaximo");
    if (precoMaximo) {
      result = result.filter((s) => s.precoBase <= Number(precoMaximo));
    }

    const tipoAnuncio = url.searchParams.get("tipoAnuncio");
    if (tipoAnuncio) {
      result = result.filter((s) => s.tipoAnuncio === Number(tipoAnuncio));
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

  http.get("*/api/ServicoOferecido/ObterServicosPorPrestador", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const prestadorId = url.searchParams.get("prestadorId");
    return HttpResponse.json(MOCK_SERVICOS.filter((servico) => servico.prestadorId === prestadorId));
  }),

  http.get("*/api/Avaliacao/ObterAvaliacoesPorPrestador", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const prestadorId = url.searchParams.get("prestadorId") ?? "";
    return HttpResponse.json(MOCK_AVALIACOES[prestadorId] ?? []);
  }),

  http.get("*/api/Avaliacao/ObterAvaliacaoPorAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const agendamentoId = url.searchParams.get("agendamentoId") ?? "";
    const avaliacao = MOCK_AVALIACOES_POR_AGENDAMENTO[agendamentoId];
    if (!avaliacao) return new HttpResponse(null, { status: 404 });
    return HttpResponse.json(avaliacao);
  }),

  http.get("*/api/Avaliacao/ObterAvaliacaoClientePorAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const agendamentoId = url.searchParams.get("agendamentoId") ?? "";
    const avaliacao = MOCK_AVALIACOES_CLIENTE_POR_AGENDAMENTO[agendamentoId];
    if (!avaliacao) return new HttpResponse(null, { status: 404 });
    return HttpResponse.json(avaliacao);
  }),

  http.post("*/api/Avaliacao/CriarAvaliacao", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = await request.json() as Partial<Avaliacao>;
    const avaliacao: Avaliacao = {
      id: crypto.randomUUID(),
      agendamentoId: String(body.agendamentoId ?? ""),
      clienteId: String(body.clienteId ?? ""),
      prestadorId: String(body.prestadorId ?? ""),
      servicoPrestadorId: String(body.servicoPrestadorId ?? ""),
      servicoTitulo: "Serviço avaliado",
      notaServico: Number(body.notaServico ?? 0),
      notaPrestador: Number(body.notaPrestador ?? 0),
      comentario: body.comentario ?? null,
      dataAvaliacao: new Date().toISOString(),
      clienteNome: "Usuário Demo",
    };

    MOCK_AVALIACOES_POR_AGENDAMENTO[avaliacao.agendamentoId] = avaliacao;
    const listaPrestador = MOCK_AVALIACOES[avaliacao.prestadorId ?? ""] ?? [];
    listaPrestador.unshift(avaliacao);
    MOCK_AVALIACOES[avaliacao.prestadorId ?? ""] = listaPrestador;
    return HttpResponse.json(avaliacao);
  }),

  http.post("*/api/Avaliacao/CriarAvaliacaoCliente", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = await request.json() as Partial<AvaliacaoCliente>;
    const avaliacao: AvaliacaoCliente = {
      id: crypto.randomUUID(),
      agendamentoId: String(body.agendamentoId ?? ""),
      clienteId: String(body.clienteId ?? ""),
      prestadorId: String(body.prestadorId ?? ""),
      nota: Number(body.nota ?? 0),
      comentario: body.comentario ?? null,
      dataAvaliacao: new Date().toISOString(),
      clienteNome: "Usuario Demo",
      prestadorNome: "Prestador Demo",
    };

    MOCK_AVALIACOES_CLIENTE_POR_AGENDAMENTO[avaliacao.agendamentoId] = avaliacao;
    return HttpResponse.json(avaliacao);
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
  http.get("*/api/Cliente/ObterPerfilPublico", async () => HttpResponse.json(MOCK_PERFIL_CLIENTE_PUBLICO)),
  http.get("*/api/Prestador/ObterPrestadorPorUsuarioId", async () => HttpResponse.json(MOCK_PRESTADOR)),
  http.get("*/api/Prestador/ObterPerfilPublico", async () => HttpResponse.json(MOCK_PERFIL_PUBLICO)),

  http.post("*/api/Agendamento/CriarAgendamento", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json({ id: crypto.randomUUID() }, { status: 201 });
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
    if (alvo) {
      alvo.clienteJaAvaliouPrestador = Boolean(MOCK_AVALIACOES_POR_AGENDAMENTO[alvo.id]);
      alvo.prestadorJaAvaliouCliente = Boolean(MOCK_AVALIACOES_CLIENTE_POR_AGENDAMENTO[alvo.id]);
      alvo.podeClienteAvaliarPrestador = alvo.status === StatusAgendamento.Concluido && !alvo.clienteJaAvaliouPrestador;
      alvo.podePrestadorAvaliarCliente = alvo.status === StatusAgendamento.Concluido && !alvo.prestadorJaAvaliouCliente;
    }
    return HttpResponse.json(alvo ?? null, { status: alvo ? 200 : 404 });
  }),

  http.get("*/api/Agendamento/ObterSolicitacoesPendentesPrestador", async () => {
    await delay(MOCK_DELAY);
    return HttpResponse.json(
      MOCK_AGENDAMENTOS_PRESTADOR.filter(
        (a) => a.status === StatusAgendamento.Solicitado && a.aguardandoRespostaDe === TipoUsuario.Prestador,
      ),
    );
  }),

  http.post("*/api/Agendamento/AceitarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string };
    const todos = [...MOCK_AGENDAMENTOS_PRESTADOR, ...MOCK_AGENDAMENTOS_CLIENTE];
    const alvo = todos.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = StatusAgendamento.Aceito;
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
      alvo.status = StatusAgendamento.Recusado;
      alvo.dataResposta = new Date().toISOString();
      alvo.motivoRecusa = body.motivoRecusa ?? "Sem motivo informado";
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/Agendamento/IniciarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string };
    const alvo = MOCK_AGENDAMENTOS_PRESTADOR.find((a) => a.id === body.id);
    if (alvo) alvo.status = StatusAgendamento.EmAndamento;
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/Agendamento/ConcluirAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string };
    const alvo = MOCK_AGENDAMENTOS_PRESTADOR.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = StatusAgendamento.AguardandoPagamento;
      alvo.dataConclusao = new Date().toISOString();
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.get("*/api/Pagamento/ObterPagamentoPorAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const url = new URL(request.url);
    const agendamentoId = url.searchParams.get("agendamentoId");
    const pagamento = MOCK_PAGAMENTOS.find((item) => item.agendamentoId === agendamentoId);
    if (!pagamento) return new HttpResponse(null, { status: 404 });
    return HttpResponse.json(pagamento);
  }),

  http.post("*/api/Pagamento/IniciarPagamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { agendamentoId?: string };
    const agendamento = [...MOCK_AGENDAMENTOS_CLIENTE, ...MOCK_AGENDAMENTOS_PRESTADOR].find((item) => item.id === body.agendamentoId);
    if (!agendamento) return new HttpResponse(null, { status: 404 });

    let pagamento = MOCK_PAGAMENTOS.find((item) => item.agendamentoId === agendamento.id);
    if (!pagamento) {
      pagamento = {
        id: crypto.randomUUID(),
        agendamentoId: agendamento.id,
        valor: agendamento.valorTotal,
        status: StatusPagamento.Processando,
        checkoutExternoId: `pref-${agendamento.id}`,
        checkoutUrl: `https://mercadopago.mock/checkout/${agendamento.id}`,
        statusExterno: "pending",
        dataCriacao: new Date().toISOString(),
        dataProcessamento: new Date().toISOString(),
        dataConfirmacao: null,
        motivoRecusa: null,
      };
      MOCK_PAGAMENTOS.push(pagamento);
    }

    return HttpResponse.json(pagamento);
  }),

  http.post("*/api/Agendamento/CancelarAgendamento", async ({ request }) => {
    await delay(MOCK_DELAY);
    const body = (await request.json()) as { id?: string; motivoRecusa?: string };
    const todos = [...MOCK_AGENDAMENTOS_PRESTADOR, ...MOCK_AGENDAMENTOS_CLIENTE];
    const alvo = todos.find((a) => a.id === body.id);
    if (alvo) {
      alvo.status = StatusAgendamento.Cancelado;
      alvo.motivoRecusa = body.motivoRecusa ?? "Cancelado";
    }
    return HttpResponse.json(alvo ?? { ok: true });
  }),

  http.post("*/api/ServicoOferecido/CriarServicoCliente", async () => HttpResponse.json({ id: 999 }, { status: 201 })),
  http.get("*/api/Usuario/ObterPerfilUsuario", async () => HttpResponse.json(MOCK_PERFIL)),
  http.put("*/api/Usuario/AtualizarPrefilUsuario", async ({ request }) => {
    const body = (await request.json()) as Partial<PerfilForm>;
    Object.assign(MOCK_PERFIL, body);
    return HttpResponse.json(MOCK_PERFIL);
  }),
];
