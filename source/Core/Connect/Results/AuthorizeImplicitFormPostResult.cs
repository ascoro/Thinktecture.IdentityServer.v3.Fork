﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license
 */

using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityServer.Core.Connect.Models;

namespace Thinktecture.IdentityServer.Core.Connect.Results
{
    public class AuthorizeImplicitFormPostResult : IHttpActionResult
    {
        private readonly AuthorizeResponse _response;

        public AuthorizeImplicitFormPostResult(AuthorizeResponse response)
        {
            _response = response;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpResponseMessage>(Execute());
        }

        private HttpResponseMessage Execute()
        {
            string form;
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Thinktecture.IdentityServer.Core.Connect.FormPostResponse.html"))
            {
                using (var reader = new StreamReader(stream))
                {
                    form = reader.ReadToEnd();
                }
            }

            form = form.Replace("{{redirect_uri}}", _response.RedirectUri.AbsoluteUri);
            form = form.Replace("{{id_token}}", _response.IdentityToken);
            form = form.Replace("{{state}}", _response.State ?? "");

            var content = new StringContent(form, Encoding.UTF8, "text/html");
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            //_logger.InformationFormat("Post back form: {0}", form);
            return message;
        }
    }
}