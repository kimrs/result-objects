using Microsoft.Extensions.DependencyInjection;
using Repositories.Persons;

namespace Repositories;

public static class RegisterServices
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
        => services
            .AddSingleton<Repository>()
            .AddSingleton<Domain.Persons.GetAll.IRepository>(sp => sp.GetRequiredService<Repository>())
            .AddSingleton<Domain.Persons.Create.IRepository>(sp => sp.GetRequiredService<Repository>());
}