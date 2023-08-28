namespace Prism.ProAssistant.Storage.Tests.Effects;

using Core;
using Domain;
using Domain.Accounting.Document;
using Domain.Accounting.Document.Events;
using Domain.Accounting.Reporting;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Storage.Effects;
using Storage.Events;

public class ProjectAccountingPeriodWhenAccountingDocumentUpdatedTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectAccountingPeriodWhenAccountingDocumentUpdated>>();
        var queryService = new Mock<IQueryService>();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<AccountingReportingPeriod>>();
        stateProvider.Setup(x => x.GetContainerAsync<AccountingReportingPeriod>()).ReturnsAsync(container.Object);

        var id = Identifier.GenerateString();

        var context = new EventContext<AccountingDocument>()
        {
            Context = new UserOrganization(),
            Event = DomainEvent.FromEvent(id, Identifier.GenerateString(), new AccountingDocumentUpdated
            {
                StreamId = id,
                Document = new AccountingDocument
                {
                    Id = id
                }
            }),
            CurrentState = new AccountingDocument
            {
                Id = id,
                Date = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            PreviousState = new AccountingDocument
            {
                Id = id,
                Date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };


        // Act
        var projectAccountingPeriod = new ProjectAccountingPeriodWhenAccountingDocumentUpdated(logger.Object, queryService.Object, stateProvider.Object);
        await projectAccountingPeriod.Handle(context);

        // Assert
        queryService.Verify(x => x.SearchAsync<AccountingDocument>(It.IsAny<Filter>(), It.IsAny<Filter>()), Times.Exactly(2));
    }
}