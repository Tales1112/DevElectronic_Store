using DevElectronic_Store.Identidade.API.Services.Interface;
using DevElectronic_Store.WebApi.Core.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using static DevElectronic_Store.Identidade.API.Model.UserViewModel;

namespace DevElectronic_Store.Identidade.API.Controllers
{
    [Route("api/identidade")]
    [ApiController]
    public class AuthController : MainController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("nova-conta")]
        public async Task<IActionResult> Registrar(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true
            };

            var result = await _authenticationService._userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
                var clientResult = await _authenticationService.RegistarCliente(usuarioRegistro);

                if (clientResult.Erros.Mensagens.Any())
                {
                    await _authenticationService._userManager.DeleteAsync(user);
                    return CustomResponse(clientResult);
                }
                return CustomResponse(await _authenticationService.GerarJwt(usuarioRegistro.Email));
            }

            foreach (var error in result.Errors)
            {
                AdicionarErroProcessamento(error.Description);
            }
            return CustomResponse();
        }
        [HttpPost("autenticar")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _authenticationService._signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

            if (result.Succeeded)
            {
                return CustomResponse(await _authenticationService.GerarJwt(usuarioLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("Usuario temporariamente bloqueado por tentativas invalidas");
            }

            AdicionarErroProcessamento("Usuario ou Senha Incorreto");
            return CustomResponse();
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                AdicionarErroProcessamento("Refresh Token Invalido");
                return CustomResponse();
            }
            var token = await _authenticationService.ObterRefreshToken(Guid.Parse(refreshToken));

            if(token is null)
            {
                AdicionarErroProcessamento("Refresh Token expirado");
                return CustomResponse();
            }
            return CustomResponse(await _authenticationService.GerarJwt(token.Username));
        }

    }
}
