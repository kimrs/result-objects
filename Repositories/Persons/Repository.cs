using Domain;
using GetAll = Domain.Persons.GetAll;
using Create = Domain.Persons.Create;
using Results;

namespace Repositories.Persons;

public class Repository
    : Domain.Persons.GetAll.IRepository, Create.IRepository
{
    private const int Max = 2;
    private readonly List<Person> _persons = new(Max);

    public Result<ValueTuple> Handle(Create.Request request)
    {
        if (_persons.Count > Max)
        {
            return new FullDbException();
        }
        _persons.Add(new Person($"{request.Name}"));
        return new ValueTuple();
    }

    public Result<Domain.Persons.GetAll.Response> Handle()
    {
        var d = _persons
            .Select(x => Name.Create(x.Name))
            .OfType<Completed<Name>>()
            .Select(x => x.Value);

        return new Domain.Persons.GetAll.Response(d);
    }
}