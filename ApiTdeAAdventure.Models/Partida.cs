using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiTdeAAdventure.Models
{
    /// <summary>
    /// Colleccion de partidas: historial de cada partida finalizada
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Partida
    {
        /// <summary>
        /// Identificador del registro de la partida
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Referencia al id del jugador 
        /// </summary>
        [BsonElement("jugador_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string JugadorId { get; set; } = string.Empty;

        /// <summary>
        /// Puntaje alcanzado en la sesion
        /// </summary>
        [BsonElement("puntuacion_obtenida")]
        public int PuntuacionObtenida { get; set; }

        /// <summary>
        /// Monedas recogidas durante la partida
        /// </summary>
        [BsonElement("monedas_recolectadas")]
        public int MonedasRecolectadas { get; set; }

        /// <summary>
        /// Segundos sobrevividos
        /// </summary>
        [BsonElement("tiempo_supervivencia")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TiempoSupervivencia { get; set; }

        /// <summary>
        /// Velocidad final de la pantalla
        /// </summary>
        [BsonElement("velocidad_final_pantalla")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal VelocidadFinalPantalla { get; set; }

        /// <summary>
        /// Fecha y hora UTC de la partida
        /// </summary>
        [BsonElement("fecha_partida")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime FechaPartida { get; set; }
    }
}
