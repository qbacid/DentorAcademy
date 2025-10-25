# Integration Services Documentation

This document provides setup instructions and usage examples for the Vimeo and Azure Blob Storage integration services.

---

## 🎬 Vimeo Video Integration

### Overview
The Vimeo service allows you to validate videos, retrieve metadata, and generate embed URLs for course content. Video metadata is cached for 3 hours to reduce API calls.

### Setup Instructions

#### 1. Create Vimeo Account and App
1. Go to [https://vimeo.com](https://vimeo.com) and create an account (if you don't have one)
2. Navigate to [https://developer.vimeo.com/apps](https://developer.vimeo.com/apps)
3. Click **"Create App"**
4. Fill in the app details:
   - **App Name**: DentorAcademy
   - **App Description**: Educational platform video integration
   - **App URL**: Your application URL

#### 2. Generate Access Token
1. In your app dashboard, go to the **"Authentication"** tab
2. Scroll to **"Generate an Access Token"**
3. Select the following scopes:
   - ✅ **public** (view public videos)
   - ✅ **private** (view private videos you own)
4. Click **"Generate Token"**
5. **Copy the token** (you won't be able to see it again!)

#### 3. Configure Application
Add the access token to `appsettings.json`:

```json
{
  "Vimeo": {
    "AccessToken": "YOUR_VIMEO_ACCESS_TOKEN_HERE",
    "ApiBaseUrl": "https://api.vimeo.com",
    "CacheExpirationHours": 3,
    "EnableCaching": true
  }
}
```

**⚠️ Security Note**: Never commit your access token to source control. Use User Secrets for development:
```bash
dotnet user-secrets set "Vimeo:AccessToken" "your-token-here"
```

### Usage Examples

#### Example 1: Validate a Video Before Saving
```csharp
@inject IVimeoService VimeoService

private async Task ValidateAndSaveVideo(string vimeoUrl)
{
    // Validate video exists
    var isValid = await VimeoService.ValidateVideoAsync(vimeoUrl);
    
    if (!isValid)
    {
        // Show error to user
        errorMessage = "Video not found or is not accessible";
        return;
    }
    
    // Get video metadata
    var video = await VimeoService.GetVideoMetadataAsync(vimeoUrl);
    
    if (video != null)
    {
        // Save to course content
        courseContent.VideoUrl = video.EmbedUrl;
        courseContent.VideoTitle = video.Title;
        courseContent.VideoDuration = video.Duration;
        
        await SaveCourseContent(courseContent);
    }
}
```

#### Example 2: Display Video in Course Content
```razor
@if (!string.IsNullOrEmpty(lesson.VideoUrl))
{
    var videoId = VimeoService.ExtractVideoId(lesson.VideoUrl);
    var embedUrl = VimeoService.GetEmbedUrl(videoId);
    
    <div class="video-container">
        <iframe src="@embedUrl" 
                width="100%" 
                height="500" 
                frameborder="0" 
                allow="autoplay; fullscreen; picture-in-picture">
        </iframe>
    </div>
}
```

#### Example 3: Get Video Metadata
```csharp
var video = await VimeoService.GetVideoMetadataAsync("123456789");

Console.WriteLine($"Title: {video.Title}");
Console.WriteLine($"Duration: {video.Duration} seconds");
Console.WriteLine($"Thumbnail: {video.ThumbnailUrl}");
Console.WriteLine($"Available: {video.IsAvailable}");
```

### Supported URL Formats
The service automatically extracts video IDs from various URL formats:
- `https://vimeo.com/123456789`
- `https://vimeo.com/video/123456789`
- `https://player.vimeo.com/video/123456789`
- `123456789` (just the ID)

---

## ☁️ Azure Blob Storage Integration

### Overview
The Azure Blob Storage service manages file uploads/downloads with automatic SAS URL generation for secure access. Files are organized in a structured folder hierarchy.

### Setup Instructions

#### 1. Create Azure Storage Account
1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"** → **"Storage account"**
3. Fill in the details:
   - **Subscription**: Your subscription
   - **Resource Group**: Create new or use existing
   - **Storage account name**: `dentoracademy` (must be unique globally)
   - **Region**: Choose closest to your users
   - **Performance**: Standard
   - **Redundancy**: LRS (Locally redundant storage) for development
4. Click **"Review + Create"** → **"Create"**

#### 2. Get Connection String
1. Once created, go to your storage account
2. Navigate to **"Security + networking"** → **"Access keys"**
3. Click **"Show keys"**
4. Copy **"Connection string"** from key1 or key2

#### 3. Create Containers (Optional - auto-created by service)
1. Go to **"Data storage"** → **"Containers"**
2. Create the following containers:
   - `course-images` (Private)
   - `course-documents` (Private)
   - `course-materials` (Private)
   - `user-uploads` (Private)

#### 4. Configure Application
Add the connection string to `appsettings.json`:

```json
{
  "AzureStorage": {
    "ConnectionString": "YOUR_AZURE_STORAGE_CONNECTION_STRING_HERE",
    "DefaultContainer": "course-content",
    "SasExpirationMinutes": 60,
    "UsePublicAccess": false,
    "CourseImagesContainer": "course-images",
    "CourseDocumentsContainer": "course-documents",
    "CourseMaterialsContainer": "course-materials",
    "UserUploadsContainer": "user-uploads"
  }
}
```

**⚠️ Security Note**: Never commit connection strings to source control. Use User Secrets:
```bash
dotnet user-secrets set "AzureStorage:ConnectionString" "your-connection-string-here"
```

### File Structure
Files are automatically organized in the following structure:

```
courses/
├── {courseId}/
│   ├── images/
│   │   └── 20241015143020_a1b2c3d4_course-banner.jpg
│   ├── documents/
│   │   └── 20241015143025_e5f6g7h8_syllabus.pdf
│   ├── materials/
│   │   └── 20241015143030_i9j0k1l2_worksheet.docx
│   ├── certificates/
│   │   └── 20241015143035_m3n4o5p6_certificate-template.pdf
│   └── quiz-explanations/
│       └── 20241015143040_q7r8s9t0_explanation-diagram.png
quizzes/
└── {quizId}/
    └── explanations/
users/
└── {userId}/
    └── uploads/
```

### Usage Examples

#### Example 1: Upload Course Image
```csharp
@inject IAzureBlobStorageService BlobStorage

private async Task UploadCourseImage(int courseId, IBrowserFile file)
{
    try
    {
        using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB
        
        var result = await BlobStorage.UploadCourseFileAsync(
            courseId: courseId,
            fileStream: stream,
            fileName: file.Name,
            contentType: file.ContentType,
            fileType: CourseFileType.Image
        );
        
        // Save URL to database
        course.ImageUrl = await BlobStorage.GenerateSasUrlAsync(result.BlobName);
        
        Console.WriteLine($"Uploaded: {result.BlobUrl}");
        Console.WriteLine($"Size: {result.SizeBytes / 1024} KB");
    }
    catch (BlobStorageException ex)
    {
        errorMessage = $"Upload failed: {ex.Message}";
    }
}
```

#### Example 2: Generate Download Link for Course Material
```csharp
private async Task<string> GetDownloadLink(string blobName)
{
    // Generate SAS URL valid for 60 minutes with read/write access
    var sasUrl = await BlobStorage.GenerateSasUrlAsync(
        blobName: blobName,
        containerName: "course-materials",
        expirationMinutes: 60
    );
    
    return sasUrl;
}
```

#### Example 3: Upload PDF Document
```csharp
private async Task UploadCoursePdf(int courseId, Stream pdfStream, string fileName)
{
    var result = await BlobStorage.UploadCourseFileAsync(
        courseId: courseId,
        fileStream: pdfStream,
        fileName: fileName,
        contentType: "application/pdf",
        fileType: CourseFileType.Document
    );
    
    // Store in database
    var material = new CourseMaterial
    {
        CourseId = courseId,
        Title = fileName,
        BlobName = result.BlobName,
        FileSize = result.SizeBytes,
        ContentType = result.ContentType,
        UploadedAt = result.UploadedAt
    };
    
    await SaveMaterial(material);
}
```

#### Example 4: List Files in Course Folder
```csharp
private async Task<List<BlobMetadataDto>> GetCourseImages(int courseId)
{
    var files = await BlobStorage.ListFilesAsync(
        folder: $"courses/{courseId}/images",
        containerName: "course-images"
    );
    
    return files;
}
```

#### Example 5: Delete Old File
```csharp
private async Task DeleteOldCourseImage(string blobName)
{
    var deleted = await BlobStorage.DeleteFileAsync(
        blobName: blobName,
        containerName: "course-images"
    );
    
    if (deleted)
    {
        Console.WriteLine("File deleted successfully");
    }
}
```

#### Example 6: Check File Exists Before Downloading
```csharp
private async Task<string?> GetSafeDownloadUrl(string blobName)
{
    var exists = await BlobStorage.FileExistsAsync(blobName);
    
    if (!exists)
    {
        return null;
    }
    
    return await BlobStorage.GenerateSasUrlAsync(blobName);
}
```

---

## 🔐 Security Best Practices

### Vimeo
1. **Never expose your access token** in client-side code
2. Use User Secrets for development: `dotnet user-secrets set "Vimeo:AccessToken" "token"`
3. Use Azure Key Vault or environment variables for production
4. Rotate tokens periodically
5. Only grant necessary scopes (public + private)

### Azure Blob Storage
1. **Never expose connection strings** in client-side code
2. Use User Secrets for development
3. Use Managed Identity in Azure for production (no connection strings needed!)
4. Always use **Private** container access levels
5. Generate SAS URLs with **minimal expiration time** (60 minutes default)
6. Set appropriate SAS permissions (ReadWrite only when needed)
7. Monitor blob access logs in Azure Portal

---

## 🧪 Testing the Services

### Test Vimeo Service
```csharp
// In a test page or component
@page "/test-vimeo"
@inject IVimeoService VimeoService

<h3>Test Vimeo Service</h3>

<input @bind="videoUrl" placeholder="Enter Vimeo URL" />
<button @onclick="TestVideo">Test</button>

@if (videoInfo != null)
{
    <div>
        <h4>@videoInfo.Title</h4>
        <p>Duration: @videoInfo.Duration seconds</p>
        <img src="@videoInfo.ThumbnailUrl" />
        <iframe src="@videoInfo.EmbedUrl" width="640" height="360"></iframe>
    </div>
}

@code {
    private string videoUrl = "";
    private VimeoVideoDto? videoInfo;
    
    private async Task TestVideo()
    {
        videoInfo = await VimeoService.GetVideoMetadataAsync(videoUrl);
    }
}
```

### Test Azure Blob Storage
```csharp
// In a test page or component
@page "/test-storage"
@inject IAzureBlobStorageService BlobStorage

<h3>Test Azure Storage</h3>

<InputFile OnChange="HandleFileUpload" />

@if (!string.IsNullOrEmpty(uploadedUrl))
{
    <p>File uploaded successfully!</p>
    <a href="@uploadedUrl" target="_blank">Download Link (60 min)</a>
}

@code {
    private string? uploadedUrl;
    
    private async Task HandleFileUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        using var stream = file.OpenReadStream();
        
        var result = await BlobStorage.UploadFileAsync(
            stream, 
            file.Name, 
            file.ContentType
        );
        
        uploadedUrl = await BlobStorage.GenerateSasUrlAsync(result.BlobName);
    }
}
```

---

## 📊 Monitoring & Troubleshooting

### Common Issues

#### Vimeo
- **401 Unauthorized**: Check access token is correct
- **404 Not Found**: Video doesn't exist or is private
- **403 Forbidden**: Token doesn't have required scopes

#### Azure Storage
- **Connection refused**: Check connection string
- **Container not found**: Container name typo or not created
- **403 Forbidden**: Check storage account access keys are enabled
- **SAS URL expired**: Increase expiration time or regenerate

### Logging
Both services include comprehensive logging. Check logs in:
- Development: Console output
- Production: Application Insights or your logging provider

---

## 💰 Cost Considerations

### Vimeo
- **Free tier**: 500MB storage, 25 videos/month
- **Plus ($7/month)**: 5GB storage, unlimited videos
- **API calls**: No charge for API calls within reasonable limits

### Azure Blob Storage
- **LRS Storage**: ~$0.02/GB/month
- **Operations**: ~$0.004 per 10,000 operations
- **Data transfer**: Free for first 100GB/month out
- **Estimated cost**: ~$1-5/month for small to medium courses

---

## 🚀 Next Steps

1. **Set up Vimeo**: Create app and get access token
2. **Set up Azure**: Create storage account and get connection string
3. **Configure secrets**: Use user secrets or environment variables
4. **Test integrations**: Use test pages to verify everything works
5. **Migrate existing files**: Move course images from wwwroot to Azure Blob Storage
6. **Update course creation**: Add Vimeo video validation to course management
7. **Monitor usage**: Check Azure portal for storage usage and costs

