namespace ExceptionsRefactoring;

public class User
{
    public string Email { get; private set; }

    public User()
    {
    }

    public static User Create(Email email) => new() { Email = email };
}