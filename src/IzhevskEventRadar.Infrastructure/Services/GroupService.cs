using IzhevskEventRadar.Contracts.Interfaces;
using IzhevskEventRadar.Contracts.Interfaces.Communicators;
using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace IzhevskEventRadar.Infrastructure.Services;

internal class GroupService(
    IGroupRepository groupRepository,
    IVkApiCommunicator vkApiCommunicator,
    ILogger<GroupService> logger
    ) : IGroupService
{
    private readonly IGroupRepository _groupRepository = groupRepository;
    private readonly IVkApiCommunicator _vkApiCommunicator = vkApiCommunicator;
    private readonly ILogger<GroupService> _logger = logger;

    public async Task<IReadOnlyCollection<string>> GetAllGroupIds(CancellationToken cancellationToken = default)
    {
        var result = await _groupRepository.GetGroupInternalIds(cancellationToken);

        return result;
    }

    public async Task<IReadOnlyCollection<GroupDto>> GetAllGroups(CancellationToken cancellationToken = default)
    {
        var result = await _groupRepository.GetAllGroupsInfo(cancellationToken);

        return result;
    }

    public async Task<GroupDto> CreateGroup(string internalId, CancellationToken cancellationToken = default)
    {
        var result = await _groupRepository.CreateGroup(internalId, cancellationToken);

        return result;
    }

    public async Task<GroupDto> UpdateGroup(int id, GroupPatchDto groupPatch, CancellationToken cancellationToken = default)
    {
        var result = await _groupRepository.UpdateGroup(id, groupPatch, cancellationToken);

        return result;
    }

    public async Task DeleteGroup(int id, CancellationToken cancellationToken = default)
    {
        await _groupRepository.DeleteGroup(id, cancellationToken);
    }

    //public async Task<bool> ValidateGroup(string internalId, CancellationToken cancellationToken = default)
    //{
    //    return result;
    //}
}
