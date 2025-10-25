using Dentor.Solutions.Academy.DTOs.Integration;

namespace Dentor.Solutions.Academy.Interfaces;

/// <summary>
/// Interface for Azure Blob Storage integration service
/// </summary>
public interface IAzureBlobStorageService
{
    /// <summary>
    /// Upload a file to Azure Blob Storage
    /// </summary>
    Task<BlobUploadResultDto> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? containerName = null, string? folder = null);

    /// <summary>
    /// Upload a file for a specific course
    /// </summary>
    Task<BlobUploadResultDto> UploadCourseFileAsync(int courseId, Stream fileStream, string fileName, string contentType, CourseFileType fileType);

    /// <summary>
    /// Delete a file from Azure Blob Storage
    /// </summary>
    Task<bool> DeleteFileAsync(string blobName, string? containerName = null);

    /// <summary>
    /// Check if a file exists
    /// </summary>
    Task<bool> FileExistsAsync(string blobName, string? containerName = null);

    /// <summary>
    /// Get file metadata
    /// </summary>
    Task<BlobMetadataDto?> GetFileMetadataAsync(string blobName, string? containerName = null);

    /// <summary>
    /// Generate a SAS URL for secure file access
    /// </summary>
    Task<string> GenerateSasUrlAsync(string blobName, string? containerName = null, int expirationMinutes = 60);

    /// <summary>
    /// Get public URL for a blob (for public containers)
    /// </summary>
    string GetPublicUrl(string blobName, string? containerName = null);

    /// <summary>
    /// List files in a folder
    /// </summary>
    Task<List<BlobMetadataDto>> ListFilesAsync(string? folder = null, string? containerName = null);
}

/// <summary>
/// Types of course files for organized storage
/// </summary>
public enum CourseFileType
{
    Image,
    Document,
    Material,
    Certificate,
    QuizExplanation
}

