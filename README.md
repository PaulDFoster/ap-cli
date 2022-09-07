# ap-cli

ap-cli uses Microsoft Graph V1.0 and Beta api to create the required Entitlement Management resources for a AAD Group to be added with Member role into an Access Package.
The current Microsoft Graph V1.0 api doesn't support all the required functionality.

The required arguments for the ap-cli can be specified as command line arguments or within the appsettings.json file ( -j false required arg).

To use this utility you require:

- Tenant Id
- Client Id
- Client Secret OR Certificate details defined in appsettings.json

- Group Id of the AAD group to be added to the Access Package
- Catalog Display Name to be used or created
- Access Package Name to be used or created

Defaults for Azure Public Cloud used unless others specified

There are several permissions required for the logged in Azure user including:

- Directory.ReadWrite.All
- Group.ReadWrite.All
- EntitlementManagement.ReadWrite.All

