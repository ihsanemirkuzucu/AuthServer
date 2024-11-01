using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs
{
    public class ResponseDto<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }

        [JsonIgnore]
        public bool IsSuccessful { get; private set; }

        public ErrorDto Error { get; private set; }

        public static ResponseDto<T> Success(T data, int statusCode)
        {
            return new ResponseDto<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
        }

        public static ResponseDto<T> Success(int statusCode)
        {
            return new ResponseDto<T> { Data = default, StatusCode = statusCode, IsSuccessful = true };
        }

        public static ResponseDto<T> Fail(ErrorDto error, int statusCode)
        {
            return new ResponseDto<T> { Error = error, StatusCode = statusCode, IsSuccessful = false };
        }

        public static ResponseDto<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new ResponseDto<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }
    }
}
