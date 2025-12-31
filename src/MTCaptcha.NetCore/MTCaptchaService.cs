using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTCaptcha.NetCore.Interfaces;
using MTCaptcha.NetCore.Options;
using MTCaptcha.NetCore.Models;

namespace MTCaptcha.NetCore.Services
{
    public class MTCaptchaService : IMTCaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly MTCaptchaOptions _options;
        private readonly ILogger<MTCaptchaService> _logger;

        private static readonly Dictionary<string, string> FailMessages = new()
        {
            {"token-expired", "The token has expired."},
            {"token-duplicate-cal", "The token has been verified already."},
            {"bad-request", "The request is invalid or malformed."},
            {"missing-input-privatekey", "`privatekey` parameter is missing"},
            {"missing-input-token", "'token' parameter is missing."},
            {"invalid-privatekey", "The private key is invalid or malformed."},
            {"invalid-token", "The token parameter is invalid or malformed."},
            {"invalid-token-faildecrypt", "The token parameter is invalid or malformed."},
            {"privatekey-mismatch-token", "The token and the privatekey do not match."},
            {"expired-sitekey-or-account", "The sitekey/privatekey is no longer valid due to expiration or account closure."},
            {"network-error", "Something went wrong!"},
            {"unknown-error", "Something went wrong!"}
        };

        public MTCaptchaService(HttpClient httpClient, IOptions<MTCaptchaOptions> options, ILogger<MTCaptchaService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MTCaptchaCheckTokenResponse> VerifyCheckTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("VerifyCheckTokenAsync called with empty or null token");
                return new MTCaptchaCheckTokenResponse
                {
                    Success = false,
                    Error = "Verification token required."
                };
            }

            try
            {
                // Properly URL encode query parameters to prevent injection and encoding issues
                var encodedPrivateKey = Uri.EscapeDataString(_options.PrivateKey);
                var encodedToken = Uri.EscapeDataString(token);
                var requestUrl = $"{_options.VerificationUrl}?privatekey={encodedPrivateKey}&token={encodedToken}";

                _logger.LogDebug("Verifying MTCaptcha token");

                var response = await _httpClient.GetAsync(requestUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("MTCaptcha API returned non-success status code: {StatusCode} {ReasonPhrase}", 
                        (int)response.StatusCode, response.ReasonPhrase);
                    return new MTCaptchaCheckTokenResponse
                    {
                        Success = false,
                        Error = $"HTTP Error: {(int)response.StatusCode} - {response.ReasonPhrase}"
                    };
                }

                var json = await response.Content.ReadAsStringAsync();

                // If content starts with < this is HTML -> fail early
                if (json.TrimStart().StartsWith("<"))
                {
                    _logger.LogWarning("MTCaptcha API returned HTML instead of JSON");
                    return new MTCaptchaCheckTokenResponse
                    {
                        Success = false,
                        Error = $"Invalid response format from MTCaptcha: returned HTML instead of JSON."
                    };
                }

                var result = JsonSerializer.Deserialize<MTCaptchaCheckTokenResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result == null)
                {
                    _logger.LogWarning("Failed to deserialize MTCaptcha response");
                    return new MTCaptchaCheckTokenResponse
                    {
                        Success = false,
                        Error = "Invalid response from MTCaptcha."
                    };
                }

                if (result.FailCodes != null && result.FailCodes.Count > 0)
                {
                    _logger.LogWarning("MTCaptcha verification failed with codes: {FailCodes}", 
                        string.Join(", ", result.FailCodes));
                    
                    var mapped = new Dictionary<string, string>();

                    foreach (var code in result.FailCodes)
                    {
                        mapped[code] = FailMessages.ContainsKey(code)
                            ? FailMessages[code]
                            : "Something went wrong";
                    }

                    result.FailCodeMessages = mapped;
                }
                else if (result.Success)
                {
                    _logger.LogDebug("MTCaptcha token verified successfully");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while verifying MTCaptcha token");
                return new MTCaptchaCheckTokenResponse
                {
                    Success = false,
                    Error = "Network error occurred while verifying token."
                };
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Request timeout while verifying MTCaptcha token");
                return new MTCaptchaCheckTokenResponse
                {
                    Success = false,
                    Error = "Request timeout while verifying token."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while verifying MTCaptcha token");
                return new MTCaptchaCheckTokenResponse
                {
                    Success = false,
                    Error = "An unexpected error occurred while verifying token."
                };
            }
        }
    }
}
