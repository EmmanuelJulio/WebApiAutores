using WebApiAutores.Entidades;

namespace WebApiAutores.Dtos
{
    public class LibroDTO
    {
        public int id { get; set; }
       
        public string Titulo { get; set; }

        public DateTime FechaPublicacion { get; set; }
        //public List<ComentarioDTO> Comentarios { get; set; }
    }
}
