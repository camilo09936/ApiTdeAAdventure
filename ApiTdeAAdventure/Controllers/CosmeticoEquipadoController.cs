using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;
using ApiTdeAAdventure.Repository.Interfaces;

namespace ApiTdeAAdventure.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones relacionadas con los cosmeticos equipados
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CosmeticoEquipadoController : ControllerBase
    {
        private readonly ILogger<CosmeticoEquipadoController> _logger;
        private readonly ICosmeticoEquipadoQueries _query;
        private readonly ICosmeticoEquipadoRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="logger">Logger del controlador</param>
        /// <param name="query">Consultas para acceder a los cosmeticos equipados</param>
        /// <param name="repo">Repositorio para modificar los cosmeticos equipados</param>
        public CosmeticoEquipadoController(
            ILogger<CosmeticoEquipadoController> logger,
            ICosmeticoEquipadoQueries query,
            ICosmeticoEquipadoRepository repo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el cosmetico equipado de un jugador
        /// </summary>
        /// <param name="jugadorId">Id del jugador</param>
        /// <response code="200">Devuelve el cosmetico equipado.</response>
        /// <response code="400">El ID no tiene un formato valido.</response>
        /// <response code="404">El jugador no tiene un cosmetico equipado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{jugadorId}")]
        [ProducesResponseType(typeof(CosmeticoEquipado), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CosmeticoEquipado>> Get(string jugadorId)
        {
            try
            {
                if (!ObjectId.TryParse(jugadorId, out _))
                {
                    return BadRequest();
                }

                _logger.LogInformation(
                    "Consultando cosmetico equipado del jugador con ID: {Id}",
                    jugadorId);

                var cosmetico = await _query.Get(jugadorId);

                if (cosmetico == null)
                {
                    return NotFound();
                }

                return Ok(cosmetico);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error consultando cosmetico equipado del jugador con ID: {Id}",
                    jugadorId);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Equipa un cosmetico a un jugador
        /// </summary>
        /// <param name="jugadorId">Id del jugador</param>
        /// <param name="cosmetico">Cosmetico que se desea equipar</param>
        /// <response code="200">Cosmetico equipado exitosamente.</response>
        /// <response code="400">Solicitud invalida.</response>
        /// <response code="404">No se encontro el jugador o el cosmetico no esta desbloqueado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{jugadorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Equipar(
            string jugadorId,
            [FromBody] CosmeticoEquipado cosmetico)
        {
            try
            {
                if (!ObjectId.TryParse(jugadorId, out _))
                {
                    return BadRequest();
                }

                if (cosmetico == null ||
                    string.IsNullOrWhiteSpace(cosmetico.CosmeticoId))
                {
                    return BadRequest();
                }

                _logger.LogInformation(
                    "Equipando cosmetico {CosmeticoId} al jugador {JugadorId}",
                    cosmetico.CosmeticoId,
                    jugadorId);

                var resultado = await _repo.Equipar(jugadorId, cosmetico);

                if (!resultado)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error equipando cosmetico al jugador {JugadorId}",
                    jugadorId);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Desequipa el cosmetico actual de un jugador
        /// </summary>
        /// <param name="jugadorId">Id del jugador</param>
        /// <response code="204">Cosmetico desequipado exitosamente.</response>
        /// <response code="400">El ID no tiene un formato valido.</response>
        /// <response code="404">No se encontro el jugador.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{jugadorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Desequipar(string jugadorId)
        {
            try
            {
                if (!ObjectId.TryParse(jugadorId, out _))
                {
                    return BadRequest();
                }

                _logger.LogInformation(
                    "Desequipando cosmetico del jugador con ID: {Id}",
                    jugadorId);

                var resultado = await _repo.Desequipar(jugadorId);

                if (!resultado)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error desequipando cosmetico del jugador con ID: {Id}",
                    jugadorId);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}