// -----------------------------------------------------------------------
//  <copyright file = "PatientControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers
{
    public class PatientControllerTests
    {
        [Fact]
        public async Task FindMany()
        {
            await CrudTests.FindMany<PatientController, Patient>(c => c.FindMany());
        }

        [Fact]
        public async Task FindOne()
        {
            await CrudTests.FindOne<PatientController, Patient>(c => c.FindOne(Identifier.GenerateString()));
        }

        [Fact]
        public async Task Search()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<SearchPatients>(), CancellationToken.None))
                .ReturnsAsync(new List<Patient>());

            // Act
            var controller = new PatientController(mediator.Object);
            var result = await controller.Search(new SearchPatients(string.Empty, string.Empty, string.Empty, string.Empty));

            // Assert
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            mediator.Verify(x => x.Send(It.IsAny<SearchPatients>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task UpsertOne()
        {
            await CrudTests.UpsertOne<PatientController, Patient>(c => c.UpsertOne(new Patient
            {
                Id = Identifier.GenerateString()
            }));
        }
    }
}