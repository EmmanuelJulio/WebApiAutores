using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresControllers : ControllerBase

    {
        private readonly ApplicationDbContext _context;

        public AutoresControllers(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("/listado")]  // /listado
        [HttpGet("listado")]// /api/autores/listado
        [HttpGet] //api/autores
        public async Task<ActionResult<List<Autor>>> Get()
        {
            return await _context.Autores.ToListAsync();
        }

        [HttpGet("PrimerAutor")] //api/autore/PrimerAutor
        public async Task<ActionResult<Autor>> ActionResult()
        {
            return await _context.Autores.FirstOrDefaultAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Autor>> Get(int id)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            };
            return Ok(autor);
        }
        //[HttpGet("{id:int/{param2=persona}}")]
        //public async Task<ActionResult<Autor>> Get(int id, string param2)
        //{
        //    var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);
        //    if (autor == null)
        //    {
        //        return NotFound();
        //    };
        //    return Ok(autor);
        //}
        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get([FromQuery] string nombre)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            };
            return Ok(autor);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.nombre == autor.nombre);
            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre{autor.nombre}");
            }
            _context.Add(autor);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{id:int}")]//ap/autores/1
        public async Task<ActionResult<List<Autor>>> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El autor no coincide con el id de la url");
            }
            _context.Update(autor);
            _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id:int}")]
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
