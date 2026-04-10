using HomeTask.Domain.Contratos;
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
                retorno.ServicoOferecidoId = agendamento.ServicoOferecidoId;
                retorno.DataHoraAgendada = agendamento.DataHoraAgendada;
                retorno.DuracaoMinutos = agendamento.DuracaoMinutos;
                retorno.Status = agendamento.Status;
                retorno.EnderecoServico = agendamento.EnderecoServico;
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

                retorno.Id = contrato.Id;
                retorno.ClienteId = contrato.ClienteId;
                retorno.PrestadorId = contrato.PrestadorId;
                retorno.ServicoOferecidoId = contrato.ServicoOferecidoId;
                retorno.DataHoraAgendada = contrato.DataHoraAgendada;
                retorno.DuracaoMinutos = contrato.DuracaoMinutos;
                retorno.Status = contrato.Status;
                retorno.EnderecoServico = contrato.EnderecoServico;
                retorno.Observacoes = contrato.Observacoes;
                retorno.ValorTotal = contrato.ValorTotal;
                retorno.DataSolicitacao = contrato.DataSolicitacao;
                retorno.DataResposta = contrato.DataResposta;
                retorno.DataConclusao = contrato.DataConclusao;
                retorno.MotivoRecusa = contrato.MotivoRecusa;

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
                retorno.ServicoOferecidoId = contrato.ServicoOferecidoId;
                retorno.DataHoraAgendada = contrato.DataHoraAgendada;
                retorno.DuracaoMinutos = contrato.DuracaoMinutos;
                retorno.Status = contrato.Status;
                retorno.EnderecoServico = contrato.EnderecoServico;
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

        public AgendamentoContrato ConverterViewModelparaContrato(AgendamentoViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new AgendamentoContrato();

                retorno.Id = viewModel.Id;
                retorno.ClienteId = viewModel.ClienteId;
                retorno.PrestadorId = viewModel.PrestadorId;
                retorno.ServicoOferecidoId = viewModel.ServicoOferecidoId;
                retorno.DataHoraAgendada = viewModel.DataHoraAgendada;
                retorno.DuracaoMinutos = viewModel.DuracaoMinutos;
                retorno.Status = viewModel.Status;
                retorno.EnderecoServico = viewModel.EnderecoServico;
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
    }
}
