using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Repository.Interfaces;

namespace ApiTdeAAdventure.Repository.Implements
{
    /// <summary>
    /// Implementacion de las operaciones sobre los cosmeticos equipados
    /// </summary>
    public class CosmeticoEquipadoRepository : ICosmeticoEquipadoRepository
    {
        private readonly IMongoCollection<Jugador> _jugadores;

        /// <summary>
        /// Inicializa el repositorio con la base de datos Mongo
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public CosmeticoEquipadoRepository(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            _jugadores = db.GetCollection<Jugador>("jugadores");
        }

        ///<inheritdoc/>
        public async Task<bool> Equipar(string jugadorId, CosmeticoEquipado cosmetico)
        {
            try
            {
                var jugador = await _jugadores
                    .Find(j => j.Id == jugadorId)
                    .FirstOrDefaultAsync();

                if (jugador == null)
                    return false;

                if (cosmetico == null)
                    return false;

                if (!jugador.CosmeticosDesbloqueados.Contains(cosmetico.CosmeticoId))
                    return false;

                var filtro = Builders<Jugador>.Filter.Eq(j => j.Id, jugadorId);

                var actualizacion = Builders<Jugador>.Update
                    .Set(j => j.CosmeticoEquipado, cosmetico);

                var resultado = await _jugadores.UpdateOneAsync(
                    filtro,
                    actualizacion
                );

                return resultado.ModifiedCount > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<bool> Desequipar(string jugadorId)
        {
            try
            {
                var filtro = Builders<Jugador>.Filter.Eq(j => j.Id, jugadorId);

                var actualizacion = Builders<Jugador>.Update
                    .Set(j => j.CosmeticoEquipado, null);

                var resultado = await _jugadores.UpdateOneAsync(
                    filtro,
                    actualizacion
                );

                return resultado.ModifiedCount > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}