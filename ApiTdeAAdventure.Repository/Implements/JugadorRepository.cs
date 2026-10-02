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
    /// Implementacion de las escrituras sobre la coleccion de jugadores
    /// </summary>
    public class JugadorRepository : IJugadorRepository
    {
        private readonly IMongoCollection<Jugador> _jugadores;
        /// <summary>
        /// Inicializa el repositorio con la base de datos de mongo.
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>
        public JugadorRepository(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _jugadores = db.GetCollection<Jugador>("jugadores");
        }

        ///<inheritdoc/>
        public async Task<Jugador> Add(Jugador jugador)
        {
            try
            {
                //Mongo agrega el _id y lo deja en jugador.Id
                await _jugadores.InsertOneAsync(jugador);
                return jugador;
            }
            catch (Exception)
            {
                throw;
            }
        }
        ///<inheritdoc/>
        public async Task<Jugador?> Update(Jugador jugador)
        {
            try
            {
                var filtro = Builders<Jugador>.Filter.Eq(j => j.Id, jugador.Id);

                //Solo se actualizan los datos de cuenta. Modedas, puntaje y cosmeticos
                //se modifican unicamente por sus propios endpoints (partida, tienda, equipar).
                var cambios = Builders<Jugador>.Update
                    .Set(j => j.NombreUsuario, jugador.NombreUsuario)
                    .Set(j => j.Email, jugador.Email)
                    .Set(j => j.Password, jugador.Password);

                var opciones = new FindOneAndUpdateOptions<Jugador>
                {
                    ReturnDocument = ReturnDocument.After
                };

                // Devuelve el documento ya actualizado, o null si el id no existe
                return await _jugadores.FindOneAndUpdateAsync(filtro, cambios, opciones);
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
                await _jugadores.DeleteOneAsync(j => j.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}