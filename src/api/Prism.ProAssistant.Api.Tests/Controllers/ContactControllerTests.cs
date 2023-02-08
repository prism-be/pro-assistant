// -----------------------------------------------------------------------
//  <copyright file = "ContactControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class ContactControllerTests
{
    [Fact]
    public async Task FindMany()
    {
        await CrudTests.FindMany<ContactController, Contact>(c => c.FindMany());
    }

    [Fact]
    public async Task FindOne()
    {
        await CrudTests.FindOne<ContactController, Contact>(c => c.FindOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task Search()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        var service = new Mock<ISearchContactsService>();
        service.Setup(x => x.Search(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Contact>());

        // Act
        var controller = new ContactController(crudService.Object, service.Object);
        var result = await controller.Search(new SearchContacts(string.Empty, string.Empty, string.Empty, string.Empty));

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        service.Verify(x => x.Search(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpsertOne()
    {
        await CrudTests.UpsertOne<ContactController, Contact>(c => c.UpsertOne(new Contact
        {
            Id = Identifier.GenerateString()
        }));
    }
}