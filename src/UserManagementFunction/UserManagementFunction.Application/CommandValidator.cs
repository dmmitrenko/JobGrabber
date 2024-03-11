using UserManagementFunction.Infrastructure.Models;

namespace UserManagementFunction.Application;
public static class CommandValidator
{
    public static ValidationResult ValidateAddSubscription(Dictionary<string, string> parameters)
    {
        var result = new ValidationResult();
        var requiredParameters = new[] { "title", "ctg", "exp" };

        foreach (var paramName in requiredParameters)
        {
            if (!parameters.TryGetValue(paramName, out var value) || string.IsNullOrWhiteSpace(value))
            {
                result.Errors.Add($"Missing or invalid parameter: {paramName}");
            }
        }

        if (!double.TryParse(parameters["Experience"], out _))
        {
            result.Errors.Add("Experience must be a valid number.");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}
