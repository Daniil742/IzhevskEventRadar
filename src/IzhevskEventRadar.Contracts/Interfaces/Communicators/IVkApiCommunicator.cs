using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Contracts.Interfaces.Communicators;

public interface IVkApiCommunicator
{
    Task<IReadOnlyCollection<PostDto>> GetWallPostsAsync(string internalGroupId, CancellationToken cancellationToken = default);
}
