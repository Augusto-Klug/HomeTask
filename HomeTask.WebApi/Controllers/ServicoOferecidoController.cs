using System.Security.Claims;
using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ServicoOferecidoController : ControllerBase
    {
        private readonly IServicoPrestadorService _servicoPrestadorService;
        private readonly IServicoClienteService _servicoClienteService;
        private readonly IClienteService _clienteService;
        private readonly IPrestadorService _prestadorService;

        public ServicoOferecidoController(
            IServicoPrestadorService servicoPrestadorService, 
            IServicoClienteService servicoClienteService,
            IClienteService clienteService,
            IPrestadorService prestadorService)
        {
            _servicoPrestadorService = servicoPrestadorService;
            _servicoClienteService = servicoClienteService;
            _clienteService = clienteService;
            _prestadorService = prestadorService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> ObterServicoPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var (prestadorAtualId, clienteAtualId, _) = await ObterContextoUsuarioAtualAsync(cancellationToken);
            var servicoPrestador = await _servicoPrestadorService.ObterPorIdAsync(id, cancellationToken);
            if (servicoPrestador != null)
            {
                if (!PodeVisualizar(servicoPrestador.PrestadorId, prestadorAtualId, clienteAtualId, true))
                    return NotFound();
                return servicoPrestador;
            }

            var servicoCliente = await _servicoClienteService.ObterPorIdAsync(id, cancellationToken);
            if (servicoCliente != null)
            {
                if (!PodeVisualizar(servicoCliente.ClienteId, prestadorAtualId, clienteAtualId, false))
                    return NotFound();
                return servicoCliente;
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicoPrestadorDto>>> ObterServicosPorPrestador([FromQuery] Guid prestadorId, CancellationToken cancellationToken)
        {
            var servicos = await _servicoPrestadorService.ObterPorPrestadorAsync(prestadorId, cancellationToken);
            return servicos.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<ServicoBuscaPaginadaDto>> BuscarServicos(
            CategoriaServico? categoria,
            string? cidade,
            decimal? precoMaximo,
            int pagina = 1,
            int tamanhoPagina = 30,
            CancellationToken cancellationToken = default)
        {
            var (_, _, usuarioId) = await ObterContextoUsuarioAtualAsync(cancellationToken);
            var resultado = await _servicoPrestadorService.BuscarTodosPaginadoAsync(
                categoria,
                cidade,
                precoMaximo,
                usuarioId,
                pagina,
                tamanhoPagina,
                cancellationToken);
            return resultado;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicoClienteDto>>> BuscarPedidos(CategoriaServico? categoria, string? cidade, decimal? precoMaximo, CancellationToken cancellationToken)
        {
            var servicos = await _servicoClienteService.BuscarPedidosAsync(categoria, cidade, precoMaximo, cancellationToken);
            return servicos.ToList();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServicoPrestadorDto>> CriarServicoPrestador(ServicoPrestadorDto dto, CancellationToken cancellationToken)
        {
            var idUsuario = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(idUsuario, cancellationToken);
            
            if (prestador == null)
                return StatusCode(StatusCodes.Status403Forbidden, "Usuário não possui um perfil de prestador.");

            dto.PrestadorId = prestador.Id;
            var servico = await _servicoPrestadorService.CriarAsync(dto, cancellationToken);
            return servico;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServicoClienteDto>> CriarServicoCliente(ServicoClienteDto dto, CancellationToken cancellationToken)
        {
            var idUsuario = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var cliente = await _clienteService.ObterPorUsuarioIdAsync(idUsuario, cancellationToken);

            if (cliente == null)
                return StatusCode(StatusCodes.Status403Forbidden, "Usuário não possui um perfil de cliente.");

            dto.ClienteId = cliente.Id;
            var servico = await _servicoClienteService.CriarAsync(dto, cancellationToken);
            return servico;
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ServicoPrestadorDto>> AtualizarServicoPrestador(ServicoPrestadorDto dto, CancellationToken cancellationToken)
        {
            var servico = await _servicoPrestadorService.AtualizarAsync(dto, cancellationToken);
            return servico;
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

        private async Task<(Guid? PrestadorId, Guid? ClienteId, Guid? UsuarioId)> ObterContextoUsuarioAtualAsync(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return (null, null, null);

            var prestador = await _prestadorService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);
            var cliente = await _clienteService.ObterPorUsuarioIdAsync(usuarioId, cancellationToken);

            return (prestador?.Id, cliente?.Id, usuarioId);
        }

        private static bool PodeVisualizar(Guid? donoId, Guid? prestadorAtualId, Guid? clienteAtualId, bool ehServicoPrestador)
        {
            if (!donoId.HasValue)
                return true;

            if (ehServicoPrestador && prestadorAtualId.HasValue)
                return donoId.Value != prestadorAtualId.Value;

            if (!ehServicoPrestador && clienteAtualId.HasValue)
                return donoId.Value != clienteAtualId.Value;

            return true;
        }
    }
}
