﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace MyAddressBookPlus
{
    public static class KeyVaultService
    {
        public static string CacheConnection { get; set; }

        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            var clientCredentials = new ClientCredential(WebConfigurationManager.AppSettings["ClientId"], WebConfigurationManager.AppSettings["ClientSecret"]);
            var result = await authContext.AcquireTokenAsync(resource, clientCredentials);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }

    }
}