// -----------------------------------------------------------------------
//  <copyright file = "User.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Business.Security;

public class User
{
    public string? Id { get; set; }
    
    public string? Organization { get; set; }
    
    public string? Name { get; set; }
    
    public bool IsAuthenticated { get; set; }
}