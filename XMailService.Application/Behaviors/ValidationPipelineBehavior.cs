using FluentValidation;
using FluentValidation.Results;
using MediatR;
using XMailService.Shared.Result;

namespace XMailService.Application.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : ResultBase
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null)
        {
            return await next();
        }

        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        IReadOnlyList<Error> errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(error.ErrorCode, error.PropertyName, error.ErrorMessage));

        TResponse result = (TResponse)Activator.CreateInstance(typeof(TResponse), [errors])!;

        return result;
    }
}
