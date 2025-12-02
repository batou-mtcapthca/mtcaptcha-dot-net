using System;

namespace MTCaptcha.NetCore.Options
{
    public class MTCaptchaOptions
    {
        /// <summary>
        /// The private key used for backend verification (MTCaptcha API requires "privatekey").
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// MTCaptcha API verification endpoint.
        /// </summary>
        public string VerificationUrl { get; set; } =
            "https://service.mtcaptcha.com/mtcv1/api/checktoken.json";
    }
}