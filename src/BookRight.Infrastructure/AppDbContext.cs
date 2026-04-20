using Microsoft.EntityFrameworkCore;
using BookRight.Domain.Entities;

namespace BookRight.Infrastructure;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}