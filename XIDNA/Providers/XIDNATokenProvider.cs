using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XIDNA.Models;

namespace XIDNA.Providers
{
    public class XIDNATokenProvider<TUser, TKey> : TotpSecurityStampBasedTokenProvider<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, TKey> manager, TUser user)
        {
            //var dbContext = new ApplicationDbContext();
            //var userInfo = dbContext.Users.FirstOrDefault(x => x.Id == user.Id.ToString());
            //if (userInfo == null)
            //{
            //    return false;
            //}
            //else
            //{
            //   var status= XIDNATOTPGenerator.VerifyOTP(userInfo.TOTPSecrectKey, token);
            //    if (status)
            //        return true;
            //    else
            //        return false;
            //}
            return true;
        }

        public override Task<bool> IsValidProviderForUserAsync(UserManager<TUser, TKey> manager, TUser user)
        {
            //Verify the TOTP security token genration or not
            return base.IsValidProviderForUserAsync(manager, user);
        }
    }
}