using API_JWTToken_Products.Models;

namespace API_JWTToken_Products.Interface
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDto request);
        Task<string> Login(LoginDto request);
    }
}
