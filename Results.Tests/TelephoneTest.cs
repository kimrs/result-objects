namespace Results.Tests;

public class Telephone
{
    private string _value;
    private Telephone(string value) { _value = value; }

    public static Result<Telephone> Create(string value)
        => value
            .FailWhen(x => !x.All(char.IsDigit), "Telephone number must contain only digits.")
            .FailWhen(x => x.Length != 8, "Telephone number must be 8 digits long.")
            .Then(x => new Telephone(x));
}

public class TelephoneTest
{
    [Fact]
    public void Test()
    {
        var t = Telephone.Create("abc");
        var g = Telephone.Create("1234567a");
        var r = Telephone.Create("12345678");
    }
}