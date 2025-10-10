using Domain.Persons.GetAll;
using MediatR;
using Results;

namespace Api.Persons.GetAll;

public class Handler(
    IRepository repository
) : IRequestHandler<GetAll.Request, Result<Response>>
{
    public Task<Result<Response>> Handle(GetAll.Request request, CancellationToken cancellationToken)
    {
        var response = repository.Handle();

        return Task.FromResult(response);
    }
}