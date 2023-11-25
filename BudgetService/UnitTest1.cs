using FluentAssertions;
using NSubstitute;

namespace Trainning;

public class Tests
{
    private IBudgetRepo? _budgetRepo;

    [SetUp]
    public void SetUp()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Query_Monthly()
    {
        _budgetRepo!.GetAll().Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 30
            }
        });

        var budgetService = new BudgetService(_budgetRepo);

        var budget = budgetService.Query(new DateTime(2023, 11, 1), new DateTime(2023, 11, 30));

        budget.Should().Be(30);
    }
}

public interface IBudgetRepo
{
    List<Budget> GetAll();
}

public class Budget
{
    public string YearMonth { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime startTime, DateTime endTime)
    {
        var budgets = _budgetRepo.GetAll();

        return budgets.Sum(budget => budget.Amount);
    }
}