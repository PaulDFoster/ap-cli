// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CommandLine;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ap_cli
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    try
                    {
                        AuthenticationConfig config;
                        if (o.appsettings)
                        {
                            config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
                        }
                        else
                        {
                            config = AuthenticationConfig.ReadFromArgs(o);
                        }

                        bool isUsingClientSecret = IsAppUsingClientSecret(config);

                        IConfidentialClientApplication app;

                        if (isUsingClientSecret)
                        {
                            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                                .WithClientSecret(config.ClientSecret)
                                .WithAuthority(new Uri(config.Authority))
                                .Build();
                        }
                        else
                        {
                            ICertificateLoader certificateLoader = new DefaultCertificateLoader();
                            certificateLoader.LoadIfNeeded(config.Certificate);

                            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                                .WithCertificate(config.Certificate.Certificate)
                                .WithAuthority(new Uri(config.Authority))
                                .Build();
                        }

                        app.AddInMemoryTokenCache();


                        RunAsync(app, config, o.GroupId, o.CatalogDisplayName, o.AccessPackageName,o.ApproverUserId).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }

                });

        }

        private static async Task RunAsync(IConfidentialClientApplication app, AuthenticationConfig config, string groupId, string spokeCatalogDisplayName, string accessPackageName, string approverUserId)
        {
            groupId= groupId.Trim('"');
            approverUserId = approverUserId.Trim('"');
            // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by a tenant administrator. 
            string[] scopes = new string[] { $"{config.ApiUrl}.default" }; // Generates a scope -> "https://graph.microsoft.com/.default"

            GraphServiceClient graphServiceClient = GetAuthenticatedGraphClient(app, scopes);

            AccessPackageCatalog spokeAccessPackageCatalog = await CatalogCreateIfNotExists(graphServiceClient, spokeCatalogDisplayName);

            AccessPackage accessPackage = await AccessPackageCreateIfNotExists(graphServiceClient, accessPackageName, spokeAccessPackageCatalog);

            CatalogResource accessPackageCatalogGroup = await GroupAddIfNotExist(graphServiceClient, groupId, spokeCatalogDisplayName, spokeAccessPackageCatalog);

            await GroupRoleAddIfNotExist(graphServiceClient, accessPackage, accessPackageCatalogGroup, groupId);

            await AccessPackagePolicyCreateIfNotExist(graphServiceClient, accessPackage.Id, accessPackage.DisplayName, approverUserId);

        }

        private static async Task<string> SendBetaHttpMessage(GraphServiceClient graphServiceClient, string url, string body, HttpMethod method)
        {

            using (var httpClient = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(method, url))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                await graphServiceClient.AuthenticationProvider.AuthenticateRequestAsync(request);

                var response = await httpClient.SendAsync(request);

                var res = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                return res;
            }
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfidentialClientApplication app, string[] scopes)
        {

            GraphServiceClient graphServiceClient =
                    new GraphServiceClient("https://graph.microsoft.com/V1.0/", new DelegateAuthenticationProvider(async (requestMessage) =>
                    {
                        // Retrieve an access token for Microsoft Graph (gets a fresh token if needed).
                        AuthenticationResult result = await app.AcquireTokenForClient(scopes)
                            .ExecuteAsync();

                        // Add the access token in the Authorization header of the API request.
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));

            return graphServiceClient;
        }

        /// <summary>
        /// Checks if the sample is configured for using ClientSecret or Certificate. This method is just for the sake of this sample.
        /// You won't need this verification in your production application since you will be authenticating in AAD using one mechanism only.
        /// </summary>
        /// <param name="config">Configuration from appsettings.json</param>
        /// <returns></returns>
        private static bool IsAppUsingClientSecret(AuthenticationConfig config)
        {
            string clientSecretPlaceholderValue = "[Enter here a client secret for your application]";

            if (!String.IsNullOrWhiteSpace(config.ClientSecret) && config.ClientSecret != clientSecretPlaceholderValue)
            {
                return true;
            }

            else if (config.Certificate != null)
            {
                return false;
            }

            else
                throw new Exception("You must choose between using client secret or certificate.");
        }


    }
}
