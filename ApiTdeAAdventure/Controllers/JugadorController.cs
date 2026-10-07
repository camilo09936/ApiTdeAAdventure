using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;
using ApiTdeAAdventure.Repository.Interfaces;

namespace ApiTdeAAdventure.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones relacionadas con los jugadores
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JugadorController : ControllerBase
    {
        private readonly ILogger<JugadorController> _logger;
        private readonly IJugadorQueries _query;
        private readonly IJugadorRepository _repo;

        /// <summary>
        /// Inicializa una nueva instacia de la clase <see cref="JugadorController"/>
        /// </summary>
        /// <param name="logger">Logger del controlador</param>
        /// <param name="query">Consultas para acceder a los jugadores</param>
        /// <param name="repo">Repositorio para modificar a los jugadores</param>
        public JugadorController(ILogger<JugadorController> logger, IJugadorQueries query, IJugadorRepository repo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        /// <summary>
        /// Obtiene todos los jugadores
        /// </summary>
        /// <response code="200">Devuelve una lista de jugadores.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Jugador>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Jugador>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Consultado todos los jugadores");
                var jugadores = await _query.GetAll();
                return Ok(jugadores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando todos los jugadores");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene la tabla de clasificacion global: los 10 mejores jugadores por puntuacion maxima
        /// </summary>
        /// <response code="200">Devuelve hasta 10 jugadores con posicion, nombre de usuario y puntuacion maxima.</response>
        /// <response code="500">Error interno de servidor.</response>
        [HttpGet("leaderboard")]
        [ProducesResponseType(typeof(IEnumerable<Leaderboard>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Leaderboard>>> GetLeaderboard()
        {
            try
            {
                _logger.LogInformation("Consultando la tabla de clasificacion global");
                var top = await _query.GetLeaderboard();
                return Ok(top);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando la tabla de clasificacion");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene un jugador por su ID
        /// </summary>
        /// <param name="id">Id del jugador</param>
        /// <response code="200">Devuelve el jugador correspondiente al ID proporcionado.</response>
        /// <response code="400">El ID no tiene un formato valido.</response>
        /// <response code="404">No se encontro un jugador con el ID proporcionado.</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Jugador), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Jugador>> GetById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest();
                }
                _logger.LogInformation("Consultando jugador por ID: {Id}", id);
                var jugador = await _query.Get(id);
                if (jugador == null)
                {
                    return NotFound();
                }
                return Ok(jugador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando jugador por ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Registra un nuevo jugador. Solo se usan NombreUsuario, Email y Password; el resto arranca en valores iniciales
        /// </summary>
        /// <param name="jugador">Datos del jugador a registrar</param>
        /// <response code="201">Jugador creado exitosamente.</response>
        /// <response code="400">Solicitud invalida.</response>
        /// <response code="409">El nombre de usuario o email ya existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Jugador), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Jugador>> Create([FromBody] Jugador jugador)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jugador.NombreUsuario) ||
                    string.IsNullOrWhiteSpace(jugador.Email) ||
                    string.IsNullOrWhiteSpace(jugador.Password))
                {
                    return BadRequest();
                }
                if (jugador.NombreUsuario.Length > 50 || jugador.Email.Length > 100 || jugador.Password.Length > 100 ||
                    !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(jugador.Email))
                {
                    return BadRequest("Usuario max. 50, email valido max. 100 y contraseña max. 100.");
                }
                _logger.LogInformation("Creando jugador {Usuario}", jugador.NombreUsuario);

                var nuevo = new Jugador
                {
                    NombreUsuario = jugador.NombreUsuario,
                    Email = jugador.Email,
                    Password = jugador.Password
                };
                var rs = await _repo.Add(nuevo);
                return StatusCode(StatusCodes.Status201Created, rs);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                //Lo dispara el indice unico de nombre_usuario o  de email
                return Conflict();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando jugador");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Inicia sesion. Solo se usan NombreUsuario y Password
        /// </summary>
        /// <param name="jugador">Credenciales del Jugador.</param>
        /// <response code="200">Devuelve el jugador.</response>
        /// <response code="401">Credenciales incorrectas.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(Jugador), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Jugador>> Login([FromBody] Jugador jugador)
        {
            try
            {
                _logger.LogInformation("Login de {Usuario}", jugador.NombreUsuario);
                var rs = await _query.Login(jugador.NombreUsuario, jugador.Password);
                if (rs == null)
                {
                    return Unauthorized();
                }
                return Ok(rs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login de {Usuario}", jugador.NombreUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Actualiza un jugador existente.
        /// </summary>
        /// <param name="id">Id del jugador.</param>
        /// <param name="jugador">Datos completos del jugador.</param>
        /// <response code="200">Jugador actualizado exitosamente.</response>
        /// <response code="400">Solicitud invalida.</response>
        /// <response code="404">No se encontro un jugador con el ID proporcionado.</response>
        /// <response code="409">El nombre de usuario o el email ya existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Jugador), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Jugador>> Update(string id, [FromBody] Jugador jugador)
        {
            try
            {
                _logger.LogInformation("Actualizando jugador con ID: {Id}", id);
                if(!ObjectId.TryParse(id, out _) || id != jugador.Id)
                {
                    return BadRequest();
                }
                var rs = await _repo.Update(jugador);
                if (rs == null)
                {
                    return NotFound();
                }
                return Ok(rs);
            }
            catch (MongoCommandException ex) when (ex.Code == 11000)
            {
                //FindOneUpdate reporta el indice unico como MongoCommandException (11000)
                return Conflict();
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category== ServerErrorCategory.DuplicateKey)
            {
                return Conflict();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando jugador con ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Elimina un jugador por su Id
        /// </summary>
        /// <param name="id">Id del jugador.</param>
        /// <response code="204">Jugador eliminado exitosamente.</response>
        /// <response code="400">El id no tiene formato valido.</response>
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
                    return BadRequest();
                }
                _logger.LogInformation("Eliminando un jugador con ID: {Id}", id);
                await _repo.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando jugador con ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}