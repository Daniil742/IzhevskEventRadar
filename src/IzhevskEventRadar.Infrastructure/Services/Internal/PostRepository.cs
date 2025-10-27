using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.DataBase.Context;
using IzhevskEventRadar.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IzhevskEventRadar.Infrastructure.Services.Internal;

internal class PostRepository(
    IDbContextFactory<EventRadarDbContext> dbContextFactory
    ) : IPostRepository
{
    private readonly IDbContextFactory<EventRadarDbContext> _dbContextFactory = dbContextFactory;

    public async Task AddPost(PostDto post, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
    }
}
