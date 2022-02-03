using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAutores.Dtos;

namespace WebApiAutores.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor 
            ,IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }
        private async Task<bool> EsAdmin()
        {
            var httpcontext = httpContextAccessor.HttpContext;
            var esAdmin = await authorizationService.AuthorizeAsync(httpcontext.User, "EsAdmin");
            return esAdmin.Succeeded;      
        }   
        private IUrlHelper ConstruirUrlHelper()
        {
            var Factoria = httpContextAccessor.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
            return Factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }
        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirUrlHelper();
            autorDTO.Enlaces.Add(new DatoHATEOAS(enlasce: Url.Link("optenerautor", new { id = autorDTO.id }),
                descripcion: "self",
                metodo: "GET"));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(enlasce: Url.Link("actualizarAutor", new { id = autorDTO.id }),
                     descripcion: "autor-actualizar",
                     metodo: "PUT"));

                autorDTO.Enlaces.Add(new DatoHATEOAS(enlasce: Url.Link("borrarAutor", new { id = autorDTO.id }),
                  descripcion: "autor-borrar",
                  metodo: "DELETE"));
            }
        }
    }
}
