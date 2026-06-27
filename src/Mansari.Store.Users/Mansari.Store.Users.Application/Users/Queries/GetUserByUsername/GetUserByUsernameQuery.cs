using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Queries.GetUserByUsername;

public sealed record GetUserByUsernameQuery(string Username) : IRequest<Result<UserDto>>;
