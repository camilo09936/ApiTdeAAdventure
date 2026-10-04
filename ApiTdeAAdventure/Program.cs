using MongoDB.Driver;
using ApiTdeAAdventure.Query.Implements;
using ApiTdeAAdventure.Query.Interfaces;
using ApiTdeAAdventure.Repository.Interfaces;
using ApiTdeAAdventure.Repository.Implements;

namespace ApiTdeAAdventure
{
    /// <summary>
    /// Punto de entrada a la API TdeAAdventure
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Configura y ejecuta el host de la API.
        /// </summary>
        /// <param name="args">Argumentos de la linea de comandos.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(opt =>
            {
                string path = Path.Combine(AppContext.BaseDirectory, "api.xml");
                opt.IncludeXmlComments(path);
            });

            builder.Services.AddSingleton<IMongoClient>(_ =>
                new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));
            builder.Services.AddSingleton<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>()
                  .GetDatabase(builder.Configuration["MongoDB:DatabaseName"]));

            builder.Services.AddTransient<IJugadorQueries, JugadorQueries>();
            builder.Services.AddTransient<IJugadorRepository, JugadorRepository>();
            builder.Services.AddTransient<ICosmeticoQueries, CosmeticoQueries>();
            builder.Services.AddTransient<ICosmeticoRepository, CosmeticoRepository>();
            builder.Services.AddTransient<IPartidaQueries, PartidaQueries>();
            builder.Services.AddTransient<IPartidaRepository, PartidaRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}