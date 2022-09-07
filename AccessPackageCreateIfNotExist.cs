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
        static async Task<AccessPackage> AccessPackageCreateIfNotExists(GraphServiceClient graphServiceClient, string accessPackageName, AccessPackageCatalog spokeAccessPackageCatalog)
        {
            var accessPackageList = await GetAccessPackageList(graphServiceClient, spokeAccessPackageCatalog.Id);
            AccessPackage accessPackage = accessPackageList.FirstOrDefault<AccessPackage>(x => x.DisplayName == accessPackageName);
            if (accessPackage is null)
            {
                accessPackage = await CreateAccessPackage(graphServiceClient, spokeAccessPackageCatalog.Id, accessPackageName);
                Console.WriteLine(string.Format("{0} access package created.", accessPackageName));
            }
            else
            {
                Console.WriteLine(string.Format("{0} access package already exists.", accessPackageName));
            }

            return accessPackage;
        }

        private static async Task<IEntitlementManagementAccessPackagesCollectionPage> GetAccessPackageList(GraphServiceClient graphServiceClient, string spokeAccessPackageCatalogId)
        {
            var accessPackages = await graphServiceClient.IdentityGovernance.EntitlementManagement.AccessPackages
                .Request()
                .GetAsync();

            return accessPackages;
        }

        private static async Task<AccessPackage> CreateAccessPackage(GraphServiceClient graphServiceClient, string catalogId, string accessPackageName)
        {
            try
            {

                var accessPackage = new AccessPackage
                {
                    DisplayName = accessPackageName,
                    Description = accessPackageName,
                    IsHidden = false,
                    Catalog = new AccessPackageCatalog
                    {
                        Id = catalogId,
                    }
                };

                AccessPackage ap = await graphServiceClient.IdentityGovernance.EntitlementManagement.AccessPackages
                    .Request()
                    .AddAsync(accessPackage);

                return ap;
            }
            catch (ServiceException e)
            {
                Console.WriteLine("We could not create the Access Package: " + $"{e}");
            }
            return null;
        }
    }
}
