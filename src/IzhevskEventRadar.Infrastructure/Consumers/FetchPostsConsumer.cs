using IzhevskEventRadar.Contracts.Commands;
using IzhevskEventRadar.Contracts.Interfaces.Communicators;
using MassTransit;

namespace IzhevskEventRadar.Infrastructure.Consumers;

internal class FetchPostsConsumer(
    IVkApiCommunicator vkApiService,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<FetchPostsCommand>
{
    private readonly IVkApiCommunicator _vkApiService = vkApiService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Consume(ConsumeContext<FetchPostsCommand> context)
    {
        var posts = await _vkApiService.GetWallPostsAsync(context.Message.InternalGroupId, context.CancellationToken);

        if (!posts.Any())
            return;

#if DEBUG 

        posts = posts.Take(1).ToList();

#endif

        var publishTasks = posts.Select(p =>
            _publishEndpoint.Publish(new PostFetchedEvent(p, context.Message.InternalGroupId), context.CancellationToken)//ProcessPostWithAiCommand(context.Message.InternalGroupId, p)
        );

        await Task.WhenAll(publishTasks);
    }
}