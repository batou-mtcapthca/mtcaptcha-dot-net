using System.Threading.Tasks;
using MTCaptcha.NetCore.Models;

namespace MTCaptcha.NetCore.Interfaces
{
    /// <summary>
    /// Service interface for verifying MTCaptcha tokens.
    /// </summary>
    public interface IMTCaptchaService
    {
        /// <summary>
        /// Verifies a MTCaptcha token with the MTCaptcha API.
        /// </summary>
        /// <param name="token">The verification token received from the client-side MTCaptcha widget.</param>
        /// <returns>A <see cref="MTCaptchaCheckTokenResponse"/> containing the verification result, including success status, error messages, and failure codes.</returns>
        Task<MTCaptchaCheckTokenResponse> VerifyCheckTokenAsync(string token);
    }
}
