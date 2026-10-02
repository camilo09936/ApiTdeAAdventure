using System;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ApiTdeAAdventure.Models
{
    /// <summary>
    /// Coleccion de jugadores: credenciales, saldo y estadisticas.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Jugador
    {
        /// <summary>
        /// Identificador Unico (Object Id en Mongo, string en la API)
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Nombre de usuario unico
        /// </summary>
        [BsonElement("nombre_usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Correo único
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña
        /// </summary>
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Mayor puntaje obtenido
        /// </summary>
        [BsonElement("puntuacion_maxima")]
        public int PuntuacionMaxima { get; set; }

        /// <summary>
        /// Saldo de monedas
        /// </summary>
        [BsonElement("monedas")]
        public int Monedas { get; set; }

        /// <summary>
        /// Total de partidas finalizadas
        /// </summary>
        [BsonElement("partidas_jugadas")]
        public int PartidasJugadas { get; set; }

        /// <summary>
        /// IDs de cosmeticos ya comprados
        /// </summary>
        [BsonElement("cosmeticos_desbloqueados")]
        public List<string> CosmeticosDesbloqueados { get; set; } = new List<string>();

        /// <summary>
        /// Cosmetico actualmente equipado
        /// </summary>
        [BsonElement("cosmeticos_equipados")]
        public CosmeticoEquipado? CosmeticoEquipado { get; set; }
    }
}