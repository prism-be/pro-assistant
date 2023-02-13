// -----------------------------------------------------------------------
//  <copyright file = "AdminControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class AdminControllerTests
{
    [Fact]
    public async Task Rebuild_Ok()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        var propertyUpdatePublisher = new Mock<IPropertyUpdatePublisher>();

        crudService.Setup(x => x.FindMany<Contact>()).ReturnsAsync(new List<Contact>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                BirthDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                PhoneNumber = "1234567890"
            },
            new()
            {
                Id = Identifier.GenerateString(),
                BirthDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                PhoneNumber = "1234567890"
            }
        });
        
        crudService.Setup(x => x.FindMany<Tariff>()).ReturnsAsync(new List<Tariff>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                BackgroundColor = "#000000",
                Name = Identifier.GenerateString()
            },
            new()
            {
                Id = Identifier.GenerateString(),
                BackgroundColor = "#000000",
                Name = Identifier.GenerateString()
            }
        });

        // Act
        var adminController = new AdminController(crudService.Object, propertyUpdatePublisher.Object);
        await adminController.Rebuild();

        // Assert
        propertyUpdatePublisher.Verify(x => x.Publish(It.Is<PropertyUpdated>(p => p.ItemType == nameof(Contact))), Times.Exactly(4));
        propertyUpdatePublisher.Verify(x => x.Publish(It.Is<PropertyUpdated>(p => p.ItemType == nameof(Tariff))), Times.Exactly(2));
    }
}