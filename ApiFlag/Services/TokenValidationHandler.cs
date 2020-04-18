using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using ApiFlag.Models;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using ApiFlag.ResponseCode;

namespace ApiFlag.Services
{
    /// <summary>
    /// Validador de tokens para solicitud de autorización mediante un DelegatingHandler
    /// </summary>
    internal class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;
            var vrlResponse = new ResponseModel();
           


            // determine whether a jwt exists or not
            if (!TryRetrieveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessageForUser = CodeManager.MSG_User_200;
                vrlResponse.RsMessage = "Se ha denegado la autorización para esta solicitud.";

                vrlResponse.RsContent = statusCode;

                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
                var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
                var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

                SecurityToken securityToken;
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audienceToken,
                    ValidIssuer = issuerToken,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey
                };

                // Extract and assign Current Principal and user
                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException s)
            {
                statusCode = HttpStatusCode.Unauthorized;
                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessageForUser = CodeManager.MSG_User_200;
                vrlResponse.RsMessage = s.Message;
                vrlResponse.RsContent = statusCode;
            }
            catch (Exception e)
            {
                statusCode = HttpStatusCode.InternalServerError;
                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessageForUser = CodeManager.MSG_User_200;
                vrlResponse.RsMessage = e.Message;
                vrlResponse.RsContent = statusCode;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(vrlResponse), System.Text.Encoding.UTF8, "application/json")
            });

        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }
    

    /// <summary>
    /// Decodifica el JWT
    /// </summary>
    /// <param name="tokenint">JWT que viene del bearer</param>
    /// <returns>Secret key</returns>
    private static string hs256(string tokenint) {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtInput = tokenint;

            var rspon = "";
            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken != true)
            {
                return rspon= "0";
            }
            if (readableToken == true)
            {
                var token = jwtHandler.ReadJwtToken(jwtInput);

                //Extract the headers of the JWT
                var headers = token.Header;
                var jwtHeader = "{";
                foreach (var h in headers)
                {
                    jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
                }
                jwtHeader += "}";
                rspon = "Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented);

                //Extract the payload of the JWT
                var claims = token.Claims;
                var jwtPayload = "{";
                var sk = "";
                foreach (Claim c in claims)
                {
                    jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";

                    if (c.Type == "base64") {
                        sk = c.Value.ToString();
                    }
                }
                jwtPayload += "}";
                rspon += "\r\nPayload:\r\n" + JToken.Parse(jwtPayload).ToString(Formatting.Indented);

                var base64EncodedBytes = System.Convert.FromBase64String(sk);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes).ToString();
                
            }
            return "0";
        }

    }


}