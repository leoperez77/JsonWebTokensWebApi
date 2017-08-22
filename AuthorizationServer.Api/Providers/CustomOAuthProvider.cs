using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AuthorizationServer.Data;
namespace AuthorizationServer.Api.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            string symmetricKeyAsBase64 = string.Empty;

            //first try to get the client details from the Authorization Basic header
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                //no details in the Authorization Header so try to find matching post values
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                context.SetError("client_not_authorized", "invalid client details");
                return Task.FromResult<object>(null);
            }

            var dataLayer = new RepoManager(new DataLayerDapper()).DataLayer;
            var audienceDto = dataLayer.GetAudience(clientId);

            if (audienceDto == null || !clientSecret.Equals(audienceDto.Secret))
            {
                context.SetError("unauthorized_client", "unauthorized client");
                return Task.FromResult<object>(null);
            }

            //var audience = AudiencesStore.FindAudience(context.ClientId);

            //if (audience == null)
            //{
            //    context.SetError("invalid_clientId", string.Format("Invalid client_id '{0}'", context.ClientId));
            //    return Task.FromResult<object>(null);
            //}

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            
            //Dummy check here, you need to do your DB checks against memebrship system http://bit.ly/SPAAuthCode
            //if (context.UserName != context.Password)
            //{
            //    context.SetError("invalid_grant", "The user name or password is incorrect");
            //    //return;
            //    return Task.FromResult<object>(null);
            //}

            //if (context.Password  != "Mutombo")
            //{
            //    context.SetError("invalid_grant", "The user name or password is incorrect");
            //    return Task.FromResult<object>(null);
            //}

            var identity = new ClaimsIdentity("JWT");

            //identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            //identity.AddClaim(new Claim("sub", context.UserName));
            //identity.AddClaim(new Claim(ClaimTypes.Role, "Manager"));
            //identity.AddClaim(new Claim(ClaimTypes.Role, "Supervisor"));
            identity.AddClaim(new Claim("clientID", context.ClientId));
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                         "audience", (context.ClientId == null) ? string.Empty : context.ClientId
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            return Task.FromResult<object>(null);
        }
    }
}