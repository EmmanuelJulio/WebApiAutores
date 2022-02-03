using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;
using WebApiAutores.interfaces;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/V1/Libros")]
    public class LibrosController : ControllerBase
    {
      private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly IGenericRepository<Libro> repository;

        public LibrosController(ApplicationDbContext context,IMapper mapper,IGenericRepository<Libro> repository)
        {
            _context = context;
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "opteneLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(libroDb => libroDb.AutoresLibro)
                .ThenInclude(autorLibroDB=>autorLibroDB.autor)
                .FirstOrDefaultAsync(x => x.id == id);

            if(libro == null)
                return NotFound();

            libro.AutoresLibro= libro.AutoresLibro.OrderBy(x=>x.Orden).ToList();
            return mapper.Map<LibroDTOConAutores>(libro);
        }
        [HttpPost(Name ="crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libro)
        {
            if (libro.Autores == null)
                return BadRequest("No se puede crear un libro sin autores");

            var autores = await _context.Autores.Where(AutorBD => libro.Autores.Contains(AutorBD.Id))
                .Select(x => x.Id).ToListAsync();

            if (libro.Autores.Count != autores.Count)
                return BadRequest("No existe uno de los autores");

            var Olibro = mapper.Map<Libro>(libro);
            AsignarOrdenAutores(Olibro);


           var libroDTO=mapper.Map<LibroDTO>(Olibro);
             await repository.Add(Olibro);
            return CreatedAtRoute("opteneLibro", new { id = Olibro.id }, libroDTO);
        }
        [HttpPut("{id:int}",Name ="actualizarLibro")]
        public async Task<ActionResult> Put(int id,LibroCreacionDTO libroCreacionDTO)
        {
            var libroDb = await _context.Libros
                .Include(x => x.AutoresLibro)
                .FirstOrDefaultAsync(x => x.id == id);
            if (libroDb == null)
                return NotFound();

            libroDb = mapper.Map(libroCreacionDTO, libroDb);
            AsignarOrdenAutores(libroDb);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibro != null)
            {
                for (int i = 0; i < libro.AutoresLibro.Count; i++)
                {
                    libro.AutoresLibro[i].Orden = i;
                }
            }
        }
        [HttpPatch("{id:int}",Name ="patchLibro")]
        public async Task<ActionResult>Patch(int id,JsonPatchDocument<LibroPatchDTO> PatchDocument)
        {
            if(PatchDocument==null)
                return BadRequest();

            var libroDB= await _context.Libros.FirstOrDefaultAsync(x => x.id == id);
            if(libroDB==null)
                return NotFound();

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);
            PatchDocument.ApplyTo(libroDTO,ModelState);
            var esValido = TryValidateModel(libroDTO);

            if(!esValido)
                return BadRequest(ModelState);

            mapper.Map(libroDTO, libroDB);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}",Name ="borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Libros.AnyAsync(x => x.id == id);
            if (!existe)
                return NotFound();
            _context.Remove(new Libro() { id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
