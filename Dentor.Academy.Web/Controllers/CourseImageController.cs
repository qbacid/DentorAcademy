using Dentor.Academy.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Dentor.Academy.Web.Controllers;

/// <summary>
/// API controller for serving course images stored in the database
/// </summary>
[ApiController]
[Route("api/course-images")]
[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
public class CourseImageController : ControllerBase
{
    private readonly IDbContextFactory<QuizDbContext> _contextFactory;
    private readonly ILogger<CourseImageController> _logger;

    public CourseImageController(IDbContextFactory<QuizDbContext> contextFactory, ILogger<CourseImageController> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get course thumbnail image
    /// </summary>
    [HttpGet("thumbnail/{courseId}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> GetThumbnail(int courseId)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new { c.ThumbnailImage, c.ThumbnailContentType, c.UpdatedAt })
                .FirstOrDefaultAsync();

            if (course?.ThumbnailImage == null)
            {
                return NotFound();
            }

            // Generate ETag for cache validation
            var etag = GenerateETag(course.ThumbnailImage, course.UpdatedAt);
            
            // Check if client has cached version
            if (Request.Headers.IfNoneMatch.FirstOrDefault() == etag)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            Response.Headers.ETag = etag;
            Response.Headers.CacheControl = "public, max-age=3600";
            
            // Security headers
            Response.Headers["X-Content-Type-Options"] = "nosniff";

            return File(course.ThumbnailImage, course.ThumbnailContentType ?? "image/jpeg");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving thumbnail for course {CourseId}", courseId);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Get course cover image
    /// </summary>
    [HttpGet("cover/{courseId}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> GetCoverImage(int courseId)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new { c.CoverImage, c.CoverImageContentType, c.UpdatedAt })
                .FirstOrDefaultAsync();

            if (course?.CoverImage == null)
            {
                return NotFound();
            }

            // Generate ETag for cache validation
            var etag = GenerateETag(course.CoverImage, course.UpdatedAt);
            
            // Check if client has cached version
            if (Request.Headers.IfNoneMatch.FirstOrDefault() == etag)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            Response.Headers.ETag = etag;
            Response.Headers.CacheControl = "public, max-age=3600";
            
            // Security headers
            Response.Headers["X-Content-Type-Options"] = "nosniff";

            return File(course.CoverImage, course.CoverImageContentType ?? "image/jpeg");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cover image for course {CourseId}", courseId);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Generate an ETag based on image content and last update time
    /// </summary>
    private static string GenerateETag(byte[] imageData, DateTime? updatedAt)
    {
        var timestamp = updatedAt?.Ticks ?? 0;
        var content = $"{imageData.Length}-{timestamp}";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
        return $"\"{Convert.ToBase64String(hash).Substring(0, 16)}\"";
    }
}
