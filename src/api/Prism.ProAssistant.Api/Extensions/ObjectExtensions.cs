// -----------------------------------------------------------------------
//  <copyright file = "ObjectExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace Prism.ProAssistant.Api.Extensions;

public static class ObjectExtensions
{
    public static ActionResult<T> ToActionResult<T>(this T? item)
    {
        if (item == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(item);
    }
}