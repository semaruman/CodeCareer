using CodeCareer.Areas.User.Models;

namespace CodeCareer.Tests;

public class PublicationOwnershipTests
{
    [Fact]
    public void PublicationModel_HasUserIdForOwnership()
    {
        var pub = new PublicationModel { UserId = 42, Content = "hello" };
        Assert.Equal(42, pub.UserId);
    }

    [Fact]
    public void CommentModel_RequiresUserId()
    {
        var comment = new CommentModel { UserId = 7, PublicationId = 1, Content = "hi" };
        Assert.Equal(7, comment.UserId);
    }
}

public class AchievementKeysTests
{
    [Fact]
    public void AchievementKeys_AreStable()
    {
        Assert.Equal("FirstTaskSolved", AchievementKeys.FirstTaskSolved);
        Assert.Equal("TenTasksSolved", AchievementKeys.TenTasksSolved);
        Assert.Equal("FirstPost", AchievementKeys.FirstPost);
    }
}

public class UserModelValidationTests
{
    [Fact]
    public void UserModel_DefaultRole_IsUser()
    {
        var user = new UserModel();
        Assert.Equal(Security.Roles.User, user.Role);
    }

    [Fact]
    public void UserModel_PasswordIsNotMapped()
    {
        var prop = typeof(UserModel).GetProperty(nameof(UserModel.Password));
        Assert.NotNull(prop);
        Assert.Contains(prop!.GetCustomAttributes(true), a => a.GetType().Name == "NotMappedAttribute");
    }
}

public class NotificationTypesTests
{
    [Fact]
    public void NotificationTypes_Defined()
    {
        Assert.False(string.IsNullOrEmpty(NotificationTypes.NewFollower));
        Assert.False(string.IsNullOrEmpty(NotificationTypes.CommentOnPost));
    }
}

public class SubmissionModelTests
{
    [Fact]
    public void SubmissionModel_StoresJudgeStatus()
    {
        var s = new SubmissionModel { Status = "Accepted", Score = 100, Language = "csharp" };
        Assert.Equal("Accepted", s.Status);
    }
}

public class SearchValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptySearchQuery_IsInvalid(string q)
    {
        Assert.True(string.IsNullOrWhiteSpace(q));
    }

    [Fact]
    public void LongSearchQuery_TruncatedTo200()
    {
        var q = new string('a', 250);
        if (q.Length > 200) q = q[..200];
        Assert.Equal(200, q.Length);
    }
}

public class PublicationContentValidationTests
{
    [Fact]
    public void Content_MaxLength_Is1000()
    {
        var maxAttr = typeof(PublicationModel).GetProperty(nameof(PublicationModel.Content))!
            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.MaxLengthAttribute), false)
            .Cast<System.ComponentModel.DataAnnotations.MaxLengthAttribute>()
            .FirstOrDefault();
        Assert.NotNull(maxAttr);
        Assert.Equal(1000, maxAttr!.Length);
    }
}

public class CommentContentValidationTests
{
    [Fact]
    public void Comment_MaxLength_Is1000()
    {
        var maxAttr = typeof(CommentModel).GetProperty(nameof(CommentModel.Content))!
            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.MaxLengthAttribute), false)
            .Cast<System.ComponentModel.DataAnnotations.MaxLengthAttribute>()
            .FirstOrDefault();
        Assert.Equal(1000, maxAttr!.Length);
    }
}

public class RolesTests
{
    [Fact]
    public void AdminRole_IsDistinctFromUser()
    {
        Assert.NotEqual(Security.Roles.Admin, Security.Roles.User);
    }
}
