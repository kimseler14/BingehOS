using BingehOS.Modules.Automation.Application;

namespace BingehOS.UnitTests;

public class AutomationConditionEvaluatorTests
{
    private static readonly IReadOnlyDictionary<string, object?> Payload =
        new Dictionary<string, object?>
        {
            ["status"] = "Requested",
            ["currentStock"] = 4,
            ["quantity"] = 2
        };

    [Fact]
    public void Equals_Matches_String_Field()
    {
        Assert.True(AutomationConditionEvaluator.Evaluate(
            """{"field":"status","operator":"equals","value":"requested"}""",
            Payload));
    }

    [Fact]
    public void NotEquals_Rejects_Matching_Value()
    {
        Assert.False(AutomationConditionEvaluator.Evaluate(
            """{"field":"status","operator":"not-equals","value":"Requested"}""",
            Payload));
    }

    [Fact]
    public void Greater_And_Less_Compare_Numeric_Fields()
    {
        Assert.True(AutomationConditionEvaluator.Evaluate(
            """{"field":"currentStock","operator":"less","value":5}""",
            Payload));
        Assert.True(AutomationConditionEvaluator.Evaluate(
            """{"field":"quantity","operator":"greater","value":1}""",
            Payload));
    }

    [Fact]
    public void Conditions_Array_Requires_All_Conditions()
    {
        Assert.True(AutomationConditionEvaluator.Evaluate(
            """{"conditions":[{"field":"status","operator":"equals","value":"Requested"},{"field":"currentStock","operator":"less","value":5}]}""",
            Payload));
    }

    [Fact]
    public void Invalid_Or_Missing_Field_Does_Not_Match()
    {
        Assert.False(AutomationConditionEvaluator.Evaluate(
            """{"field":"missing","operator":"equals","value":"anything"}""",
            Payload));
        Assert.False(AutomationConditionEvaluator.Evaluate(
            """{"field":"currentStock","operator":"greater","value":"not-a-number"}""",
            Payload));
    }
}
