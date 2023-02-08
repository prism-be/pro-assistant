// -----------------------------------------------------------------------
//  <copyright file = "User.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Business.Security;

public interface IUser
{
    string? Id { get; set; }
    string? Organization { get; set; }
    string? Name { get; set; }
    bool IsAuthenticated { get; set; }
}

public class User : IUser
{
    public string? Id { get; set; }
    
    public string? Organization { get; set; }
    
    public string? Name { get; set; }
    
    public bool IsAuthenticated { get; set; }
}