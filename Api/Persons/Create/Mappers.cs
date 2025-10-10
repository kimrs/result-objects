using Domain;
using Results;
using Domains = Domain.Persons.Create;


namespace Api.Persons.Create;

public static class Mappers
{
    public static Result<Domains.Request> ToDomain(this Request request)
        => Name.Create(request.Name)
            .Then(x => new Domains.Request {Name = x});
    
    public static Result<Request> ToRequest(this Dto dto)
        => new Request(dto.Name);
}