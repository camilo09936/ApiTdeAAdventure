using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;
using ApiTdeAAdventure.Repository.Interfaces;

namespace ApiTdeAAdventure.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones relacionadas con las partidas del juego
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PartidaController : ControllerBase
    {
        private readonly ILogger<PartidaController> _logger;
        private readonly IPartidaQueries _query;
        private readonly IPartidaRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PartidaController"/>
        /// </summary>
        public PartidaController(ILogger<PartidaController> logger, IPartidaQueries query, IPartidaRepository repo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene todas las partidas registradas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Partida>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Partida>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Consultando todas las partidas");
                var partidas = await _query.GetAll();
                return Ok(partidas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando todas las partidas");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene una partida específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Partida), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Partida>> GetById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("El formato del ID no es válido.");
                }

                _logger.LogInformation("Consultando partida por ID: {Id}", id);
                var partida = await _query.Get(id);

                if (partida == null)
                {
                    return NotFound();
                }

                return Ok(partida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando partida por ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene todas las partidas realizadas por un jugador específico
        /// </summary>
        [HttpGet("jugador/{jugadorId}")]
        [ProducesResponseType(typeof(IEnumerable<Partida>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Partida>>> GetByJugador(string jugadorId)
        {
            try
            {
                if (!ObjectId.TryParse(jugadorId, out _))
                {
                    return BadRequest("El formato del ID del jugador no es válido.");
                }

                _logger.LogInformation("Consultando partidas del jugador: {JugadorId}", jugadorId);
                var partidas = await _query.GetByJugador(jugadorId);

                return Ok(partidas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando partidas del jugador: {JugadorId}", jugadorId);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Registra una nueva partida finalizada
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Partida), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Partida>> Create([FromBody] Partida partida)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partida.JugadorId))
                {
                    return BadRequest("El ID del jugador es obligatorio.");
                }

                // Aseguramos que la fecha se guarde en UTC si el cliente no la envía correctamente
                if (partida.FechaPartida == default)
                {
                    partida.FechaPartida = DateTime.UtcNow;
                }

                _logger.LogInformation("Registrando nueva partida para el jugador: {JugadorId}", partida.JugadorId);
                var nuevaPartida = await _repo.Add(partida);

                return CreatedAtAction(nameof(GetById), new { id = nuevaPartida.Id }, nuevaPartida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando nueva partida");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Elimina una partida del historial por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("El formato del ID no es válido.");
                }

                _logger.LogInformation("Eliminando partida con ID: {Id}", id);
                await _repo.Delete(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando partida con ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}