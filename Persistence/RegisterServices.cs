using Domain.Persons.Enroll;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Persons;

namespace Persistence;

public static class RegisterServices
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
        => services
            .AddSingleton<Repository>()
            .AddSingleton<Domain.Persons.GetAll.IRepository>(sp => sp.GetRequiredService<Repository>())
            .AddSingleton<IRepository>(sp => sp.GetRequiredService<Repository>());
}