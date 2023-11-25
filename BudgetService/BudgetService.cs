using System.Globalization;

namespace BudgetService.Training;

public class BudgetService
{
    private readonly Dictionary<int, int> _monthDaysDictionary = new()
    {
        {1, 31},
        {2, 28},
        {3, 31},
        {4, 30},
        {5, 31},
        {6, 30},
        {7, 31},
        {8, 31},
        {9, 30},
        {10, 31},
        {11, 30},
        {12, 31}
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
            var dateTime = DateTime.ParseExact(budget.YearMonth, new[] { "yyyyMM" }, CultureInfo.CurrentCulture);

            return budget.Amount / _monthDaysDictionary[dateTime.Month] * DecideIntervalDays(startTime, endTime, interval, dateTime);
        });
    }

    private int DecideIntervalDays(DateTime startTime, DateTime endTime, TimeSpan interval, DateTime dateTime)
    {
        var intervalDays = interval.Days + 1;
        if (dateTime.Month == endTime.Month)
        {
            intervalDays = endTime.Day;
        }
        else if (dateTime.Month == startTime.Month)
        {
            intervalDays = _monthDaysDictionary[startTime.Month] - startTime.Day + 1;
        }
        else
        {
            intervalDays = _monthDaysDictionary[dateTime.Month];
        }

        return intervalDays;
    }
}