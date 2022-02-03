using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Dtos
{
    public class LibroPatchDTO
    {
        [PrimeraLetraMayusculaAtribute]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public DateTime FechaPublicaion { get; set; }
    }
}
