namespace Dentor.Academy.Web.Configuration;

/// <summary>
/// Configuration settings for Vimeo API integration
/// </summary>
public class VimeoSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = "https://api.vimeo.com";
    public int CacheExpirationHours { get; set; } = 3;
    public bool EnableCaching { get; set; } = true;
}

/// <summary>
/// Configuration settings for Azure Blob Storage integration
/// </summary>
public class AzureStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DefaultContainer { get; set; } = "course-content";
    public int SasExpirationMinutes { get; set; } = 60;
    public bool UsePublicAccess { get; set; } = false;
    
    // Container names for different file types
    public string CourseImagesContainer { get; set; } = "course-images";
    public string CourseDocumentsContainer { get; set; } = "course-documents";
    public string CourseMaterialsContainer { get; set; } = "course-materials";
    public string UserUploadsContainer { get; set; } = "user-uploads";
}

