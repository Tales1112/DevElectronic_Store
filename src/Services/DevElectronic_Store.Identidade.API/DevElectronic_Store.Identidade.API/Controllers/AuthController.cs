using DevElectronic_Store.Identidade.API.Services.Interface;
using DevElectronic_Store.WebApi.Core.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static DevElectronic_Store.Identidade.API.Model.UserViewModel;

namespace DevElectronic_Store.Identidade.API.Controllers
{
    [Route("api/identidade")]
    [ApiController]
    public class AuthController : MainController
    {
        private readonly AuthenticationService _authenticationService;

        public AuthController(AuthenticationService authenticationService)
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
                var clientResult = await RegistrarCliente(usuarioRegistro);
            }

        }
        private async Task<bool> RegistrarCliente(UsuarioRegistro usuarioRegistro)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($@"http://localhost:44337");
            var content = new StringContent(JsonSerializer.Serialize(usuarioRegistro), Encoding.UTF8, "application/json");
            httpClient.PostAsync("",);
            return true;
        }
    }
}
