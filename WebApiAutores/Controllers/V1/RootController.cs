﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Dtos;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/V1")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name ="ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHATEOAS = new List<DatoHATEOAS>();
            var esAdmin = await authorizationService.AuthorizeAsync(User, "EsAdmin");

            datosHATEOAS.Add(new DatoHATEOAS(enlasce: Url.Link("ObtenerRoot", new { }),
                descripcion: "self", metodo: "GET"));

            datosHATEOAS.Add(new DatoHATEOAS(enlasce:Url.Link("obtenerAutores", new { }), descripcion: "autores",
                metodo: "GET"));
            if (esAdmin.Succeeded)
            {

                datosHATEOAS.Add(new DatoHATEOAS(enlasce: Url.Link("crearAutor", new { }), descripcion: "autores-crear",
                    metodo: "POST"));
                datosHATEOAS.Add(new DatoHATEOAS(enlasce: Url.Link("crearLibro", new { }), descripcion: "libro-crear",
                    metodo: "POST"));

            }
            return datosHATEOAS;
        }
    }
}
