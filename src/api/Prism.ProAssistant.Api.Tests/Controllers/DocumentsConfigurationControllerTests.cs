// -----------------------------------------------------------------------
//  <copyright file = "DocumentsConfigurationControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class DocumentsConfigurationControllerTests
{

    [Fact]
    public async Task FindMany()
    {
        await CrudTests.FindMany<DocumentsConfigurationController, DocumentConfiguration>(c => c.FindMany());
    }

    [Fact]
    public async Task FindOne()
    {
        await CrudTests.FindOne<DocumentsConfigurationController, DocumentConfiguration>(c => c.FindOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task RemoveOne()
    {
        await CrudTests.RemoveOne<DocumentsConfigurationController, DocumentConfiguration>(c => c.RemoveOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task UpsertOne()
    {
        await CrudTests.UpsertOne<DocumentsConfigurationController, DocumentConfiguration>(c => c.UpsertOne(new DocumentConfiguration
        {
            Id = Identifier.GenerateString()
        }));
    }
}