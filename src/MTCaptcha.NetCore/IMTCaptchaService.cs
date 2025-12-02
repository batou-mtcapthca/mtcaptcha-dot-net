using System.Threading.Tasks;
using MTCaptcha.NetCore.Models;

namespace MTCaptcha.NetCore.Interfaces
{
    public interface IMTCaptchaService
    {
        Task<MTCaptchaCheckTokenResponse> VerifyCheckTokenAsync(string token);
    }
}
