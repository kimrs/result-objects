using Domain;
using Results;

namespace Persistence.Persons;

public class Repository
    : Domain.Persons.GetAll.IRepository,
    Domain.Persons.Enroll.IRepository
{
    private const int Max = 2;
    private readonly List<Person> _persons = new(Max);

    public Task<Result<ValueTuple>> Handle(
        Domain.Persons.Enroll.Request request)
    {
        if (_persons.Count > Max)
        {
            throw new FullDbException();
        }

        _persons.Add(new Person($"{request.Name}"));

        return new ValueTuple().ToTask();
    }

    public Task<Result<Domain.Persons.GetAll.Response>> Handle()
    {
        var names = _persons
            .Select(x => Name.Create(x.Name))
            .OfType<Completed<Name>>()
            .Select(x => x.Value);

        return new Domain.Persons.GetAll.Response(names).ToTask();
    }
}