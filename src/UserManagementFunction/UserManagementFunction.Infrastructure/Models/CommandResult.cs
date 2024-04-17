using UserManagementFunction.Domain.Enums;

namespace UserManagementFunction.Infrastructure.Models;
public record CommandResult(bool Success, Commands CommandType, object? Response = null, IEnumerable<string>? Errors = default);