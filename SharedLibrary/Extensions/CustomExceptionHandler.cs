using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.DTOs;
using SharedLibrary.Exceptions;

namespace SharedLibrary.Extensions
{
    public static class CustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature is not null)
                    {
                        var exception = errorFeature.Error;
                        ErrorDto errorDto = null;
                        if (exception is CustomException)
                        {
                            errorDto = new ErrorDto(exception.Message, true);
                        }
                        else
                        {
                            errorDto = new ErrorDto(exception.Message, false);
                        }

                        var response = ResponseDto<NoDataDto>.Fail(errorDto,
                            HttpStatusCode.InternalServerError.GetHashCode());
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}
