namespace Results;

public static partial class Extensions
{
    public class Validator<T>
    {
        private T Input { get; }
        private (Func<T, bool>, string)[] FailCases { get; }
        
        internal Validator(T input, (Func<T, bool>, string)[] failCases)
        {
            Input = input;
            FailCases = failCases;
        }

        public Validator<T> FailWhen(Func<T, bool> condition, string errorMessage)
            => new (Input, [..FailCases, (condition, errorMessage)]);

        public Result<TR> Then<TR>(Func<T, TR> onSuccess)
        {
            var errorMessages = FailCases
                .Where(x => x.Item1(Input))
                .Select(x => new ValidationFailure(x.Item2))
                .ToArray();

            return errorMessages.Length > 0
                ?  new InvalidInput<TR>(errorMessages)
                : onSuccess(Input);
        }
    }

    public static Validator<T> FailWhen<T>(this T input, Func<T, bool> condition, string errorMessage)
        => new (input, [(condition, errorMessage)]);
}