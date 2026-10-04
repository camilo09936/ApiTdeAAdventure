using ApiTdeAAdventure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiTdeAAdventure.Query.Interfaces
{
    /// <summary>
    /// Consultas de lectura sobre la coleccion de cosmeticos
    /// </summary>
    public interface ICosmeticoQueries
    {
        /// <summary>
        /// Obtiene todos los cosmeticos del catalogo
        /// </summary>
        Task<IEnumerable<Cosmetico>> GetAll();

        /// <summary>
        /// Obtiene un cosmetico por su id
        /// </summary>
        Task<Cosmetico?> Get(string id);

        /// <summary>
        /// Obtiene los cosmeticos filtrados por tipo (skin, arma, accesorio, etc.)
        /// </summary>
        Task<IEnumerable<Cosmetico>> GetByTipo(string tipo);
    }
}
