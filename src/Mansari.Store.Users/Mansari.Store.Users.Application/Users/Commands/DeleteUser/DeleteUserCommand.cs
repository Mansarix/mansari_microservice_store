using Mansari.Store.Users.Application.Common;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : IRequest<Result>;
