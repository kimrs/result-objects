namespace Results.Tests;

public class ExceptionalFoo
{
    private ExceptionalFoo() { }
    
    public static Result<ExceptionalFoo> Create() => new Exception();
}
