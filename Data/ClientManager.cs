using IdentityServer4.Models;
using System.Collections.Generic;

namespace LogistiqueAuthority.Data
{

    internal static class ClientManager
    {
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                    new Client
                    {
                         ClientName = "LogistiqueWebApp",
                         ClientId = "t8agr5xKt4$3",
                         AllowedGrantTypes = GrantTypes.ClientCredentials,
                         ClientSecrets = { new Secret("eb300de4-add9-42f4-a3ac-abd3c60f1919".Sha256()) },
                         AllowedScopes = new List<string> { "logistique.api.solver"}
                    }
            };
    }
}
