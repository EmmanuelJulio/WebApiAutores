namespace WebApiAutores.interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task Add<T>(T entity) where T : class;
        Task Update<T>(T entity) where T : class;
        Task Delete<T>(T entity) where T : class;
        Task <List<T>> GetAll<T>() where T : class;
         Task<T>  GetById(int id);

    }
}
