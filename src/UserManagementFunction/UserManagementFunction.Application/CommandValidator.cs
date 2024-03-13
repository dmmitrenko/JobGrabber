using System.Text.RegularExpressions;
using UserManagementFunction.Infrastructure.Models;
using UserManagementFunction.Infrastructure.Settings;

namespace UserManagementFunction.Application;
public static class CommandValidator
{
    private const string SubscriptionTitleRegex = "^[a-zA-Z0-9 .-]+$";

    public static ValidationResult ValidateAddSubscription(Dictionary<string, string> parameters, AddSubscriptionCommandSettings commandParameters)
    {
        var result = new ValidationResult();

        var requiredParameters = new[] { 
            commandParameters.TitleParameter, 
            commandParameters.SpecialtyParameter,
            commandParameters.ExperienceParameter 
        };

        foreach (var paramName in requiredParameters)
        {
            if (!parameters.TryGetValue(paramName, out var value) || string.IsNullOrWhiteSpace(value))
            {
                result.Errors.Add($"Missing or invalid parameter: {paramName}");
            }
        }

        if (parameters.TryGetValue(commandParameters.ExperienceParameter, out var experience))
        {
            if (!double.TryParse(experience, out _))
            {
                result.Errors.Add("Experience must be a valid number.");
            }
        }

        if (parameters.TryGetValue(commandParameters.TitleParameter, out var titleValue))
        {
            if (!Regex.IsMatch(titleValue, SubscriptionTitleRegex))
            {
                result.Errors.Add($"{commandParameters.TitleParameter} may contain only letters and numbers.");
            }
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    public static ValidationResult ValidateDeleteSubscription(Dictionary<string, string> parameters, DeleteSubscriptionCommandSettings commandParameters)
    {
        var result = new ValidationResult();

        var requiredParameters = new[] {
            commandParameters.SubscriptionTitleParameter,
        };

        if (parameters.TryGetValue(commandParameters.SubscriptionTitleParameter, out var titleValue))
        {
            if (!Regex.IsMatch(titleValue, SubscriptionTitleRegex))
            {
                result.Errors.Add($"{commandParameters.SubscriptionTitleParameter} may contain only letters and numbers.");
            }
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}
