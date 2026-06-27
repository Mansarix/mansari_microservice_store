using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Domain.Enums;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
    string? SearchTerm,
    UserStatus? Status,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<UserDto>>>;
