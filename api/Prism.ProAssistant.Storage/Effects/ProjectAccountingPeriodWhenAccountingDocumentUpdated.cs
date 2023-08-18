namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain;
using Domain.Accounting.Document;
using Domain.Accounting.Document.Events;
using Domain.Accounting.Reporting;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Appointments.Events;
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

        var document = @event.ToEvent<AccountingDocumentUpdated>();
        
        var startPeriod = new DateTime(document.Date.Year, document.Date.Month, 1);
        var endPeriod = startPeriod.AddMonths(1);

        ProjectAccountingPeriodBase.Project(startPeriod, endPeriod, _queryService, _stateProvider);
    }
}