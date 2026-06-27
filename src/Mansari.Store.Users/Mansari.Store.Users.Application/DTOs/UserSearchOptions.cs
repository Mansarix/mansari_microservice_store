using Mansari.Store.Users.Domain.Enums;

namespace Mansari.Store.Users.Application.DTOs;

public sealed class UserSearchOptions
{
    public string? SearchTerm { get; init; }
    public UserStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
