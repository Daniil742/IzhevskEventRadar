using IzhevskEventRadar.Contracts.Interfaces;
using IzhevskEventRadar.Contracts.Models;
using IzhevskEventRadar.Infrastructure.Interfaces;

namespace IzhevskEventRadar.Infrastructure.Services;

internal class PostService(
    IPostRepository postRepository
    ) : IPostService
{
    private readonly IPostRepository _postRepository = postRepository;

    public async Task AddPost(PostDto post)
    {
        await _postRepository.AddPost(post);
    }
}
