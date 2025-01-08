using System.Text.Json;

namespace API.Middlewares.Models
{
    public class ErrorDetails
    {
        public ErrorDetails(int statusCode, string message, string stackTrace = "")
        {
            StatusCode = statusCode;
            Message = message;
            StackTrace = stackTrace;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}