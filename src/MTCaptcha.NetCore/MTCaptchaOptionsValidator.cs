using Microsoft.Extensions.Options;
using System;

namespace MTCaptcha.NetCore.Options
{
    /// <summary>
    /// Validates MTCaptchaOptions to ensure required configuration is present and valid.
    /// </summary>
    public class MTCaptchaOptionsValidator : IValidateOptions<MTCaptchaOptions>
    {
        /// <summary>
        /// Validates the MTCaptchaOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being validated.</param>
        /// <param name="options">The options instance to validate.</param>
        /// <returns>A <see cref="ValidateOptionsResult"/> indicating whether validation succeeded or failed.</returns>
        public ValidateOptionsResult Validate(string? name, MTCaptchaOptions options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail("MTCaptchaOptions cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(options.PrivateKey))
            {
                return ValidateOptionsResult.Fail("PrivateKey is required and cannot be empty. Please configure your MTCaptcha private key.");
            }

            if (string.IsNullOrWhiteSpace(options.VerificationUrl))
            {
                return ValidateOptionsResult.Fail("VerificationUrl is required and cannot be empty.");
            }

            if (!Uri.TryCreate(options.VerificationUrl, UriKind.Absolute, out var uri) || 
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return ValidateOptionsResult.Fail($"VerificationUrl must be a valid HTTP or HTTPS URL. Current value: '{options.VerificationUrl}'");
            }

            return ValidateOptionsResult.Success;
        }
    }
}

