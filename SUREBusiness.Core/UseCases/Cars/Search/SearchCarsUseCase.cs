using SUREBusiness.Core.Queries;

namespace SUREBusiness.Core.UseCases.Cars.Search;

public sealed class SearchCarsUseCase(ICarQuery carQuery) : ISearchCarsUseCase
{
    public async Task<SearchCarsResponse> ExecuteAsync(SearchCarsRequest input)
    {
        var pagedResult = await carQuery.QueryAsync(new()
        {
            CurrentHolder = input.CurrentHolder,
            Status = input.Status,

            Page = input.Page,
            PageSize = input.PageSize
        });

        return new SearchCarsResponse(pagedResult);
    }
}