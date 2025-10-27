using MassTransit;

namespace IzhevskEventRadar.Infrastructure.Consumers.ConsumerDefinitions;

internal class StorageConsumerDefinition : ConsumerDefinition<StorageConsumer>
{
    public StorageConsumerDefinition()
    {
        EndpointName = "storage-comsumer";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<StorageConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
    }
}
