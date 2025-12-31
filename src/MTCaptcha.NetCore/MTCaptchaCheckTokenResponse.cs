using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTCaptcha.NetCore.Models
{
    /// <summary>
    /// Represents the response from the MTCaptcha token verification API.
    /// </summary>
    public class MTCaptchaCheckTokenResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the token verification was successful.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message, if any, returned by the MTCaptcha API.
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the list of failure codes returned by the MTCaptcha API when verification fails.
        /// </summary>
        [JsonPropertyName("fail_codes")]
        public List<string>? FailCodes { get; set; }

        /// <summary>
        /// Gets or sets a dictionary mapping failure codes to user-friendly error messages.
        /// This property is populated by the service and is not part of the API response.
        /// </summary>
        public Dictionary<string, string>? FailCodeMessages { get; set; }
    }
}
