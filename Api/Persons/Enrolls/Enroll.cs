using Domain.Persons.Enroll;
using Results;

namespace Api.Persons.Enrolls;

public static class Enroll
{
    public static Task<IResult> Handle(
        Dto dto,
        IRepository repository
    ) => dto
        .ToDomain()
        .Then(repository.Handle)
        .ToHttpResult(_ => Microsoft.AspNetCore.Http.Results.Created());
}