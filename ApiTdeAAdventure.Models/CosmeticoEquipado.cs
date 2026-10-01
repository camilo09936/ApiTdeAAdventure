using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiTdeAAdventure.Models
{
    /// <summary>
    /// Objeto embebido con el cosmetico equipado
    /// </summary>
    public class CosmeticoEquipado
    {
        /// <summary>
        /// ID del cosmetico
        /// </summary>
        [BsonElement("cosmetico_id")]
        public string CosmeticoId { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de cosmetico
        /// </summary>
        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del cosmetico
        /// </summary>
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
