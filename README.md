# Coding Guidelines

## Follow Hexagonal Architecture
The underlying architecture chosen for this project is **Hexagonal Architecture** (also known as **Ports and Adapters**).  
This provides a clear separation of business logic and implementation details without introducing unnecessary complexity.  
It also ensures a clear separation between different integrations.

The application is divided into:

- **Krs.Domain** - Business logic  
- **Krs.Api** - Entrypoint for migration  
- **Krs.Persistence** - Integration against the database
- **Krs.Facebook** - Integration against Facebook

---

## Favour Vertical Slices
Organize code by feature, not by technical concern.

### ❌ Don’t:
```
Persistence.csproj
  ├── Configurations
  │     └── PersonConfiguration.cs
  ├── Repositories
  │     └── PersonRepository.cs
  └── ViewModels
        └── PersonViewModel.cs
```

### ✅ Do:
```
Person
  ├── Configuration.cs
  ├── Repository.cs
  └── ViewModel.cs
```

This makes it easier to find related code without jumping between multiple folders.

---

## Enable `Nullable` for New Projects
```xml
<Nullable>enable</Nullable>
```
We want to limit the use of `null` in our models because it makes our code ambiguous.  
`null` is often used to indicate that a value is missing, but it can also mean that something went wrong.  
Even Microsoft is inconsistent in its use of `null`:

```csharp
_ = new NameValueCollection()["nonexistent"]; // returns null
_ = new Dictionary<string, string>()["nonexistent"]; // throws Exception
```

---

## Enable `TreatWarningsAsErrors` for New Projects
```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```
We want to ensure that our code is safe. This is a low-hanging fruit that helps us catch potential issues early.

---

## Enable `ImplicitUsings` for New Projects
```xml
<ImplicitUsings>enable</ImplicitUsings>
```
This makes our code less verbose.

---

## Do Not Use Unnecessary Prefixes or Postfixes for Type Names
A common convention is to prefix the type name with parts of its namespace.  
We consider this unnecessary because the namespace already provides that context.

### ❌ Don’t:
```csharp
namespace Kimrs.Domain.Persons.Create;
class CreatePersonCommand;
```

### ✅ Do:
```csharp
namespace Kimrs.Domain.Persons.Create;
class Command;
```

A common argument against this is that it makes searching for types harder.
We consider this a non-issue: searching for the aggregate name (e.g., `Person`) 
should find all related types across all projects because we use vertical slices.
We also find shorter names more readable, especially when viewing the file hierarchy.

### ❌ Don’t:
```
Domain
  └── Persons
       └── Create
            ├── CreatePersonCommand.cs
            ├── CreatePersonCommandHandler.cs
            └── CreatePersonMapper.cs
```

### ✅ Do:
```
Domain
  └── Persons
       └── Create
            ├── Command.cs
            ├── CommandHandler.cs
            └── Mapper.cs
```

---

## Favour the Whole Namespace Over `aliases` When Types Collide
### ❌ Don’t:
```csharp
using DomainResponse = Kimrs.Domain.Persons.Create.Response;

public Response ToDto(
    this DomainResponse response
);
```
### ✅ Do:
```csharp
public Response ToDto(
    this Kimrs.Domain.Persons.Create.Response response
);
```
We are divided on this rule and will revisit it in two months.
The argument for using the full namespace instead of an alias is that we are unlikely to be consistent in naming aliases, which might cause confusion.
Hopefully, this rule will not make our code too verbose.

---

## Mapping

### Map Manually. Do Not Use AutoMapper or Similar Libraries
Mapping, though sometimes tedious, is too simple to warrant an external library.
We should not be afraid of large DTOs. AI can generate them in seconds.

---

### Use Extension Methods for Mapping
Use the naming convention `To[TargetRole]` for mapping methods.
By *target role*, we mean the context that the target type is used in.  
When mapping a `Person` domain object to a DTO, “DTO” is the target role.

```csharp
var dto = person.ToDto();
var person = dto.ToDomain();
```

We use **static extension methods** for mapping because:

- They do not depend on instance state.  
- The objects being mapped should not be aware of other types they can be mapped to/from.  
- The team prefers the extension method syntax.

---

### Use the `required` Keyword for Properties in Classes
A common argument for using a third-party mapping library is that it ensures all properties are mapped.  
By using the `required` keyword, we get a **compile-time error** if a property is not set.

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

---

**TODO:** Check out `<EnforceCodeStyleInBuild>` to catch misuse of Async.

---

## Kimrs.Domain
This project contains the **business logic** of the application. It has the most restrictive guidelines.

### No Dependencies on External Systems
`Kimrs.Domain` may depend on other *pure* projects (e.g., `Kimrs.Shared`) but not on projects that interact with external systems (e.g., `Kimrs.Api`, `Kimrs.Persistence`) or NuGet packages that are not maintained by us.  

Keeping `Kimrs.Domain` pure ensures that we can change external systems without affecting business logic.

---

### Do Not Use `null`
In general, we want to limit the use of `null`. In `Kimrs.Domain`, we do **not allow** `null` at all.
For implications of using `null`, see [above](#enable-nullable-for-new-projects).

If you find yourself wanting to use `null`, reconsider why your design requires it. Ask:

- Why is the value missing?  
- Is the value truly optional?  
- Is it only present when an enum has a certain state?  
- Could I solve this by adding more types?

#### Example: If `Person` has a `CustomerId` that is only present if the person is a customer.
Introduce a new type that has that property instead of using the same type with a `null` property.

### ❌ Don’t:
```csharp
class Person
{
    public required Name Name { get; }
    public required CustomerId? CustomerId { get; }
}
```
### ✅ Do:
```csharp
class Person
{
    public required Name Name { get; }

    public class Customer : Person
    {
        public required CustomerId CustomerId { get; }
    }
}
```

#### Example: You find that the value is truly optional. 
Explicitly model that optionality instead of using `null`.

### ❌ Don’t:
```csharp
class Telephone
{
    public required string Number { get; }
}

class Person
{
    public required Telephone? Telephone { get; }
}
```
### ✅ Do:
```csharp
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
            => new(number);
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

// Example usage
Telephone = input is null
    ? Telephone.WasNotProvidedByUser
    : Telephone.Create(input);
```

This makes optionality explicit and prevents anemic models.

---

### Use Value Objects Instead of Primitives
### ❌ Don’t:
```csharp
public void CreatePerson(string name, string telephone);
```
### ✅ Do:
```csharp
public void CreatePerson(Name name, Telephone telephone);
```
This prevents accidentally passing a name where a telephone is expected (and vice versa).

---

### Validate Always, But Only Once
Validation of input is done **when creating domain objects**.
Never validate again inside methods.
All methods should assume their parameters are valid.

### ❌ Don’t:
```csharp
public void HirePerson(Name name, Telephone telephone)
{
    if (name is null) throw new ArgumentNullException(nameof(name));
    ...
}
```

---

### Validate in a `Create` Method That Returns a `Result<T>`
All domain objects must have a `Create` method that validates input and returns
either a successful instance or a validation failure.
This is the **only allowed way** to instantiate domain objects.

`Kimrs.Results` contains a `Result<T>` type that we use to indicate success or failure.

### ✅ Do:
```csharp
public class Telephone
{
    private readonly string _value;

    private Telephone(string value) => _value = value;

    public static Result<Telephone> Create(string value)
    {
        if (!value.All(char.IsDigit))
            return new ValidationFailure("Telephone number must contain only digits.");

        if (value.Length != 8)
            return new ValidationFailure("Telephone number must be 8 digits long.");

        return new Telephone(value);
    }
}
```

All value objects should expose a `Create` method that either returns a valid instance or a validation failure.