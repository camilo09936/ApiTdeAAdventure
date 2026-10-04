using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;

namespace ApiTdeAAdventure.Query.Implements
{
    /// <summary>
    /// Implementacion de las consultas sobre la coleccion de partidas
    /// </summary>
    public class PartidaQueries : IPartidaQueries
    {
        private readonly IMongoCollection<Partida> _partidas;

        /// <summary>
        /// Inicializa las consultas con la base de datos de Mongo
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public PartidaQueries(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            _partidas = db.GetCollection<Partida>("partidas");
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Partida>> GetAll()
        {
            try
            {
                return await _partidas
                    .Find(_ => true)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<Partida?> Get(string id)
        {
            try
            {
                return await _partidas
                    .Find(p => p.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Partida>> GetByJugador(string jugadorId)
        {
            try
            {
                return await _partidas
                    .Find(p => p.JugadorId == jugadorId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}