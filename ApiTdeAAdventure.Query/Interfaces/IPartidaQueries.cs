using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTdeAAdventure.Models;

namespace ApiTdeAAdventure.Query.Interfaces
{
    /// <summary>
    /// Consultas de lectura sobre la coleccion de partidas
    /// </summary>
    public interface IPartidaQueries
    {
        /// <summary>
        /// Obtiene todas las partidas
        /// </summary>
        Task<IEnumerable<Partida>> GetAll();

        /// <summary>
        /// Obtiene una partida por su id
        /// </summary>
        Task<Partida?> Get(string id);

        /// <summary>
        /// Obtiene las partidas realizadas por un jugador
        /// </summary>
        Task<IEnumerable<Partida>> GetByJugador(string jugadorId);
    }
}