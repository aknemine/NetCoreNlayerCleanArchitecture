using System.Net;
using System.Text.Json.Serialization;

namespace App.Application;

public class ApplicationResult<T>
{
    public T? Data { get; set; }
    public List<string>? ErrorMessage { get; set; }
    [JsonIgnore]  public bool IsSuccess => ErrorMessage == null || ErrorMessage.Count == 0;
    [JsonIgnore]  public bool IsFail => !IsSuccess;
    [JsonIgnore]  public HttpStatusCode Status { get; set; }
    [JsonIgnore] public string? UrlAsCreated { get; set; }

    //static factory method
    public static ApplicationResult<T> Success(T data,HttpStatusCode status=HttpStatusCode.OK)
    {
        return new ApplicationResult<T>() 
        { 
            Data = data, 
            Status=status 
        };

    }

    //static factory method
    public static ApplicationResult<T> SuccessAsCreated(T data,string urlAsCreated)
    {
        return new ApplicationResult<T>()
        {
            Data = data,
            Status = HttpStatusCode.Created,
            UrlAsCreated=urlAsCreated
        };

    }

    //static factory method
    public static ApplicationResult<T> Fail(List<string> errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ApplicationResult<T>() 
        { 
            ErrorMessage = errorMessage,
            Status=status
        };
    }

    //static factory method
    public static ApplicationResult<T> Fail(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ApplicationResult<T>() 
        { 
            ErrorMessage = [errorMessage],
            Status=status
        };
    }
}

public class ApplicationResult
{
    public List<string>? ErrorMessage { get; set; }
    [JsonIgnore] public bool IsSuccess => ErrorMessage == null || ErrorMessage.Count == 0;
    [JsonIgnore] public bool IsFail => !IsSuccess;
    [JsonIgnore] public HttpStatusCode Status { get; set; }

    //static factory method
    public static ApplicationResult Success(HttpStatusCode status = HttpStatusCode.OK)
    {
        return new ApplicationResult()
        {
            Status = status
        };

    }

    //static factory method
    public static ApplicationResult Fail(List<string> errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ApplicationResult()
        {
            ErrorMessage = errorMessage,
            Status = status
        };
    }

    //static factory method
    public static ApplicationResult Fail(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ApplicationResult()
        {
            ErrorMessage = [errorMessage],
            Status = status
        };
    }
}
