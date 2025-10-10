using MediatR;
using Results;

namespace Api.Persons.Create;

public class Handler(
    Domain.Persons.Create.IRepository repository
) : IRequestHandler<Request, Result<ValueTuple>>
{
    public Task<Result<ValueTuple>> Handle(
        Request request,
        CancellationToken cancellationToken
    )
    {
        var response = request
            .ToDomain()
            .Then(repository.Handle);
        
        return Task.FromResult(response);
    }
}