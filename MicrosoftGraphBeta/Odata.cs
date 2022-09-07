using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ap_cli
{
    class CatalogResource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("originId")]
        public string OriginId { get; set; }

        [JsonPropertyName("originSystem")]
        public string OriginSystem { get; set; }

        [JsonPropertyName("isPendingOnboarding")]
        public bool IsPendingOnboarding { get; set; }

        [JsonPropertyName("addedBy")]
        public string AddedBy { get; set; }

        [JsonPropertyName("addedOn")]
        public DateTimeOffset AddedOn { get; set; }

        [JsonPropertyName("attributes")]
        public object[] Attributes { get; set; }
    }
    class OdataResponse
    {
        [JsonPropertyName("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonPropertyName("value")]
        public CatalogResource[] CatalogResources { get; set; }
    }

    class OdataResourceResponse
    {
        [JsonPropertyName("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("requestType")]
        public string RequestType { get; set; }

        [JsonPropertyName("requestState")]
        public string RequestState { get; set; }

        [JsonPropertyName("requestStatus")]
        public string RequestStatus { get; set; }

        [JsonPropertyName("catalogId")]
        public Guid CatalogId { get; set; }

        [JsonPropertyName("executeImmediately")]
        public bool ExecuteImmediately { get; set; }

        [JsonPropertyName("justification")]
        public object Justification { get; set; }

        [JsonPropertyName("expirationDateTime")]
        public object ExpirationDateTime { get; set; }
    }
}
