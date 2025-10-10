namespace Results.Tests;

public class SuccessfulResult
{
    [Fact]
    public void WithOneThenStatement()
    {
        var shouldBeCompleted = SuccessfulFoo.Create()
            .Then(_ =>  SuccessfulBar.Create());
        
        Assert.IsType<Completed<SuccessfulBar>>(shouldBeCompleted);
    }

    [Fact]
    public void WithTwoThenStatements()
    {
        var shouldBeCompleted = SuccessfulFoo.Create()
            .Then(_ =>  SuccessfulBar.Create())
            .Then(_ =>  SuccessfulFoo.Create());
        
        Assert.IsType<Completed<SuccessfulFoo>>(shouldBeCompleted);
    }

    private static Result<Bat> ToBat(SuccessfulFoo foo) => foo;

    [Fact]
    public void WithTwoThenStatementss()
    {
        var shouldBeCompleted = SuccessfulFoo.Create()
            .Then(ToBat);
        
        Assert.IsType<Completed<Bat>>(shouldBeCompleted);
    }

    private abstract class Command;

    private class Insert : Command
    {
        public Insert(Bat bat)
        {
            
        }

        public static Result<Insert> Create(Bat bat)
            => new Insert(bat);
    }

    [Fact]
    public void RequiresExplicitTypeParametersWhenUsingConstructor()
    {
        var shouldBeCompleted = SuccessfulFoo.Create()
            .Then<SuccessfulFoo, Insert>(x => new Insert(x));
        
        Assert.IsType<Completed<Insert>>(shouldBeCompleted);
    }

    [Fact]
    public void CanInferTypeParametersWhenUsingCreate()
    {
        var shouldBeCompleted = SuccessfulFoo.Create()
            .Then(Insert.Create);
        
        Assert.IsType<Completed<Insert>>(shouldBeCompleted);
    }

    [Fact]
    public void CanCastByUsingTypeParameters()
    {
        var shouldBeCommand = SuccessfulFoo.Create()
            .Then<SuccessfulFoo, Command>(x => new Insert(x));
        
        Assert.IsType<Completed<Command>>(shouldBeCommand);
    }

    private Result<Command> ReturnCommand(Command command) => command;

    [Fact]
    public void CanChainIntoMethodThatAcceptsBaseType()
    {
        var shouldBeCommand = SuccessfulFoo.Create()
            .Then(Insert.Create)
            .Then(ReturnCommand);

        Assert.IsType<Completed<Command>>(shouldBeCommand);
    }
    
}