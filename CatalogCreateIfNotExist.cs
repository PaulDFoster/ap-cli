using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ap_cli
{
    internal partial class Program
    {
         static async Task<AccessPackageCatalog> CatalogCreateIfNotExists(GraphServiceClient graphServiceClient, string spokeCatalogDisplayName)
        {
            var catalogList = await GetCatalogList(graphServiceClient);
            AccessPackageCatalog spokeAccessPackageCatalog = catalogList.FirstOrDefault<AccessPackageCatalog>(x => x.DisplayName == spokeCatalogDisplayName);
            if (spokeAccessPackageCatalog is null)
            {
                spokeAccessPackageCatalog = await CreateCatalog(graphServiceClient, spokeCatalogDisplayName);
                Console.WriteLine(string.Format("{0} catalog created.", spokeCatalogDisplayName));
            }
            else
            {
                Console.WriteLine(string.Format("{0} catalog already exists.", spokeCatalogDisplayName));
            }

            return spokeAccessPackageCatalog;
        }

        private static async Task<IEntitlementManagementCatalogsCollectionPage> GetCatalogList(GraphServiceClient graphServiceClient)
        {

            try
            {
                var catalogs = await graphServiceClient.IdentityGovernance.EntitlementManagement.Catalogs
                    .Request()
                    .GetAsync();

                Console.WriteLine($"Found {catalogs.Count()} catalogs in the tenant");
                return catalogs;
            }
            catch (ServiceException e)
            {
                Console.WriteLine("We could not retrieve the App Catalog's list: " + $"{e}");
            }
            return null;
        }

        private static async Task<AccessPackageCatalog> CreateCatalog(GraphServiceClient graphServiceClient, string spokeCatalogDisplayName)
        {
            try
            {
                var accessPackageCatalog = new AccessPackageCatalog
                {
                    DisplayName = spokeCatalogDisplayName,
                    Description = spokeCatalogDisplayName,
                    IsExternallyVisible = true,
                    //Type note supported CatalogType = AccessPackageCatalogType.ServiceManaged,
                };

                var createdAccessPackageCatalog = await graphServiceClient.IdentityGovernance.EntitlementManagement.Catalogs
                    .Request()
                    .AddAsync(accessPackageCatalog);
                return createdAccessPackageCatalog;
            }
            catch (ServiceException e)
            {
                Console.WriteLine("We could not create the Spoke Specific App Catalog: " + $"{e}");
                return null;
            }
        }


    }
}
