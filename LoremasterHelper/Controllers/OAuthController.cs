using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoremasterHelper.Models;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace LoremasterHelper.Controllers
{
    public class OAuthController : BaseController
    {
        private const string STATE_SESSION_KEY = "OATHSTATEVAR";

        // GET: OAuth
        public ActionResult Index(string region)
        {
            if (Config.IsRegionSupported(region))
            {
                var stateGuid = Guid.NewGuid().ToString() + "_" + region;

                Session[STATE_SESSION_KEY] = stateGuid.ToString();

                return Redirect(string.Format("{0}?client_id={1}&scope={2}&state={3}&redirect_uri={4}&response_type={5}",
                    Config.OAuthBaseUrl(region) + "/oauth/authorize",
                    Config.AuthApiKey,
                    Config.OAuthScope,
                    stateGuid,
                    Config.OAuthRedirectUrl,
                    Config.OAuthResponseType));
            }
            return View();
        }

        public ActionResult Logout(string region)
        {
            AuthContext.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Calculator", "Home", new { region = region ?? "us" });
        }

        public ActionResult Token(string state, string code)
        {
            OAuthTokenForm form = new OAuthTokenForm();

            if (string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(code) || Session[STATE_SESSION_KEY] == null || state != Session[STATE_SESSION_KEY] as string)
            {
                form.ErrorMessage = "WoW Loremaster was not granted access to your WoW profile.";
            }
            else
            {
                var region = state.Split('_')[1];
                var postBody = string.Format("redirect_uri={0}&scope={1}&grant_type=authorization_code&code={2}",
                    Config.OAuthRedirectUrl, Config.OAuthScope, code);
                var postBytes = System.Text.Encoding.ASCII.GetBytes(postBody);


                HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(Config.OAuthBaseUrl(region) + "/oauth/token");
                tokenRequest.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(Config.AuthApiKey + ":" + Config.AuthApiSecret)));
                tokenRequest.ContentType = "application/x-www-form-urlencoded";
                tokenRequest.ContentLength = postBytes.Length;
                tokenRequest.Method = "POST";
                using (Stream body = tokenRequest.GetRequestStream())
                {
                    body.Write(postBytes, 0, postBytes.Length);
                }

                var resp = "";

                using (HttpWebResponse tokenResponse = (HttpWebResponse)tokenRequest.GetResponse())
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    resp = reader.ReadToEnd();
                }

                if (!string.IsNullOrWhiteSpace(resp))
                {
                    JObject respObject = JObject.Parse(resp);

                    var accessToken = (string)respObject["access_token"];
                    var tokenExpirationSeconds = (int)respObject["expires_in"];
                    var scope = (string)respObject["scope"];

                    if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(scope) || tokenExpirationSeconds < 0)
                    {
                        form.ErrorMessage = "Unable to retreive account authorization.";
                    }
                    else
                    {
                        if (scope != Config.OAuthScope)
                        {
                            form.ErrorMessage = "You must allow Loremaster Helper to access your wow profile to use this service.";
                        }
                        else
                        {
                            var bnetUser = BNetContext.GetUser(region, accessToken);
                            if (bnetUser == null)
                            {
                                form.ErrorMessage = "Unable to retreive account authorization.";
                            }
                            else
                            {
                                var identity = new ClaimsIdentity(
                                    new[]
                                    {
                                    new Claim(ClaimTypes.Name, bnetUser.Id),
                                    new Claim(ClaimTypes.Locality, region),
                                    new Claim(ClaimTypes.SerialNumber, accessToken),
                                    new Claim(ClaimTypes.GivenName, bnetUser.BattleTag)
                                    },
                                    DefaultAuthenticationTypes.ApplicationCookie);
                                AuthContext.SignIn(new AuthenticationProperties()
                                {
                                    IsPersistent = true,
                                    ExpiresUtc = DateTime.SpecifyKind(DateTime.Now.AddSeconds(tokenExpirationSeconds), DateTimeKind.Utc)
                                }, identity);

                                return RedirectToAction("Calculator", "Home", new { region = region });
                            }
                        }
                    }
                }
                else
                {
                    form.ErrorMessage = "Unable to retreive account authorization.";
                }
            }
            return View(form);
        }
      
    }
}