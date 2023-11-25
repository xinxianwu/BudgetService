using System.Globalization;

namespace BudgetService.Training;

public class BudgetService
{
    private readonly Dictionary<int, int> _monthdays = new()
    {
        {
            11, 30
        },
        { 10, 31 },
        { 9, 30 }
    };

    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime startTime, DateTime endTime)
    {
        var budgets = _budgetRepo.GetAll();

        var interval = endTime - startTime;
        return budgets.Where(budget =>
        {
            // var dateTime = DateTime.ParseExact(budget.YearMonth, new[] { "yyyyMM" }, CultureInfo.CurrentCulture);
            var yearMonth = int.Parse(budget.YearMonth);
            var startYearMonth = int.Parse(startTime.ToString("yyyyMM"));
            var endTImeYearMonth = int.Parse(endTime.ToString("yyyyMM"));
            return yearMonth >= startYearMonth && yearMonth <= endTImeYearMonth;
        }).Sum(budget =>
        {
            var intervalDays = interval.Days + 1;
            var dateTime = DateTime.ParseExact(budget.YearMonth, new[] { "yyyyMM" }, CultureInfo.CurrentCulture);
            if (dateTime.Month == endTime.Month)
            {
                intervalDays = endTime.Day;
            }
            else if (dateTime.Month == startTime.Month)
            {
                intervalDays = _monthdays[startTime.Month] - startTime.Day + 1;
            }
            else
            {
                intervalDays = _monthdays[dateTime.Month];
            }

            return budget.Amount / _monthdays[dateTime.Month] * intervalDays;
        });
    }
}