using Hanssens.Net;
using Microsoft.Extensions.DependencyInjection;
using ToDoWebApi.Services;

namespace ToDoWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            //builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<ILocalStorageConfiguration>(
                // setup a configuration with encryption enabled (defaults to 'false')
                // note that adding EncryptionSalt is optional, but recommended
                new LocalStorageConfiguration()
                {
                    EnableEncryption = true,
                    EncryptionSalt = "todosalt",
                    Filename = "todo"
                    //ToDo: consider making a separate Filename per user id eg. SSO like IdentityServer4 (OAuth/OpenID protocols, ASP.NET Core).
                });
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
/*
Opgavebeskrivelse
Udvikling af en simpel todo/opgave-liste, hvor det er muligt at:
- (R, GET) Se en liste med opgaver
- (C, POST) Oprette en opgave
- (U, PUT) Markere om en opgave som v�rende f�rdig
- (R, GET og client-side .filter) Filtrere p� om en opgave er f�rdig eller ej
- (U, PUT) Sortere r�kkef�lgen af opgaver (kun hvis det kan n�s)
Teknologierne du skal benytte:
- Frit valg til front-end (Vi bruger selv Vue)
- .NET Core som API
- (sqllite?) Frit valg til storage, men det skal v�re persistant server-side
Det du prim�rt skal have fokus p� er:
- Teknisk opbygning og kode-strukturering
- Fors�ge at separere front-end og back-end (headless)


Note:
Brug todo record? I stedet for en todo klasse (old school)
HUSK AT BRUGE GOD TID P� WebAPI'et, da det er en WEB BACKEND UDVIKLER-stilling.
...brug deserialisering (f.eks. TodoDTO ifm. metode parameter) af (json) requests for at forhindre injection...
...test med "-tegnet i tekster... dvs. s� burde serialisering (brig en string property i en TodoDTO-klasse) til json g� fint....!?
...in-memory database eller sqllite... initialis�r det med nogle data, s� siden ikke er tom fra start...

ToDo: Implementer ILogger... !?


CRUD:

[Route("/todo/")]
[POST]
public JsonResult CreateItem([FromBody]item) //or something like this
// Use the id from the response to add it to the front-end list.

[Route("/todo/")]
[GET]
// get all the items
public JsonResult GetAll()

[Route("/todo/{id}")]
[PUT]
// replace the item
public JsonResult UpdateItem([FromUri]int id, [FromBody]TodoDTO item) //or something like this

[Route("/todo/{id}")]
[DELETE]
// replace the item
public JsonResult UpdateItem([FromUri]int id) //or something like this
 
*/