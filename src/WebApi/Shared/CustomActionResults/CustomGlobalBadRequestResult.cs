using Helpers.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Shared.CustomActionResults
{
    public sealed class CustomGlobalBadRequestResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var modelErrorCollection = context.ModelState.Values
                .Select(modelStateEntry => modelStateEntry.Errors);

            if (!Equals(modelErrorCollection, null))
            {
                var errorMessages = new List<string>();

                modelErrorCollection.ForEach(modelError =>
                {
                    modelError.ForEach(error =>
                    {
                        errorMessages.Add(error.ErrorMessage);
                    });
                });

                errorMessages.TrimExcess();

                var apiResponse = ApiResponse.Failed(errorMessages);

                var actionResult = new ObjectResult(apiResponse)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

                return actionResult.ExecuteResultAsync(context);
            }

            return new BadRequestObjectResult(ApiResponse.Failed())
                .ExecuteResultAsync(context);
        }
    }
}
