using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Autofac;
using dataislandcommon.Classes.Identity;
using dimain.Models.oauth;
using dimain.Services.System;
using dataislandcommon.Utilities;
using System.Security.Cryptography;

namespace DataIsland.Classes
{
    public class DataIslandAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public DataIslandAuthorizationServerProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context
                //if you want to force sending clientId/secrects once obtain access tokens.
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return;
            }
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDiRefreshTokenService tokenService = ioc.Resolve<IDiRefreshTokenService>();


                client = tokenService.FindClient(context.ClientId);

                if (client == null)
                {
                    context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                    return;
                }

                if (client.ApplicationType == ApplicationTypes.NativeConfidential)
                {
                    if (string.IsNullOrWhiteSpace(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret should be sent.");
                        return;
                    }
                    else
                    {
                        if (client.Secret != tokenService.GetHash(clientSecret))
                        {
                            context.SetError("invalid_clientId", "Client secret is invalid.");
                            return;
                        }
                    }
                }

                if (!client.Active)
                {
                    context.SetError("invalid_clientId", "Client is inactive.");
                    return;
                }

                context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
                context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

                context.Validated();
                return;
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            DiUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", context.UserName
                    }
                });

            var ticket = new AuthenticationTicket(oAuthIdentity, props);
            context.Validated(ticket);

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

    }
}