using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Dtos
{
    public class LibroCreacionDTO
    {
      
        [PrimeraLetraMayusculaAtribute]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public List<int> Autores { get; set; }
        public DateTime FechaPublicaion { get; set; }
    }
}
