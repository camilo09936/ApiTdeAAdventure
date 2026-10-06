using System.Threading.Tasks;
using ApiTdeAAdventure.Models;

namespace ApiTdeAAdventure.Repository.Interfaces
{
    /// <summary>
    /// Operaciones sobre los cosmeticos equipados de los jugadores
    /// </summary>
    public interface ICosmeticoEquipadoRepository
    {
        /// <summary>
        /// Equipa un cosmetico a un jugador
        /// </summary>
        Task<bool> Equipar(string jugadorId, CosmeticoEquipado cosmetico);

        /// <summary>
        /// Desequipa el cosmetico actual de un jugador
        /// </summary>
        Task<bool> Desequipar(string jugadorId);
    }
}