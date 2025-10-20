using Api.Persons.Enrolls;
using Api.Persons.GetAlls;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddRepositories();

var app = builder.Build();
app.UseExceptionHandler("/Error");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();


app.MapPost("/person/", Enroll.Handle);
app.MapGet("/person/", GetAll.Handle);
app.Map("/Error",
        (HttpContext _) => Microsoft.AspNetCore.Http.Results.Problem(statusCode: StatusCodes.Status500InternalServerError))
    .ExcludeFromDescription();

app.Run();