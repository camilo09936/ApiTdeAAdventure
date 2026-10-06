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
    /// Implementacion de las escrituras sobre la coleccion de partidas
    /// </summary>
    public class PartidaRepository : IPartidaRepository
    {
        private readonly IMongoCollection<Partida> _partidas;

        /// <summary>
        /// Inicializa el repositorio con la base de datos de mongo.
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public PartidaRepository(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _partidas = db.GetCollection<Partida>("partidas");
        }

        ///<inheritdoc/>
        public async Task<Partida> Add(Partida partida)
        {
            try
            {
                // Mongo agrega el _id y lo deja en partida.Id
                await _partidas.InsertOneAsync(partida);
                return partida;
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
                await _partidas.DeleteOneAsync(p => p.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}