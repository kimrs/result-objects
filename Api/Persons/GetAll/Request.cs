using Domain.Persons.GetAll;
using MediatR;
using Results;

namespace Api.Persons.GetAll;

public class Request : IRequest<Result<Response>>;