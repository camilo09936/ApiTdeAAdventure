using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiTdeAAdventure.Models;

namespace ApiTdeAAdventure.Repository.Interfaces
{
    /// <summary>
    /// Operaciones de escritura sobre la coleccion de partidas
    /// </summary>
    public interface IPartidaRepository
    {
        /// <summary>
        /// Inserta una nueva partida y la devuelve con su id generado
        /// </summary>
        Task<Partida?> Add(Partida partida);

        /// <summary>
        /// Elimina una partida por su id
        /// </summary>
        Task Delete(string id);
    }
}