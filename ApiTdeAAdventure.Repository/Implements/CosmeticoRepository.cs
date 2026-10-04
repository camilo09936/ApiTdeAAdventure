using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using System.Threading.Tasks;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Repository.Interfaces;

namespace ApiTdeAAdventure.Repository.Implements
{
    /// <summary>
    /// Implementacion de las escrituras sobre la coleccion de cosmeticos
    /// </summary>
    public class CosmeticoRepository : ICosmeticoRepository
    {
        private readonly IMongoCollection<Cosmetico> _cosmeticos;

        /// <summary>
        /// Inicializa el repositorio con la base de datos de mongo.
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public CosmeticoRepository(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _cosmeticos = db.GetCollection<Cosmetico>("cosmeticos");
        }

        ///<inheritdoc/>
        public async Task<Cosmetico> Add(Cosmetico cosmetico)
        {
            try
            {
                // Mongo agrega el _id y lo deja en cosmetico.Id
                await _cosmeticos.InsertOneAsync(cosmetico);
                return cosmetico;
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<Cosmetico?> Update(Cosmetico cosmetico)
        {
            try
            {
                var filtro = Builders<Cosmetico>.Filter.Eq(c => c.Id, cosmetico.Id);

                var cambios = Builders<Cosmetico>.Update
                    .Set(c => c.Nombre, cosmetico.Nombre)
                    .Set(c => c.Tipo, cosmetico.Tipo)
                    .Set(c => c.Precio, cosmetico.Precio)
                    .Set(c => c.SpritePath, cosmetico.SpritePath);

                var opciones = new FindOneAndUpdateOptions<Cosmetico>
                {
                    ReturnDocument = ReturnDocument.After
                };

                // Devuelve el documento ya actualizado, o null si el id no existe
                return await _cosmeticos.FindOneAndUpdateAsync(filtro, cambios, opciones);
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task Delete(string id)
        {
            try
            {
                await _cosmeticos.DeleteOneAsync(c => c.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}