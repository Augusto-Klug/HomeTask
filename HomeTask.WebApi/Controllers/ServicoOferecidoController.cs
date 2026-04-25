using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeTask.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ServicoOferecidoController : ControllerBase
    {
        private readonly IServicoPrestadorService _servicoPrestadorService;
        private readonly IServicoClienteService _servicoClienteService;
        private readonly IClienteService _clienteService;
        private readonly IPrestadorService _prestadorService;
        private readonly IConversorServicoOferecido _conversorServico;

        public ServicoOferecidoController(
            IServicoPrestadorService servicoPrestadorService, 
            IServicoClienteService servicoClienteService,
            IClienteService clienteService,
            IPrestadorService prestadorService,
            IConversorServicoOferecido conversorServico)
        {
            _servicoPrestadorService = servicoPrestadorService;
            _servicoClienteService = servicoClienteService;
            _clienteService = clienteService;
            _prestadorService = prestadorService;
            _conversorServico = conversorServico;
        }

        [HttpGet]
        public async Task<IActionResult> ObterServicoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var servicoPrestador = await _servicoPrestadorService.ObterPorIdAsync(id, cancellationToken);
            if (servicoPrestador != null)
            {
                var contrato = _conversorServico.ConverterEntidadeparaPrestadorContrato(servicoPrestador);
                var viewModel = _conversorServico.ConverterContratoparaPrestadorViewModel(contrato);
                return Ok(viewModel);
            }

            var servicoCliente = await _servicoClienteService.ObterPorIdAsync(id, cancellationToken);
            if (servicoCliente != null)
            {
                var contrato = _conversorServico.ConverterEntidadeparaClienteContrato(servicoCliente);
                var viewModel = _conversorServico.ConverterContratoparaClienteViewModel(contrato);
                return Ok(viewModel);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ObterServicosPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            var servicos = await _servicoPrestadorService.ObterPorPrestadorAsync(prestadorId, cancellationToken);
            var viewModels = servicos.Select(s =>
            {
                var c = _conversorServico.ConverterEntidadeparaPrestadorContrato(s);
                return _conversorServico.ConverterContratoparaPrestadorViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarServicos(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken)
        {
            var servicos = await _servicoPrestadorService.BuscarTodosAsync(categoria, cidade, precoMaximo, cancellationToken);
            
            var viewModels = servicos.Select(s =>
            {
                if (s is ServicoPrestador sp)
                {
                    var c = _conversorServico.ConverterEntidadeparaPrestadorContrato(sp);
                    return (object)_conversorServico.ConverterContratoparaPrestadorViewModel(c);
                }
                else
                {
                    var sc = (ServicoCliente)s;
                    var c = _conversorServico.ConverterEntidadeparaClienteContrato(sc);
                    return (object)_conversorServico.ConverterContratoparaClienteViewModel(c);
                }
            });

            return Ok(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPedidos(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken)
        {
            var servicos = await _servicoClienteService.BuscarPedidosAsync(categoria, cidade, precoMaximo, cancellationToken);
            var viewModels = servicos.Select(s =>
            {
                var c = _conversorServico.ConverterEntidadeparaClienteContrato(s);
                return _conversorServico.ConverterContratoparaClienteViewModel(c);
            });

            return Ok(viewModels);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarServicoPrestador(ServicoPrestadorViewModel viewmodel, CancellationToken cancellationToken)
        {
            var idUsuario = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(idUsuario, cancellationToken);
            
            if (prestador == null)
                return Forbid("Usuário não possui um perfil de prestador.");

            viewmodel.PrestadorId = prestador.Id;
            var contrato = _conversorServico.ConverterPrestadorViewModelparaContrato(viewmodel);
            var servico = _conversorServico.ConverterPrestadorContratoparaEntidade(contrato);

            if (servico == null) return BadRequest();

            var servicoCriado = await _servicoPrestadorService.CriarAsync(servico, cancellationToken);
            var contratoCriado = _conversorServico.ConverterEntidadeparaPrestadorContrato(servicoCriado);
            var viewModelCriado = _conversorServico.ConverterContratoparaPrestadorViewModel(contratoCriado);

            return Ok(viewModelCriado);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarServicoCliente(ServicoClienteViewModel viewmodel, CancellationToken cancellationToken)
        {
            var idUsuario = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var cliente = await _clienteService.ObterPorUsuarioIdAsync(idUsuario, cancellationToken);

            if (cliente == null)
                return Forbid("Usuário não possui um perfil de cliente.");

            viewmodel.ClienteId = cliente.Id;
            var contrato = _conversorServico.ConverterClienteViewModelparaContrato(viewmodel);
            var servico = _conversorServico.ConverterClienteContratoparaEntidade(contrato);

            if (servico == null) return BadRequest();

            var servicoCriado = await _servicoClienteService.CriarAsync(servico, cancellationToken);
            var contratoCriado = _conversorServico.ConverterEntidadeparaClienteContrato(servicoCriado);
            var viewModelCriado = _conversorServico.ConverterContratoparaClienteViewModel(contratoCriado);

            return Ok(viewModelCriado);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> AtualizarServicoPrestador(ServicoPrestadorViewModel viewmodel, CancellationToken cancellationToken)
        {
            var contrato = _conversorServico.ConverterPrestadorViewModelparaContrato(viewmodel);
            var servico = _conversorServico.ConverterPrestadorContratoparaEntidade(contrato);
            if (servico == null) return BadRequest();

            var servicoAtualizado = await _servicoPrestadorService.AtualizarAsync(servico, cancellationToken);
            var contratoAtualizado = _conversorServico.ConverterEntidadeparaPrestadorContrato(servicoAtualizado);
            return Ok(_conversorServico.ConverterContratoparaPrestadorViewModel(contratoAtualizado));
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoverServico(Guid id, CancellationToken cancellationToken)
        {
            var removido = await _servicoPrestadorService.RemoverAsync(id, cancellationToken) || 
                           await _servicoClienteService.RemoverAsync(id, cancellationToken);

            if (!removido) return NotFound();
            return Ok();
        }
    }
}
