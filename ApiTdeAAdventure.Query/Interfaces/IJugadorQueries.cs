using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiTdeAAdventure.Models;

namespace ApiTdeAAdventure.Query.Interfaces
{
    /// <summary>
    /// Consultas de lectura sobre la coleccion de jugadores
    /// </summary>
    public interface IJugadorQueries
    {
        /// <summary>
        /// Obtiene todos los jugadores
        /// </summary>
        Task<IEnumerable<Jugador>> GetAll();

        /// <summary>
        /// Obtiene un jugador por su id
        /// </summary>
        Task<Jugador?> Get(string id);

        /// <summary>
        /// Buscar un jugador por usuario y contraseña. Devuelve null si no coincide.
        /// </summary>
        Task<Jugador?> Login(string nombreUsuario, string password);

        /// <summary>
        /// Obtiene los 10 mejores jugadores ordenados por puntuacion maxima
        /// </summary>
        Task<IEnumerable<Leaderboard>> GetLeaderboard();
    }
}