using CodeCareer.Areas.User.Models;
using CodeCareer.Security;
using Microsoft.AspNetCore.Identity;

namespace CodeCareer.Tests;

public class PasswordServiceTests
{
    private readonly PasswordService _sut = new();

    [Fact]
    public void HashPassword_ProducesNonPlaintextHash()
    {
        var user = new UserModel { Email = "a@test.com" };
        var hash = _sut.HashPassword(user, "Password123!");
        Assert.StartsWith("AQAAAA", hash);
        Assert.NotEqual("Password123!", hash);
    }

    [Fact]
    public void VerifyPassword_ValidPassword_ReturnsTrue()
    {
        var user = new UserModel { Email = "a@test.com" };
        user.PasswordHash = _sut.HashPassword(user, "Password123!");
        var ok = _sut.VerifyPassword(user, "Password123!", out var rehash);
        Assert.True(ok);
        Assert.False(rehash);
    }

    [Fact]
    public void VerifyPassword_InvalidPassword_ReturnsFalse()
    {
        var user = new UserModel { Email = "a@test.com" };
        user.PasswordHash = _sut.HashPassword(user, "Password123!");
        var ok = _sut.VerifyPassword(user, "WrongPassword!", out _);
        Assert.False(ok);
    }

    [Fact]
    public void VerifyPassword_LegacyPlaintext_UpgradesOnMatch()
    {
        var user = new UserModel { Email = "legacy@test.com", PasswordHash = "oldplain" };
        var ok = _sut.VerifyPassword(user, "oldplain", out var needsRehash);
        Assert.True(ok);
        Assert.True(needsRehash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public void MeetsPolicy_WeakPassword_Fails(string password)
    {
        var ok = _sut.MeetsPolicy(password, out var error);
        Assert.False(ok);
        Assert.NotNull(error);
    }

    [Fact]
    public void MeetsPolicy_StrongPassword_Passes()
    {
        var ok = _sut.MeetsPolicy("ValidPass1!", out var error);
        Assert.True(ok);
        Assert.Null(error);
    }
}

public class LoginLockoutServiceTests
{
    [Fact]
    public void IsLockedOut_AfterFailures_BlocksUser()
    {
        var sut = new LoginLockoutService();
        for (var i = 0; i < 5; i++) sut.RecordFailure("user@test.com");
        Assert.True(sut.IsLockedOut("user@test.com", out var remaining));
        Assert.NotNull(remaining);
    }

    [Fact]
    public void Reset_ClearsLockout()
    {
        var sut = new LoginLockoutService();
        sut.RecordFailure("user@test.com");
        sut.Reset("user@test.com");
        Assert.False(sut.IsLockedOut("user@test.com", out _));
    }
}
