using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;
using ApiTdeAAdventure.Repository.Interfaces;

namespace ApiTdeAAdventure.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones relacionadas con los cosméticos del juego
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CosmeticoController : ControllerBase
    {
        private readonly ILogger<CosmeticoController> _logger;
        private readonly ICosmeticoQueries _query;
        private readonly ICosmeticoRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CosmeticoController"/>
        /// </summary>
        /// <param name="logger">Logger del controlador</param>
        /// <param name="query">Consultas para acceder a los cosméticos</param>
        /// <param name="repo">Repositorio para modificar los cosméticos</param>
        public CosmeticoController(ILogger<CosmeticoController> logger, ICosmeticoQueries query, ICosmeticoRepository repo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene todos los cosméticos del catálogo
        /// </summary>
        /// <response code="200">Devuelve la lista de cosméticos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Cosmetico>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Cosmetico>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Consultando todos los cosméticos");
                var cosmeticos = await _query.GetAll();
                return Ok(cosmeticos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando todos los cosméticos");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene un cosmético por su ID
        /// </summary>
        /// <param name="id">ID del cosmetico</param>
        /// <response code="200">Devuelve el cosmetico correspondiente al ID.</response>
        /// <response code="404">No se encontro el cosmetico con el ID proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Cosmetico), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cosmetico>> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Consultando cosmético por ID: {Id}", id);
                var cosmetico = await _query.Get(id);

                if (cosmetico == null)
                {
                    return NotFound();
                }

                return Ok(cosmetico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando cosmético por ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene los cosméticos filtrados por tipo (ej: 'skin', 'arma', 'accesorio')
        /// </summary>
        /// <param name="tipo">Tipo de cosmetico a filtrar</param>
        /// <response code="200">Devielve la lista de cosmeticos filtrados por tipo.</response>
        /// <response code="400">El tipo de cosmetico esta vacio o no es valido.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(typeof(IEnumerable<Cosmetico>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Cosmetico>>> GetByTipo(string tipo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipo))
                {
                    return BadRequest("El tipo de cosmético no puede estar vacío.");
                }

                _logger.LogInformation("Consultando cosméticos por tipo: {Tipo}", tipo);
                var cosmeticos = await _query.GetByTipo(tipo);
                return Ok(cosmeticos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando cosméticos por tipo: {Tipo}", tipo);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Registra un nuevo cosmético en el catálogo
        /// </summary>
        /// <param name="cosmetico">Datos del cosmetico a registrar</param>
        /// <response code="201">Cosmetico creado exitosamente.</response>
        /// <response code="400">Solicitud invalida o valores fuera de rango.</response>
        /// <response code="409">Ya existe un cosmetico con ese ID.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Cosmetico), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Cosmetico>> Create([FromBody] Cosmetico cosmetico)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cosmetico.Id) || cosmetico.Id.Length > 50 ||
                    string.IsNullOrWhiteSpace(cosmetico.Nombre) || cosmetico.Nombre.Length > 50 ||
                    string.IsNullOrWhiteSpace(cosmetico.Tipo) || cosmetico.Tipo.Length > 20 ||
                    string.IsNullOrWhiteSpace(cosmetico.SpritePath) || cosmetico.Precio < 0)
                {
                    return BadRequest("Id (Max 50), Nombre (Max 50), Tipo (Max 20) y SpritePatch son obligatorios, y el precio no puede ser negativo.");
                }

                _logger.LogInformation("Creando cosmético: {Nombre}", cosmetico.Nombre);
                var nuevo = await _repo.Add(cosmetico);

                // Retorna 201 Created y la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                //El _id (cosmetico_id) ya existe en la coleccion
                return Conflict("Ya existe un cosmetico con este Id.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando cosmético");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Actualiza un cosmético existente en el catálogo
        /// </summary>
        /// <param name="id">ID del cosmetico a actualizar</param>
        /// <param name="cosmetico">Datos actualizados del cosmetico</param>
        /// <response code="200">Cosmetico actualizado exitosamente.</response>
        /// <response code="400">El ID de la URL no coincide con el del cuerpo.</response>
        /// <response code="404">No se encontro el cosmetico para actualizar.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Cosmetico), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cosmetico>> Update(string id, [FromBody] Cosmetico cosmetico)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || id != cosmetico.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");
                }
                if (cosmetico.Precio < 0)
                {
                    return BadRequest("El precio no puede ser negativo.");
                }

                _logger.LogInformation("Actualizando cosmético con ID: {Id}", id);
                var actualizado = await _repo.Update(cosmetico);

                if (actualizado == null)
                {
                    return NotFound("No se encontró el cosmético para actualizar.");
                }

                return Ok(actualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando cosmético con ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Elimina un cosmético del catálogo por su ID
        /// </summary>
        /// <param name="id">ID del cosmetico a eliminar</param>
        /// <response code="204">Cosmetico eliminado exitosamente.</response>
        /// <response code="400">El formato del ID no es valido o esta vacio.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("El ID del cosmetico no puede estar vacio.");
                }
                _logger.LogInformation("Eliminando cosmético con ID: {Id}", id);
                await _repo.Delete(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando cosmético con ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Procesa la compra de un cosmetico para un jugador descontando sus monedas.
        /// </summary>
        /// <param name="jugadorId">ID del jugador en formato ObjectId.</param>
        /// <param name="cosmeticoId">ID del cosmetico a comprar.</param>
        /// <response code="200">Compra realizada con exito.</response>
        /// <response code="400">Datos invalidos, monedas insuficientes o cosmetico ya poseído.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("comprar/{jugadorId}/{cosmeticoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Comprar(string jugadorId, string cosmeticoId)
        {
            try
            {
                if (!ObjectId.TryParse(jugadorId, out _) || string.IsNullOrWhiteSpace(cosmeticoId))
                {
                    return BadRequest("El ID del jugador y el ID del cosmetico son obligatorio.");
                }
                _logger.LogInformation("Procesando compra del cosmetico {CosmeticoId} para el jugador {JugadorId}", cosmeticoId, jugadorId);
                var jugador = await _repo.Comprar(jugadorId, cosmeticoId);
                if (jugador == null)
                {
                    return BadRequest("No se pudo completar la compra. Verifica que el jugador y el cosmetico existan, que tenga monedas suficientes y que no posea ya el cosmetico.");
                }
                return Ok(new { monedas = jugador.Monedas, cosmeticosDesbloqueados = jugador.CosmeticosDesbloqueados });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la compra del cosmetico {CosmeticoId} para el jugador {JugadorId}", cosmeticoId, jugadorId);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}