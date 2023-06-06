namespace Prism.ProAssistant.Domain.Accounting.Forecast;

public class ForecastWeeklyBudgetCalculator
{
    private readonly Forecast _forecast;

    public ForecastWeeklyBudgetCalculator(Forecast forecast)
    {
        _forecast = forecast;
    }

    public void Compute()
    {
        InitializeBudgetWeeks();

        foreach (var prevision in _forecast.Previsions)
        {
            switch (prevision.RecurringType)
            {
                case RecurringType.Daily:
                    ComputeBudgetDailyPrevision(prevision);
                    break;
                case RecurringType.WorkDaily:
                    ComputeBudgetWorkDailyPrevision(prevision);
                    break;
                case RecurringType.Weekly:
                    ComputeBudgetWeeklyPrevision(prevision);
                    break;
            }
        }
    }

    private void ComputeBudgetWeeklyPrevision(ForecastPrevision prevision)
    {
        var currentDay = prevision.StartDate;
        
        while (currentDay <= prevision.EndDate)
        {
            var budget = _forecast.WeeklyBudgets.Find(x => x.Monday <= currentDay && x.Monday.AddDays(7) > currentDay);
            if (budget == null)
            {
                currentDay = currentDay.AddDays(7);
                continue;
            }

            budget.Amount += (prevision.Type == ForecastPrevisionType.Income ? 1 : -1) * prevision.Amount;
            
            currentDay = currentDay.AddDays(7);
        }
    }

    private void ComputeBudgetWorkDailyPrevision(ForecastPrevision prevision)
    {
        var currentDay = prevision.StartDate;
        
        while (currentDay <= prevision.EndDate)
        {
            if (currentDay.DayOfWeek != DayOfWeek.Saturday && currentDay.DayOfWeek != DayOfWeek.Sunday)
            {
                var budget = _forecast.WeeklyBudgets.Find(x => x.Monday <= currentDay && x.Monday.AddDays(7) > currentDay);
                if (budget == null)
                {
                    currentDay = currentDay.AddDays(1);
                    continue;
                }

                budget.Amount += (prevision.Type == ForecastPrevisionType.Income ? 1 : -1) * prevision.Amount;
            }
            
            currentDay = currentDay.AddDays(1);
        }
    }

    private void ComputeBudgetDailyPrevision(ForecastPrevision prevision)
    {
        var currentDay = prevision.StartDate;
        
        while (currentDay <= prevision.EndDate)
        {
            var budget = _forecast.WeeklyBudgets.Find(x => x.Monday <= currentDay && x.Monday.AddDays(7) > currentDay);
            if (budget == null)
            {
                currentDay = currentDay.AddDays(1);
                continue;
            }

            budget.Amount += (prevision.Type == ForecastPrevisionType.Income ? 1 : -1) * prevision.Amount;
            currentDay = currentDay.AddDays(1);
        }
    }

    private void InitializeBudgetWeeks()
    {
        var startDate = new DateTime(_forecast.Year, 1, 1);
        
        _forecast.WeeklyBudgets = new List<ForecastWeeklyBudget>();
        var monday = startDate.AddDays(DayOfWeek.Monday - startDate.DayOfWeek).AddDays(-7);

        while (monday.Year <= _forecast.Year)
        {
            _forecast.WeeklyBudgets.Add(new ForecastWeeklyBudget
            {
                Monday = monday,
                Amount = 0,
                WeekOfYear = monday.DayOfYear / 7
            });
            
            monday = monday.AddDays(7);
        }
    }
}