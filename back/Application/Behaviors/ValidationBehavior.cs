using FluentValidation;
using MediatR;
using Domain.Constants;
using Domain.Exceptions;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count != 0)
        {
            var domainErrors = failures.Select(f => new ValidationError(f.PropertyName, f.ErrorCode));
            throw new ValidationException(domainErrors);
        }

        return await next();
    }
}