using System;
using System.Net;
using System.Collections.Generic;

public class WeatherServiceException : Exception
{
    // Additional properties to store contextual information about the error
    public HttpStatusCode? StatusCode { get; private set; }
    public string Endpoint { get; private set; }
    public IDictionary<string, string> Parameters { get; private set; }

    // Basic constructor with only a message
    public WeatherServiceException(string message) : base(message)
    {
    }

    // Constructor with message and inner exception
    public WeatherServiceException(string message, Exception inner)
        : base(message, inner)
    {
    }

    // Constructor with message and HTTP status code
    public WeatherServiceException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    // Constructor with message, status code, and inner exception
    public WeatherServiceException(string message, HttpStatusCode statusCode, Exception inner)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }

    // Full constructor including endpoint and parameters for complete context
    public WeatherServiceException(string message, HttpStatusCode statusCode, string endpoint, IDictionary<string, string> parameters, Exception inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        Endpoint = endpoint;
        Parameters = parameters ?? new Dictionary<string, string>();
    }

    // Override the ToString() method to include detailed information about the exception
    public override string ToString()
    {
        string paramDetails = Parameters != null ? string.Join(", ", Parameters) : "No parameters";
        return $"{base.ToString()}, StatusCode: {StatusCode}, Endpoint: {Endpoint}, Parameters: {paramDetails}";
    }
}
