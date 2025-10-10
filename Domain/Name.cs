using Results;

namespace Domain;

public class Name
{
    private string _value;
    private Name(string value) => _value = value;

    private class NameTooLongException() : DomainException("The name is too long.");
    public static Result<Name> Create(string value)
    {
        if (value.Length > 10)
        {
            return new NameTooLongException();
        }

        return new Name(value);
    }
    
    public override string ToString() => _value;
}