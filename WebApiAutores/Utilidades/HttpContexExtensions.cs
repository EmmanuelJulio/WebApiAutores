using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utilidades
{
    public static class HttpContexExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if(httpContext== null) { throw new ArgumentNullException(nameof(httpContext)); }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("CantidadTotalRegistros",cantidad.ToString());
        }
    }
}
