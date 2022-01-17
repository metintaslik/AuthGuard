using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthGuard.API.Models.Responses
{
    public class BaseResponse<T> where T : class
    {
        public BaseResponse(bool isError, int? httpStatusCode = null, int? errorCode = null, string? errorDetail = null, T? model = null)
        {
            IsError = isError;
            HttpStatusCode = httpStatusCode;
            ErrorCode = errorCode;
            ErrorDetail = errorDetail;
            Model = model;
        }

        public bool IsError { get; set; }
        [JsonIgnore]
        public int? HttpStatusCode { get; set; }
        public int? ErrorCode { get; set; }
        public string? ErrorDetail { get; set; }
        public T? Model { get; set; }
    }

    public enum ErrorType
    {
        [Display(Name = "UnExpectedErrorOccured")]
        UnExpectedErrorOccured = -1,

        [Display(Name = "ModelCannotBeLeftBlank")]
        ModelCannotBeLeftBlank = 0,

        [Display(Name = "Forbidden")]
        Forbidden = 1,
    }
}