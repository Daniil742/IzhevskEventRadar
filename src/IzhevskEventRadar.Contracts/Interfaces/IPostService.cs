using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Contracts.Interfaces;

public interface IPostService
{
    Task AddPost(PostDto post);
}
