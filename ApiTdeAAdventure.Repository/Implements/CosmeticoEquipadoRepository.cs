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
        private readonly IMongoCollection<Cosmetico> _cosmeticos;

        /// <summary>
        /// Inicializa el repositorio con la base de datos Mongo
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public CosmeticoEquipadoRepository(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            _jugadores = db.GetCollection<Jugador>("jugadores");
            _cosmeticos = db.GetCollection<Cosmetico>("cosmeticos");
        }

        ///<inheritdoc/>
        public async Task<bool> Equipar(string jugadorId, CosmeticoEquipado cosmetico)
        {
            try
            {
                if (cosmetico == null || string.IsNullOrWhiteSpace(cosmetico.CosmeticoId))
                    return false;

                var infoCosmetico = await _cosmeticos
                    .Find(c => c.Id == cosmetico.CosmeticoId)
                    .FirstOrDefaultAsync();

                if (infoCosmetico == null)
                    return false;

                var jugador = await _jugadores
                    .Find(j => j.Id == jugadorId)
                    .FirstOrDefaultAsync();

                if (jugador == null)
                    return false;

                if (!jugador.CosmeticosDesbloqueados.Contains(cosmetico.CosmeticoId))
                    return false;

                cosmetico.Nombre = infoCosmetico.Nombre;
                cosmetico.Tipo = infoCosmetico.Tipo;

                var filtro = Builders<Jugador>.Filter.Eq(j => j.Id, jugadorId);
                var actualizacion = Builders<Jugador>.Update.Set(j => j.CosmeticoEquipado, cosmetico);

                var resultado = await _jugadores.UpdateOneAsync(filtro, actualizacion);

                return resultado.MatchedCount > 0;
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
                var actualizacion = Builders<Jugador>.Update.Set(j => j.CosmeticoEquipado, null);

                var resultado = await _jugadores.UpdateOneAsync(filtro, actualizacion);

                return resultado.MatchedCount > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}