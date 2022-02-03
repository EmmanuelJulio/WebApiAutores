using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;
using WebApiAutores.interfaces;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/V1/libros/{LibroId:int}/comentarios")]
    public class ComentarioController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IGenericRepository<Comentario> repository;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentarioController(ApplicationDbContext context,
            IGenericRepository<Comentario> repository,
            IMapper mapper,UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.repository = repository;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet("{id:int}",Name ="optenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetComentario(int id)
        {
            var comentario =await context.comentarios.FirstOrDefaultAsync(x=>x.Id==id);
            if (comentario == null)
                return NotFound();

            var Ocomentario = mapper.Map<ComentarioDTO>(comentario);
            return Ocomentario;
        }

        [HttpGet(Name ="obtenerComentarioPorIdLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int LibroId)
        {
            var libro = await context.Libros.AnyAsync(x => x.id == LibroId);
            if (!libro)
                return NotFound();

            var comentarios = await context.comentarios
                .Where(comentarios=>comentarios.LibroId==LibroId)
                .ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(Name ="crearComentario")]
        public async Task<ActionResult> Post(int LibroId,ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim=>claim.Type=="email").FirstOrDefault();
            var email = emailClaim.Value;
            var user=await userManager.FindByEmailAsync(email);
            var usuarioId=user.Id;
            var libro = await context.Libros.AnyAsync(x => x.id == LibroId);
            if(!libro)
                return NotFound();

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = LibroId;
            comentario.UsuarioId = usuarioId;
            await repository.Add(comentario);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("optenerComentario", new { id = comentario.Id ,LibroId=LibroId}, comentarioDTO);
        }
        [HttpPut("{id:int}",Name ="actualizarComentario")]
        public async Task<ActionResult<ComentarioCreacionDTO>> Put(ComentarioCreacionDTO comentario,int id,int LibroId)
        {
            var libro = await context.Libros.AnyAsync(x => x.id == LibroId);
            if (!libro)
                return NotFound("No se encuentra el libro");

            var ExisteComentario = await context.comentarios.AnyAsync(x => x.Id == id);
            if (!ExisteComentario) 
                return NotFound("No se encuentra el comentario para actualizar");

            var comentarioDTO = mapper.Map<Comentario>(comentario);
            comentarioDTO.Id = id;
            comentarioDTO.LibroId = LibroId;
            await repository.Update(comentarioDTO);
         
            return NoContent();
        }
    }
}
