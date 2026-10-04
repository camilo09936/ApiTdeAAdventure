using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiTdeAAdventure.Query.Implements
{
    /// <summary>
    /// Implementacion de las consultas sobre la coleccion de cosmeticos
    /// </summary>
    public class CosmeticoQueries : ICosmeticoQueries
    {
        private readonly IMongoCollection<Cosmetico> _cosmeticos;

        /// <summary>
        /// Inicializa las consultas con la base de datos de Mongo
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public CosmeticoQueries(IMongoDatabase db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            _cosmeticos = db.GetCollection<Cosmetico>("cosmeticos");
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Cosmetico>> GetAll()
        {
            try
            {
                return await _cosmeticos.Find(_ => true).ToListAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<Cosmetico?> Get(string id)
        {
            try
            {
                return await _cosmeticos.Find(c => c.Id == id).FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Cosmetico>> GetByTipo(string tipo)
        {
            try
            {
                return await _cosmeticos
                    .Find(c => c.Tipo == tipo)
                    .ToListAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
