using Mansari.Store.Users.Domain.Enums;

namespace Mansari.Store.Users.Application.DTOs;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string NationalCode { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? Email { get; init; }
    public DateOnly BirthDate { get; init; }
    public Gender Gender { get; init; }
    public UserStatus Status { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
    public string FullName { get; init; } = string.Empty;
}
