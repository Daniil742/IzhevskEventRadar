namespace IzhevskEventRadar.Api.BuilderExtensions;

public static class GrpcServicesBuilder
{
    public static void UseGrpc(this IEndpointRouteBuilder endpoints)
    {
        //endpoints.MapGrpcService<EventGrpcServiceImpl>();
    }
}
