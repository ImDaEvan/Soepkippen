using Microsoft.EntityFrameworkCore;
using SoepkipAPI.Data.Interfaces;

namespace SoepkipAPI.Data.Repository;

/// <summary>
/// Base class for repository models, use this if you want to create a new model that has a database table
/// </summary>
/// <typeparam name="TEntity">The entity model that will be stored in the database</typeparam>
public abstract class RepositoryBase<TEntity> : IModelRepository<TEntity> where TEntity : class
{
    //The database 'table' of the entity
    protected readonly DbSet<TEntity> _dbSet;
    
    //The context where the db set will be grabbed from
    public DbContext Context { get; }

    protected RepositoryBase(DbContext context)
    {
        Context = context;
        _dbSet = context.Set<TEntity>();
    }
    
    /// <summary>
    /// Reads every entity from the context
    /// </summary>
    /// <returns></returns>
    public List<TEntity> ReadAll()
    {
        return _dbSet.ToList();
    }

    public TEntity? Read(string id)
    {
        return _dbSet.Find(id);
    }

    public void Write(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public void Delete(string id)
    {
        var entity = _dbSet.Find(id);

        if (entity == null) throw new("Deletion of trash item failed: id not found");
        
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        var rowsAffected = await Context.SaveChangesAsync();
        return rowsAffected;
    }
}