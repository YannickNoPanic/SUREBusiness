namespace SUREBusiness.Core.Common;

public interface IUseCase<Tout>
{
    Task<Tout> ExecuteAsync();
}


public interface IUseCase<Tin, Tout>
    where Tin : notnull
{
    Task<Tout> ExecuteAsync(Tin input);
}
