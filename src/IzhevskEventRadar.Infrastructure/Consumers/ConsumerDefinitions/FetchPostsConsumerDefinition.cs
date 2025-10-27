using MassTransit;

namespace IzhevskEventRadar.Infrastructure.Consumers.ConsumerDefinitions;

internal class FetchPostsConsumerDefinition : ConsumerDefinition<FetchPostsConsumer>
{
    public FetchPostsConsumerDefinition()
    {
        EndpointName = "fetch-posts-consumer";
    }

    //protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FetchPostsConsumer> consumerConfigurator)
    //{
    //    endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
    //}
}
