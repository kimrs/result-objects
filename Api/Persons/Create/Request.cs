using MediatR;
using Results;

namespace Api.Persons.Create;

public record Request(string Name) : IRequest<Result<ValueTuple>>;