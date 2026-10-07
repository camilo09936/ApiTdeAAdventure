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
        /// <response code="200">Devuelve la lista completa de partidas.</response>
        /// <response code="500">Error interno del servidor.</response>
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
        /// <param name="id">ID de la partida</param>
        /// <response code="200">Devuelve la partida correspondeinte al ID.</response>
        /// <response code="400">El formato del ID no es valido.</response>
        /// <response code="404">No se encontro la partida con el ID proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
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
        /// <param name="jugadorId">ID del jugador en formato ObjectId</param>
        /// <response code="200">Devuelve las partidas asociadas al jugador.</response>
        /// <response code="400">El formato del ID del jugador no es valido.</response>
        /// <response code="500">Error interno del servidor.</response>
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
        /// Registra una nueva partida finalizada y actualiza el jugador: suma monedas, suma 1 a partidas jugadas y actualiza el record si la puntuacion lo supera.
        /// </summary>
        /// <param name="partida">Datos de la partida jugada</param>
        /// <response code="201">Partida registrada exitosamente.</response>
        /// <response code="400">Solicitud invalida o valores fuera de rango.</response>
        /// <response code="404">No existe un jugador registrado con ese ID.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Partida), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Partida>> Create([FromBody] Partida partida)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partida.JugadorId) || !ObjectId.TryParse(partida.JugadorId, out _))
                {
                    return BadRequest("El ID del jugador es obligatorio y debe tener 24 caracteres.");
                }

                if (partida.PuntuacionObtenida < 0 || partida.MonedasRecolectadas < 0 || 
                    partida.TiempoSupervivencia < 0 || partida.TiempoSupervivencia > 9999.99m ||
                    partida.VelocidadFinalPantalla < 0 || partida.VelocidadFinalPantalla > 999.99m)
                {
                    return BadRequest("Los valores de la partida no pueden ser negativos ni superar los rangos permitidos.");
                }

                //El id y la fecha los decide el servidor, no el cliente.
                partida.Id = null;
                partida.FechaPartida = DateTime.UtcNow;

                _logger.LogInformation("Registrando nueva partida para el jugador: {JugadorId}", partida.JugadorId);
                var nuevaPartida = await _repo.Add(partida);
                if (nuevaPartida == null)
                {
                    return NotFound("No existe un jugador con ese ID.");
                }
                return CreatedAtAction(nameof(GetById), new { id = nuevaPartida.Id }, nuevaPartida);
            }
            catch (FormatException)
            {
                return BadRequest("El formato del ID del jugador no es válido.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando nueva partida.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Elimina una partida del historial por su ID
        /// </summary>
        /// <param name="id">ID de la partida a eliminar</param>
        /// <response code="204">Partida eliminada exitosamente.</response>
        /// <response code="400">El formato del ID no es valido.</response>
        /// <response code="500">Error interno del servidor.</response>
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