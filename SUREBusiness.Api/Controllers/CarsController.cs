using Microsoft.AspNetCore.Mvc;
using SUREBusiness.Api.Dtos;
using SUREBusiness.Api.Profiles;
using SUREBusiness.Core.Common;
using SUREBusiness.Core.Entities;
using SUREBusiness.Core.UseCases.Cars.Add;
using SUREBusiness.Core.UseCases.Cars.Get;
using SUREBusiness.Core.UseCases.Cars.Search;
using SUREBusiness.Core.UseCases.Cars.Update;

namespace SUREBusiness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CarsController : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarDto>> Get(
        [FromServices] IGetCarUseCase useCase,
        int id)
    {
        var response = await useCase.ExecuteAsync(new GetCarRequest(id));

        if (!response.Success || response.Result is null)
            return NotFound(new { error = response.ErrorMessages });

        return Ok(response.Result.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CarDto>>> Search(
         [FromServices] ISearchCarsUseCase useCase,
         [FromQuery] string? currentHolder,
         [FromQuery] CarStatus? status,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 25)
    {
        var request = new SearchCarsRequest(
            currentHolder,
            status,
            page,
            pageSize);

        var response = await useCase.ExecuteAsync(request);

        if (!response.Success || response.Result is null)
            return Conflict(new { error = response.ErrorMessages });

        var dto = response.Result!.Items.Select(x => x.ToDto());

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Create(
        [FromServices] IAddCarUseCase useCase,
        [FromBody] CreateCarDto dto)
    {
        var request = dto.ToRequest();
        var response = await useCase.ExecuteAsync(request);

        if (!response.Success)
            return BadRequest(new { error = response.ErrorMessages });

        return CreatedAtAction(nameof(Get), new { id = response.Result!.Id }, response.Result.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(
        [FromServices] IUpdateCarUseCase useCase,
        int id,
        [FromBody] UpdateCarDto dto)
    {
        var request = dto.ToRequest(id);
        var response = await useCase.ExecuteAsync(request);

        if (!response.Success)
            return BadRequest(new { error = response.ErrorMessages });

        return NoContent();
    }
}