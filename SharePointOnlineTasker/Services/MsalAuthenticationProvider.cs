using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using File = System.IO.File;
using Prompt = Microsoft.Identity.Client.Prompt;

namespace SharePointOnlineTasker.Services
{
    public class MsalAuthenticationProvider : IAuthenticationProvider
    {
        /// <summary>
        /// Instantiate a Singleton of the Semaphore with a value of 1
        /// This means that only 1 thread can be granted access at a time
        /// </summary>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly IConfiguration _configuration;
        private readonly ILogger<MsalAuthenticationProvider> _logger;
        private readonly string _credentialsFilePath;
        private readonly IPublicClientApplication _clientApp;

        public MsalAuthenticationProvider(IConfiguration configuration, ILogger<MsalAuthenticationProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _credentialsFilePath = Path.Combine(configuration["DataStorePath"], configuration["CredentialsFile"]);
            _clientApp = PublicClientApplicationBuilder.Create(configuration["AppRegistration:ClientId"])
                .WithAuthority($"https://login.microsoftonline.com/{configuration["AppRegistration:TenantId"]}")
                .WithDefaultRedirectUri()
                .Build();
            _clientApp.UserTokenCache.SetBeforeAccessAsync(BeforeAccessNotificationAsync);
            _clientApp.UserTokenCache.SetAfterAccessAsync(AfterAccessNotificationAsync);
        }

        private async Task AfterAccessNotificationAsync(TokenCacheNotificationArgs args)
        {
            if (args.HasStateChanged)
            {
                await _semaphoreSlim.WaitAsync();

                try
                {
                    byte[] data = args.TokenCache.SerializeMsalV3();
                    await File.WriteAllBytesAsync(_credentialsFilePath, data);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to serialize token cache");
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }

        private async Task BeforeAccessNotificationAsync(TokenCacheNotificationArgs args)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (!File.Exists(_credentialsFilePath))
                {
                    args.TokenCache.DeserializeMsalV3(null);
                }
                else
                {
                    byte[] data = await File.ReadAllBytesAsync(_credentialsFilePath);
                    args.TokenCache.DeserializeMsalV3(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to deserialize token cache");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = await GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        public async Task<string> GetTokenAsync()
        {
            string[] scopes = _configuration.GetSection("Scopes").Get<string[]>();

            AuthenticationResult authResult = await _clientApp.AcquireTokenInteractive(scopes)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync();
            return authResult.AccessToken;
        }
    }
}