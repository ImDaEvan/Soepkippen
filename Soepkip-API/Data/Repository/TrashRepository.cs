using Microsoft.EntityFrameworkCore;
using SoepkipAPI.Data.Context;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Models;

namespace SoepkipAPI.Data.Repository;

public class TrashRepository : RepositoryBase<TrashItem>, ITrashRepository
{
    public TrashRepository(TrashContext context) : base(context)
    {
    }

    /// <summary>
    /// Returns all the trash within the timestamp range (inclusive)
    /// </summary>
    /// <param name="dateLeft">The minimum date</param>
    /// <param name="dateRight">The maximum date</param>
    /// <returns>A list of all trash models within the range</returns>
    public List<TrashItem> ReadRange(DateTime dateLeft, DateTime dateRight)
    {
        return _dbSet.Where(x => x.timestamp >= dateLeft && x.timestamp <= dateRight)
            .ToList();
       }
}