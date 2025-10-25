using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Dentor.Academy.Web.Configuration;
using Dentor.Academy.Web.DTOs.Integration;
using Dentor.Academy.Web.Interfaces;

namespace Dentor.Academy.Web.Services;

/// <summary>
/// Service for Azure Blob Storage integration
/// Handles file uploads, downloads, and SAS URL generation
/// 
/// Setup Instructions:
/// 1. Create an Azure Storage Account at https://portal.azure.com
/// 2. Go to "Access keys" and copy the connection string
/// 3. Add to appsettings.json under "AzureStorage:ConnectionString"
/// 4. Create the following containers (or they will be auto-created):
///    - course-images (for course thumbnails and content images)
///    - course-documents (for PDFs, slides, etc.)
///    - course-materials (for downloadable resources)
///    - user-uploads (for user-generated content)
/// 5. Set appropriate access levels for each container in Azure Portal
/// 
/// File Structure:
/// - courses/{courseId}/images/{filename}
/// - courses/{courseId}/documents/{filename}
/// - courses/{courseId}/materials/{filename}
/// - courses/{courseId}/certificates/{filename}
/// - quizzes/{quizId}/explanations/{filename}
/// - users/{userId}/uploads/{filename}
/// </summary>
public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureStorageSettings _settings;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(
        IOptions<AzureStorageSettings> settings,
        ILogger<AzureBlobStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        if (string.IsNullOrEmpty(_settings.ConnectionString))
        {
            throw new InvalidOperationException(
                "Azure Storage connection string is not configured. Please add it to appsettings.json");
        }

        _blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
    }

    /// <summary>
    /// Upload a file to Azure Blob Storage
    /// </summary>
    public async Task<BlobUploadResultDto> UploadFileAsync(
        Stream fileStream, 
        string fileName, 
        string contentType, 
        string? containerName = null, 
        string? folder = null)
    {
        try
        {
            containerName ??= _settings.DefaultContainer;
            
            // Get or create container
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            // Generate unique blob name
            var blobName = GenerateBlobName(fileName, folder);
            var blobClient = containerClient.GetBlobClient(blobName);

            // Upload with metadata
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
                Metadata = new Dictionary<string, string>
                {
                    { "UploadedAt", DateTime.UtcNow.ToString("O") },
                    { "OriginalFileName", fileName }
                }
            };

            // Reset stream position if needed
            if (fileStream.CanSeek)
                fileStream.Position = 0;

            await blobClient.UploadAsync(fileStream, uploadOptions);

            var properties = await blobClient.GetPropertiesAsync();

            _logger.LogInformation("Successfully uploaded file {FileName} to {Container}/{BlobName}", 
                fileName, containerName, blobName);

            return new BlobUploadResultDto
            {
                BlobName = blobName,
                BlobUrl = blobClient.Uri.ToString(),
                Container = containerName,
                ContentType = contentType,
                SizeBytes = properties.Value.ContentLength,
                UploadedAt = DateTime.UtcNow,
                Folder = folder
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to Azure Blob Storage", fileName);
            throw new BlobStorageException(fileName, "Failed to upload file to Azure Blob Storage", ex);
        }
    }

    /// <summary>
    /// Upload a file for a specific course with organized folder structure
    /// </summary>
    public async Task<BlobUploadResultDto> UploadCourseFileAsync(
        int courseId, 
        Stream fileStream, 
        string fileName, 
        string contentType, 
        CourseFileType fileType)
    {
        var (containerName, folder) = GetCourseFileLocation(courseId, fileType);
        return await UploadFileAsync(fileStream, fileName, contentType, containerName, folder);
    }

    /// <summary>
    /// Delete a file from Azure Blob Storage
    /// </summary>
    public async Task<bool> DeleteFileAsync(string blobName, string? containerName = null)
    {
        try
        {
            containerName ??= _settings.DefaultContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            
            if (response.Value)
            {
                _logger.LogInformation("Successfully deleted blob {BlobName} from {Container}", 
                    blobName, containerName);
            }
            else
            {
                _logger.LogWarning("Blob {BlobName} not found in {Container}", 
                    blobName, containerName);
            }

            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob {BlobName}", blobName);
            throw new BlobStorageException(blobName, "Failed to delete file from Azure Blob Storage", ex);
        }
    }

    /// <summary>
    /// Check if a file exists in Azure Blob Storage
    /// </summary>
    public async Task<bool> FileExistsAsync(string blobName, string? containerName = null)
    {
        try
        {
            containerName ??= _settings.DefaultContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if blob {BlobName} exists", blobName);
            return false;
        }
    }

    /// <summary>
    /// Get file metadata from Azure Blob Storage
    /// </summary>
    public async Task<BlobMetadataDto?> GetFileMetadataAsync(string blobName, string? containerName = null)
    {
        try
        {
            containerName ??= _settings.DefaultContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var properties = await blobClient.GetPropertiesAsync();

            return new BlobMetadataDto
            {
                BlobName = blobName,
                BlobUrl = blobClient.Uri.ToString(),
                Container = containerName,
                ContentType = properties.Value.ContentType,
                SizeBytes = properties.Value.ContentLength,
                LastModified = properties.Value.LastModified.UtcDateTime,
                Metadata = properties.Value.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata for blob {BlobName}", blobName);
            throw new BlobStorageException(blobName, "Failed to get file metadata", ex);
        }
    }

    /// <summary>
    /// Generate a SAS URL for secure file access with read/write permissions
    /// </summary>
    public async Task<string> GenerateSasUrlAsync(
        string blobName, 
        string? containerName = null, 
        int expirationMinutes = 60)
    {
        try
        {
            containerName ??= _settings.DefaultContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                throw new BlobStorageException(blobName, "Blob not found");
            }

            // Check if we can generate SAS tokens
            if (!blobClient.CanGenerateSasUri)
            {
                _logger.LogWarning("Cannot generate SAS token. Returning regular URL for {BlobName}", blobName);
                return blobClient.Uri.ToString();
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b", // b = blob
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // 5 min grace period
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
            };

            // Set permissions: Read and Write as specified
            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            _logger.LogDebug("Generated SAS URL for {BlobName} expiring in {Minutes} minutes", 
                blobName, expirationMinutes);

            return sasUri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating SAS URL for blob {BlobName}", blobName);
            throw new BlobStorageException(blobName, "Failed to generate SAS URL", ex);
        }
    }

    /// <summary>
    /// Get public URL for a blob (for public containers)
    /// </summary>
    public string GetPublicUrl(string blobName, string? containerName = null)
    {
        containerName ??= _settings.DefaultContainer;
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// List files in a folder
    /// </summary>
    public async Task<List<BlobMetadataDto>> ListFilesAsync(string? folder = null, string? containerName = null)
    {
        try
        {
            containerName ??= _settings.DefaultContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var results = new List<BlobMetadataDto>();
            var prefix = string.IsNullOrEmpty(folder) ? null : $"{folder}/";

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                
                results.Add(new BlobMetadataDto
                {
                    BlobName = blobItem.Name,
                    BlobUrl = blobClient.Uri.ToString(),
                    Container = containerName,
                    ContentType = blobItem.Properties.ContentType ?? "application/octet-stream",
                    SizeBytes = blobItem.Properties.ContentLength ?? 0,
                    LastModified = blobItem.Properties.LastModified?.UtcDateTime ?? DateTime.UtcNow,
                    Metadata = blobItem.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                });
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files in folder {Folder}", folder);
            throw new BlobStorageException($"Failed to list files in folder {folder}", ex);
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Generate a unique blob name with optional folder prefix
    /// Format: {folder}/{timestamp}_{guid}_{filename}
    /// </summary>
    private string GenerateBlobName(string fileName, string? folder)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var guid = Guid.NewGuid().ToString("N")[..8]; // Short GUID
        var sanitizedFileName = SanitizeFileName(fileName);
        var uniqueFileName = $"{timestamp}_{guid}_{sanitizedFileName}";

        return string.IsNullOrEmpty(folder) 
            ? uniqueFileName 
            : $"{folder}/{uniqueFileName}";
    }

    /// <summary>
    /// Sanitize file name to remove invalid characters
    /// </summary>
    private string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return sanitized.ToLowerInvariant().Replace(" ", "-");
    }

    /// <summary>
    /// Get container and folder location for course files
    /// </summary>
    private (string containerName, string folder) GetCourseFileLocation(int courseId, CourseFileType fileType)
    {
        return fileType switch
        {
            CourseFileType.Image => 
                (_settings.CourseImagesContainer, $"courses/{courseId}/images"),
            
            CourseFileType.Document => 
                (_settings.CourseDocumentsContainer, $"courses/{courseId}/documents"),
            
            CourseFileType.Material => 
                (_settings.CourseMaterialsContainer, $"courses/{courseId}/materials"),
            
            CourseFileType.Certificate => 
                (_settings.DefaultContainer, $"courses/{courseId}/certificates"),
            
            CourseFileType.QuizExplanation => 
                (_settings.CourseImagesContainer, $"courses/{courseId}/quiz-explanations"),
            
            _ => (_settings.DefaultContainer, $"courses/{courseId}")
        };
    }

    #endregion
}
