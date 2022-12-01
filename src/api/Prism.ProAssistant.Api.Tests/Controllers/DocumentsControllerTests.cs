// -----------------------------------------------------------------------
//  <copyright file = "DocumentsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class DocumentsControllerTests
{

    [Fact]
    public async Task FindMany()
    {
        await CrudTests.FindMany<DocumentsController, DocumentConfiguration>(c => c.FindMany());
    }

    [Fact]
    public async Task FindOne()
    {
        await CrudTests.FindOne<DocumentsController, DocumentConfiguration>(c => c.FindOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task RemoveOne()
    {
        await CrudTests.RemoveOne<DocumentsController, DocumentConfiguration>(c => c.RemoveOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task UpsertOne()
    {
        await CrudTests.UpsertOne<DocumentsController, DocumentConfiguration>(c => c.UpsertOne(new DocumentConfiguration
        {
            Id = Identifier.GenerateString()
        }));
    }
}