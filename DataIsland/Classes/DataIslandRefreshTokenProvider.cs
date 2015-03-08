using dimain.Models.oauth;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using dataislandcommon.Utilities;
using dimain.Services.System;

namespace DataIsland.Classes
{
    public class DataIslandRefreshTokenProvider : IAuthenticationTokenProvider
    {

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDiRefreshTokenService tokenService = ioc.Resolve<IDiRefreshTokenService>();

                var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

                var token = new RefreshToken()
                {
                    Id = tokenService.GetHash(refreshTokenId),
                    ClientId = clientid,
                    Subject = context.Ticket.Identity.Name,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                token.ProtectedTicket = context.SerializeTicket();

                var result = await tokenService.AddRefreshToken(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDiRefreshTokenService tokenService = ioc.Resolve<IDiRefreshTokenService>();
                string hashedTokenId = tokenService.GetHash(context.Token);

                var refreshToken = await tokenService.FindRefreshToken(hashedTokenId);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await tokenService.RemoveRefreshToken(hashedTokenId);
                }
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            this.CreateAsync(context).Wait();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            this.ReceiveAsync(context).Wait();
        }
    }
}