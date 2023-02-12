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
        // Arrange
        var crudService = new Mock<ICrudService>();
        crudService.Setup(x => x.FindMany<Contact>()).ReturnsAsync(new List<Contact>
        {
            new()
            {
                FirstName = Identifier.GenerateString(),
                LastName = Identifier.GenerateString(),
                Title = Identifier.GenerateString(),
                Id = Identifier.GenerateString()
            }
        });
        var searchService = new Mock<ISearchContactsService>();

        // Act
        var controller = new ContactController(crudService.Object, searchService.Object);
        var result = await controller.FindMany();

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        crudService.Verify(x => x.FindMany<Contact>(), Times.Once);
    }

    [Fact]
    public async Task FindOne()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        crudService.Setup(x => x.FindOne<Contact>(It.IsAny<string>())).ReturnsAsync(new Contact
        {
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Id = Identifier.GenerateString()
        });
        var searchService = new Mock<ISearchContactsService>();

        // Act
        var controller = new ContactController(crudService.Object, searchService.Object);
        var result = await controller.FindOne(Identifier.GenerateString());

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        crudService.Verify(x => x.FindOne<Contact>(It.IsAny<string>()), Times.Once);
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
        // Arrange
        var crudService = new Mock<ICrudService>();
        crudService.Setup(x => x.UpsertOne(It.IsAny<Contact>())).ReturnsAsync(new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));
        var searchService = new Mock<ISearchContactsService>();

        // Act
        var controller = new ContactController(crudService.Object, searchService.Object);
        var result = await controller.UpsertOne(new Contact
        {
            Id = Identifier.GenerateString()
        });

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        crudService.Verify(x => x.UpsertOne(It.IsAny<Contact>()), Times.Once);
    }
}