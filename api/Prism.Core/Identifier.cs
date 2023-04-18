// -----------------------------------------------------------------------
//  <copyright file = "Identifier.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using Acme.Core.Extensions;

namespace Prism.Core;

public static class Identifier
{
    public static string GenerateString()
    {
        var bytes = RandomNumberGenerator.GetBytes(12);
        return bytes.ToHexadecimalString();
    }
}