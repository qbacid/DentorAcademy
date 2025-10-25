using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Dentor.Academy.Web.Configuration;
using Dentor.Academy.Web.DTOs.Integration;
using Dentor.Academy.Web.Interfaces;

namespace Dentor.Academy.Web.Services;

/// <summary>
/// Service for Vimeo video integration
/// Validates videos and retrieves metadata using Vimeo API v3
/// 
/// Setup Instructions:
/// 1. Create a Vimeo account at https://vimeo.com
/// 2. Go to https://developer.vimeo.com/apps
/// 3. Create a new app
/// 4. Generate an access token with 'public' and 'private' scopes
/// 5. Add the access token to appsettings.json under "Vimeo:AccessToken"
/// </summary>
public class VimeoService : IVimeoService
{
    private readonly HttpClient _httpClient;
    private readonly VimeoSettings _settings;
    private readonly IMemoryCache _cache;
    private readonly ILogger<VimeoService> _logger;
    
    private const string CacheKeyPrefix = "vimeo_video_";
    private static readonly Regex VimeoUrlRegex = new(
        @"(?:https?:\/\/)?(?:www\.)?(?:player\.)?vimeo\.com\/(?:video\/)?(\d+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public VimeoService(
        HttpClient httpClient,
        IOptions<VimeoSettings> settings,
        IMemoryCache cache,
        ILogger<VimeoService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _cache = cache;
        _logger = logger;

        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_settings.ApiBaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.AccessToken}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json;version=3.4");
    }

    /// <summary>
    /// Validate a Vimeo video exists and is accessible
    /// </summary>
    public async Task<bool> ValidateVideoAsync(string videoIdOrUrl)
    {
        try
        {
            var video = await GetVideoMetadataAsync(videoIdOrUrl);
            return video != null && video.IsAvailable;
        }
        catch (VimeoVideoNotFoundException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error validating Vimeo video {VideoId}", videoIdOrUrl);
            return false;
        }
    }

    /// <summary>
    /// Get video metadata from Vimeo with caching
    /// </summary>
    public async Task<VimeoVideoDto?> GetVideoMetadataAsync(string videoIdOrUrl)
    {
        var videoId = ExtractVideoId(videoIdOrUrl);
        if (string.IsNullOrEmpty(videoId))
        {
            _logger.LogWarning("Invalid Vimeo video ID or URL: {Input}", videoIdOrUrl);
            return null;
        }

        // Check cache first
        var cacheKey = $"{CacheKeyPrefix}{videoId}";
        if (_settings.EnableCaching && _cache.TryGetValue<VimeoVideoDto>(cacheKey, out var cachedVideo))
        {
            _logger.LogDebug("Retrieved Vimeo video {VideoId} from cache", videoId);
            return cachedVideo;
        }

        try
        {
            // Call Vimeo API
            var response = await _httpClient.GetAsync($"/videos/{videoId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new VimeoVideoNotFoundException(videoId);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Vimeo API error for video {VideoId}: {StatusCode} - {Error}", 
                    videoId, response.StatusCode, errorContent);
                throw new VimeoVideoNotFoundException(videoId, 
                    $"Vimeo API returned {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var vimeoResponse = JsonSerializer.Deserialize<VimeoApiResponse>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (vimeoResponse == null)
            {
                throw new VimeoVideoNotFoundException(videoId, "Failed to parse Vimeo response");
            }

            var videoDto = new VimeoVideoDto
            {
                VideoId = videoId,
                Title = vimeoResponse.Name ?? "Untitled Video",
                Description = vimeoResponse.Description,
                Duration = vimeoResponse.Duration,
                ThumbnailUrl = vimeoResponse.Pictures?.Sizes?.LastOrDefault()?.Link,
                EmbedUrl = GetEmbedUrl(videoId),
                PlayerUrl = GetVideoUrl(videoId),
                IsAvailable = vimeoResponse.Status == "available",
                CreatedAt = vimeoResponse.CreatedTime,
                Width = vimeoResponse.Width,
                Height = vimeoResponse.Height
            };

            // Cache the result
            if (_settings.EnableCaching)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(_settings.CacheExpirationHours));
                
                _cache.Set(cacheKey, videoDto, cacheOptions);
                _logger.LogDebug("Cached Vimeo video {VideoId} for {Hours} hours", 
                    videoId, _settings.CacheExpirationHours);
            }

            return videoDto;
        }
        catch (VimeoVideoNotFoundException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error retrieving Vimeo video {VideoId}", videoId);
            throw new VimeoVideoNotFoundException(videoId, "Network error accessing Vimeo API", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving Vimeo video {VideoId}", videoId);
            throw new VimeoVideoNotFoundException(videoId, "Error retrieving video metadata", ex);
        }
    }

    /// <summary>
    /// Extract video ID from various Vimeo URL formats
    /// Supports:
    /// - https://vimeo.com/123456789
    /// - https://vimeo.com/video/123456789
    /// - https://player.vimeo.com/video/123456789
    /// - Just the ID: 123456789
    /// </summary>
    public string? ExtractVideoId(string videoIdOrUrl)
    {
        if (string.IsNullOrWhiteSpace(videoIdOrUrl))
            return null;

        // If it's already just numbers, return it
        if (long.TryParse(videoIdOrUrl.Trim(), out _))
            return videoIdOrUrl.Trim();

        // Try to extract from URL
        var match = VimeoUrlRegex.Match(videoIdOrUrl);
        if (match.Success && match.Groups.Count > 1)
        {
            return match.Groups[1].Value;
        }

        _logger.LogWarning("Could not extract Vimeo video ID from: {Input}", videoIdOrUrl);
        return null;
    }

    /// <summary>
    /// Get embeddable player URL for a video
    /// </summary>
    public string GetEmbedUrl(string videoId)
    {
        return $"https://player.vimeo.com/video/{videoId}";
    }

    /// <summary>
    /// Get direct video page URL
    /// </summary>
    public string GetVideoUrl(string videoId)
    {
        return $"https://vimeo.com/{videoId}";
    }

    #region Vimeo API Response Models

    private class VimeoApiResponse
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Duration { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public VimeoPictures? Pictures { get; set; }
    }

    private class VimeoPictures
    {
        public List<VimeoPictureSize>? Sizes { get; set; }
    }

    private class VimeoPictureSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Link { get; set; }
    }

    #endregion
}
