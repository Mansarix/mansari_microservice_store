using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Mansari.Store.Users.Domain.Common;

namespace Mansari.Store.Users.Api.Interceptors;

public sealed class ExceptionHandlingInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join(" | ", ex.Errors.Select(x => x.ErrorMessage))));
        }
        catch (DomainException ex)
        {
            var statusCode = ex.ErrorCode switch
            {
                "NOT_FOUND" => StatusCode.NotFound,
                "CONFLICT" => StatusCode.AlreadyExists,
                "VALIDATION_ERROR" => StatusCode.InvalidArgument,
                _ => StatusCode.FailedPrecondition
            };

            throw new RpcException(new Status(statusCode, ex.Message));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Unexpected server error."));
        }
    }
}
