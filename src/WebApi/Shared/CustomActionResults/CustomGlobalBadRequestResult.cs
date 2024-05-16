using Helpers.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Shared.CustomActionResults
{
    public sealed class CustomGlobalBadRequestResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var errorMessages = context.ModelState.Values
                .Select(modelStateEntry => modelStateEntry.Errors)
                .Select(error => error.First().ErrorMessage);

            var apiResponse = ApiResponse.Failed(errorMessages);

            var actionResult = new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            return actionResult.ExecuteResultAsync(context);
        }
    }
}
