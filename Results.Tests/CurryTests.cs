namespace Results.Tests;

public class CurryTests
{
    private class Money
    {
        private Money(decimal value) { }

        public static Result<Money> Create(decimal value)
            => new Money(value);
    }
    
    private class Owner
    {
        private Owner(string value) { }

        public static Result<Owner> Create(string value)
            => new Owner(value);
    }

    private class Account
    {
        private Account(Money money, Owner owner) { }
        
        private static readonly Func<Money, Owner, Result<Account>> _create = (money, owner) => new Account(money, owner);

        public static Result<Func<Money, Result<Func<Owner, Result<Account>>>>> Create => _create.Curry();
    }

    [Fact]
    public void CanCurryWhenMultipleParametersAreNeededAndMakeItLookNice()
    {
        var result = Account.Create
            .Apply(Money.Create(42))
            .Apply(Owner.Create("John Doe"));

        Assert.IsType<Completed<Account>>(result);
    }
}