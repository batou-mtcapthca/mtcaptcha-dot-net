using System;

namespace MTCaptcha.NetCore.Options
{
    /// <summary>
    /// Configuration options for MTCaptcha service.
    /// </summary>
    public class MTCaptchaOptions
    {
        /// <summary>
        /// Gets or sets the private key used for backend verification (MTCaptcha API requires "privatekey").
        /// This key is obtained from your MTCaptcha account dashboard.
        /// </summary>
        /// <value>The private key for MTCaptcha verification. Must not be empty.</value>
        public string PrivateKey { get; set; } = string.Empty;
    }
}