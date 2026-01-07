namespace SUREBusiness.Core.Common;

public abstract class UseCaseResponse : IUseCaseResponse
{
    public UseCaseResponse() { }
    public UseCaseResponse(string errorMessages)
    {
        this.errorMessages = [errorMessages];
    }
    public UseCaseResponse(params string[] errorMessages)
    {
        this.errorMessages = [.. errorMessages];
    }

    public bool Success => !ErrorMessages.Any();

    public IEnumerable<string> ErrorMessages => errorMessages;

    private List<string> errorMessages { get; } = [];
}

public abstract class UseCaseResponse<T> : IUseCaseResponse<T?> where T : class
{
    public UseCaseResponse(string errorMessages)
    {
        this.errorMessages = [errorMessages];
    }
    public UseCaseResponse(params string[] errorMessages)
    {
        this.errorMessages = [.. errorMessages];
    }
    public UseCaseResponse(T result)
    {
        Result = result;
    }

    public bool Success => !ErrorMessages.Any();

    public IEnumerable<string> ErrorMessages => errorMessages.AsReadOnly();

    protected List<string> errorMessages { get; } = [];

    public T? Result { get; set; }
}