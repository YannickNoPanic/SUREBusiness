namespace SUREBusiness.Core.Common;

public interface IUseCaseResponse
{
    bool Success { get; }
    IEnumerable<string> ErrorMessages { get; }
}

public interface IUseCaseResponse<T>
{
    bool Success { get; }
    IEnumerable<string> ErrorMessages { get; }
    public T Result { get; set; }
}

