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
        private readonly IMongoCollection<Jugador> _jugadores;

        /// <summary>
        /// Inicializa el repositorio con la base de datos de mongo.
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public PartidaRepository(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _partidas = db.GetCollection<Partida>("partidas");
            _jugadores = db.GetCollection<Jugador>("jugadores");
        }

        ///<inheritdoc/>
        public async Task<Partida?> Add(Partida partida)
        {
            try
            {
                var filtro = Builders<Jugador>.Filter.Eq(j => j.Id, partida.JugadorId);

                //Una sola operacion atomica sobre jugador:
                //Monedas + recolectadas, PartidasJugadas + 1 y puntuacionMaxima= el mayor
                var cambios = Builders<Jugador>.Update
                    .Inc(j => j.Monedas, partida.MonedasRecolectadas)
                    .Inc(j => j.PartidasJugadas, 1)
                    .Max(j => j.PuntuacionMaxima, partida.PuntuacionObtenida);

                var rs = await _jugadores.UpdateOneAsync(filtro, cambios);
                if (rs.MatchedCount == 0)
                {
                    return null; //Si el jugador no existe: no se guarda partida
                }
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