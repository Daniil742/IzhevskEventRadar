using IzhevskEventRadar.Contracts.Interfaces;
using IzhevskEventRadar.Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace IzhevskEventRadar.Api.Endpoints;

public static partial class Endpoints
{
    public static IEndpointRouteBuilder RegisterGroupSettingsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var mapGroup = endpoints.MapGroup("/api/groups");

        mapGroup.MapGet("", async (
            [FromServices] IGroupService groupService,
            CancellationToken cancellationToken)
            =>
        {
            var groups = await groupService.GetAllGroups(cancellationToken);

            return Results.Ok(groups);
        });

        mapGroup.MapPost("", async (
            [FromServices] IGroupService groupService,
            [FromBody] string internalId,
            CancellationToken cancellationToken)
            =>
        {
            var newGroup = await groupService.CreateGroup(internalId, cancellationToken);

            return Results.Created($"/api/groups/{newGroup.Id}", newGroup);
        });

        mapGroup.MapPatch("{id}", async (
            int id,
            [FromServices] IGroupService groupService,
            [FromBody] GroupPatchDto groupPatch,
            CancellationToken cancellationToken)
            =>
        {
            try
            {
                var updatedGroup = await groupService.UpdateGroup(id, groupPatch, cancellationToken);

                return Results.Ok(updatedGroup);
            }
            catch (Exception ex)
            {
                return Results.NotFound();
            }
        });

        mapGroup.MapDelete("{id}", async (
            int id,
            [FromServices] IGroupService groupService,
            CancellationToken cancellationToken)
            =>
        {
            await groupService.DeleteGroup(id, cancellationToken);

            return Results.NoContent();
        });

        //mapGroup.MapPost("{internalId}/validate", async (
        //    string internalId,
        //    [FromServices] IGroupService groupService,
        //    CancellationToken cancellationToken)
        //    =>
        //{
        //    var isValid = await groupService.ValidateGroup(internalId, cancellationToken);

        //    return isValid
        //        ? Results.Ok(isValid)
        //        : Results.BadRequest(isValid);
        //});

        return endpoints;
    }
}
