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

        public async Task<string> Login(string userNm, string password)
        {
            var jwtAppSettingOptions = Configuration.GetSection("JwtIssuerOptions");
            var token = "false";
            if (userNm == jwtAppSettingOptions["username"] && password == jwtAppSettingOptions["password"])
            {
                token = "true";
            }
            return token;
        }

        public async Task<string> LoginToken(string userNm, string password)
        {
            var jwtAppSettingOptions = Configuration.GetSection("JwtIssuerOptions");
            var token = "";
            if (userNm == jwtAppSettingOptions["username"] && password == jwtAppSettingOptions["password"])
            {
                token = GenerateEncodedToken(userNm, password);
            }
            return token;
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

        public static SymmetricSecurityKey SigningKey(string key)
        {
            string SecretKey = key;
            SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
            return _signingKey;
        }

    }
}
