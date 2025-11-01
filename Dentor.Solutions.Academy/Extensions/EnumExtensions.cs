using Dentor.Solutions.Academy.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dentor.Solutions.Academy.Extensions;

public static class EnumExtensions
{
    //get enum display name
    public static string GetDisplayName(this ContentMimeType value)
    {
        var name = System.Enum.GetName(typeof(ContentMimeType), value) ?? value.ToString();
        var field = typeof(ContentMimeType).GetField(name);

        // Prefer [DisplayName("...")]
        var displayNameAttr = field?.GetCustomAttribute<DisplayNameAttribute>();
        if (!string.IsNullOrWhiteSpace(displayNameAttr?.DisplayName))
            return displayNameAttr!.DisplayName;

        // Fallback to [Display(Name = "...")]
        var displayAttr = field?.GetCustomAttribute<DisplayAttribute>();
        var display = displayAttr?.GetName();
        if (!string.IsNullOrWhiteSpace(display))
            return display!;

        // Default to enum identifier
        return name;
    }
    
    public static string GetDisplayName(this CourseContentType value)
    {
        var name = System.Enum.GetName(typeof(CourseContentType), value) ?? value.ToString();
        var field = typeof(CourseContentType).GetField(name);

        // Prefer [DisplayName("...")]
        var displayNameAttr = field?.GetCustomAttribute<DisplayNameAttribute>();
        if (!string.IsNullOrWhiteSpace(displayNameAttr?.DisplayName))
            return displayNameAttr!.DisplayName;

        // Fallback to [Display(Name = "...")]
        var displayAttr = field?.GetCustomAttribute<DisplayAttribute>();
        var display = displayAttr?.GetName();
        if (!string.IsNullOrWhiteSpace(display))
            return display!;

        // Default to enum identifier
        return name;
    }
}