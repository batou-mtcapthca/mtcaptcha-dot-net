# MTCaptcha .NET Plugin

The **MTCaptcha .NET Plugin** provides a simple and secure way to integrate MTCaptcha into any .NET / ASP.NET Core application.

## Features
- .NET 6/7/8 support
- Server-side token validation
- Simple DI registration
- Works with MVC, Razor Pages, Minimal API

## Installation
```
Install-Package MTCaptcha.NetPlugin
```

## Configuration
appsettings.json:
```
{
  "MTCaptcha": {
    "SiteKey": "YOUR_SITE_KEY",
    "PrivateKey": "YOUR_PRIVATE_KEY"
  }
}
```

Program.cs:
```
builder.Services.AddMTCaptcha(builder.Configuration);
```

## Usage
```
public class CaptchaController
{
    private readonly IMTCaptchaValidator _validator;

    public CaptchaController(IMTCaptchaValidator validator)
    {
        _validator = validator;
    }

    public async Task<IActionResult> Validate(CaptchaRequest request)
    {
        var result = await _validator.ValidateAsync(request.Token);
        return Ok(new { success = result });
    }
}
```

## License
MIT
