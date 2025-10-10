namespace Results.Tests;

public class SuccessfulBar : Bat
{
    private SuccessfulBar() { }

    public static Result<SuccessfulBar> Create() => new SuccessfulBar();
}
