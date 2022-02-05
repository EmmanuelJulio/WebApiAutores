using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.PruebasUnitarias

{
    [TestClass]
    public class PrimeraLetraMayusculaAtributeTest
    {
        [TestMethod]
        public void PrimeraLetraMinusculaDevuelveError()
        {
            //Preparacion
            var PrimeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            var valor = "felipe";
            var contex = new ValidationContext(new { Nombre = valor });

            //Ejecucion

            var resultado = PrimeraLetraMayuscula.GetValidationResult(valor,contex);
            //Verificacion

            Assert.AreEqual("la primera letra debe ser mayuscuala",resultado.ErrorMessage);
        }
        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparacion
            var PrimeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            var valor = "felipe";
            var contex = new ValidationContext(new { Nombre = valor });

            //Ejecucion

            var resultado = PrimeraLetraMayuscula.GetValidationResult(valor, contex);
            //Verificacion

            Assert.IsNotNull(resultado);
        }
        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {
            //Preparacion
            var PrimeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            var valor = "Felipe";
            var contex = new ValidationContext(new { Nombre = valor });

            //Ejecucion

            var resultado = PrimeraLetraMayuscula.GetValidationResult(valor, contex);
            //Verificacion

            Assert.IsNull(resultado);
        }

    }
}