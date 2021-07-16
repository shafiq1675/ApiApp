using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
//using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace ApiApp
{
    public class CustomAuthenticationFilter : AuthorizeAttribute, IAuthenticationFilter
    {
        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            string authParameter = string.Empty;
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;
            string[] tokenAndUser = null;
            if (authorization == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Authorization Header", request);
                return;
            }
            if (authorization.Scheme != "Bearer")
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid authorization schema", request);
                return;
            }

            tokenAndUser = authorization.Parameter.Split(':');
            string token = tokenAndUser[0];
            string user = tokenAndUser[1];
            if (string.IsNullOrEmpty(token))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Token", request);
                return;
            }
            try
            {
                string validUserName = TokenManager.ValidateToken(token);
                if(validUserName != user)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Invalid Token for the user", request);
                    return;
                }
                context.Principal = TokenManager.GetPrincipal(token);
            }
            catch(Exception ex)
            {
                context.ErrorResult = new AuthenticationFailureResult(ex.Message, request);
                return;
            }
        }

        //public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        //{
        //    string authParameter = string.Empty;
        //    HttpRequestMessage request = context.Request;
        //    AuthenticationHeaderValue authorization = request.Headers.Authorization;
        //    if (authorization == null)
        //    {
        //        context.ErrorResult = new AuthenticationFailureResult("Missing Authorization Header", request);
        //        return;
        //    }
        //    if(authorization ==null && authorization.Scheme != "Bearer")
        //    {
        //        context.ErrorResult = new AuthenticationFailureResult("Invalid authorization schema",request);
        //        return;
        //    }

        //    if (string.IsNullOrEmpty(authorization.Parameter))
        //    {
        //        context.ErrorResult = new AuthenticationFailureResult("Missing Token", request);
        //        return;
        //    }

        //    context.Principal = TokenManager.GetPrincipal(authorization.Parameter);
        //}

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var result = await context.Result.ExecuteAsync(cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized) {
                result.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=localhost"));
            }
            context.Result = new ResponseMessageResult(result);
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public string ReasonPhrase;
        public HttpRequestMessage Request { set; get; }
        public AuthenticationFailureResult( string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
           return Task.FromResult(Execute());
        }

        public HttpResponseMessage Execute()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            responseMessage.ReasonPhrase = ReasonPhrase;
            responseMessage.RequestMessage = Request;
            return responseMessage;
        }
    }
}