using MediatR;
using Results;
using HttpResults =  Microsoft.AspNetCore.Http.Results;

namespace Api.Persons.GetAll;

public static class Delegates
{
    public static Task<IResult> Delegate(ISender sender)
        => sender.Send(new Request())
            .Then(x => x.ToDto())
            .ToHttpResult(HttpResults.Ok);
}