using Microsoft.Graph;
using System;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ap_cli.MicrosoftGraphBeta;

namespace ap_cli
{
    internal partial class Program
    {
        static async Task<CatalogResource> GroupAddIfNotExist(GraphServiceClient graphServiceClient, string groupId, string spokeCatalogDisplayName, AccessPackageCatalog spokeAccessPackageCatalog)
        {
            CatalogResource[] catalogResources = await GetAccessPackageCatalogResources(graphServiceClient, spokeAccessPackageCatalog.Id);
            CatalogResource accessPackageCatalogGroup = catalogResources.FirstOrDefault<CatalogResource>(x => x.OriginId == groupId);
            if (accessPackageCatalogGroup is null)
            {
                OdataResourceResponse odataResourceResponse = await AddGroupToCatalog(graphServiceClient, spokeAccessPackageCatalog.Id, groupId);
                // Check if the resource was successfully added

                // Assuming it was, get the new catalog group id
                catalogResources = await GetAccessPackageCatalogResources(graphServiceClient, spokeAccessPackageCatalog.Id);
                accessPackageCatalogGroup = catalogResources.First<CatalogResource>(x => x.OriginId == groupId);
                Console.WriteLine(string.Format("{0} group created in catalog {1}.", groupId, spokeCatalogDisplayName));
            }
            else
            {
                Console.WriteLine(string.Format("{0} group already exists in catalog {1}.", groupId, spokeCatalogDisplayName));
            }

            return accessPackageCatalogGroup;
        }

        /// <summary>
        /// Microsoft Graph BETA api
        /// </summary>
        /// <param name="graphServiceClient"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        private static async Task<CatalogResource[]> GetAccessPackageCatalogResources(GraphServiceClient graphServiceClient, string catalogId)
        {

            string url = string.Format("https://graph.microsoft.com/beta/identityGovernance/entitlementManagement/accessPackageCatalogs/{0}/accesspackageresources", catalogId);

            string res = await SendBetaHttpMessage(graphServiceClient, url, "", HttpMethod.Get);

            //Needs to find record of Group Id and get the Catalog Group Id from it
            OdataResponse odataResponse = JsonSerializer.Deserialize<OdataResponse>(res);

            return odataResponse.CatalogResources;
        }

        /// <summary>
        /// Microsoft Graph BETA api
        /// </summary>
        /// <param name="graphServiceClient"></param>
        /// <param name="catalogId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private static async Task<OdataResourceResponse> AddGroupToCatalog(GraphServiceClient graphServiceClient, string catalogId, string groupId)
        {

            using (var httpClient = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/beta/identityGovernance/entitlementManagement/accessPackageResourceRequests"))
            {
                AccessPackageResourceRequest accessPackageResourceRequest = new AccessPackageResourceRequest();
                accessPackageResourceRequest.catalogId = catalogId;
                accessPackageResourceRequest.requestType = "AdminAdd";
                AccessPackageResource accessPackageResource = new AccessPackageResource();
                accessPackageResource.originId = groupId;
                accessPackageResource.originSystem = "AadGroup";


                accessPackageResourceRequest.accessPackageResource = accessPackageResource;
                JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.WriteIndented = false;

                string body = JsonSerializer.Serialize<AccessPackageResourceRequest>(accessPackageResourceRequest, jsonSerializerOptions);
                Console.WriteLine(body);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                await graphServiceClient.AuthenticationProvider.AuthenticateRequestAsync(request);

                var response = await httpClient.SendAsync(request);
                var res = await response.Content.ReadAsStringAsync();
                Console.WriteLine(res);
                response.EnsureSuccessStatusCode();
                OdataResourceResponse odataResourceResponse = JsonSerializer.Deserialize<OdataResourceResponse>(res);

                return odataResourceResponse;

            }
        }


    }
}
