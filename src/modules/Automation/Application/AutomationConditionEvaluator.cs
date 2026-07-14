using System.Globalization;
using System.Text.Json;

namespace BingehOS.Modules.Automation.Application;

public static class AutomationConditionEvaluator
{
    public static bool Evaluate(
        string? conditionJson,
        IReadOnlyDictionary<string, object?> payload)
    {
        if (string.IsNullOrWhiteSpace(conditionJson))
            return true;

        using var document = JsonDocument.Parse(conditionJson);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("conditions", out var conditions))
        {
            return conditions.ValueKind == JsonValueKind.Array &&
                   conditions.EnumerateArray().All(condition => EvaluateCondition(condition, payload));
        }

        return EvaluateCondition(root, payload);
    }

    private static bool EvaluateCondition(
        JsonElement condition,
        IReadOnlyDictionary<string, object?> payload)
    {
        if (condition.ValueKind != JsonValueKind.Object ||
            !condition.TryGetProperty("field", out var fieldElement) ||
            !condition.TryGetProperty("operator", out var operatorElement) ||
            !condition.TryGetProperty("value", out var expected))
        {
            return false;
        }

        var field = fieldElement.GetString();
        var operation = operatorElement.GetString()?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(operation) ||
            !payload.TryGetValue(field, out var actual))
        {
            return false;
        }

        var actualText = Convert.ToString(actual, CultureInfo.InvariantCulture) ?? string.Empty;
        var expectedText = expected.ValueKind == JsonValueKind.String
            ? expected.GetString() ?? string.Empty
            : expected.ToString();

        return operation switch
        {
            "equals" or "eq" => string.Equals(actualText, expectedText, StringComparison.OrdinalIgnoreCase),
            "not-equals" or "not_equals" or "notequals" or "ne" =>
                !string.Equals(actualText, expectedText, StringComparison.OrdinalIgnoreCase),
            "greater" or "greater-than" or "greater_than" or "gt" =>
                TryCompareNumbers(actualText, expectedText, out var greater) && greater > 0,
            "less" or "less-than" or "less_than" or "lt" =>
                TryCompareNumbers(actualText, expectedText, out var less) && less < 0,
            _ => false
        };
    }

    private static bool TryCompareNumbers(string actual, string expected, out int result)
    {
        if (decimal.TryParse(actual, NumberStyles.Number, CultureInfo.InvariantCulture, out var actualNumber) &&
            decimal.TryParse(expected, NumberStyles.Number, CultureInfo.InvariantCulture, out var expectedNumber))
        {
            result = decimal.Compare(actualNumber, expectedNumber);
            return true;
        }

        result = 0;
        return false;
    }
}
