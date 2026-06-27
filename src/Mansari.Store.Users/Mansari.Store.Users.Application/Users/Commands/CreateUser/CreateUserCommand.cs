using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Domain.Enums;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string NationalCode,
    string MobileNumber,
    string Username,
    string? Email,
    DateOnly BirthDate,
    Gender Gender) : IRequest<Result<UserDto>>;
