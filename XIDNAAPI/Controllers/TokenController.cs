using Microsoft.Ajax.Utilities;
//using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using XICore;
using XIDatabase;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNAAPI.Handlers;
using XIDNAAPI.Requestes;

namespace XIDNAAPI.Controllers
{
    //[RoutePrefix("api/connect")]
    //[XIDNAAuthorizationFilter]
    public class TokenController : ApiController
    {

        [HttpPost]
       // [Route("token")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GenerateToken([FromBody] LoginRequest request)
        {
            try
            {
                //TODO validate username and password then generate jwt token
                XIInfraUsers xifUser = null;
                cXIAppUsers User = new cXIAppUsers();
                CXiAPI oXIAPI = new CXiAPI();
                cConnectionString oConString = new cConnectionString();
                XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIAppDbContext"].ConnectionString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(request.UserName))
                {
                    Params["sUserName"] = request.UserName;
                }
                xifUser = Connection.Select<XIInfraUsers>("XIAPPUsers_AU_T", Params).FirstOrDefault();
                
                if (xifUser  != null)
                {
                    var deCryptPwd = oXIAPI.DecryptData(xifUser.sPasswordHash, true, xifUser.UserID.ToString());
                    if(deCryptPwd == request.Password)
                    {
                        var token = GenerateAccessToken(new IdentityUserDto()
                        {
                            Email = xifUser.sEmail,
                            UserName = xifUser.sUserName,
                            UserId = xifUser.UserID.ToString(),
                        });
                        return Ok(new
                        {
                            access_token = token,
                        });
                    }
                    else
                    {
                        return Ok("Invalid Password");
                    }
                }
                else
                {
                    return Ok("Invalid Username");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("getData")]
        public async Task<IHttpActionResult> GetData()
        {
            try
            {

                IEnumerable<string> tokens = null;
                var access_token = Request.Headers.TryGetValues("access_token", out tokens);

                var userid = getJWTTokenClaim(tokens.FirstOrDefault(), "nameid");
                var username = getJWTTokenClaim(tokens.FirstOrDefault(), "unique_name");
                var email = getJWTTokenClaim(tokens.FirstOrDefault(), "email");
                return Ok(new List<string> { "user1", "user2", "user3" });
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        public string getJWTTokenClaim(string token, string claimName)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
                return claimValue;
            }
            catch (Exception)
            {
                //TODO: Logger.Error
                return null;
            }
        }




        private string GenerateAccessToken(IdentityUserDto user)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%444446666778890";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret)); var myIssuer = "http://localhost:63722/";
            var myAudience = "http://localhost:63722/"; var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                 new Claim(ClaimTypes.Name, user.UserName.ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToString()),
                           }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            }; 
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
    public class IdentityUserDto
    {
        public string UserId { get; set;}
        public string UserName { get; set;}

        public string Email { get; set;}
    }
}
