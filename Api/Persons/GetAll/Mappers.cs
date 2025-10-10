using Domain.Persons.GetAll;
using Results;

namespace Api.Persons.GetAll;

public static class Mappers
{
    public static Result<string[]> ToDto(this Response response)
        => response.Names
            .Select(x => $"{x}")
            .ToArray();
}