// -----------------------------------------------------------------------
//  <copyright file = "UsertManyServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Business.Tests.Services;

public class UsertManyServiceTests
{
    [Fact]
    public async Task Upsert_Ok()
    {
        // Arrange
        var upsertOne = new Mock<IUpsertOneService>();

        // Act
        var items = new List<Contact>(new[]
        {
            new Contact
            {
                Id = Identifier.GenerateString()
            },
            new Contact
            {
                Id = Identifier.GenerateString()
            },
            new Contact
            {
                Id = Identifier.GenerateString()
            },
            new Contact
            {
                Id = Identifier.GenerateString()
            }
        });

        var service = new UpsertManyService(upsertOne.Object);
        await service.Upsert(items);

        // Assert
        upsertOne.Verify(x => x.Upsert(It.IsAny<Contact>()), Times.Exactly(4));
    }
}