// -----------------------------------------------------------------------
//  <copyright file = "Identifier.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using MongoDB.Bson;

namespace Prism.ProAssistant.Business.Security;

public static class Identifier
{
    public static string GenerateString()
    {
        return ObjectId.GenerateNewId().ToString();
    }
}