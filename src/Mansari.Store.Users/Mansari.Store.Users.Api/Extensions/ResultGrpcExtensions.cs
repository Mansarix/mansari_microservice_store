using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;

namespace Mansari.Store.Users.Api.Extensions;

public static class ResultGrpcExtensions
{
    public static T EnsureSuccess<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return result.Value;

        throw result.ToRpcException();
    }

    public static void EnsureSuccess(this Result result)
    {
        if (result.IsSuccess)
            return;

        throw result.ToRpcException();
    }

    public static RpcException ToRpcException(this Result result)
    {
        return new RpcException(new Status(MapStatusCode(result.ErrorCode), result.ErrorMessage));
    }

    private static StatusCode MapStatusCode(string errorCode)
    {
        return errorCode switch
        {
            ErrorCodes.NotFound => StatusCode.NotFound,
            ErrorCodes.Conflict => StatusCode.AlreadyExists,
            ErrorCodes.Validation => StatusCode.InvalidArgument,
            ErrorCodes.Domain => StatusCode.FailedPrecondition,
            _ => StatusCode.Unknown
        };
    }

    public static UserResponse ToGrpcResponse(this UserDto dto)
    {
        return new UserResponse
        {
            Id = dto.Id.ToString(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FullName = dto.FullName,
            NationalCode = dto.NationalCode,
            MobileNumber = dto.MobileNumber,
            Username = dto.Username,
            Email = dto.Email ?? string.Empty,
            BirthDate = dto.BirthDate.ToString("yyyy-MM-dd"),
            Gender = MapGender(dto.Gender),
            Status = MapStatus(dto.Status),
            IsDeleted = dto.IsDeleted,
            CreatedAtUtc = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.CreatedAtUtc, DateTimeKind.Utc)),
            UpdatedAtUtc = dto.UpdatedAtUtc.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(dto.UpdatedAtUtc.Value, DateTimeKind.Utc)) : null,
            DeletedAtUtc = dto.DeletedAtUtc.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(dto.DeletedAtUtc.Value, DateTimeKind.Utc)) : null
        };
    }

    private static Mansari.Store.Users.Api.Contracts.Gender MapGender(Mansari.Store.Users.Domain.Enums.Gender gender)
    {
        return gender switch
        {
            Mansari.Store.Users.Domain.Enums.Gender.Male => Mansari.Store.Users.Api.Contracts.Gender.GenderMale,
            Mansari.Store.Users.Domain.Enums.Gender.Female => Mansari.Store.Users.Api.Contracts.Gender.GenderFemale,
            Mansari.Store.Users.Domain.Enums.Gender.Other => Mansari.Store.Users.Api.Contracts.Gender.GenderOther,
            _ => Mansari.Store.Users.Api.Contracts.Gender.GenderUnspecified
        };
    }

    private static UserStatus MapStatus(Mansari.Store.Users.Domain.Enums.UserStatus status)
    {
        return status switch
        {
            Mansari.Store.Users.Domain.Enums.UserStatus.Active => UserStatus.StatusActive,
            Mansari.Store.Users.Domain.Enums.UserStatus.Inactive => UserStatus.StatusInactive,
            Mansari.Store.Users.Domain.Enums.UserStatus.Suspended => UserStatus.StatusSuspended,
            _ => UserStatus.StatusUnspecified
        };
    }
}
