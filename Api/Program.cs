using Repositories;
using CreatePerson = Api.Persons.Create;
using GetAllPersons = Api.Persons.GetAll;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
    .AddRepositories();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapPost("/person/", CreatePerson.Delegates.Delegate);
app.MapGet("/person/", GetAllPersons.Delegates.Delegate);

app.Run();