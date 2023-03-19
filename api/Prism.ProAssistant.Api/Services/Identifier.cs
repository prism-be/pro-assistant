// -----------------------------------------------------------------------
//  <copyright file = "Identifier.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Bson;

namespace Prism.ProAssistant.Api.Services;

public static class Identifier
{
    public static string GenerateString()
    {
        return ObjectId.GenerateNewId().ToString();
    }
}