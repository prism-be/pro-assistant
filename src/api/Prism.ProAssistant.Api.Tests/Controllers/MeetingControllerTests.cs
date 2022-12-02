// -----------------------------------------------------------------------
//  <copyright file = "MeetingControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers
{
    public class MeetingControllerTests
    {

        [Fact]
        public async Task FindOne()
        {
            await CrudTests.FindOne<MeetingController, Meeting>(c => c.FindOne(Identifier.GenerateString()));
        }

        [Fact]
        public async Task Search()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<SearchMeetings>(), CancellationToken.None))
                .ReturnsAsync(new List<Meeting>());

            // Act
            var controller = new MeetingController(mediator.Object);
            var result = await controller.Search(new SearchMeetings(DateTime.Today, DateTime.Today.AddDays(-7), null));

            // Assert
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            mediator.Verify(x => x.Send(It.IsAny<SearchMeetings>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task UpsertOne()
        {
            await CrudTests.UpsertOne<MeetingController, Meeting>(c => c.UpsertOne(new Meeting
            {
                Id = Identifier.GenerateString(),
                PatientId = Identifier.GenerateString()
            }));
        }

        [Fact]
        public async Task UpsertOne_NoPatient()
        {
            await CrudTests.UpsertOne<MeetingController, Meeting>(c => c.UpsertOne(new Meeting
                {
                    Id = Identifier.GenerateString(),
                    PatientId = string.Empty
                }),
                m =>
                {
                    m.Setup(x => x.Send(It.IsAny<UpsertOne<Patient>>(), CancellationToken.None)).ReturnsAsync(new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));
                });
        }
    }
}