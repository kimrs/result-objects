using Domain.Persons.GetAll;
using Results;

namespace Api.Persons.GetAlls;

public static class GetAll
{
    public static Task<IResult> Handle(IRepository repository)
        => repository.Handle()
            .Then(x => x.ToDto())
            .ToHttpResult(Microsoft.AspNetCore.Http.Results.Ok);
}