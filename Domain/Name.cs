using Results;

namespace Domain;

public class Name
{
    private readonly string _value;
    private Name(string value) => _value = value;

    public static Result<Name> Create(string value)
        => value
            .FailWhen(x => !x.All(char.IsLetter), $"{nameof(Name)} can only contain letters.")
            .FailWhen(x => x.Length > 100, $"{nameof(Name)} can not be longer than 100 characters.")
            .FailWhen(x => x.Length <= 1, $"{nameof(Name)} must be longer than 1 character.")
            .Then(x => new Name(x));
    
    public override string ToString() => _value;
}