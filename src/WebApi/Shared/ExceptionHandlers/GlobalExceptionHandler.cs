using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ExceptionHandling;
using WebApi.Models;

namespace WebApi.Shared.ExceptionHandlers
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(
            ExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            var serverErrorResponse = ApiResponse.Failed(
                ApiResponse.DefaultMessage.ServerError);

            return Task.CompletedTask;
        }
    }
}
