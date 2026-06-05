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

export enum StatusAgendamento {
  Solicitado = 1,
  Aceito = 2,
  Recusado = 3,
  EmAndamento = 4,
  Concluido = 5,
  Cancelado = 6,
}
