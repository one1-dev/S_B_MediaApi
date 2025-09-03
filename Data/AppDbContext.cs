using Microsoft.EntityFrameworkCore;
using S_B_MicroService.Domain.Models;

namespace S_B_MicroService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ContractEntity> Contracts => Set<ContractEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ContractEntity>().ToTable("contracts");
        b.Entity<ContractEntity>().Property(p => p.Name).HasMaxLength(200);
    }
}
