using System;
using System.Text.Json.Serialization;

namespace ap_cli.MicrosoftGraphBeta
{

    public partial class OdataAccessPackageResourceResponse
    {
        [JsonPropertyName("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("catalogId")]
        public Guid CatalogId { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isHidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("isRoleScopesVisible")]
        public bool IsRoleScopesVisible { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdDateTime")]
        public DateTimeOffset CreatedDateTime { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedDateTime")]
        public DateTimeOffset ModifiedDateTime { get; set; }

        [JsonPropertyName("accessPackageResourceRoleScopes@odata.context")]
        public Uri AccessPackageResourceRoleScopesOdataContext { get; set; }

        [JsonPropertyName("accessPackageResourceRoleScopes")]
        public AccessPackageResourceRoleScope[] AccessPackageResourceRoleScopes { get; set; }
    }

    public partial class AccessPackageResourceRoleScope
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdDateTime")]
        public DateTimeOffset CreatedDateTime { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedDateTime")]
        public DateTimeOffset ModifiedDateTime { get; set; }

        [JsonPropertyName("accessPackageResourceRole@odata.context")]
        public Uri AccessPackageResourceRoleOdataContext { get; set; }

        [JsonPropertyName("accessPackageResourceRole")]
        public AccessPackageResourceRole AccessPackageResourceRole { get; set; }

        [JsonPropertyName("accessPackageResourceScope@odata.context")]
        public Uri AccessPackageResourceScopeOdataContext { get; set; }

        [JsonPropertyName("accessPackageResourceScope")]
        public AccessPackageResourceScope AccessPackageResourceScope { get; set; }
    }

    public partial class AccessPackageResourceRole
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("originSystem")]
        public string OriginSystem { get; set; }

        [JsonPropertyName("originId")]
        public string OriginId { get; set; }
    }

    public partial class AccessPackageResourceScope
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("originId")]
        public Guid OriginId { get; set; }

        [JsonPropertyName("originSystem")]
        public string OriginSystem { get; set; }

        [JsonPropertyName("roleOriginId")]
        public object RoleOriginId { get; set; }

        [JsonPropertyName("isRootScope")]
        public bool IsRootScope { get; set; }

        [JsonPropertyName("url")]
        public object Url { get; set; }
    }

}
