using Hanssens.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ToDoWebApi.BLL.Filters;
using ToDoWebApi.DAL.Interfaces;
using ToDoWebApi.DAL.Services;

/*
Opgavebeskrivelse
Udvikling af en simpel todo/opgave-liste, hvor det er muligt at:
- Se en liste med opgaver
- Oprette en opgave
- Markere om en opgave som v�rende f�rdig
- Filtrere p� om en opgave er f�rdig eller ej
- Sortere r�kkef�lgen af opgaver (kun hvis det kan n�s)
Teknologierne du skal benytte:
- Frit valg til front-end (Vi bruger selv Vue)
- .NET Core som API
- Frit valg til storage, men det skal v�re persistant server-side
Det du prim�rt skal have fokus p� er:
- Teknisk opbygning og kode-strukturering
- Fors�ge at separere front-end og back-end (headless)
*/

namespace ToDoWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Host.ConfigureLogging(logging =>
            //{
            //    // Let us hijack the logging and send it to the console
            //    logging.ClearProviders();
            //    logging.AddConsole();
            //});
            builder.Services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });
            
            builder.Services.Configure<LocalStorageConfiguration>(
                builder.Configuration.GetSection("LocalStorageConfiguration")
            );

            // Add services to the container.
            builder.Services.AddControllers(
                options => {
                    options.Filters.Add(typeof(UnhandledExceptionFilter));
                }
            );

            // Define a CORS Policy, for production...
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsPolicy", builder =>
                {
                    builder.WithOrigins(
                        "https://localhost:7038",
                        "http://localhost:5038",
                        "https://www.huskeliste.dk"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            //builder.Services.AddSingleton<ILocalStorageConfiguration, LocalStorageConfiguration>();

            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseCors(builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });

                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
                app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}