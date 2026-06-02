using HomeTask.Application.Interfaces;
using HomeTask.Application.Dtos;
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
        public async Task<ActionResult<IEnumerable<CidadeDto>>> Listar(CancellationToken cancellationToken)
        {
            var cidades = await _cidadeService.ListarAsync(cancellationToken);
            return cidades.ToList();
        }

        /// <summary>
        /// Busca cidades por nome ou estado para autocomplete
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CidadeDto>>> Buscar([FromQuery] string termo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return BadRequest("Informe um termo para busca.");

            var cidades = await _cidadeService.BuscarAsync(termo, cancellationToken);
            return cidades.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<CidadeDto>> ObterPorId([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var cidade = await _cidadeService.ObterPorIdAsync(id, cancellationToken);
            if (cidade == null)
                return NotFound();

            return cidade;
        }
    }
}
