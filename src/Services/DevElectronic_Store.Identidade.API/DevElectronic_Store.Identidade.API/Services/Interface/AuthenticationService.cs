using DevElectronic_Store.Identidade.API.Data;
using DevElectronic_Store.Identidade.API.Extensions;
using DevElectronic_Store.WebApi.Core.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NetDevPack.Security.Jwt.Interfaces;

namespace DevElectronic_Store.Identidade.API.Services.Interface
{
    public class AuthenticationService
    {
        public readonly SignInManager<IdentityUser> _signInManager;
        public readonly UserManager<IdentityUser> _userManager;
        private readonly AppTokenSettings _appTokenSettings;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IJsonWebKeySetService _jwksService;
        private readonly IAspNetUser _aspNetUser;

        public AuthenticationService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userMananger,
                                     IOptions<AppTokenSettings> appTokenSettings, ApplicationDbContext applicationDbContext,
                                     IJsonWebKeySetService jwksService, IAspNetUser aspNetUser)
        {
            _signInManager = signInManager;
            _userManager = userMananger;
            _appTokenSettings = appTokenSettings.Value;
            _applicationDbContext = applicationDbContext;
            _jwksService = jwksService;
            _aspNetUser = aspNetUser;
        }

       
    }
}
