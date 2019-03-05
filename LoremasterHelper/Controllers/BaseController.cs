using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Elmah;


namespace LoremasterHelper.Controllers
{

    public class BaseController : Controller
    {
        protected const string ACCOUNT_CHAR_CACHE_KEY = "_1";
        protected const string CHAR_DETAILS_CACHE_KEY = "_2";
        protected IAuthenticationManager AuthContext
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        protected string UserBattletag
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var btClaim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
                    if (btClaim != null)
                    {
                        return btClaim.Value;
                    }
                }
                return "";
            }
        }

        protected string UserToken
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    
                    var tokenClaim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber);
                    if (tokenClaim != null)
                    {
                        return tokenClaim.Value;
                    }
                }
                return "";
            }
        }

        protected string UserRegion
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {

                    var claim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.Locality);
                    if (claim != null)
                    {
                        return claim.Value;
                    }
                }
                return "";
            }
        }

        protected void LogError(Exception e)
        {
            var context = System.Web.HttpContext.Current;
            ErrorLog.GetDefault(context).Log(new Error(e, context));
        }
    }
}