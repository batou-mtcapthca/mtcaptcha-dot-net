# MTCaptcha .NET Core

The **MTCaptcha .NET Core** library provides a simple and secure way to integrate MTCaptcha into any .NET Core / ASP.NET Core application.

## Features
- .NET Core support
- Server-side token validation
- Simple DI registration
- Works with MVC, Razor Pages, Minimal API

## Installation
```
Install-Package MTCaptcha.NetCore
```

Or via .NET CLI:
```
dotnet add package MTCaptcha.NetCore
```

## Configuration

### Option 1: Using appsettings.json

appsettings.json:
```json
{
  "MTCaptcha": {
    "PrivateKey": "YOUR_PRIVATE_KEY"
  }
}
```

Program.cs:
```csharp
builder.Services.AddMTCaptcha(builder.Configuration.GetSection("MTCaptcha"));
```

### Option 2: Using Action configuration

Program.cs:
```csharp
builder.Services.AddMTCaptcha(options =>
{
    options.PrivateKey = "YOUR_PRIVATE_KEY";
});
```

**Note:** The verification URL is automatically configured and does not need to be specified.

## Usage

### MVC Controller Example
```csharp
using MTCaptcha.NetCore.Interfaces;

public class CaptchaController : Controller
{
    private readonly IMTCaptchaService _captchaService;

    public CaptchaController(IMTCaptchaService captchaService)
    {
        _captchaService = captchaService;
    }

    [HttpPost]
    public async Task<IActionResult> Validate(string token)
    {
        var result = await _captchaService.VerifyCheckTokenAsync(token);
        
        if (result.Success)
        {
            return Ok(new { success = true });
        }
        
        return BadRequest(new { 
            success = false, 
            error = result.Error,
            failCodes = result.FailCodes,
            failCodeMessages = result.FailCodeMessages
        });
    }
}
```

### Minimal API Example
```csharp
app.MapPost("/validate-captcha", async (string token, IMTCaptchaService captchaService) =>
{
    var result = await captchaService.VerifyCheckTokenAsync(token);
    return result.Success 
        ? Results.Ok(new { success = true })
        : Results.BadRequest(new { success = false, error = result.Error });
});
```

## Response Model

The `VerifyCheckTokenAsync` method returns a `MTCaptchaCheckTokenResponse` object:

```csharp
public class MTCaptchaCheckTokenResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<string>? FailCodes { get; set; }
    public Dictionary<string, string>? FailCodeMessages { get; set; }
}
```

## License
MIT
