# GitHub Copilot Project Instructions

## Context
This project uses:
- .NET 9 with Blazor Server
- Fluent UI for Blazor (`Microsoft.FluentUI.AspNetCore.Components`)
- Simple Architecture with services, interfaces, and DTOs
- Individual Authentication using .AspNet core Identity
- Entity Framework Core migrations

## Coding Guidelines
- Use **Fluent UI components only** — do not use Bootstrap or MudBlazor.
- Prefer reusable Blazor components with parameters and partial classes.
- Use async/await properly for all database and service operations.
- Follow naming conventions: `ServiceNameService`, `IServiceName` for interfaces.
- Keep code minimal and readable; prefer C# 13 syntax. 
- Use camel case naming convention

## UI Guidelines
- Use `<FluentGrid>`, `<FluentCard>`, `<FluentButton>`, etc.
- Use responsive layout (FluentGrid or FluentStack).
- Avoid inline CSS; use Fluent tokens and styles.


## Other General Instructions
- Only change code that is necessary to implement the requested feature.
- Do not create additional readme files or documentation if not requested
- Do not change the existing code structure without permission.
- Do not start existing code enhancements without permission.
- Do not generate entity framawork migrations, ask to generate the command in the terminal tool

## Output Example
When generating components, follow this structure:

```razor
@page "/courses"
<FluentStack Orientation="Orientation.Vertical">
    
    <FluentStack Orientation="Orientation.Horizontal" Width="100%">
       <!-- Component Header Here if needed-->
    </FluentStack>
    <FluentDivider />
     <FluentStack Orientation="Orientation.Horizontal" Width="100%">
        <!-- Component Content Here -->
    </FluentStack>
</FluentStack>

@code {
 //code logic here
}