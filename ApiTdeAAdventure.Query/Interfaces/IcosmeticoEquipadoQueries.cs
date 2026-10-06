using System.Threading.Tasks;
using ApiTdeAAdventure.Models;

namespace ApiTdeAAdventure.Query.Interfaces
{
    /// <summary>
    /// Consultas de lectura sobre los cosmeticos equipados de los jugadores
    /// </summary>
    public interface ICosmeticoEquipadoQueries
    {
        /// <summary>
        /// Obtiene el cosmetico equipado de un jugador por su id
        /// </summary>
        Task<CosmeticoEquipado?> Get(string jugadorId);
    }
}