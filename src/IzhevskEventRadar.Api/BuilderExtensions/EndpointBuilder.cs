using IzhevskEventRadar.Api.Endpoints;

namespace IzhevskEventRadar.Api.BuilderExtensions;

public static class EndpointBuilder
{
    public static void UseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.RegisterGroupSettingsEndpoints();
        endpoints.RegisterEventsEndpoints();
    }
}
