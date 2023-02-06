using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.BL
{
    public class JwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;
        
        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
           

        }
        public string GenerateEncodedToken(string username,string password)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: null,
                notBefore: _jwtOptions.NotBefore.AddMinutes(-15),
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_jwtOptions.ExpiresMin)),
                signingCredentials: _jwtOptions.SigningCredentials);
            jwt.Payload["userName"] = username;
            jwt.Payload["password"] = password;
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
