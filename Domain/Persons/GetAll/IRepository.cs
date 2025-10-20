using Results;

namespace Domain.Persons.GetAll;

public interface IRepository
{
    Task<Result<Response>> Handle();
}