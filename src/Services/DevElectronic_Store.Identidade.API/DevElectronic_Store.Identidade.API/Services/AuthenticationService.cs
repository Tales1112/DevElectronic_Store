using DevElectronic_Store.Core.Communication;
using DevElectronic_Store.Identidade.API.Data;
using DevElectronic_Store.Identidade.API.Extensions;
using DevElectronic_Store.Identidade.API.Model;
using DevElectronic_Store.WebApi.Core.Identidade;
using DevElectronic_Store.WebApi.Core.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static DevElectronic_Store.Identidade.API.Model.UserViewModel;

namespace DevElectronic_Store.Identidade.API.Services.Interface
{
    public class AuthenticationService : IAuthenticationService
    {
        public SignInManager<IdentityUser> _signInManager { get; }
        public UserManager<IdentityUser> _userManager { get; }
        private readonly AppTokenSettings _appTokenSettings;
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IJsonWebKeySetService _jwksService;
        private readonly IAspNetUser _aspNetUser;
        private readonly IHttpClientFactory _httpClient;

        public AuthenticationService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userMananger,
                                     IOptions<AppTokenSettings> appTokenSettings, IOptions<AppSettings> appSettings, ApplicationDbContext applicationDbContext,
                                     IJsonWebKeySetService jwksService, IAspNetUser aspNetUser, IHttpClientFactory httpClient)
        {
            _signInManager = signInManager;
            _userManager = userMananger;
            _appTokenSettings = appTokenSettings.Value;
            _appSettings = appSettings.Value;
            _applicationDbContext = applicationDbContext;
            _jwksService = jwksService;
            _aspNetUser = aspNetUser;
            _httpClient = httpClient;
        }
        public async Task<ResponseResult> RegistarCliente(UsuarioRegistro usuarioRegistro)
        {
            var client = _httpClient.CreateClient();

            client.BaseAddress = new Uri(_appSettings.ClienteUrl);

            var content = ObterConteudo(usuarioRegistro);

            var response = await client.PostAsync("/cliente/registrar-cliente/", content);

            if (!TratarErrosResponse(response)) return await DeserializerObjectResponse<ResponseResult>(response);

            return RetornoOk();
        }
        public async Task<UsuarioRespostaLogin> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await ObterClaimUsuario(claims, user);
            var encodedToken = CodificarToken(identityClaims);

            var refreshToken = await GerarRefreshToken(email);

            return ObterRespostaLogin(encodedToken, user, claims, refreshToken);
        }
        private string CodificarToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var currentIssuer = $"{_aspNetUser.ObterHttpContext().Request.Scheme}://{_aspNetUser.ObterHttpContext().Request.Host}";

            var Key = _jwksService.GenerateSigningCredentials();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                // A onde o token for valido ele podera ser usado.
                //Audience = _appSettings.ValidoEm, 
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                // Seguranca Simetrica
                //SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
                SigningCredentials = Key
            });

            return tokenHandler.WriteToken(token);
        }
        private async Task<ClaimsIdentity> ObterClaimUsuario(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }
        private async Task<RefreshToken> GerarRefreshToken(string email)
        {
            var refreshToken = new RefreshToken
            {
                Username = email,
                ExpirationDate = DateTime.UtcNow.AddHours(_appTokenSettings.RefreshTokenExpiration)
            };

            _applicationDbContext.RemoveRange(_applicationDbContext.RefreshTokens.Where(u => u.Username == email));

            await _applicationDbContext.SaveChangesAsync();

            return refreshToken;
        }
        private UsuarioRespostaLogin ObterRespostaLogin(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken)
        {
            return new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };
        }
        public async Task<RefreshToken> ObterRefreshToken(Guid refreshToken)
        {
            var token = await _applicationDbContext.RefreshTokens.AsNoTracking()
                                                   .FirstOrDefaultAsync(u => u.Token == refreshToken);

            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now ? token : null;
        }
        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        private ResponseResult RetornoOk()
        {
            return new ResponseResult();
        }
        private async Task<T> DeserializerObjectResponse<T>(HttpResponseMessage responseMessage)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        }
        private bool TratarErrosResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest) return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
        private StringContent ObterConteudo(object dado)
        {
            return new StringContent(JsonSerializer.Serialize(dado),
                                     Encoding.UTF8, "application/json");
        }
    }
}
