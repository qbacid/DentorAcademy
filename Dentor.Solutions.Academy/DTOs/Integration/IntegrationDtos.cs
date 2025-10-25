namespace Dentor.Solutions.Academy.DTOs.Integration;

/// <summary>
/// DTO for Vimeo video metadata
/// </summary>
public class VimeoVideoDto
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; } // Duration in seconds
    public string? ThumbnailUrl { get; set; }
    public string EmbedUrl { get; set; } = string.Empty;
    public string PlayerUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// DTO for Azure Blob Storage upload result
/// </summary>
public class BlobUploadResultDto
{
    public string BlobName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? Folder { get; set; }
}

/// <summary>
/// DTO for blob metadata
/// </summary>
public class BlobMetadataDto
{
    public string BlobName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime LastModified { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Exception thrown when Vimeo video is not found or inaccessible
/// </summary>
public class VimeoVideoNotFoundException : Exception
{
    public string VideoId { get; }

    public VimeoVideoNotFoundException(string videoId) 
        : base($"Vimeo video '{videoId}' not found or is not accessible")
    {
        VideoId = videoId;
    }

    public VimeoVideoNotFoundException(string videoId, string message) 
        : base(message)
    {
        VideoId = videoId;
    }

    public VimeoVideoNotFoundException(string videoId, string message, Exception innerException) 
        : base(message, innerException)
    {
        VideoId = videoId;
    }
}

/// <summary>
/// Exception thrown when Azure Blob Storage operations fail
/// </summary>
public class BlobStorageException : Exception
{
    public string? BlobName { get; }

    public BlobStorageException(string message) : base(message)
    {
    }

    public BlobStorageException(string blobName, string message) : base(message)
    {
        BlobName = blobName;
    }

    public BlobStorageException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public BlobStorageException(string blobName, string message, Exception innerException) 
        : base(message, innerException)
    {
        BlobName = blobName;
    }
}

