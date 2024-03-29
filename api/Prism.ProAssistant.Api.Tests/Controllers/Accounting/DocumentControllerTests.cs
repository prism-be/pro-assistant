﻿namespace Prism.ProAssistant.Api.Tests.Controllers.Accounting;

using Api.Controllers.Data.Accounting;
using Core;
using Domain.Accounting.Document;
using Domain.Accounting.Document.Events;
using FluentAssertions;
using Infrastructure.Providers;
using Moq;
using Storage;
using Storage.Events;

public class DocumentControllerTests
{
    [Fact]
    public async Task Delete_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var document = new AccountingDocument
        {
            Id = Identifier.GenerateString(),
            Amount = 100,
            Date = DateTime.Now,
            Title = Identifier.GenerateString()
        };

        // Act
        var controller = new DocumentController(eventStore.Object, queryService.Object);
        await controller.Delete(document);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<AccountingDocument>(It.IsAny<AccountingDocumentDeleted>()), Times.Once);
    }

    [Fact]
    public async Task GetNextNumber_OkFound()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        queryService.Setup(x => x.MaxAsync(It.IsAny<Func<AccountingDocument, int?>>(), It.IsAny<Filter[]>())).ReturnsAsync(2);

        // Act
        var controller = new DocumentController(eventStore.Object, queryService.Object);
        var result = await controller.GetNextNumber(2023);

        // Assert
        result.Number.Should().Be(3);
    }


    [Fact]
    public async Task GetNextNumber_OkNotFound()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new DocumentController(eventStore.Object, queryService.Object);
        var result = await controller.GetNextNumber(2023);

        // Assert
        result.Number.Should().Be(1);
    }

    [Fact]
    public async Task Insert_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var document = new AccountingDocument
        {
            Id = Identifier.GenerateString(),
            Amount = 100,
            Date = DateTime.Now,
            Title = Identifier.GenerateString()
        };

        // Act
        var controller = new DocumentController(eventStore.Object, queryService.Object);
        await controller.Insert(document);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<AccountingDocument>(It.IsAny<AccountingDocumentCreated>()), Times.Once);
    }

    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var eventStore = new Mock<IEventStore>();
        var queryService = new Mock<IQueryService>();

        // Act
        var controller = new DocumentController(eventStore.Object, queryService.Object);
        await controller.List();

        // Assert
        queryService.Verify(x => x.ListAsync<AccountingDocument>(), Times.Once);
    }

    [Fact]
    public async Task ListPeriod_Ok()
    {
        // Arrange
        var eventStore = new Mock<IEventStore>();
        var queryService = new Mock<IQueryService>();
        var period = DateTime.Now.Year;

        // Act
        var controller = new DocumentController(eventStore.Object, queryService.Object);
        await controller.List(period);

        // Assert
        queryService.Verify(x => x.SearchAsync<AccountingDocument>(It.IsAny<Filter[]>()), Times.Once);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var document = new AccountingDocument
        {
            Id = Identifier.GenerateString(),
            Amount = 100,
            Date = DateTime.Now,
            Title = Identifier.GenerateString()
        };

        // Act
        var controller = new DocumentController(eventStore.Object, queryService.Object);
        await controller.Update(document);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<AccountingDocument>(It.IsAny<AccountingDocumentUpdated>()), Times.Once);
    }
}