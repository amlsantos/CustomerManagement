namespace CustomerManagement.Logic.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; private set; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException();
        
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    /*
     * Static helper methods
     */
    public static Result Fail(string message)
    {
        return new Result(false, message);
    }

    public static Result<T> Fail<T>(string message)
    {
        return new Result<T>(default(T), false, message);
    }

    public static Result Ok()
    {
        return new Result(true, string.Empty);
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }
}

public class Result<T> : Result
{
    private readonly T _value;
    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException();

            return _value;
        }
    }

    protected internal Result(T value, bool isSuccess, string error) : base(isSuccess, error)
    {
        _value = value;
    }
}