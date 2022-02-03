using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.interfaces;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;


//[assembly:ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiAutores
{
    public class Startup
    {
       public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            //aca se quita el auto formato de los claims para que se pueda acceder mas facil a ellos
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(FiltroDeExepcion));
                options.Conventions.Add(new SuaggerAgrupaPorVercion());

            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


            services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("defautlConnection")));

            //inyecion de dependencias de sercvicios
            services.AddTransient<IGenericRepository<Autor>, GenericRepository<Autor>>();
            services.AddTransient<IGenericRepository<Libro>, GenericRepository<Libro>>();
            services.AddTransient<IGenericRepository<Comentario>, GenericRepository<Comentario>>();
            
            //configuracion de automapper
            services.AddAutoMapper(typeof(Startup));
            //configuracion de identity user
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //configuracion de suager con autentificacion de jwt
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo { 
                    Title = "WebAPIAutores",
                    Version = "V1" ,
                    Description ="Este es un web api para trabajar con autores y libros, utilizando diferentes mecanismos de comunicacion y vercionado",
                    Contact= new OpenApiContact
                    {
                        Email ="emajulio.ej@gmail.com",
                        Name ="Emmanuel Julio",
                        Url = new Uri("https://www.linkedin.com/in/emmanuel-julio-084a6283/")
                    }
                });
                c.SwaggerDoc("V2", new OpenApiInfo { Title = "WebAPIAutores", Version = "V2" });
                c.OperationFilter<AgregarParametroHETEOAS>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.OperationFilter<AgregarParametroXvercion>();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

            });
            // configuracion de jwt 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones=>opciones.TokenValidationParameters= new TokenValidationParameters
                {
                    ValidateIssuer=false,
                    ValidateAudience=false,
                    ValidateLifetime=true,
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey= new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["LlaveJwt"])),
                    ClockSkew= TimeSpan.Zero
                });


            //Configuracion de politica de acceso
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
                //options.AddPolicy("admin", politica => politica.RequireClaim("esVendedor"));
            });




            // Configuracion de los cors para permitir el acceso solamente desde dominios conocidos
            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader()
                .WithExposedHeaders(new string[] {"cantidadTotalDeRegistros"});
            }));
            services.AddDataProtection();

            services.AddTransient<HashService>();

            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor,ActionContextAccessor>();
        
        }
        public void configure(IApplicationBuilder app,IWebHostEnvironment env,ILogger<Startup> logger)
        {

          
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
                app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "WebApiAutores v1");
                c.SwaggerEndpoint("/swagger/V2/swagger.json", "WebApiAutores v2");
                });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
