using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Infrastructure.Interfaces;

internal interface IPostRepository
{
    Task AddPost(PostDto post, CancellationToken cancellationToken = default);
}
