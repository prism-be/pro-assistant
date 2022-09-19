// -----------------------------------------------------------------------
//  <copyright file = "JwtConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Business.Security;

public class JwtConfiguration
{
    public const string Audience = "proassistant-web";
    public const string Issuer = "proassistant-authentication";

    public JwtConfiguration()
    {
        PrivateKey = PublicKey = string.Empty;
    }

    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
}