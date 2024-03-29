using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DevElectronic_Store.WebApi.Core.Usuario
{
    public interface IAspNetUser
    {
        string Name { get; }

        bool EstaAutenticado();
        IEnumerable<Claim> ObterClaims();
        HttpContext obterHttpContext();
        string ObterUserEmail();
        Guid ObterUserId();
        string ObterUserRefreshToken();
        string ObterUserToken();
        bool PossuiRole(string role);
    }
}