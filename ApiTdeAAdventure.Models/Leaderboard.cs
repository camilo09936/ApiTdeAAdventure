using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTdeAAdventure.Models
{
    /// <summary>
    /// Fila de la tabla de clasificacion global.
    /// </summary>
    public class Leaderboard
    {
        /// <summary>
        /// Posicion en la tabla (1= primer lugar)
        /// </summary>
        public int Posicion {  get; set; }

        /// <summary>
        /// Nombre de usuario del jugador
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Mayor puntaje obtenido
        /// </summary>
        public int PuntuacionMaxima { get; set; }
    }
}