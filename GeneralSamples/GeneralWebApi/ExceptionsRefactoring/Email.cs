using System.Text.RegularExpressions;

namespace ExceptionsRefactoring
{
    public class Email : ValueObject
    {
        private readonly string _value;

        public Email(string value)
        {
            _value = value;
        }

        public static Result<Email?> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return Result.Fail<Email>("Email should not be empty.");
            if (email.Length > 256) return Result.Fail<Email>("Email is too long");
            if (!Regex.IsMatch(email, @"^(.+)@(.+)$")) return Result.Fail<Email>("Email is invalid");
            return Result.Ok(new Email(email));
        }

        public override string ToString() => _value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return _value;
        }

        public static explicit operator Email(string email) => Create(email).Value;

        public static implicit operator string(Email email) => email._value;
    }
}