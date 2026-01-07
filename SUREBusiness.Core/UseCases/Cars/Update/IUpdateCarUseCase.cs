namespace SUREBusiness.Core.UseCases.Cars.Update;

public interface IUpdateCarUseCase
{
    Task<UpdateCarResponse> ExecuteAsync(UpdateCarRequest input);
}
