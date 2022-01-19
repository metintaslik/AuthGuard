namespace AuthGuard.Security.Models.Response
{
    public class ApiBaseResponse<T> where T : class
    {
        public bool IsError { get; set; }
        public int? ErrorCode { get; set; }
        public string? ErrorDetail { get; set; }
        public T? Model { get; set; }
    }
}