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
        await CrudTests.FindMany<DocumentsController, Document>(c => c.FindMany());
    }

    [Fact]
    public async Task FindOne()
    {
        await CrudTests.FindOne<DocumentsController, Document>(c => c.FindOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task RemoveOne()
    {
        await CrudTests.RemoveOne<DocumentsController, Document>(c => c.RemoveOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task UpsertOne()
    {
        await CrudTests.UpsertOne<DocumentsController, Document>(c => c.UpsertOne(new Document
        {
            Id = Identifier.GenerateString()
        }));
    }
}