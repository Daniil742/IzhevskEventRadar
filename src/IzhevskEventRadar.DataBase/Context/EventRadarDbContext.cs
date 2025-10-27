using IzhevskEventRadar.DataBase.DataModels;
using Microsoft.EntityFrameworkCore;

namespace IzhevskEventRadar.DataBase.Context;

public class EventRadarDbContext : DbContext
{
    public EventRadarDbContext(DbContextOptions<EventRadarDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupDataModel>()
            .HasData(InitialData.InitGroups());

        base.OnModelCreating(modelBuilder); 
    }

    public DbSet<GroupDataModel> Groups { get; set; }

    public DbSet<PostDataModel> Posts { get; set; }

    public DbSet<EventDataModel> Events { get; set; }
}
