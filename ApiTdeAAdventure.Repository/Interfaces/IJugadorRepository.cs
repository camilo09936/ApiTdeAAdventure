using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiTdeAAdventure.Models;

namespace ApiTdeAAdventure.Repository.Interfaces
{
    /// <summary>
    /// Operaciones de escritura sobre la coleccion de jugadores
    /// </summary>
    public interface IJugadorRepository
    {
        /// <summary>
        /// Inserta un nuevo jugador y lo devuelve con su id generado
        /// </summary>
        Task<Jugador> Add(Jugador jugador);

        /// <summary>
        /// Reemplaza un jugador existente. Devuelve null si no existe
        /// </summary>
        Task<Jugador?> Update(Jugador jugador);

        /// <summary>
        /// Elimina un jugador por si id
        /// </summary>
        Task Delete(string id);
    }
}