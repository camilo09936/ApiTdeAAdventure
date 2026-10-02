using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiTdeAAdventure.Models
{
    /// <summary>
    /// Catalogo de la tienda. El id es el cosmetico_id
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Cosmetico
    {
        /// <summary>
        /// identificador unico de cosmetico
        /// </summary>
        [BsonId]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del articulo cosmetico
        /// </summary>
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Categoria del cosmetico
        /// </summary>
        [BsonElement("tipo")]
        public string Tipo {  get; set; } = string.Empty;

        /// <summary>
        /// Costo en monedas del cosmetico
        /// </summary>
        [BsonElement("precio")]
        public int Precio { get; set; }

        /// <summary>
        /// Ruta del recurso grafico en Unity
        /// </summary>
        [BsonElement("sprite_path")]
        public string SpritePath {  get; set; } = string.Empty;
    }
}