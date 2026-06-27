using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mansari.Store.Users.Api.Contracts;
using Mansari.Store.Users.Api.Extensions;
using Mansari.Store.Users.Application.Users.Commands.CreateUser;
using Mansari.Store.Users.Application.Users.Commands.DeleteUser;
using Mansari.Store.Users.Application.Users.Commands.UpdateUser;
using Mansari.Store.Users.Application.Users.Queries.GetUserById;
using Mansari.Store.Users.Application.Users.Queries.GetUserByNationalCode;
using Mansari.Store.Users.Application.Users.Queries.GetUserByUsername;
using Mansari.Store.Users.Application.Users.Queries.GetUsers;
using Mansari.Store.Users.Domain.Enums;
using MediatR;

namespace Mansari.Store.Users.Api;

public sealed class UsersGrpcService : Users.UsersBase
{
    private readonly IMediator _mediator;

    public UsersGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<UserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new CreateUserCommand(
            request.FirstName,
            request.LastName,
            request.NationalCode,
            request.MobileNumber,
            request.Username,
            string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
            ParseBirthDate(request.BirthDate),
            MapGender(request.Gender)), context.CancellationToken);

        return result.EnsureSuccess().ToGrpcResponse();
    }

    public override async Task<UserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new UpdateUserCommand(
            ParseGuid(request.Id),
            request.FirstName,
            request.LastName,
            request.NationalCode,
            request.MobileNumber,
            request.Username,
            string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
            ParseBirthDate(request.BirthDate),
            MapGender(request.Gender),
            MapStatus(request.Status)), context.CancellationToken);

        return result.EnsureSuccess().ToGrpcResponse();
    }

    public override async Task<Empty> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new DeleteUserCommand(ParseGuid(request.Id)), context.CancellationToken);
        result.EnsureSuccess();

        return new Empty();
    }

    public override async Task<UserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(ParseGuid(request.Id)), context.CancellationToken);
        return result.EnsureSuccess().ToGrpcResponse();
    }

    public override async Task<UserResponse> GetUserByNationalCode(GetUserByNationalCodeRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetUserByNationalCodeQuery(request.NationalCode), context.CancellationToken);
        return result.EnsureSuccess().ToGrpcResponse();
    }

    public override async Task<UserResponse> GetUserByUsername(GetUserByUsernameRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetUserByUsernameQuery(request.Username), context.CancellationToken);
        return result.EnsureSuccess().ToGrpcResponse();
    }

    public override async Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
    {
        var status = request.Status == UserStatus.StatusUnspecified
            ? (Mansari.Store.Users.Domain.Enums.UserStatus?)null
            : MapStatus(request.Status);

        var result = await _mediator.Send(new GetUsersQuery(
            string.IsNullOrWhiteSpace(request.SearchTerm) ? null : request.SearchTerm,
            status,
            request.PageNumber <= 0 ? 1 : request.PageNumber,
            request.PageSize <= 0 ? 20 : request.PageSize), context.CancellationToken);

        var paged = result.EnsureSuccess();

        return new GetUsersResponse
        {
            Items = { paged.Items.Select(x => x.ToGrpcResponse()) },
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    private static Guid ParseGuid(string value)
    {
        if (!Guid.TryParse(value, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid id format."));

        return id;
    }

    private static DateOnly ParseBirthDate(string value)
    {
        if (!DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Birth date must use yyyy-MM-dd format."));

        return result;
    }

    private static Gender MapGender(Mansari.Store.Users.Api.Contracts.Gender gender)
    {
        return gender switch
        {
            Mansari.Store.Users.Api.Contracts.Gender.GenderMale => Gender.Male,
            Mansari.Store.Users.Api.Contracts.Gender.GenderFemale => Gender.Female,
            Mansari.Store.Users.Api.Contracts.Gender.GenderOther => Gender.Other,
            _ => Gender.Unspecified
        };
    }

    private static Mansari.Store.Users.Domain.Enums.UserStatus MapStatus(UserStatus status)
    {
        return status switch
        {
            UserStatus.StatusActive => Mansari.Store.Users.Domain.Enums.UserStatus.Active,
            UserStatus.StatusInactive => Mansari.Store.Users.Domain.Enums.UserStatus.Inactive,
            UserStatus.StatusSuspended => Mansari.Store.Users.Domain.Enums.UserStatus.Suspended,
            _ => Mansari.Store.Users.Domain.Enums.UserStatus.Active
        };
    }
}
