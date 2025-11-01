namespace Dentor.Solutions.Academy.Components.Pages;

/// <summary>
/// Represents a single item in a breadcrumb navigation
/// </summary>
public class BreadcrumbItem
{
    /// <summary>
    /// The text to display for this breadcrumb item
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The URL to navigate to when clicking this breadcrumb item
    /// Leave empty for the current/active item
    /// </summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>
    /// Constructor for convenience
    /// </summary>
    public BreadcrumbItem()
    {
    }

    /// <summary>
    /// Constructor with parameters for easy initialization
    /// </summary>
    /// <param name="text">The display text</param>
    /// <param name="href">The navigation URL</param>
    public BreadcrumbItem(string text, string href = "")
    {
        Text = text;
        Href = href;
    }
}

