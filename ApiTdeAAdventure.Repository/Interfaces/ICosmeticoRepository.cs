using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiTdeAAdventure.Models;

namespace ApiTdeAAdventure.Repository.Interfaces
{
	/// <summary>
	/// Operaciones de escritura sobre la coleccion de cosmeticos
	/// </summary>
	public interface ICosmeticoRepository
	{
		/// <summary>
		/// Inserta un nuevo cosmético y lo devuelve con su id generado
		/// </summary>
		Task<Cosmetico> Add(Cosmetico cosmetico);

		/// <summary>
		/// Actualiza un cosmético existente. Devuelve null si no existe
		/// </summary>
		Task<Cosmetico?> Update(Cosmetico cosmetico);

		/// <summary>
		/// Elimina un cosmético por su id
		/// </summary>
		Task Delete(string id);
	}
}