using Azure.Core;
using Azure.Identity;

namespace Mcp.Azure.Tests.Common
{
    public record AzureTestConfig
    {
        public string SubscriptionId { get; init; }
        public string TenantId { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public TokenCredential Credential { get; init; }
        public string PrincipalId { get; init; }

        public AzureTestConfig(
            string subscriptionId,
            string tenantId,
            string clientId,
            string clientSecret,
            TokenCredential credential,
            string principalId)
        {
            SubscriptionId = subscriptionId;
            TenantId = tenantId;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Credential = credential;
            PrincipalId = principalId;
        }
    }
}