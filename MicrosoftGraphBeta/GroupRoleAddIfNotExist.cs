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
        static async Task GroupRoleAddIfNotExist(GraphServiceClient graphServiceClient, AccessPackage accessPackage, CatalogResource accessPackageCatalogGroup, string groupId)
        {

            OdataAccessPackageResourceResponse odataAccessPackageResourceResponse = await GetExistingAccessPackageResourceRoles(graphServiceClient, accessPackage.Id);
            AccessPackageResourceRoleScope[] accessPackageResourceRoleScopes = odataAccessPackageResourceResponse.AccessPackageResourceRoleScopes;
            AccessPackageResourceRoleScope accessPackageResourceRoleScope = accessPackageResourceRoleScopes.FirstOrDefault<AccessPackageResourceRoleScope>(x => x.AccessPackageResourceRole.OriginId == String.Format("Member_{0}", groupId));
            if (accessPackageResourceRoleScope is null)
            {
                // Add resource to the acces package with role
                await AddResourceToAccessPackageWithRole(graphServiceClient, accessPackage.Id, groupId, accessPackageCatalogGroup.Id);
                Console.WriteLine(string.Format("{0} group created in the access package {1}.", groupId, accessPackage.DisplayName));
            }
            else
            {
                Console.WriteLine(string.Format("{0} group already exists in the access package {1}.", groupId, accessPackage.DisplayName));
            }
        }

        /// <summary>
        /// Microsoft Graph BETA api
        /// </summary>
        /// <param name="graphServiceClient"></param>
        /// <param name="accessPackageId"></param>
        /// <param name="groupId"></param>
        /// <param name="catalogGroupId"></param>
        /// <returns></returns>
        private static async Task AddResourceToAccessPackageWithRole(GraphServiceClient graphServiceClient, string accessPackageId, string groupId, string catalogGroupId)
        {

            string body = string.Format("{{\"accessPackageResourceRole\":{{\"originId\":\"Member_{0}\",\"displayName\":\"Member\",\"originSystem\":\"AadGroup\",\"accessPackageResource\":{{\"id\":\"{1}\",\"resourceType\":\"Security Group\",\"originId\":\"{2}\",\"originSystem\":\"AadGroup\"}}  }}, \"accessPackageResourceScope\":{{   \"originId\":\"{3}\",\"originSystem\":\"AadGroup\" }}}}", groupId, catalogGroupId, groupId, groupId);
            string url = string.Format("https://graph.microsoft.com/beta/identityGovernance/entitlementManagement/accessPackages/{0}/accessPackageResourceRoleScopes", accessPackageId);

            string res = await SendBetaHttpMessage(graphServiceClient, url, body, HttpMethod.Post);
        }


        /// <summary>
        /// Microsoft Graph BETA api
        /// </summary>
        /// <param name="graphServiceClient"></param>
        /// <param name="accessPackageId"></param>
        /// <returns></returns>
        private static async Task<OdataAccessPackageResourceResponse> GetExistingAccessPackageResourceRoles(GraphServiceClient graphServiceClient, string accessPackageId)
        {

            string url = string.Format("https://graph.microsoft.com/beta/identityGovernance/entitlementManagement/accessPackages/{0}?$expand=accessPackageResourceRoleScopes($expand=accessPackageResourceRole,accessPackageResourceScope)", accessPackageId);

            string res = await SendBetaHttpMessage(graphServiceClient, url, "", HttpMethod.Get);

            OdataAccessPackageResourceResponse odataAccessPackageResoureRolesResponse = JsonSerializer.Deserialize<OdataAccessPackageResourceResponse>(res);
            return odataAccessPackageResoureRolesResponse;
        }

    }
}
