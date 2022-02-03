using WebApiAutores.Dtos;

namespace WebApiAutores.Utilidades
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecorsPorPagina)
                .Take(paginacionDTO.RecorsPorPagina);
        }
    }
}
