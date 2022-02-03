using Microsoft.EntityFrameworkCore;
using WebApiAutores.interfaces;

namespace WebApiAutores.Servicios
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Add<T>(T entity) where T : class
        {
              await context.Set<T>().AddAsync(entity);
             await context.SaveChangesAsync();
           
        }

        public async Task Delete<T>(T entity) where T : class
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
           
        }

        public async Task<List<T>> GetAll<T>() where T : class
        {
            return await context.Set<T>().Select(a=>a).ToListAsync();
            
        }

        public async Task<T> GetById(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task Update<T>(T entity) where T : class
        {
            context.Set<T>().Update(entity);
           await context.SaveChangesAsync();
            
        }
    }
}
