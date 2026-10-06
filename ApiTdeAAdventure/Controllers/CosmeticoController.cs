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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Cosmetico), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cosmetico>> GetById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("El formato del ID no es válido.");
                }

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
        [HttpPost]
        [ProducesResponseType(typeof(Cosmetico), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cosmetico>> Create([FromBody] Cosmetico cosmetico)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cosmetico.Nombre) || string.IsNullOrWhiteSpace(cosmetico.Tipo))
                {
                    return BadRequest("El nombre y el tipo del cosmético son obligatorios.");
                }

                _logger.LogInformation("Creando cosmético: {Nombre}", cosmetico.Nombre);
                var nuevo = await _repo.Add(cosmetico);

                // Retorna 201 Created y la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
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
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Cosmetico), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cosmetico>> Update(string id, [FromBody] Cosmetico cosmetico)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _) || id != cosmetico.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del cuerpo o no es válido.");
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
    }
}