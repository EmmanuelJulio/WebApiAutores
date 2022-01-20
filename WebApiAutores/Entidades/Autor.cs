using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor :IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres")]
        [PrimeraLetraMayusculaAtribute]
        public string nombre { get; set; }

        //[Range(28, 120)]
        //[NotMapped]
        //public int Edad { get; set; }
        //[CreditCard]
        //public string CreditCard { get; set; }
        //[Url]
        //public string Url { get; set; }
        //public int Menor { get; set; }
        //public int Mayor { get; set; }
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                var primeraletra = nombre[0].ToString();
                if (primeraletra != primeraletra.ToUpper())
                {
                    yield return new ValidationResult("la primera letra debe ser mayuscula",
                        new string[] { nameof(nombre) });
                }
            }

            //if (Menor > Mayor)
            //{
            //    yield return new ValidationResult("Este valor no puede ser mas grande que el campo mayot ", 
            //        new string[] { nameof(Menor) });
            //}
        }
    }
}
