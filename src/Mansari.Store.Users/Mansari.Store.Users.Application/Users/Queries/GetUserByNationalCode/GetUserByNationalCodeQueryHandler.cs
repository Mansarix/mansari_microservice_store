using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Application.Mappings;
using Mansari.Store.Users.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Queries.GetUserByNationalCode;

public sealed class GetUserByNationalCodeQueryHandler : IRequestHandler<GetUserByNationalCodeQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetUserByNationalCodeQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByNationalCodeQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByNationalCodeAsync(request.NationalCode, cancellationToken: cancellationToken);

        return user is null
            ? Result<UserDto>.Failure(ErrorCodes.NotFound, "User not found.")
            : Result<UserDto>.Success(user.ToDto());
    }
}
