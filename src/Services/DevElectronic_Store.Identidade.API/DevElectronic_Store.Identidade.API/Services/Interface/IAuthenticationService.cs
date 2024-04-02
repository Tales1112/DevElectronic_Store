using DevElectronic_Store.Core.Communication;
using DevElectronic_Store.Identidade.API.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace DevElectronic_Store.Identidade.API.Services.Interface
{
    public interface IAuthenticationService
    {
        SignInManager<IdentityUser> _signInManager { get; }
        UserManager<IdentityUser> _userManager { get; }
        Task<UserViewModel.UsuarioRespostaLogin> GerarJwt(string email);
        Task<RefreshToken> ObterRefreshToken(Guid refreshToken);
        Task<ResponseResult> RegistarCliente(UserViewModel.UsuarioRegistro usuarioRegistro);
    }
}