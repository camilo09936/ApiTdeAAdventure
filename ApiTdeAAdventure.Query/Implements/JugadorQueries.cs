using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using System.Threading.Tasks;
using ApiTdeAAdventure.Models;
using ApiTdeAAdventure.Query.Interfaces;

namespace ApiTdeAAdventure.Query.Implements
{
    /// <summary>
    /// Implementacion de las consultas sobre la coleccion de jugadores
    /// </summary>
    public class JugadorQueries : IJugadorQueries
    {
        private readonly IMongoCollection<Jugador> _jugadores;
        /// <summary>
        /// Inicializa las consultas con la base de datos de Mongo
        /// </summary>
        /// <param name="db">Base de datos MongoDB</param>

        public JugadorQueries(IMongoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _jugadores = db.GetCollection<Jugador>("jugadores");
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Jugador>> GetAll()
        {
            try
            {
                return await _jugadores.Find(_ => true).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<Jugador?> Get(string id)
        {
            try
            {
                return await _jugadores.Find(j => j.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<Jugador?> Login(string nombreUsuario, string password)
        {
            try
            {
                return await _jugadores
                    .Find(j => j.NombreUsuario == nombreUsuario && j.Password == password)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}