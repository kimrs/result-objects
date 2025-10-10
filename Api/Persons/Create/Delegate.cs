using MediatR;
using Results;
using HttpResults =  Microsoft.AspNetCore.Http.Results;

namespace Api.Persons.Create;

public static class Delegates
{
    public static Task<IResult> Delegate(Dto dto, ISender sender)
        => dto
            .ToRequest()
            .Then(x => sender.Send(x))
            .ToHttpResult(_ => HttpResults.Created());
}