# Integration Services - Quick Reference

## ✅ Implementation Complete

Both Vimeo and Azure Blob Storage integration services have been successfully implemented and are ready to use.

---

## 📦 Files Created

### Interfaces
- `/Interfaces/IVimeoService.cs` - Vimeo service contract
- `/Interfaces/IAzureBlobStorageService.cs` - Azure Blob Storage service contract

### Services
- `/Services/VimeoService.cs` - Vimeo API integration with 3-hour caching
- `/Services/AzureBlobStorageService.cs` - Azure Blob Storage with SAS URL generation

### DTOs
- `/DTOs/Integration/IntegrationDtos.cs` - All integration DTOs and exceptions

### Configuration
- `/Configuration/IntegrationSettings.cs` - Settings classes for both services

### Documentation
- `/INTEGRATION_SERVICES_GUIDE.md` - Complete setup and usage guide

### Configuration Files Updated
- `appsettings.json` - Added Vimeo and AzureStorage sections
- `Program.cs` - Registered services with DI container
- `Dentor.Academy.Web.csproj` - Added Azure.Storage.Blobs package

---

## 🚀 Quick Start

### 1. Install Package
```bash
cd Dentor.Academy.Web
dotnet restore
```

### 2. Configure Vimeo (Optional - only if using video features)
```bash
# Add to User Secrets (never commit to source control!)
dotnet user-secrets set "Vimeo:AccessToken" "your-vimeo-token-here"
```

Get your Vimeo token at: https://developer.vimeo.com/apps

### 3. Configure Azure Storage (Optional - only if using file uploads)
```bash
# Add to User Secrets
dotnet user-secrets set "AzureStorage:ConnectionString" "your-connection-string-here"
```

Get connection string from Azure Portal → Storage Account → Access Keys

---

## 💡 Usage Examples

### Vimeo Service

```csharp
@inject IVimeoService VimeoService

// Validate a video
var isValid = await VimeoService.ValidateVideoAsync("https://vimeo.com/123456789");

// Get video metadata
var video = await VimeoService.GetVideoMetadataAsync("123456789");

// Extract video ID from URL
var videoId = VimeoService.ExtractVideoId("https://player.vimeo.com/video/123456789");

// Get embed URL
var embedUrl = VimeoService.GetEmbedUrl("123456789");
// Returns: https://player.vimeo.com/video/123456789
```

### Azure Blob Storage Service

```csharp
@inject IAzureBlobStorageService BlobStorage

// Upload a course image
var result = await BlobStorage.UploadCourseFileAsync(
    courseId: 1,
    fileStream: stream,
    fileName: "banner.jpg",
    contentType: "image/jpeg",
    fileType: CourseFileType.Image
);

// Generate SAS URL (60 min expiration, read/write access)
var downloadUrl = await BlobStorage.GenerateSasUrlAsync(result.BlobName);

// Check if file exists
var exists = await BlobStorage.FileExistsAsync("courses/1/images/file.jpg");

// Delete file
var deleted = await BlobStorage.DeleteFileAsync("courses/1/images/old-file.jpg");

// List files in folder
var files = await BlobStorage.ListFilesAsync("courses/1/images");
```

---

## 📁 File Organization Structure

Azure Blob Storage automatically organizes files:

```
containers/
├── course-images/
│   └── courses/{courseId}/images/20241015143020_a1b2c3d4_filename.jpg
├── course-documents/
│   └── courses/{courseId}/documents/20241015143025_e5f6g7h8_syllabus.pdf
├── course-materials/
│   └── courses/{courseId}/materials/20241015143030_i9j0k1l2_worksheet.docx
└── course-content/
    └── courses/{courseId}/certificates/20241015143035_m3n4o5p6_cert.pdf
```

Files are named with:
- Timestamp (yyyyMMddHHmmss)
- Short GUID (8 chars)
- Sanitized original filename

---

## 🔐 Security Features

### Vimeo
- ✅ Access token stored in configuration (never in code)
- ✅ Token validation on startup
- ✅ Caching reduces API calls (3 hours default)
- ✅ Comprehensive error handling

### Azure Blob Storage
- ✅ Connection string stored in configuration
- ✅ **SAS URLs with read/write permissions** (as requested)
- ✅ Time-limited access (60 minutes default, configurable)
- ✅ Private container access (no public access)
- ✅ Automatic file naming prevents overwrites
- ✅ Comprehensive error handling

---

## ⚙️ Configuration Options

### appsettings.json

```json
{
  "Vimeo": {
    "AccessToken": "YOUR_TOKEN",
    "ApiBaseUrl": "https://api.vimeo.com",
    "CacheExpirationHours": 3,
    "EnableCaching": true
  },
  "AzureStorage": {
    "ConnectionString": "YOUR_CONNECTION_STRING",
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

---

## 🧪 Testing

Both services are registered and ready to use:

1. **Vimeo Service**: Test with any public Vimeo video ID
2. **Azure Storage**: Test with a simple file upload

See `INTEGRATION_SERVICES_GUIDE.md` for complete testing examples.

---

## 📊 Status

- ✅ Services implemented
- ✅ Interfaces defined
- ✅ DTOs created
- ✅ Configuration added
- ✅ DI registration complete
- ✅ NuGet packages added
- ✅ Documentation complete
- ✅ Compiled successfully
- ⚠️ Needs configuration (Vimeo token + Azure connection string)

---

## 📚 Full Documentation

See **INTEGRATION_SERVICES_GUIDE.md** for:
- Detailed setup instructions with screenshots
- Complete usage examples
- Error handling patterns
- Security best practices
- Cost considerations
- Troubleshooting guide

---

## 🎯 Next Steps

1. **Get Vimeo Access Token**: https://developer.vimeo.com/apps
2. **Create Azure Storage Account**: https://portal.azure.com
3. **Add secrets to User Secrets** (development) or environment variables (production)
4. **Test the services** using the examples above
5. **Integrate into course management** pages

---

## 💰 Estimated Costs

- **Vimeo**: Free tier includes 500MB storage, 25 videos/month
- **Azure Storage**: ~$1-5/month for typical course platform usage
  - $0.02/GB/month for storage
  - $0.004 per 10,000 operations
  - First 100GB data transfer free

---

## 🆘 Need Help?

Check the troubleshooting section in `INTEGRATION_SERVICES_GUIDE.md` or review the inline code comments in the service implementations.

