using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Domain.Enums;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string NationalCode,
    string MobileNumber,
    string Username,
    string? Email,
    DateOnly BirthDate,
    Gender Gender,
    UserStatus Status) : IRequest<Result<UserDto>>;
