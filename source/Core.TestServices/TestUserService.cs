﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license
 */
using System;
/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license
 */
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.TestServices
{
    public class TestUserService : IMultiTenantUserService, IUserService
    {
        public Task<AuthenticateResult> AuthenticateLocalAsync(string tenant, string username, string password)
        {
            if (tenant + "_" + username != password) return Task.FromResult<AuthenticateResult>(null);

            return Task.FromResult(new AuthenticateResult(username, username));
        }

        public Task<ExternalAuthenticateResult> AuthenticateExternalAsync(string tenant, string subject, ExternalIdentity user)
        {
            if (user == null)
            {
                return Task.FromResult<ExternalAuthenticateResult>(null);
            }

            var claims = user.Claims;
            if (claims == null)
            {
                return Task.FromResult<ExternalAuthenticateResult>(null);
            }

            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name || x.Type == Constants.ClaimTypes.Name);
            if (name == null)
            {
                return null;
            }

            return Task.FromResult(new ExternalAuthenticateResult(user.Provider.Name, Guid.NewGuid().ToString("D"), name.Value));
        }

        public Task<IEnumerable<Claim>> GetProfileDataAsync(string sub, IEnumerable<string> requestedClaimTypes = null)
        {
            var claims = new List<Claim>();

            if (requestedClaimTypes == null)
            {
                claims.Add(new Claim(Constants.ClaimTypes.Subject, sub));
                return Task.FromResult<IEnumerable<Claim>>(claims);
            }

            foreach (var requestedClaim in requestedClaimTypes)
            {
                if (requestedClaim == Constants.ClaimTypes.Subject)
                {
                    claims.Add(new Claim(Constants.ClaimTypes.Subject, sub));
                }
                else
                {
                    claims.Add(new Claim(requestedClaim, requestedClaim));
                }
            }

            return Task.FromResult<IEnumerable<Claim>>(claims);
        }

        public Task<AuthenticateResult> AuthenticateLocalAsync(string username, string password)
        {
            if (username != password) return Task.FromResult<AuthenticateResult>(null);

            return Task.FromResult(new AuthenticateResult(username, username));
        }


        public Task<ExternalAuthenticateResult> AuthenticateExternalAsync(string subject, ExternalIdentity user)
        {
            if (user == null)
            {
                return Task.FromResult<ExternalAuthenticateResult>(null);
            }

            var claims = user.Claims;
            if (claims == null)
            {
                return Task.FromResult<ExternalAuthenticateResult>(null);
            }

            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name || x.Type == Constants.ClaimTypes.Name);
            if (name == null)
            {
                return null;
            }

            return Task.FromResult(new ExternalAuthenticateResult(user.Provider.Name, Guid.NewGuid().ToString("D"), name.Value));
        }
    }
}