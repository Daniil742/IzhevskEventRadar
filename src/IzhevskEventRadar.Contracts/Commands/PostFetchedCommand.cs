using IzhevskEventRadar.Contracts.Models;

namespace IzhevskEventRadar.Contracts.Commands;

public record PostFetchedEvent(PostDto Post, string InternalGroupId);
