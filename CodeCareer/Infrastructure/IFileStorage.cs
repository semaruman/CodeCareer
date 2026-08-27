namespace CodeCareer.Infrastructure;

public interface IFileStorage
{
    Task<string?> SaveAvatarAsync(int userId, IFormFile file, CancellationToken cancellationToken = default);
}
