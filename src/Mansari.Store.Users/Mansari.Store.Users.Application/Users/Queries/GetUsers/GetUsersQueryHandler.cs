using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Application.Mappings;
using Mansari.Store.Users.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.SearchTerm,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var mappedItems = items.Select(x => x.ToDto()).ToList();

        return Result<PagedResult<UserDto>>.Success(
            new PagedResult<UserDto>(mappedItems, totalCount, request.PageNumber, request.PageSize));
    }
}
