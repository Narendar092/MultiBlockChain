//using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Linq;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using Antlr.Runtime;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace XIDNAAPI.Handlers
{
    public class XIDNAAuthorizationFilter : AuthorizeAttribute
    {

        private const string AUTH_HEADER = "access_token";
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            IEnumerable<string> tokens = null;

            var test = actionContext.Request.Headers.TryGetValues(AUTH_HEADER, out tokens);

            var token = tokens.FirstOrDefault();
            return ValidateCurrentToken(token);
        }
        public bool ValidateCurrentToken(string token)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%444446666778890";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret)); var myIssuer = "https://systemsdna.com";
            var myAudience = "https://systemsdna.com"; var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}