namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain.Accounting.Document;
using Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

[SideEffect(typeof(AccountingDocument))]
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

    public async Task Handle(EventContext<AccountingDocument> context)
    {
        _logger.LogInformation("Projecting accounting period from change on document {AccountingDocumentId}", context.Event.StreamId);

        if (context.CurrentState != null)
        {
            await Project(context.CurrentState);
        }

        if (context.PreviousState != null)
        {
            await Project(context.PreviousState);
        }
    }

    private async Task Project(AccountingDocument document)
    {
        var startPeriod = new DateTime(document.Date.Year, document.Date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endPeriod = startPeriod.AddMonths(1);

        await ProjectAccountingPeriodBase.Project(startPeriod, endPeriod, _queryService, _stateProvider);
    }
}