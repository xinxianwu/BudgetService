using FluentAssertions;
using NSubstitute;

namespace BudgetService.Training;

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
            },
            new()
            {
                YearMonth = "202310",
                Amount = 31
            }
        });

        var budgetService = new BudgetService(_budgetRepo);

        var budget = budgetService.Query(new DateTime(2023, 11, 1), new DateTime(2023, 11, 30));

        budget.Should().Be(30);
    }

    [Test]
    public void Query_oneDay()
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

        var budget = budgetService.Query(new DateTime(2023, 11, 1), new DateTime(2023, 11, 1));

        budget.Should().Be(1);
    }

    [Test]
    public void Query_notfoundBudget()
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

        var budget = budgetService.Query(new DateTime(2023, 10, 1), new DateTime(2023, 10, 31));

        budget.Should().Be(0);
    }

    [Test]
    public void Query_partialNotFoundBudget()
    {
        _budgetRepo!.GetAll().Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202309",
                Amount = 30
            },
            new()
            {
                YearMonth = "202311",
                Amount = 60
            }
        });
        var budgetService = new BudgetService(_budgetRepo);

        var budget = budgetService.Query(new DateTime(2023, 09, 29), new DateTime(2023, 11, 1));

        budget.Should().Be(4);
    }

    [Test]
    public void Query_Corss_Years()
    {
        _budgetRepo!.GetAll().Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202312",
                Amount = 31
            },
            new()
            {
                YearMonth = "202401",
                Amount = 62
            }
        });
        var budgetService = new BudgetService(_budgetRepo);

        var budget = budgetService.Query(new DateTime(2023, 12, 29), new DateTime(2024, 01, 31));

        budget.Should().Be(65);
    }
}