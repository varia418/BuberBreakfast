using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.Models;
using BuberBreakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BuberBreakfast.Controllers;


public class BreakfastsController : ApiController
{
    private readonly IBreakfastService _breakfastService;

    public BreakfastsController(IBreakfastService breakfastService)
    {
        _breakfastService = breakfastService;
    }

    [HttpPost]
    public IActionResult CreateBreakfast(CreateBreakfastRequest request)
    {
        ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(request);

        if (requestToBreakfastResult.IsError)
        {
            return Problem(requestToBreakfastResult.Errors);
        }

        var breakfast = requestToBreakfastResult.Value;
        ErrorOr<Created> createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

        return createBreakfastResult.Match(
            created => CreatedAsGetBreakfast(breakfast),
            errors => Problem(errors)
        );
    }

    [HttpGet("{breakfastId:guid}")]
    public IActionResult GetBreakfast(Guid breakfastId)
    {
        ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(breakfastId);

        return getBreakfastResult.Match(
            breakfast => Ok(MapBreakfastResponse(breakfast)),
            errors => Problem(errors)
        );
    }

    [HttpPut("{breakfastId:guid}")]
    public IActionResult UpsertBreakfast(Guid breakfastId, UpsertBreakfastRequest request)
    {
        var requestToBreakfastResult = Breakfast.From(breakfastId, request);

        if (requestToBreakfastResult.IsError)
        {
            return Problem(requestToBreakfastResult.Errors);
        }

        var breakfast = requestToBreakfastResult.Value;
        var upsertBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);

        return upsertBreakfastResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAsGetBreakfast(breakfast) : NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{breakfastId:guid}")]
    public IActionResult DeleteBreakfast(Guid breakfastId)
    {
        var deleteBreakfastResult = _breakfastService.DeleteBreakfast(breakfastId);

        return deleteBreakfastResult.Match(
            deleted => NoContent(),
            errors => Problem(errors)
        );
    }

    private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
    {
        return new BreakfastResponse(
            breakfast.Id,
            breakfast.Name,
            breakfast.Description,
            breakfast.StartDateTime,
            breakfast.EndDateTime,
            breakfast.LastModifiedDateTime,
            breakfast.Savory,
            breakfast.Sweet
        );
    }

    private CreatedAtActionResult CreatedAsGetBreakfast(Breakfast breakfast)
    {
        return CreatedAtAction(
            actionName: nameof(GetBreakfast),
            routeValues: new { breakfastId = breakfast.Id },
            value: MapBreakfastResponse(breakfast)
        );
    }
}