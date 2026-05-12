using HomeTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeTask.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CidadeController : ControllerBase
    {
        private readonly ICidadeService _cidadeService;

        public CidadeController(ICidadeService cidadeService)
        {
            _cidadeService = cidadeService;
        }

        /// <summary>
        /// Lista todas as cidades para uso em dropdowns
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Listar(CancellationToken cancellationToken)
        {
            return Ok(await _cidadeService.ListarAsync(cancellationToken));
        }

        /// <summary>
        /// Busca cidades por nome ou estado para autocomplete
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Buscar([FromQuery] string termo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return BadRequest("Informe um termo para busca.");

            return Ok(await _cidadeService.BuscarAsync(termo, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> ObterPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var cidade = await _cidadeService.ObterPorIdAsync(id, cancellationToken);
            if (cidade == null)
                return NotFound();

            return Ok(cidade);
        }
    }
}
