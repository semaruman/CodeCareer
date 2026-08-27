namespace CodeCareer.Infrastructure;

public class LocalFileStorage : IFileStorage
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private const long MaxBytes = 2 * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorage> _logger;

    public LocalFileStorage(IWebHostEnvironment environment, ILogger<LocalFileStorage> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string?> SaveAvatarAsync(int userId, IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        if (file.Length > MaxBytes)
        {
            _logger.LogWarning("Avatar upload rejected for user {UserId}: file too large", userId);
            return null;
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Avatar upload rejected for user {UserId}: invalid extension {Extension}", userId, extension);
            return null;
        }

        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{userId}_{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/avatars/{fileName}";
    }
}
