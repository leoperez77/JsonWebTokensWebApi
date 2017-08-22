﻿using AuthorizationServer.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace AuthorizationServer.Api.Formats
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string AudiencePropertyKey = "audience";

        private readonly string _issuer = string.Empty;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            //if (data == null)
            //{
            //    throw new ArgumentNullException("data");
            //}

            //string audienceId = data.Properties.Dictionary.ContainsKey(AudiencePropertyKey) ? data.Properties.Dictionary[AudiencePropertyKey] : null;

            //if (string.IsNullOrWhiteSpace(audienceId))
            //    throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");

            //Audience audience = AudiencesStore.FindAudience(audienceId);
            //string symmetricKeyAsBase64 = audience.Base64Secret;
            //var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            //var signingKey = new SigningCredentials(new SymmetricSecurityKey(keyByteArray), "HS256");
            //var issued = data.Properties.IssuedUtc;
            //var expires = data.Properties.ExpiresUtc;
            //var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);
            //var handler = new JwtSecurityTokenHandler();
            //var jwt = handler.WriteToken(token);

            //return jwt;

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            string audienceId = data.Properties.Dictionary.ContainsKey("audience") ? data.Properties.Dictionary["audience"] : null;

            if (string.IsNullOrWhiteSpace(audienceId) || audienceId.Length != 32)
            {
                throw new InvalidOperationException("audience missing from AuthenticationTicket.Properties");
            }

            var dataLayer = new RepoManager(new DataLayerDapper()).DataLayer;
            var audienceDto = dataLayer.GetAudience(audienceId);

            if (audienceDto == null)
            {
                throw new InvalidOperationException("invalid_client");
            }

            var keyByteArray = Convert.FromBase64String(audienceDto.Secret);       
            var signingKey = new SigningCredentials(new SymmetricSecurityKey(keyByteArray), "HS256");
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;
            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);
            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}