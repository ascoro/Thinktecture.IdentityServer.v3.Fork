﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Connect.Models;
using Thinktecture.IdentityServer.Core.Connect.Services;
using Thinktecture.IdentityServer.Core.Services;
using UnitTests.Plumbing;

namespace UnitTests.TokenRequest_Validation
{
    [TestClass]
    public class TokenRequestValidation_Valid
    {
        const string Category = "TokenRequest Validation - General - Valid";

        ILogger _logger = new DebugLogger();
        ICoreSettings _settings = new TestSettings();
        IUserService _users = new TestUserService();
        ICustomRequestValidator _customRequestValidator = new DefaultCustomRequestValidator();

        [TestMethod]
        [TestCategory(Category)]
        public async Task Valid_Code_Request()
        {
            var client = await _settings.FindClientByIdAsync("codeclient");
            var store = new TestAuthorizationCodeStore();

            var code = new AuthorizationCode
            {
                Client = client,
                IsOpenId = true,
                RedirectUri = new Uri("https://server/cb"),
            };

            await store.StoreAsync("valid", code);

            var validator = ValidatorFactory.CreateTokenValidator(_settings, _logger,
                authorizationCodeStore: store,
                customRequestValidator: _customRequestValidator);

            var parameters = new NameValueCollection();
            parameters.Add(Constants.TokenRequest.GrantType, Constants.GrantTypes.AuthorizationCode);
            parameters.Add(Constants.TokenRequest.Code, "valid");
            parameters.Add(Constants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client);

            Assert.IsFalse(result.IsError);
        }

        [TestMethod]
        [TestCategory(Category)]
        public async Task Valid_ClientCredentials_Request()
        {
            var client = await _settings.FindClientByIdAsync("client");

            var validator = ValidatorFactory.CreateTokenValidator(_settings, _logger,
                customRequestValidator: _customRequestValidator);

            var parameters = new NameValueCollection();
            parameters.Add(Constants.TokenRequest.GrantType, Constants.GrantTypes.ClientCredentials);
            parameters.Add(Constants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client);

            Assert.IsFalse(result.IsError);
        }

        [TestMethod]
        [TestCategory(Category)]
        public async Task Valid_ClientCredentials_Request_Restricted_Client()
        {
            var client = await _settings.FindClientByIdAsync("client_restricted");

            var validator = ValidatorFactory.CreateTokenValidator(_settings, _logger,
                customRequestValidator: _customRequestValidator);

            var parameters = new NameValueCollection();
            parameters.Add(Constants.TokenRequest.GrantType, Constants.GrantTypes.ClientCredentials);
            parameters.Add(Constants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client);

            Assert.IsFalse(result.IsError);
        }

        [TestMethod]
        [TestCategory(Category)]
        public async Task Valid_ResourceOwner_Request()
        {
            var client = await _settings.FindClientByIdAsync("roclient");

            var validator = ValidatorFactory.CreateTokenValidator(_settings, _logger,
                userService: _users,
                customRequestValidator: _customRequestValidator);

            var parameters = new NameValueCollection();
            parameters.Add(Constants.TokenRequest.GrantType, Constants.GrantTypes.Password);
            parameters.Add(Constants.TokenRequest.UserName, "bob");
            parameters.Add(Constants.TokenRequest.Password, "bob");
            parameters.Add(Constants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client);

            Assert.IsFalse(result.IsError);
        }

        [TestMethod]
        [TestCategory(Category)]
        public async Task Valid_ResourceOwner_Request_Restricted_Client()
        {
            var client = await _settings.FindClientByIdAsync("roclient_restricted");

            var validator = ValidatorFactory.CreateTokenValidator(_settings, _logger,
                userService: _users,
                customRequestValidator: _customRequestValidator);

            var parameters = new NameValueCollection();
            parameters.Add(Constants.TokenRequest.GrantType, Constants.GrantTypes.Password);
            parameters.Add(Constants.TokenRequest.UserName, "bob");
            parameters.Add(Constants.TokenRequest.Password, "bob");
            parameters.Add(Constants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client);

            Assert.IsFalse(result.IsError);
        }

        [TestMethod]
        [TestCategory(Category)]
        public async Task Valid_AssertionFlow_Request()
        {
            var client = await _settings.FindClientByIdAsync("assertionclient");

            var validator = ValidatorFactory.CreateTokenValidator(_settings, _logger,
                assertionGrantValidator: new TestAssertionValidator(),
                customRequestValidator: _customRequestValidator);

            var parameters = new NameValueCollection();
            parameters.Add(Constants.TokenRequest.GrantType, "assertionType");
            parameters.Add(Constants.TokenRequest.Assertion, "assertion");
            parameters.Add(Constants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client);

            Assert.IsFalse(result.IsError);
        }
    }
}