using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
