using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebApiAutores.Test.Mock;

namespace WebApiAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTest
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_OptenemosCuatroLincks()
        {
            //preparacion
            var AuthorizationService = new AuthorizationServiceMock();
            AuthorizationService.resultado = AuthorizationResult.Success();
            var rotController = new RootController(AuthorizationService);
            rotController.Url = new URLHelperMock();
            //Ejecucion
            var result = await rotController.Get();
            //Verificacion
            Assert.AreEqual(4, result.Value.Count());
        }
        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_OptenemosDosLincks()
        {
            //preparacion
            var AuthorizationService = new AuthorizationServiceMock();
            AuthorizationService.resultado = AuthorizationResult.Failed();
            var rotController = new RootController(AuthorizationService);
            rotController.Url = new URLHelperMock();
            //Ejecucion
            var result = await rotController.Get();
            //Verificacion
            Assert.AreEqual(2, result.Value.Count());
        }
        public async Task SiUsuarioNoEsAdmin_OptenemosDosLincks_usandoMoq()
        {
            //preparacion
            var MockAuthorizationService = new Mock<IAuthorizationService>();
            MockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            MockAuthorizationService.Setup(x => x.AuthorizeAsync(
               It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(),
               It.IsAny<string>()
               )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURLHelper = new Mock<IUrlHelper>();
            mockURLHelper.Setup(x => x.Link(It.IsAny<string>(),
                It.IsAny<object>()))
                .Returns(string.Empty);
          
            var rotController = new RootController(MockAuthorizationService.Object);
            rotController.Url = mockURLHelper.Object;
            //Ejecucion
            var result = await rotController.Get();
            //Verificacion
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
