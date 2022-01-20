using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/Libros")]
    public class LibrosController : ControllerBase
    {
      private readonly ApplicationDbContext _context;

        public LibrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            var libro= await _context.Libros.Include(x=>x.Autor).FirstOrDefaultAsync(x => x.id == id);
             return libro;
        }
        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            var existeautor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if(!existeautor)
                return BadRequest("No existe el autor de ese ejemplar");

            _context.Add(libro);
            await _context.SaveChangesAsync();
            return Ok(libro);
        }
    }
}
