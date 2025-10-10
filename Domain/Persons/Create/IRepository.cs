using Results;

namespace Domain.Persons.Create;

public interface IRepository
{
    Result<ValueTuple> Handle(Request request);
}