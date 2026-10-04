using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;

namespace ApiTdeAAdventure.Query.Implements
{
    /// <summary>
    /// Implementacion de las consultas sobre los cosmeticos equipados
    /// </summary>
    public class CosmeticoEquipadoQueries : ICosmeticoEquipadoQueries
    {
        private readonly IMongoCollection<Jugador> _jugadores;

        /// <summary>
        /// Inicializa las consultas con la base de datos de Mongo
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public CosmeticoEquipadoQueries(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            _jugadores = db.GetCollection<Jugador>("jugadores");
        }

        ///<inheritdoc/>
        public async Task<CosmeticoEquipado?> Get(string jugadorId)
        {
            try
            {
                var jugador = await _jugadores
                    .Find(j => j.Id == jugadorId)
                    .FirstOrDefaultAsync();

                return jugador?.CosmeticoEquipado;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}