using Domain.Persons.GetAll;
using Results;

namespace Api.Persons.GetAlls;

public static class Map
{
    public static Result<Dto> ToDto(this Response domain)
        => new Dto(Names: domain.Names.Select(x => $"{x}").ToArray());
}