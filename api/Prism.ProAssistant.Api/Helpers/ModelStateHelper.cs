namespace Prism.ProAssistant.Api.Helpers;

using Microsoft.AspNetCore.Mvc.ModelBinding;

public static class ModelStateHelper
{
    public static void Validate(bool validState)
    {
        if (!validState)
        {
            throw new BadHttpRequestException("Invalid model state");
        }
    }
}