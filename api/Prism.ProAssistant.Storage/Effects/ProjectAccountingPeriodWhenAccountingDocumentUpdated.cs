namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain;
using Domain.Accounting.Document.Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

[SideEffect(typeof(AccountingDocumentCreated))]
[SideEffect(typeof(AccountingDocumentUpdated))]
[SideEffect(typeof(AccountingDocumentDeleted))]
public class ProjectAccountingPeriodWhenAccountingDocumentUpdated
{
    private readonly ILogger<ProjectAccountingPeriodWhenAccountingDocumentUpdated> _logger;
    private readonly IQueryService _queryService;
    private readonly IStateProvider _stateProvider;

    public ProjectAccountingPeriodWhenAccountingDocumentUpdated(ILogger<ProjectAccountingPeriodWhenAccountingDocumentUpdated> logger, IQueryService queryService, IStateProvider stateProvider)
    {
        _logger = logger;
        _queryService = queryService;
        _stateProvider = stateProvider;
    }

    public async Task Handle(DomainEvent @event)
    {
        _logger.LogInformation("Projecting accounting period from change on document {AccountingDocumentId}", @event.StreamId);

        DateTime startPeriod;
        var document = @event.ToEvent<AccountingDocumentUpdated>().Document;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (document != null)
        {
            startPeriod = new DateTime(document.Date.Year, document.Date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        else
        {
            var date = @event.ToEvent<AccountingDocumentDeleted>().Date;
            startPeriod = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        
        
        var endPeriod = startPeriod.AddMonths(1);

        await ProjectAccountingPeriodBase.Project(startPeriod, endPeriod, _queryService, _stateProvider);
    }
}