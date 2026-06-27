using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Application.Mappings;
using Mansari.Store.Users.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Queries.GetUserByUsername;

public sealed class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetUserByUsernameQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByUsernameAsync(request.Username, cancellationToken: cancellationToken);

        return user is null
            ? Result<UserDto>.Failure(ErrorCodes.NotFound, "User not found.")
            : Result<UserDto>.Success(user.ToDto());
    }
}
