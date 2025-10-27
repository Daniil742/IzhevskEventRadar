using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Contracts.Interfaces;

public interface IGroupService
{
    Task<IReadOnlyCollection<string>> GetAllGroupIds(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GroupDto>> GetAllGroups(CancellationToken cancellationToken = default);

    Task<GroupDto> CreateGroup(string internalId, CancellationToken cancellationToken = default);

    Task<GroupDto> UpdateGroup(int id, GroupPatchDto groupPatch, CancellationToken cancellationToken = default);

    Task DeleteGroup(int id, CancellationToken cancellationToken = default);

    //Task<bool> ValidateGroup(string internalId, CancellationToken cancellationToken = default);
}
