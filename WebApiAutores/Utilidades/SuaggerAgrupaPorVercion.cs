using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilidades
{
    public class SuaggerAgrupaPorVercion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace;
            var vercionApi = namespaceControlador.Split('.').Last().ToUpper();
            controller.ApiExplorer.GroupName = vercionApi;
        }
    }
}
