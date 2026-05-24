using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorAgendamento : IConversorAgendamento
    {
        public AgendamentoContrato ConverterAgendamentoparaContrato(Domain.Entities.Agendamento? agendamento)
        {
            if (agendamento == null)
                throw new ArgumentNullException(nameof(agendamento));

            var retorno = new AgendamentoContrato();
                retorno.Id = agendamento.Id;
                retorno.ClienteId = agendamento.ClienteId;
                retorno.PrestadorId = agendamento.PrestadorId;
                retorno.ServicosOferecidosIds = agendamento.AgendamentoServicos.Select(s => s.ServicoBaseId).ToList();
                retorno.DataHoraAgendada = agendamento.DataHoraAgendada;
                retorno.DuracaoMinutos = agendamento.DuracaoMinutos;
                retorno.Status = agendamento.Status;
                retorno.EnderecoId = agendamento.EnderecoId;
                retorno.Observacoes = agendamento.Observacoes;
                retorno.ValorTotal = agendamento.ValorTotal;
                retorno.DataSolicitacao = agendamento.DataSolicitacao;
                retorno.DataResposta = agendamento.DataResposta;
                retorno.DataConclusao = agendamento.DataConclusao;
                retorno.MotivoRecusa = agendamento.MotivoRecusa;

                return retorno;
        }

        public Domain.Entities.Agendamento? ConverterContratoparaAgendamento(AgendamentoContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Domain.Entities.Agendamento();
                retorno.DefinirDados(
                    contrato.Id,
                    contrato.ClienteId,
                    contrato.PrestadorId,
                    contrato.DataHoraAgendada,
                    contrato.DuracaoMinutos,
                    contrato.Status,
                    contrato.EnderecoId,
                    contrato.Observacoes,
                    contrato.ValorTotal,
                    contrato.DataSolicitacao,
                    contrato.DataResposta,
                    contrato.DataConclusao,
                    contrato.MotivoRecusa);

                return retorno;
        }

        public AgendamentoViewModel? ConverterContratoparaViewModel(AgendamentoContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new AgendamentoViewModel();

                retorno.Id = contrato.Id;
                retorno.ClienteId = contrato.ClienteId;
                retorno.PrestadorId = contrato.PrestadorId;
                retorno.ServicosOferecidosIds = contrato.ServicosOferecidosIds;
                retorno.DataHoraAgendada = contrato.DataHoraAgendada;
                retorno.DuracaoMinutos = contrato.DuracaoMinutos;
                retorno.Status = contrato.Status;
                retorno.EnderecoId = contrato.EnderecoId;
                retorno.Observacoes = contrato.Observacoes;
                retorno.ValorTotal = contrato.ValorTotal;
                retorno.DataSolicitacao = contrato.DataSolicitacao;
                retorno.DataResposta = contrato.DataResposta;
                retorno.DataConclusao = contrato.DataConclusao;
                retorno.MotivoRecusa = contrato.MotivoRecusa;

                return retorno;
            }
            return null;
        }

        public AgendamentoResumoViewModel? ConverterResumoContratoparaViewModel(AgendamentoResumoContrato contrato)
        {
            if (contrato == null)
                return null;

            return new AgendamentoResumoViewModel
            {
                Id = contrato.Id,
                ClienteId = contrato.ClienteId,
                ClienteNome = contrato.ClienteNome,
                PrestadorId = contrato.PrestadorId,
                PrestadorNome = contrato.PrestadorNome,
                DataHoraAgendada = contrato.DataHoraAgendada,
                DuracaoMinutos = contrato.DuracaoMinutos,
                Status = contrato.Status,
                Observacoes = contrato.Observacoes,
                ValorTotal = contrato.ValorTotal,
                DataSolicitacao = contrato.DataSolicitacao,
                DataResposta = contrato.DataResposta,
                DataConclusao = contrato.DataConclusao,
                MotivoRecusa = contrato.MotivoRecusa,
                AguardandoRespostaDe = contrato.AguardandoRespostaDe,
                Endereco = new EnderecoResumoViewModel
                {
                    Logradouro = contrato.Endereco.Logradouro,
                    Bairro = contrato.Endereco.Bairro,
                    Cidade = contrato.Endereco.Cidade,
                    Estado = contrato.Endereco.Estado
                },
                Servicos = contrato.Servicos.Select(s => new ServicoResumoViewModel
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    PrecoBase = s.PrecoBase,
                    TipoAnuncio = s.TipoAnuncio
                }).ToList()
            };
        }

        public AgendamentoContrato ConverterViewModelparaContrato(AgendamentoViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new AgendamentoContrato();

                retorno.Id = viewModel.Id;
                retorno.ClienteId = viewModel.ClienteId;
                retorno.PrestadorId = viewModel.PrestadorId;
                retorno.ServicosOferecidosIds = viewModel.ServicosOferecidosIds;
                retorno.DataHoraAgendada = viewModel.DataHoraAgendada;
                retorno.DuracaoMinutos = viewModel.DuracaoMinutos;
                retorno.Status = viewModel.Status;
                retorno.EnderecoId = viewModel.EnderecoId;
                retorno.Observacoes = viewModel.Observacoes;
                retorno.ValorTotal = viewModel.ValorTotal;
                retorno.DataSolicitacao = viewModel.DataSolicitacao;
                retorno.DataResposta = viewModel.DataResposta;
                retorno.DataConclusao = viewModel.DataConclusao;
                retorno.MotivoRecusa = viewModel.MotivoRecusa;

                return retorno;
            }
            return null;

        }

        public AgendamentoResumoContrato ConverterAgendamentoparaResumoContrato(Domain.Entities.Agendamento? agendamento)
        {
            if (agendamento == null)
                throw new ArgumentNullException(nameof(agendamento));

            return new AgendamentoResumoContrato
            {
                Id = agendamento.Id,
                ClienteId = agendamento.ClienteId,
                ClienteNome = agendamento.Cliente?.Usuario?.Nome ?? string.Empty,
                PrestadorId = agendamento.PrestadorId,
                PrestadorNome = agendamento.Prestador?.Usuario?.Nome ?? string.Empty,
                DataHoraAgendada = agendamento.DataHoraAgendada,
                DuracaoMinutos = agendamento.DuracaoMinutos,
                Status = agendamento.Status,
                Observacoes = agendamento.Observacoes,
                ValorTotal = agendamento.ValorTotal,
                DataSolicitacao = agendamento.DataSolicitacao,
                DataResposta = agendamento.DataResposta,
                DataConclusao = agendamento.DataConclusao,
                MotivoRecusa = agendamento.MotivoRecusa,
                AguardandoRespostaDe = DeterminarAguardandoRespostaDe(agendamento),
                Endereco = new EnderecoResumoContrato
                {
                    Logradouro = agendamento.Endereco?.Logradouro ?? string.Empty,
                    Bairro = agendamento.Endereco?.Bairro ?? string.Empty,
                    Cidade = agendamento.Endereco?.Cidade?.Nome ?? string.Empty,
                    Estado = agendamento.Endereco?.Cidade?.Estado ?? string.Empty
                },
                Servicos = agendamento.AgendamentoServicos.Select(s => new ServicoResumoContrato
                {
                    Id = s.ServicoBaseId,
                    Titulo = s.ServicoBase?.Titulo ?? string.Empty,
                    PrecoBase = s.ValorUnitario,
                    TipoAnuncio = s.ServicoBase is null ? null : (int)s.ServicoBase.TipoAnuncio
                }).ToList()
            };
        }

        private static string? DeterminarAguardandoRespostaDe(Domain.Entities.Agendamento agendamento)
        {
            if (agendamento.Status != StatusAgendamento.Solicitado)
                return null;

            var possuiPedidoCliente = agendamento.AgendamentoServicos
                .Any(s => s.ServicoBase is ServicoCliente);

            return possuiPedidoCliente ? "Cliente" : "Prestador";
        }
    }
}
