using Dentor.Solutions.Academy.Models;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Dentor.Solutions.Academy.Extensions;

public static class IMessageServiceExtension
{
    public static async Task AddDismissibleMessage(this IMessageService messageServiceProvider, 
        string title, 
        string resultMessage, 
        SaveEntityResult result)
    {
        var message = result != SaveEntityResult.Success
            ? $"Error:{resultMessage}"
            : $"Operation completed: {resultMessage}";

        var type = result != SaveEntityResult.Success 
            ? MessageIntent.Error 
            : MessageIntent.Success;
        
        await messageServiceProvider.ShowMessageBarAsync(options =>
        {
            options.Title = title;
            options.Body = message;
            options.Intent = type;
            options.Section = nameof(MessageSessions.MESSAGES_TOP);
            options.AllowDismiss = true;
            options.ClearAfterNavigation = true;
            options.Timeout = 3 * 1000;
        });
    }
    
    public static async Task AddDismissibleMessage(this IToastService toastService, 
        string title, 
        string resultMessage, 
        SaveEntityResult result)
    {
        var message = result != SaveEntityResult.Success
            ? $"Error:{resultMessage}"
            : $"Operation completed: {resultMessage}";

        var typeIntent = result != SaveEntityResult.Success 
            ? ToastIntent.Error 
            : ToastIntent.Success;
        
        toastService.ShowToast(
            typeIntent,
            message,
            3 * 1000
        );
    }
}