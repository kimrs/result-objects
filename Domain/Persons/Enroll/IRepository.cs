using Results;

namespace Domain.Persons.Enroll;

public interface IRepository
{
    Task<Result<ValueTuple>> Handle(Request request);
}