namespace ECommerce.Application.Common
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }

        public ErrorResponse(int statusCode, string message, object? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
