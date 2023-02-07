// -----------------------------------------------------------------------
//  <copyright file = "AppointmentControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class AppointmentControllerTests
{

    [Fact]
    public async Task FindOne()
    {
        await CrudTests.FindOne<AppointmentController, Appointment>(c => c.FindOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task Search()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<SearchAppointments>(), CancellationToken.None))
            .ReturnsAsync(new List<Appointment>());

        // Act
        var controller = new AppointmentController(mediator.Object);
        var result = await controller.Search(new SearchAppointments(DateTime.Today, DateTime.Today.AddDays(-7), null));

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<SearchAppointments>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Search_Must_Filter()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<SearchAppointments>(), CancellationToken.None))
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
                },
                new()
                {
                    Id = Identifier.GenerateString(),
                    State = (int)AppointmentState.Canceled,
                    FirstName = Identifier.GenerateString(),
                    LastName = Identifier.GenerateString(),
                    Title = Identifier.GenerateString()
                }
            });

        // Act
        var controller = new AppointmentController(mediator.Object);
        var result = await controller.Search(new SearchAppointments(DateTime.Today, DateTime.Today.AddDays(-7), null));

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<SearchAppointments>(), CancellationToken.None), Times.Once);
        var items = ((OkObjectResult)result.Result!).Value as List<Appointment>;
        items!.Count.Should().Be(3);
    }

    [Fact]
    public async Task UpsertOne()
    {
        await CrudTests.UpsertOne<AppointmentController, Appointment>(c => c.UpsertOne(new Appointment
        {
            Id = Identifier.GenerateString(),
            ContactId = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        }));
    }

    [Fact]
    public async Task UpsertOne_NoContact()
    {
        await CrudTests.UpsertOne<AppointmentController, Appointment>(c => c.UpsertOne(new Appointment
            {
                Id = Identifier.GenerateString(),
                ContactId = string.Empty,
                FirstName = Identifier.GenerateString(),
                LastName = Identifier.GenerateString(),
                Title = Identifier.GenerateString()
            }),
            m =>
            {
                m.Setup(x => x.Send(It.IsAny<UpsertOne<Contact>>(), CancellationToken.None)).ReturnsAsync(new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));
            });
    }
}