using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Queries.GetUserByNationalCode;

public sealed record GetUserByNationalCodeQuery(string NationalCode) : IRequest<Result<UserDto>>;
