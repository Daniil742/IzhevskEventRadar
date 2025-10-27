using IzhevskEventRadar.Contracts.Commands;
using IzhevskEventRadar.Contracts.Interfaces;
using MassTransit;

namespace IzhevskEventRadar.Infrastructure.Consumers;

internal class ProcessAllGroupsConsumer(
    IGroupService groupService,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<ProcessAllGroupsCommand>
{
    private readonly IGroupService _groupService = groupService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Consume(ConsumeContext<ProcessAllGroupsCommand> context)
    {
        var groupIds = await _groupService.GetAllGroupIds(context.CancellationToken);

        var publishTasks = groupIds.Select(groupId => _publishEndpoint.Publish(new FetchPostsCommand(groupId), context.CancellationToken));

        await Task.WhenAll(publishTasks);
    }
}
