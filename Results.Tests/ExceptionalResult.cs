namespace Results.Tests;

public class ExceptionalResult
{
    [Fact]
    public void ChainingWorksWhenFailed()
    {
        var shouldBeExceptionalFoo = SuccessfulFoo.Create()
            .Then(_ =>  ExceptionalFoo.Create());
        
        Assert.IsType<Exceptional<ExceptionalFoo>>(shouldBeExceptionalFoo);
    }

    [Fact]
    public void ChainingWorksWhenFailed2()
    {
        var shouldBeExceptionalFoo = ExceptionalFoo.Create()
            .Then(_ => SuccessfulFoo.Create());
        
        Assert.IsType<Exceptional<SuccessfulFoo>>(shouldBeExceptionalFoo);
    }
}