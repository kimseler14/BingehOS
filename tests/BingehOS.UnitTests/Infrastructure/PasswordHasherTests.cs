using BingehOS.Infrastructure.Security;

namespace BingehOS.UnitTests.Infrastructure;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_Then_Verify_With_Correct_Password_Returns_True()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("SecurePass123!");

        Assert.True(hasher.Verify("SecurePass123!", hash));
    }

    [Fact]
    public void Verify_With_Wrong_Password_Returns_False()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("SecurePass123!");

        Assert.False(hasher.Verify("WrongPassword", hash));
    }

    [Fact]
    public void Hash_Of_Same_Password_Twice_Produces_Different_Hashes()
    {
        var hasher = new PasswordHasher();
        var password = "SamePassword";

        var hash1 = hasher.Hash(password);
        var hash2 = hasher.Hash(password);

        Assert.NotEqual(hash1, hash2);
        Assert.True(hasher.Verify(password, hash1));
        Assert.True(hasher.Verify(password, hash2));
    }
}
