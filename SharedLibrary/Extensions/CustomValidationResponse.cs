using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.DTOs;
using System.Net;

namespace SharedLibrary.Extensions
{
    public static class CustomValidationResponse
    {
        public static void UseCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values
                        .Where(x => x.Errors.Count > 0)
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage);
                    ErrorDto errorDto = new ErrorDto(errors.ToList(), true);
                    var response = ResponseDto<NoContentResult>.Fail(errorDto, HttpStatusCode.BadRequest.GetHashCode());
                    return new BadRequestObjectResult(response);
                };
            });
        }
    }
}
