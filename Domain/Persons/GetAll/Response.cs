namespace Domain.Persons.GetAll;

public record Response(IEnumerable<Name> Names);