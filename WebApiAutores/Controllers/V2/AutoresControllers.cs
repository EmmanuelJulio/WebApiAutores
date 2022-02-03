using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.interfaces;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V2
{
    [ApiController]
    [Route("api/autores")]
    [CabeceraEstaPrecenteAttribute("x-version", "2")]
    //[Route("api/V2/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    
    public class AutoresControllers : ControllerBase

    {
        private readonly ApplicationDbContext _context;
        private readonly IGenericRepository<Autor> repository;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresControllers(ApplicationDbContext context,IGenericRepository<Autor> repository,
            IMapper mapper,IAuthorizationService authorizationService)
        {
            _context = context;
            this.repository = repository;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }
       
        [HttpGet(Name ="obtenerAutoresV2")] //api/autores
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
           var autores= await _context.Autores.ToListAsync();
            autores.ForEach(x =>x.nombre = x.nombre.ToUpper());
            var esAdmin = await authorizationService.AuthorizeAsync(User, "EsAdmin");
            return mapper.Map<List<AutorDTO>>(autores);

        }

       
        [HttpGet("{id}", Name = "optenerautorV2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id,[FromHeader] string incluirHATEOAS )
        {
            var autor = await _context.Autores
                .Include(autorDB=>autorDB.AutoresLibros)
                .ThenInclude(AutorDTOConLibros=>AutorDTOConLibros.libro)
                .FirstOrDefaultAsync(x=>x.Id==id);
           
            if (autor == null)
            {
                return NotFound();
            };
            var dto = mapper.Map<AutorDTOConLibros>(autor);
           
            return dto;
            
        }
        
     
        [HttpGet("Buscar/{nombre}",Name = "obtenerAutoresPorNombreV2")]
        public async Task<ActionResult<List<AutorDTO>>> GetByName(string nombre)
        {
            var autor = await _context.Autores.Where(autorBD=>autorBD.nombre.Contains(nombre)).ToListAsync();
            if (autor == null)
            {
                return NotFound();
            };
            
            return mapper.Map<List<AutorDTO>>(autor);
        }
        [HttpPost(Name = "crearAutorV2")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autor)
        {
            var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.nombre == autor.Nombre);
            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }

            var autorTransfer = mapper.Map<Autor>(autor);
             await repository.Add(autorTransfer);
            var autorDTo= mapper.Map<AutorDTO>(autorTransfer);
            return CreatedAtRoute("optenerautor", new { id = autorTransfer.Id }, autorDTo);

        
        }
        [HttpPut("{id:int}",Name = "actualizarAutorV2")]//ap/autores/1
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDto, int id)
        {
            var ExisteAutor = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!ExisteAutor)
            {
                return BadRequest("El autor no coincide con el id de la url");
            }
            var autor = mapper.Map<Autor>(autorCreacionDto);
            autor.Id = id;

            _context.Update(autor);
           await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}",Name = "borrarAutorV2")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound();
            _context.Remove(new Autor { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
