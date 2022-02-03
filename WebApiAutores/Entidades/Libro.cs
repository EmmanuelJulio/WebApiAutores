using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int id { get; set; }
        [PrimeraLetraMayusculaAtribute]
        [StringLength(maximumLength:250)]
        [Required]
        public string Titulo { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public DateTime? FechaPublicacion { get; set; } 
        public List<AutorLibro> AutoresLibro { get; set; }



    }

}
