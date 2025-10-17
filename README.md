# Coding Guidelines
### Follow Hexagonal Architecture
The underlying architecture chosen for this project is Hexagonal Architecture (also known as Ports and Adapters).
This provides a clear separation of business logic and implementation details without introducing unecessary fluff.
Also it will give us a clear separation between different integrations. The app is divided into:
- Mt.Domain - Business logic
- Mt.Api - Entrypoint for migration
- Mt.UniMicro - Integration against UniMicro

### Favour vertical slices!

#### _Don't_:
- Persistence.csproj
  - Configurations
    - PersonConfiguration.cs
  - Repositories
    - PersonRepository.cs
  - ViewModels
    - PersonViewModel.cs

#### Instead, _do_:
- Person
  - Configuration.cs
  - Repository.cs
  - ViewModel.cs

This makes it easier to find related code because we don't have to jump between different folders.

### Turn on `Nullable` for new projects.
```xml
<Nullable>enable</Nullable>
```
We wan't to limit the use of `null` in our models because it makes our code ambigous.
`null` is often used to indicate that a value is missing, but it can also be used to indicate that something went wrong.
Not even Microsoft manages to be consistent with their use of `null`.
```csharp
_ = new NameValueCollection()["nonexsistent"]; // returns null
_ = new Dictionary<string, string>()["nonexsistent"]; // throws Exception
```

### Turn on `TreatWarningsAsErrors` for new projects
```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```
We want to ensure that our code is safe. This is a low hanging fruit that helps us.

### Turn on `ImplicitUsings` for new projects
```xml
<ImplicitUsings>enable</ImplicitUsings>
```
This will make our code less verbose

### Do not use unecessary prefixes/postfixes for your type names
A common convention is to prefix the type name with parts of its namespace.
We consider this unecessary because the namespace already provides this context.
```csharp
// Instead of
namespace Mt.Domain.Persons.Create;
class CreatePersonCommand();

// We write
namespace Mt.Domain.Persons.Create;
class Command();
```

A common argument against is that it makes searching for types harder.
We consider this a non-issue, searching for agregate name, e.g. 'Person', should
find all related types in all projects because we use vertical slices.
We consider shorter names to be more readable, especially when looking at the file hierarchy

E.g. Instead of:
- Domain
  - Persons
    - Create
      - CreatePersonCommmand.cs
      - CreatePersonCommmandHandler.cs
      - CreatePersonMapper.cs
    
We have:
- Domain
    - Persons
        - Create
            - Commmand.cs
            - CommmandHandler.cs
            - Mapper.cs

### Favour the whole namespace over `aliases` when types collide
```csharp
// Instead of
using DomainResonse = Mt.Domain.Persons.Create.Response;

public Response ToDto(
    this DomainResponse response
)

// We write
public Response ToDto(
    this Mt.Domain.Persons.Create.Response response
);
```
We are a bit divided on this one, and will revisit it in two months.
The argument for using the full namespace Instead of an alias is that
we are not likely to to be consistent in naming our aliases. Which might
cause confusion. Hopefully, this rule will not make our code too verbose.

### Mapping
#### Map manually, do not use AutoMapper or similar libraries.
Mapping, though tedious, is an operation that is too simple to warrant the use of a library.
Also, we should not be afraid of large DTOs, AI writes them in seconds.

#### Use extension methods for mapping
Use the naming convention `To[TargetRole]` for mapping methods.
By target role, we mean the context that the target type is used in. 
When mapping a `Person` domain object to a dto, dto is the target role. 

```csharp
var dto = person.ToDto();
var person = dto.ToDomain();
```

Whe use static methods for mapping because they should not depend on instance state.
Also, the objects being mapped should not be aware of other types the could be mapped to/from.
We use extension methods becuase the team has decided that they prefer the syntax.
```csharp
// Instead of
var dto = PersonMapper.ToDto(person);
// We write
var dto = person.ToDto();
```

#### Use the `required` keyword for properties in classes
A common argument for using a third party mapping library is that it can ensure that all properties are mapped.
By using the `required` keyword, we get a compilation error if we forget to map a property.
```csharp
public class Dto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class Person
{
    public required Name FirstName { get; set; }
    public required Name LastName { get; set; }
}
```

TODO: Check out <EnforceCodeStyleInBuild> to catch missuse of Async

## Mt.Domain
This project contains the business logic of the application.
This project will have the most restrictive guidelines.

### No dependencies to external systems
We allow dependencies to other "pure" projects, e.g. Mt.Shared.
But no project that interact with other systems like `Mt.Api`, `Mt.Persistence`, `Mt.Unimicro`
or NuGet packages that are not maintained by us. 
Keeping `Mt.Domain` pure ensures that we can change external systems without affecting our business logic.

### Do not use `null`
In general, we want to limit the use of `null`. In `Mt.Domain`, we do not allow `null` at all.
For implications of using `null`, see [above](#turn-on-nullable-for-new-projects).

If you find yourself wanting to use `null`, reconsider why your design requires it. Ask yourself:
- Why is the value missing?
- Is the value truly optional?
- Is it because the value will only be present when an enum has a certain state?
- Would another type be better suited?

Consider the case where a person has a `CustomerId` and that value is only present if the person is a customer.
```csharp
// Instead of
class Person
{
    public required Name Name { get; }
    public required CustomerId? CustomerId { get; }
}

// We write
class Person
{
    public required Name Name { get; }
    public class Customer : Person
    {
        public required CustomerId CustomerId { get; }
    }
}
```

If, after enough thought, you find that the value is truly optional. Explicitly model that optionality.
```csharp
// Instead of
class Telephone
{
    public required string Number { get; }
}

class Person
{
    public required Telephone? Telephone { get; }
}

// We write
public abstract record Telephone
{
    private Telephone() { }
    public static Telephone WasNotProvidedByUser => NotProvided.Instance;
    public static Provided Create(string number)
        => Provided.CreateProvided(number);

    public sealed record Provided : Telephone
    {
        public string Number { get; }

        private Provided(string number) => Number = number;

        internal static Provided CreateProvided(string number)
        {
            return new Provided(number);
        }
    }

    public sealed record NotProvided : Telephone
    {
        public static NotProvided Instance { get; } = new();
        private NotProvided() { }
    }
}

class Person
{
    public required Telephone Telephone { get; }
}

// This should allow us to make optionality explicit when converting input into domain models
Telephone = input is null
    ? Telephone.WasNotProvidedByUser
    : Telephone.Create(input)
```

This will give us more precise models to work with and thus prevent anemia.

### Use Value Objects, not primitive types

```csharp
// Instead of
public void CreatePerson(string name, string telephone)

// We write
public void CreatePerson(Name name, Telephone telephone)
```
This prevents us from accidentally passing a name where a telephone is expected and vice versa.

### Validate always. But only once. NEVER more than once.
Validation of input is done when creating domain objects.
Never should we have to validate inside a method befor usage.
We should expect parameters to be valid.
```csharp
// Never ever should we have to do this
public void HirePerson(Name name, Telephone telephone)
{
    if (name is null) throw new ArgumentNullException(nameof(name));
    ...
}
```

### Validate in a `Create` method that returns a `Result<T>`
All domain objects should have a `Create` method that validates intput and returs a successful instance or a validation failure.
This is the only way we allow instantiation of domain objects.
`Mt.Results` contains a `Result<T>` type that we use to indicate success or failure. 
```csharp
public class Telephone
{
    private string _value;
    private Telephone(string value) { _value = value; }

    public static Result<Telephone> Create(string value)
    {
        if (!value.All(char.IsDigit))
        {
            return new ValidationFailure("Telephone number must contain only digits.");
        }
        if (value.Length != 8)
        {
            return new ValidationFailure("Telephone number must be 8 digits long.");
        }

        return new Telephone(value);
    }
}

```
All value objects should have a `Create` method that either returns a valid instance or a validation failure.







