using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Google.Apis.Auth.OAuth2;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace GAAuth
{
    public  class GetGAToken
    {
        [FunctionName("GetGAToken")]
            public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            var kvClient = new SecretClient(new Uri(Environment.GetEnvironmentVariable("KEY_VAULT_URL")), new ManagedIdentityCredential());
            string keyJson = kvClient.GetSecret("GAAuthToken").Value.Value;
 
            var cred = GoogleCredential.FromJson(keyJson).CreateScoped(new string[] { "https://www.googleapis.com/auth/analytics.readonly" });
            var token = await cred.UnderlyingCredential.GetAccessTokenForRequestAsync();
 
            return new OkObjectResult("{\"token\":\"" + token + "\"}");
        }
    }
}
