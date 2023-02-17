// -----------------------------------------------------------------------
//  <copyright file = "AppointmentControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class AppointmentControllerTests
{

    [Fact]
    public async Task FindOne()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        crudService.Setup(x => x.FindOne<Appointment>(It.IsAny<string>())).ReturnsAsync(new Appointment
        {
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Id = Identifier.GenerateString()
        });
        var searchService = new Mock<ISearchAppointmentsService>();

        // Act
        var controller = new AppointmentController(crudService.Object, searchService.Object);
        var result = await controller.FindOne(Identifier.GenerateString());

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        crudService.Verify(x => x.FindOne<Appointment>(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Opened()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        var searchService = new Mock<ISearchAppointmentsService>();
        crudService.Setup(x => x.FindMany(It.IsAny<FilterDefinition<Appointment>>()))
            .ReturnsAsync(new List<Appointment>());

        // Act
        var controller = new AppointmentController(crudService.Object, searchService.Object);
        var result = await controller.Opened();

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        crudService.Verify(x => x.FindMany(It.IsAny<FilterDefinition<Appointment>>()), Times.Once);
    }

    [Fact]
    public async Task Search()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        var searchService = new Mock<ISearchAppointmentsService>();
        searchService.Setup(x => x.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Appointment>());

        // Act
        var controller = new AppointmentController(crudService.Object, searchService.Object);
        var result = await controller.Search(new SearchAppointments(DateTime.Today, DateTime.Today.AddDays(-7), null));

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        searchService.Verify(x => x.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Search_Must_Filter()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        var searchService = new Mock<ISearchAppointmentsService>();
        searchService.Setup(x => x.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Appointment>
            {
                new()
                {
                    Id = Identifier.GenerateString(),
                    State = (int)AppointmentState.Created,
                    FirstName = Identifier.GenerateString(),
                    LastName = Identifier.GenerateString(),
                    Title = Identifier.GenerateString()
                },
                new()
                {
                    Id = Identifier.GenerateString(),
                    State = (int)AppointmentState.Confirmed,
                    FirstName = Identifier.GenerateString(),
                    LastName = Identifier.GenerateString(),
                    Title = Identifier.GenerateString()
                },
                new()
                {
                    Id = Identifier.GenerateString(),
                    State = (int)AppointmentState.Done,
                    FirstName = Identifier.GenerateString(),
                    LastName = Identifier.GenerateString(),
                    Title = Identifier.GenerateString()
                }
            });

        // Act
        var controller = new AppointmentController(crudService.Object, searchService.Object);
        var result = await controller.Search(new SearchAppointments(DateTime.Today, DateTime.Today.AddDays(-7), null));

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        searchService.Verify(x => x.Search(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
        var items = ((OkObjectResult)result.Result!).Value as List<Appointment>;
        items!.Count.Should().Be(3);
    }

    [Fact]
    public async Task UpsertOne()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        crudService.Setup(x => x.UpsertOne(It.IsAny<Appointment>())).ReturnsAsync(new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));
        var searchService = new Mock<ISearchAppointmentsService>();

        // Act
        var controller = new AppointmentController(crudService.Object, searchService.Object);
        var result = await controller.UpsertOne(new Appointment
        {
            Id = Identifier.GenerateString(),
            ContactId = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        crudService.Verify(x => x.UpsertOne(It.IsAny<Appointment>()), Times.Once);
    }

    [Fact]
    public async Task UpsertOne_NoContact()
    {
        // Arrange
        var crudService = new Mock<ICrudService>();
        crudService.Setup(x => x.UpsertOne(It.IsAny<Appointment>())).ReturnsAsync(new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));
        crudService.Setup(x => x.UpsertOne(It.IsAny<Contact>())).ReturnsAsync(new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));
        var searchService = new Mock<ISearchAppointmentsService>();

        // Act
        var controller = new AppointmentController(crudService.Object, searchService.Object);
        var result = await controller.UpsertOne(new Appointment
        {
            Id = Identifier.GenerateString(),
            ContactId = string.Empty,
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        crudService.Verify(x => x.UpsertOne(It.IsAny<Appointment>()), Times.Once);
    }
}