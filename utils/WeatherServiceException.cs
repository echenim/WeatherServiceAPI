using System;
using System.Net;
using System.Collections.Generic;

/// <summary>
/// Represents exceptions that occur during weather service operations.
/// </summary>
public class WeatherServiceException : Exception
{
    /// <summary>
    /// Gets the HTTP status code related to the exception, if applicable.
    /// </summary>
    public HttpStatusCode? StatusCode { get; private set; }

    /// <summary>
    /// Gets the endpoint that was being accessed when the exception occurred.
    /// </summary>
    public string Endpoint { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a dictionary containing the parameters that were used in the request that caused the exception.
    /// </summary>
    public IDictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();

    /// <summary>
    /// Initializes a new instance of the WeatherServiceException class with a specific message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public WeatherServiceException(string message) : base(message)
    {
        EnsureDefaults();
    }

    /// <summary>
    /// Initializes a new instance of the WeatherServiceException class with a specific message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public WeatherServiceException(string message, Exception inner)
        : base(message, inner)
    {
        EnsureDefaults();
    }

    /// <summary>
    /// Initializes a new instance of the WeatherServiceException class with a specific message and status code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="statusCode">The HTTP status code related to the exception, if applicable.</param>
    public WeatherServiceException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
        EnsureDefaults();
    }

    /// <summary>
    /// Initializes a new instance of the WeatherServiceException class with a specific message, status code, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="statusCode">The HTTP status code related to the exception, if applicable.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public WeatherServiceException(string message, HttpStatusCode statusCode, Exception inner)
        : base(message, inner)
    {
        StatusCode = statusCode;
        EnsureDefaults();
    }

    /// <summary>
    /// Initializes a new instance of the WeatherServiceException class with detailed information about the exception, including endpoint and parameters.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="statusCode">The HTTP status code related to the exception, if applicable.</param>
    /// <param name="endpoint">The endpoint accessed during the exception occurrence.</param>
    /// <param name="parameters">The parameters used in the request that caused the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception, if any.</param>
    public WeatherServiceException(string message, HttpStatusCode statusCode, string endpoint, IDictionary<string, string> parameters, Exception inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        Endpoint = endpoint ?? string.Empty;
        Parameters = parameters ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Ensures all non-nullable properties are initialized to prevent null reference issues.
    /// </summary>
    private void EnsureDefaults()
    {
        Endpoint = Endpoint ?? string.Empty;
        Parameters = Parameters ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Returns a string representation of the current exception, including detailed context information.
    /// </summary>
    /// <returns>A string containing the exception message and additional details.</returns>
    public override string ToString()
    {
        string paramDetails = string.Join(", ", Parameters);
        return $"{base.ToString()}, StatusCode: {StatusCode}, Endpoint: '{Endpoint}', Parameters: {paramDetails}";
    }
}
