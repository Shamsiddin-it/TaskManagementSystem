using System.Net;

public class Response<T>
{
    public int StatusCode { get; set; } = 0;
    public List<string> Description { get; set; } = [];
    public T? Date { get; set; }

    public Response(HttpStatusCode statusCode, string message, T date)
    {
        StatusCode = (int)statusCode;
        Description.Add(message);
        Date = date;
    }

    public Response(HttpStatusCode statusCode, List<string> message, T date)
    {
        StatusCode = (int)statusCode;
        Description = message;
        Date = date;
    }

    public Response(HttpStatusCode statusCode, string message)
    {
        StatusCode = (int)statusCode;
        Description.Add(message);
    }

    public Response(HttpStatusCode statusCode, List<string> message)
    {
        StatusCode = (int)statusCode;
        Description = message;
    }
}
