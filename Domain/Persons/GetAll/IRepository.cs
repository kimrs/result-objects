using Results;

namespace Domain.Persons.GetAll;

public interface IRepository
{
    Result<Response> Handle();
}