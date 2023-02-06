using Authorization.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.BL
{
    public class Home : IHome
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment _env { get; }
        //private JwtFactory _jwti { get; set; }        

        public Home(IConfiguration configuration, IWebHostEnvironment env)//JwtFactory jwti
        {
            Configuration = configuration;
            _env = env;
            //_jwti = jwti;
        }

        public async Task<bool> LoginClient(string token)
        {
            var isValid = DecodeToken(token);         
            return isValid;
        }

        public string GenerateEncodedToken(string username, string password)
        {
            var jwtAppSettingOptions = Configuration.GetSection("JwtIssuerOptions");
            var jwt = new JwtSecurityToken(
                issuer: jwtAppSettingOptions["Issuer"],
                audience: "Azeem",
                claims: null,
                notBefore: DateTime.Now.AddMinutes(-15),
                expires: DateTime.Now.AddDays(Convert.ToInt32(jwtAppSettingOptions["ExpiresDay"].ToString())),
                signingCredentials: new SigningCredentials(SigningKey(Configuration.GetSection("Key").ToString()), SecurityAlgorithms.HmacSha256));
            jwt.Payload["IsValid"] = true;
            try
            {
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                return encodedJwt;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool DecodeToken(string token)
        {
            try
            {
                var jwtAppSettingOptions = Configuration.GetSection("JwtIssuerOptions");
                var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidIssuer = jwtAppSettingOptions["Issuer"],

                    ValidateAudience = false,
                    ValidAudience = "Azeem",

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SigningKey(Configuration.GetSection("Key").ToString()),

                    RequireExpirationTime = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero,
                }, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                
                var isValid =Convert.ToBoolean(jwtSecurityToken.Payload["IsValid"]);
                if(isValid==true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static SymmetricSecurityKey SigningKey(string key)
        {
            string SecretKey = key;
            SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
            return _signingKey;
        }
    }
}
