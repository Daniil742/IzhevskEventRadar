using MassTransit;

namespace IzhevskEventRadar.Infrastructure.Consumers.ConsumerDefinitions;

internal class ProcessAllGroupsConsumerDefinition : ConsumerDefinition<ProcessAllGroupsConsumer>
{
	public ProcessAllGroupsConsumerDefinition()
	{
		EndpointName = "process-all-groups-consumer";
	}

    //protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProcessAllGroupsConsumer> consumerConfigurator)
    //{
    //    endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
    //}
}
