using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTCaptcha.NetCore.Models
{
    public class MTCaptchaCheckTokenResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("fail_codes")]
        public List<string>? FailCodes { get; set; }

        public Dictionary<string, string>? FailCodeMessages { get; set; }
    }
}
