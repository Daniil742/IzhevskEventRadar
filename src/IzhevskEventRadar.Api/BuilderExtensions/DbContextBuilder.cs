using IzhevskEventRadar.DataBase.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IzhevskEventRadar.Api.BuilderExtensions;

public static class DbContextBuilder
{
    public static void AddDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContextFactory<EventRadarDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("EVENTRADARCONNECTIONSTRING");

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            var dataSource = dataSourceBuilder.Build();
            options.UseNpgsql(dataSource);
        });
    }
}
