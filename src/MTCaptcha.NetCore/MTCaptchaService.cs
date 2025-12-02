using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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

        private static readonly Dictionary<string, string> FailMessages = new()
        {
            {"token-expired", "The token has expired."},
            {"token-duplicate-cal", "The token has been verified already."},
            {"bad-request", "The request is invalid or malformed."},
            {"missing-input-privatekey", "`privatekey` parameter is missing"},
            {"missing-input-token", "‘token’ parameter is missing."},
            {"invalid-privatekey", "The private key is invalid or malformed."},
            {"invalid-token", "The token parameter is invalid or malformed."},
            {"invalid-token-faildecrypt", "The token parameter is invalid or malformed."},
            {"privatekey-mismatch-token", "The token and the privatekey do not match."},
            {"expired-sitekey-or-account", "The sitekey/privatekey is no longer valid due to expiration or account closure."},
            {"network-error", "Something went wrong!"},
            {"unknown-error", "Something went wrong!"}
        };

        public MTCaptchaService(HttpClient httpClient, IOptions<MTCaptchaOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<MTCaptchaCheckTokenResponse> VerifyCheckTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new MTCaptchaCheckTokenResponse
                {
                    Success = false,
                    Error = "Verification token required."
                };
            }

            var requestUrl = $"{_options.VerificationUrl}?privatekey={_options.PrivateKey}&token={token}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return new MTCaptchaCheckTokenResponse
                {
                    Success = false,
                    Error = $"HTTP Error: {(int)response.StatusCode} - {response.ReasonPhrase}"
                };
            }
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("---- MTCaptcha RAW RESPONSE ----");
            Console.WriteLine(json);
            Console.WriteLine("--------------------------------");

            // If content starts with < this is HTML -> fail early
            if (json.TrimStart().StartsWith("<"))
            {
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
                return new MTCaptchaCheckTokenResponse
                {
                    Success = false,
                    Error = "Invalid response from MTCaptcha."
                };
            }

            if (result.FailCodes != null)
            {
                var mapped = new Dictionary<string, string>();

                foreach (var code in result.FailCodes)
                {
                    mapped[code] = FailMessages.ContainsKey(code)
                        ? FailMessages[code]
                        : "Something went wrong";
                }

                result.FailCodeMessages = mapped;
            }

            return result;
        }
    }
}
