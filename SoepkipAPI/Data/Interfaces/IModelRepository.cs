using Microsoft.EntityFrameworkCore;

namespace SoepkipAPI.Data.Interfaces;

public interface IModelRepository<TEntity> where TEntity : class
{
    public DbContext Context { get; }
    
    List<TEntity> ReadAll();
    TEntity? Read(string id);
    void Write(TEntity entity);
    void Delete(string id);
    Task<int> SaveChangesAsync();
}