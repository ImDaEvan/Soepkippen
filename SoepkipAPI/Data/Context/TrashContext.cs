using Microsoft.EntityFrameworkCore;
using SoepkipAPI.Models;

namespace SoepkipAPI.Data.Context;

public class TrashContext : DbContext
{
    public TrashContext()
    {
    }

    public TrashContext(DbContextOptions<TrashContext> options) : base(options)
    {
    }
    
    public DbSet<TrashItem> Trash { get; set; }
}