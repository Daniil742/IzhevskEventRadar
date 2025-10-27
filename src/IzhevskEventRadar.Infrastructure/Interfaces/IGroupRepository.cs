using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Infrastructure.Interfaces;

internal interface IGroupRepository
{
    Task<IReadOnlyCollection<string>> GetGroupInternalIds(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GroupDto>> GetAllGroupsInfo(CancellationToken cancellationToken = default);

    Task<GroupDto> CreateGroup(string internalId, CancellationToken cancellationToken = default);

    Task<GroupDto> UpdateGroup(int id, GroupPatchDto groupPatch, CancellationToken cancellationToken = default);

    Task DeleteGroup(int id, CancellationToken cancellationToken = default);
}
