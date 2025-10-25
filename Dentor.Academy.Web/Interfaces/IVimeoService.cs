using Dentor.Academy.Web.DTOs.Integration;

namespace Dentor.Academy.Web.Interfaces;

/// <summary>
/// Interface for Vimeo video integration service
/// </summary>
public interface IVimeoService
{
    /// <summary>
    /// Validate a Vimeo video exists and is accessible
    /// </summary>
    Task<bool> ValidateVideoAsync(string videoIdOrUrl);

    /// <summary>
    /// Get video metadata from Vimeo
    /// </summary>
    Task<VimeoVideoDto?> GetVideoMetadataAsync(string videoIdOrUrl);

    /// <summary>
    /// Extract video ID from various Vimeo URL formats
    /// </summary>
    string? ExtractVideoId(string videoIdOrUrl);

    /// <summary>
    /// Get embeddable player URL for a video
    /// </summary>
    string GetEmbedUrl(string videoId);

    /// <summary>
    /// Get direct video page URL
    /// </summary>
    string GetVideoUrl(string videoId);
}

