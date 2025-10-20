using Domain;
using Results;

namespace Api.Persons.Enrolls;

public static class Map
{
    public static Result<Domain.Persons.Enroll.Request> ToDomain(this Dto dto)
        => Name.Create(dto.Name)
            .Then(x => new Domain.Persons.Enroll.Request { Name = x });
}