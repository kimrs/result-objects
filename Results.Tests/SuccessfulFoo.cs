namespace Results.Tests;

public class SuccessfulFoo : Bat
{
    private SuccessfulFoo() { }

    public static Result<SuccessfulFoo> Create() => new SuccessfulFoo();
}
