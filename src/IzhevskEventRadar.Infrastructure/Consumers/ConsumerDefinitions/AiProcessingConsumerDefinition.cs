using MassTransit;

namespace IzhevskEventRadar.Infrastructure.Consumers.ConsumerDefinitions;

internal class AiProcessingConsumerDefinition : ConsumerDefinition<AiProcessingConsumer>
{
    public AiProcessingConsumerDefinition()
    {
        EndpointName = "ai-processing-consumer";
    }

    //protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AiProcessingConsumer> consumerConfigurator)
    //{
    //    endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
    //}
}
