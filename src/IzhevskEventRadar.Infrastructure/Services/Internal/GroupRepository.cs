using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.DataBase.Context;
using IzhevskEventRadar.DataBase.DataModels;
using IzhevskEventRadar.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IzhevskEventRadar.Infrastructure.Services.Internal;

internal class GroupRepository(
    IDbContextFactory<EventRadarDbContext> dbContextFactory
    ) : IGroupRepository
{
    private readonly IDbContextFactory<EventRadarDbContext> _dbContextFactory = dbContextFactory;

    public async Task<IReadOnlyCollection<string>> GetGroupInternalIds(CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = from g in context.Groups

                    where g.IsActive == true

                    select g.InternalId;

        var result = await query.ToArrayAsync(cancellationToken);

        return result;
    }

    public async Task<IReadOnlyCollection<GroupDto>> GetAllGroupsInfo(CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = from g in context.Groups.AsNoTracking()

                    orderby g.IsActive descending

                    select new GroupDto
                    {
                        Id = g.Id,
                        InternalId = g.InternalId,
                        IsActive = g.IsActive,
                    };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<GroupDto> CreateGroup(string internalId, CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // TODO: Добавить валидацию, что такой internalId еще не существует

        var newGroup = new GroupDataModel
        {
            InternalId = internalId
        };

        context.Groups.Add(newGroup);
        
        await context.SaveChangesAsync(cancellationToken);

        return new GroupDto
        {
            Id = newGroup.Id,
            InternalId = newGroup.InternalId
        };
    }

    public async Task<GroupDto> UpdateGroup(int id, GroupPatchDto groupPatch, CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        if (group is null)
        {
            throw new KeyNotFoundException($"Группа с Id {id} не найдена.");
        }

        if (groupPatch.InternalId is not null)
            group.InternalId = groupPatch.InternalId.Trim();

        if (groupPatch.IsActive.HasValue)
            group.IsActive = groupPatch.IsActive.Value;

        await context.SaveChangesAsync(cancellationToken);

        return new GroupDto
        {
            Id = group.Id,
            InternalId = group.InternalId,
            IsActive = group.IsActive
        };
    }

    public async Task DeleteGroup(int id, CancellationToken cancellationToken = default)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        if (group is not null)
        {
            context.Groups.Remove(group);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
